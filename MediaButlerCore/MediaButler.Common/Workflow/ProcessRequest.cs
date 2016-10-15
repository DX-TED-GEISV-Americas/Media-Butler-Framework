using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;

namespace MediaButler.Common.Workflow
{
    /// <summary>
    /// MBF base process request
    /// </summary>
    public class ProcessRequest : ChainRequest
    {
        /// <summary>
        /// Messaga request
        /// </summary>
        public CloudQueueMessage CurrentMessage { get; set; }
        /// <summary>
        /// Request Configuration data
        /// </summary>
        public string ConfigData { get; set; }
        /// <summary>
        /// Porcess Star time 
        /// </summary>
        public DateTime TimeStampProcessingStarted { get; set; }
        /// <summary>
        /// process log
        /// </summary>
        public List<string> Log { get; set; }
        /// <summary>
        /// Request constructor
        /// </summary>
        public ProcessRequest()
        {
            this.Log = new List<string>();
            this.TimeStampProcessingStarted = DateTime.Now;
            MetaData = new Dictionary<string, string>();
        }
        /// <summary>
        /// Process meta data
        /// </summary>
        public Dictionary<string, string> MetaData { get; set; }

    }
}
