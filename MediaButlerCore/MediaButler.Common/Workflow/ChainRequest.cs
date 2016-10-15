using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaButler.Common.Workflow
{
    /// <summary>
    /// Base Chain request, general propose
    /// </summary>
    public class ChainRequest
    {
        /// <summary>
        /// Current step
        /// </summary>
        internal int CurrentStepIndex { get; set; }
        /// <summary>
        /// Process Type ID
        /// </summary>
        public string ProcessTypeId { get; set; }
        /// <summary>
        /// Exceptions list in the process steps
        /// </summary>
        public string ProcessInstanceId { get; set; }
        /// <summary>
        /// Process Exception list
        /// </summary>
        public List<string> Exceptions;
        /// <summary>
        /// Request constructor
        /// </summary>
        public ChainRequest()
        {
            Exceptions = new List<string>();
            BreakChain = false;
            IsResumeable = false;
        }
        /// <summary>
        /// Flag for Stop the process
        /// </summary>
        public bool BreakChain
        {
            get;
            set;
        }
        /// <summary>
        /// Is the process resumeable
        /// </summary>
        public bool IsResumeable { get; set; }
        /// <summary>
        /// Process configuration stoarge conneciton string
        /// </summary>
        public string ProcessConfigConn { get; set; }
        /// <summary>
        /// Dispose here all the Request resource, for example TASK running in parallel.
        /// </summary>
        public virtual void DisposeRequest()
        { }
    }
}
