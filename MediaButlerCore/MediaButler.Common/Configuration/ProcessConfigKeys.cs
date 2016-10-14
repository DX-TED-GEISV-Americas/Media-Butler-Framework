using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaButler.Common.Configuration
{
    /// <summary>
    /// Configuration Key  process level
    /// </summary>
    public class ProcessConfigKeys
    {
        /// <summary>
        /// Default partition key
        /// </summary>
        public static string DefualtPartitionKey = "MediaButler.Common.workflow.ProcessHandler";
        /// <summary>
        /// Http callback default
        /// </summary>
        public static string MediaButlerHostHttpCallBackOnError = "HttpCallBackOnError";
        /// <summary>
        /// add to metadata url asset files on Sas locator step flag
        /// </summary>
        public static string CreateSasLocatorStepLogAllAssetFile = "CreateSasLocatorStep.LogAllAssetFile";
    }
}
