using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Diagnostics;

namespace MediaButler.Common.Configuration
{
    /// <summary>
    /// MBF main configuration information
    /// </summary>
    public class MBFConfiguration
    {
        /// <summary>
        /// MBF configuration table name
        /// </summary>
        private const string configurationTableName = "ButlerConfiguration";
        
        /// <summary>
        /// MBF connection string configuration key
        /// </summary>
        private const string butlerStorageConnectionConfigurationKey = "MediaButler.ConfigurationStorageConnectionString";
        
        /// <summary>
        /// Fail queue polling interval
        /// </summary>
        public static int FailedQueuePollingInterval
        {
            get
            {
                int pollingInterval = 5;
                try
                {
                    pollingInterval = Convert.ToInt32(MBFConfiguration.GetConfigurationValue("FailedQueuePollingSeconds", "general"));
                }
                catch (Exception)
                {
                    // do nothing, use default value
                    Trace.TraceWarning("Could not load Failed queue polling interval using default");
                }
                return pollingInterval;
            }
        }
        
        /// <summary>
        /// Sucees queue polling interval
        /// </summary>
        public static int SuccessQueuePollingInterval
        {
            get
            {
                int pollingInterval = 5;
                try
                {
                    pollingInterval = Convert.ToInt32(MBFConfiguration.GetConfigurationValue("SuccessQueuePollingSeconds", "general"));
                }
                catch (Exception)
                {
                    // do nothing, use default value
                    Trace.TraceWarning("Could not load Success queue polling interval using default");
                }
                return pollingInterval;
            }
        }
        
        /// <summary>
        /// New files pooling interval
        /// </summary>
        public static int BlobWatcherPollingInterval
        {
            get
            {
                int pollingInterval = 5;
                try
                {
                    pollingInterval = Convert.ToInt32(MBFConfiguration.GetConfigurationValue("BlobWatcherPollingSeconds", "general"));
                }
                catch (Exception)
                {
                    // do nothing, use default value
                    Trace.TraceWarning("Could not load Blob watcher polling interval using default");
                }
                return pollingInterval;
            }
        }
        
        /// <summary>
        /// Publci enumeration for Workflow status tracking in Workflow
        /// </summary>
        public enum WorkflowStatus
        {
            Pending,
            Started,
            Finished,
            Running,
            Failed
        }
        
        /// <summary>
        /// MBF stage storage incoming material container
        /// </summary>
        public const string DirectoryInbound = "Incoming";
       
        /// <summary>
        /// Process Handler Configuration  Key
        /// </summary>
        public const  string ProcessHandlerConfigKey = "MediaButler.Common.workflow.ProcessHandler";
        
        /// <summary>
        /// MBF stage storage processing material container
        /// </summary>
        public const string DirectoryProcessing = "Processing";
        
        /// <summary>
        /// MBF stage storage complete material container
        /// </summary>
        public const string DirectoryCompleted = "Completed";
        
        /// <summary>
        /// MBF stage storage failed material container
        /// </summary>
        public const string DirectoryFailed = "Failed";
        
        /// <summary>
        /// MBF traffic light control extension
        /// </summary>
        public const string ControlFileSuffix = ".control";
        
        /// <summary>
        /// MBF new material queue
        /// </summary>
        public const string ButlerSendQueue = "butlersend";
        
        /// <summary>
        /// MBF success process queue
        /// </summary>
        public const string ButlerSuccessQueue = "butlersuccess";
        
        /// <summary>
        /// MBF dead letter queue
        /// </summary>
        public const string ButlerResponseDeadLetter = "butlerresponsedeadletter";
        
        /// <summary>
        /// MBF fail queue name
        /// </summary>
        public const string ButlerFailedQueue = "butlerfailed";
        
        /// <summary>
        /// MBF blob stoarge conatiner for external files
        /// </summary>
        public const string ButlerExternalInfoContainer = "mediabutlerbin";
        
        /// <summary>
        /// MBF preocess status table name
        /// </summary>
        public const string ButlerWorkflowStatus = "ButlerWorkflowStatus";
        
        /// <summary>
        /// MBF flag to keep status on workflow status table
        /// </summary>
        public const string keepStatusProcess = "keepStatusProcess";
        
        /// <summary>
        /// MBF workflow step list metadata key
        /// </summary>
        public const string workflowStepListKey = "workflowStepList";
        
        /// <summary>
        ///  MBF workflow step #  metadata key
        /// </summary>
        public const string workflowStepLength = "workflowStepLength";
        
        /// <summary>
        /// MBF transcoding advance metadata key
        /// </summary>
        public const string TranscodingAdvance = "TranscodingAdvance";
        
        /// <summary>
        /// default dequeue message before fail
        /// </summary>
        public const int maxDequeueCount = 3;
        
        /// <summary>
        /// Success process code
        /// </summary>
        public const int successFinishProcessStep = -100;
        
        /// <summary>
        /// Fail process code
        /// </summary>
        public const int failFinishProcessStep = -200;
        
        /// <summary>
        /// close and fail process code
        /// </summary>
        public const int poisonFinishProcessStep = -300;
        
        /// <summary>
        /// Fatal erorr code
        /// </summary>
        public const int workflowFatalError = -400;
        
        /// <summary>
        /// Get the configuration Value from the configuration Table.
        /// </summary>
        /// <param name="configKey">This is a rowkey in Azure Table</param>
        /// <param name="processKey">This is the partition key in Azure Table</param>
        /// <returns>string configuration data</returns>
        public static string GetConfigurationValue(string configKey, string processKey)
        {
            return GetConfigurationValue(configKey, processKey, System.Configuration.ConfigurationManager.AppSettings[butlerStorageConnectionConfigurationKey]);
        }
        
        /// <summary>
        /// Get the configuration Value from the configuration Table.
        /// </summary>
        /// <param name="configKey">This is a rowkey in Azure Table</param>
        /// <param name="processKey">This is the partition key in Azure Table</param>
        /// <param name="ConfigurationConn"> Storage connection string</param>
        /// <returns>string configuration data</returns>
        public static string GetConfigurationValue(string configKey, string processKey, string ConfigurationConn)
        {
            string configurationValue = "";
            try
            {
                string storageAccountString = ConfigurationConn;
                CloudStorageAccount account = CloudStorageAccount.Parse(storageAccountString);
                CloudTableClient tableClient = account.CreateCloudTableClient();
                CloudTable configTable = tableClient.GetTableReference(configurationTableName);
                TableOperation retrieveOperation = TableOperation.Retrieve<ButlerConfigurationEntity>(processKey, configKey);
                // Execute the retrieve operation.
                TableResult retrievedResult = configTable.Execute(retrieveOperation);
                if (retrievedResult.HttpStatusCode == 200)
                {
                    ButlerConfigurationEntity resultEntity = (ButlerConfigurationEntity)retrievedResult.Result;
                    configurationValue = resultEntity.ConfigurationValue;
                }
                else
                {
                    Trace.TraceInformation("Configuration information is not for " + processKey + " / " + configKey);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Get Configuration Value Error , Check connection string and MBF Stage storage Account: " + ex.Message);
                throw new Exception("Get Configuration Value Error , Check connection string and MBF Stage storage Account: " + ex.Message);
            }
            return configurationValue;
        }
    }
}
