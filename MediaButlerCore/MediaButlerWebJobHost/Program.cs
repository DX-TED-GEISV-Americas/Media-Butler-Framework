using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using System.Threading;
using MediaButler.Common.HostWorkflow;
using System.Configuration;
using MediaButler.Common.Configuration;
using MediaButler.Common.WatcherWatcher;

namespace MediaButlerWebJobHost
{
    /// <summary>
    /// MBF Web Job HOST
    /// </summary>
    public class Program
    {
        private static readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private static string ButlerWorkFlowManagerHostConfigKey = "MediaButler.Workflow.ButlerWorkFlowManagerWorkerRole";
        private static ConfigurationData myConfigData;

        /// <summary>
        /// Get MBF storage configuration conection string
        /// </summary>
        /// <returns>connection string</returns>
        private static string GetConnString()
        {
            return ConfigurationManager.AppSettings["MediaButler.ConfigurationStorageConnectionString"];
        }

        /// <summary>
        /// Setup AMS Workflow manager confguration
        /// </summary>
        /// <param name="ConfigurationStorageConnectionString">configuration storage connection string</param>
        private static void Setup(string ConfigurationStorageConnectionString)
        {
            string json = MBFConfiguration.GetConfigurationValue("roleconfig", ButlerWorkFlowManagerHostConfigKey, ConfigurationStorageConnectionString);
            myConfigData = Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigurationData>(json);
            myConfigData.poisonQueue = MBFConfiguration.ButlerFailedQueue;
            myConfigData.inWorkQueueName = MBFConfiguration.ButlerSendQueue;
            myConfigData.ProcessConfigConn = ConfigurationStorageConnectionString;
            myConfigData.MaxCurrentProcess = myConfigData.MaxCurrentProcess;
            myConfigData.SleepDelay = myConfigData.SleepDelay;
            myConfigData.MaxDequeueCount = myConfigData.MaxDequeueCount;

        }

        /// <summary>
        /// Run MBF Workflow Manager task
        /// </summary>
        /// <returns></returns>
        [NoAutomaticTrigger]
        public static async Task RunMediaButlerWorkflow()
        {
            Setup(GetConnString());
            MediaButlerHost xHost = new MediaButlerHost(myConfigData);
            await xHost.ExecuteAsync(cancellationTokenSource.Token);
        }

        /// <summary>
        /// Run MBF watcher task
        /// </summary>
        /// <returns></returns>
        [NoAutomaticTrigger]
        public static async Task RunMediaButlerWatcher()
        {
            MediaButlerWatcherHost XHost = new MediaButlerWatcherHost(GetConnString());
            await XHost.Run();
        }

        /// <summary>
        /// Main Web JOB process, configur and satrt MBF workflow Manager and watcher process
        /// </summary>
        static void Main()
        {
            JobHostConfiguration config = new JobHostConfiguration();
            config.StorageConnectionString = GetConnString();
            config.DashboardConnectionString = config.StorageConnectionString;
            JobHost host = new JobHost(config);
            host.CallAsync(typeof(Program).GetMethod("RunMediaButlerWorkflow"));
            host.CallAsync(typeof(Program).GetMethod("RunMediaButlerWatcher"));
            host.RunAndBlock();


        }
    }
}
