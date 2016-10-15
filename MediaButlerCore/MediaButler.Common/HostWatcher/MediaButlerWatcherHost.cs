using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaButler.Common.Configuration;
using MediaButler.Common.WatcherWatcher;

namespace MediaButler.Common.WatcherWatcher
{
    /// <summary>
    /// MBF watcher process HOST
    /// </summary>
    public class MediaButlerWatcherHost
    {
        /// <summary>
        /// Storage account connection string
        /// </summary>
        private string _storageAccountString;
        /// <summary>
        /// Process control token
        /// </summary>
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        /// <summary>
        /// Process control token 
        /// </summary>
        private CancellationToken Token;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storageAccountString">Storage account connection string</param>
        public MediaButlerWatcherHost(string storageAccountString)
        {
            _storageAccountString = storageAccountString;
            Token = cancellationTokenSource.Token;
        }
        /// <summary>
        /// Execute all MBF watcher jobs
        /// </summary>
        /// <returns>null</returns>
        public async Task Run()
        {
            string s = MBFConfiguration.GetConfigurationValue("ContainersToScan", "MediaButler.Workflow.WorkerRole");
            var containers = s.Split(',');
            string[] ContainersToScan = containers;

            var taskFailedRequests = Task.Run(() => JobManager.getWorkflowFailedOperations(Token, _storageAccountString));
            var taskSuccessfulRequests = Task.Run(() => JobManager.getWorkflowSuccessOperations(Token, _storageAccountString));
            var taskProcessIncomingJobs = Task.Run(() => BlobWatcher.runInboundJobWatcher(Token, _storageAccountString, ContainersToScan));
            Task.WaitAll(taskFailedRequests, taskSuccessfulRequests, taskProcessIncomingJobs);

        }
    }
}
