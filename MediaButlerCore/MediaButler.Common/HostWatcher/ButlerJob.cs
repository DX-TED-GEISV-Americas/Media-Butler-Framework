using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaButler.Common.Configuration;


namespace MediaButler.Common.WatcherWatcher
{
    /// <summary>
    /// MBF watcher job information
    /// </summary>
    public class ButlerJob
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ButlerJob()
        {
            JobId = Guid.NewGuid();
        }
        /// <summary>
        /// JOB ID
        /// </summary>
        public Guid JobId { get; set; }
        /// <summary>
        /// Media files list
        /// </summary>
        public IList<Uri> JobMediaFiles { get; set; }
        /// <summary>
        /// JOB control file
        /// </summary>
        public Uri JobControlFile { get; set; }
        /// <summary>
        /// Workflow current status
        /// </summary>
        public Configuration.MBFConfiguration.WorkflowStatus Status { get; set; }
        /// <summary>
        /// contains information to dump in the .log file
        /// </summary>
        public string Information { get; set; }
        /// <summary>
        /// Simple job is indicated by a lack of control file
        /// </summary>
        public bool IsSimpleJob
        {
            get { return JobControlFile == null; }
        }
    }
}
