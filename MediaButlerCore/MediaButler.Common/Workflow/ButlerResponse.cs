using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaButler.Common.Workflow
{
    /// <summary>
    /// MBF response data
    /// </summary>
    public class ButlerResponse
    {
        /// <summary>
        /// constructor
        /// </summary>
        public ButlerResponse()
        {
            TimeStampProcessingCompleted = String.Format("{0:o}", DateTime.Now.ToUniversalTime());
        }
        /// <summary>
        /// Message ID
        /// </summary>
        public Guid MessageId { get; set; }
        /// <summary>
        /// Configuration stoarge connection string
        /// </summary>
        public string StorageConnectionString { get; set; }
        /// <summary>
        /// Process type ID
        /// </summary>
        public string WorkflowName { get; set; }
        /// <summary>
        /// Mezzamine file list
        /// </summary>
        public List<string> MezzanineFiles { get; set; }
        /// <summary>
        /// Process log information
        /// </summary>
        public string Log { get; set; }
        /// <summary>
        /// Time submintion process
        /// </summary>
        public string TimeStampRequestSubmitted { get; set; }
        /// <summary>
        /// Time process started
        /// </summary>
        public string TimeStampProcessingStarted { get; set; }
        /// <summary>
        /// time process finish
        /// </summary>
        public string TimeStampProcessingCompleted { get; set; }

    }
}
