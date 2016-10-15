using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaButler.Common.HostWorkflow
{
    /// <summary>
    /// Workflow  configuration
    /// </summary>
    public class ConfigurationData
    {
        /// <summary>
        /// Max # of concurrent process
        /// </summary>
        public int MaxCurrentProcess { get; set; }
        /// <summary>
        /// polling wait time
        /// </summary>
        public int SleepDelay { get; set; }
        /// <summary>
        /// incoming new process queue name
        /// </summary>
        public string inWorkQueueName { get; set; }
        /// <summary>
        /// poison message queue name
        /// </summary>
        public string poisonQueue { get; set; }
        /// <summary>
        /// max # of message dequeue from incoming queue
        /// </summary>
        public int MaxDequeueCount { get; set; }
        /// <summary>
        /// Storage configuration conection string
        /// </summary>
        public string ProcessConfigConn { get; set; }
        /// <summary>
        /// Pause process flag
        /// </summary>
        public bool IsPaused { get; set; }
    }
}
