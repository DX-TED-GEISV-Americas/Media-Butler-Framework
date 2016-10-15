using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaButler.Common.Workflow
{
    /// <summary>
    /// MBF request information
    /// </summary>
    public class ButlerRequest
    {
        /// <summary>
        /// Message GUID ID
        /// </summary>
        public Guid MessageId { get; set; }
        /// <summary>
        /// MBF storage account connection string
        /// </summary>
        public string StorageConnectionString { get; set; }
        /// <summary>
        /// Process type ID
        /// </summary>
        public string WorkflowName { get; set; }
        /// <summary>
        /// Mezzamine files list
        /// </summary>
        public List<string> MezzanineFiles { get; set; }
        /// <summary>
        /// Dot control file URI
        /// </summary>
        public string ControlFileUri { get; set; }
        /// <summary>
        /// Process instance start time
        /// </summary>
        public string TimeStampUTC { get; set; }
    }
}
