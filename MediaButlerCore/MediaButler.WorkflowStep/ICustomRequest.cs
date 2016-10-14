using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaButler.WorkflowStep
{
    /// <summary>
    /// Workflow Custom request
    /// </summary>
    public interface ICustomRequest
    {
        /// <summary>
        /// Media Services Asset ID
        /// </summary>
        string AssetId { get; set; }
        /// <summary>
        /// Media Services Account Name
        /// </summary>
        string MediaAccountName { get; set; }
        /// <summary>
        /// Media Services Account Key
        /// </summary>
        string MediaAccountKey { get; set; }
        /// <summary>
        /// Media services Storage connection string
        /// </summary>
        string MediaStorageConn { get; set; }
        /// <summary>
        /// Step configuration data
        /// </summary>
        string ConfigData { get; set; }
        /// <summary>
        /// Process start time
        /// </summary>
        DateTime TimeStampProcessingStarted { get; set; }
        /// <summary>
        /// Process LOG
        /// </summary>
        List<string> Log { get; set; }
        /// <summary>
        /// Process meta data
        /// </summary>
        Dictionary<string, string> MetaData { get; set; }
        /// <summary>
        /// Process tyoe ID
        /// </summary>
        string ProcessTypeId { get; set; }
        /// <summary>
        /// Instance process ID
        /// </summary>
        string ProcessInstanceId { get; set; }
        /// <summary>
        /// Process exceptions list
        /// </summary>
        List<string> Exceptions { get; set; }
        /// <summary>
        /// MBF process configuration storage connection string
        /// </summary>
        string ProcessConfigConn { get; set; }
        /// <summary>
        /// MBF process instance control file URL
        /// </summary>
        string ButlerRequest_ControlFileUri { get; set; }
        /// <summary>
        /// 
        /// </summary>
        List<string> ButlerRequest_MezzanineFiles { get; set; }
        /// <summary>
        /// Step configuraton data
        /// </summary>
        string StepConfiguration { get; set; }
    }
}
