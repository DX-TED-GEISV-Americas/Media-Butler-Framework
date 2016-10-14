using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaButler.Common.Configuration
{
    /// <summary>
    /// Configuration Key instance process level inside dotControl file
    /// </summary>
    public class DotControlConfigKeys
    {
        /// <summary>
        /// Grid encode  profile list files
        /// </summary>
        public static string GridEncodeStepEncodeConfigList = "GridEncodeStep.encodeConfigList";
        /// <summary>
        /// Grid encode Media Processor name
        /// </summary>
        public static string GridEncodeStepMediaProcessorName = "GridEncodeStep.MediaProcessorName";
        /// <summary>
        /// Http notification Step GET on sucess finish
        /// </summary>
        public static string httpNotificationStepGetOnFinishUrl = "httpNotificationStep.GetOnFinishUrl";
        /// <summary>
        /// Http Notification Step POST on sucess finish
        /// </summary>
        public static string httpNotificationStepPostOnFinishUrl = "httpNotificationStep.PostOnFinishUrl";
        /// <summary>
        /// Primary file name for Ingest Multi Mezzamine File step
        /// </summary>
        public static string IngestMultiMezzamineFilesPrimaryFile = "myPrimaryFile";
        /// <summary>
        /// default Standar Encoding Profile Name
        /// </summary>
        public static string StandardEncodigProfileName = "encodigProfile";
        /// <summary>
        /// Index processor encode  profile list files
        /// </summary>
        public static string Index2EncodeStepEncodeConfigList = "Index2Preview.encodeConfigList";
        /// <summary>
        /// Index processor Name
        /// </summary>
        public static string Index2EncodeStepMediaProcessorName = "Index2Preview.MediaProcessorName";
        /// <summary>
        /// Index step copy subtitles file to original asset flag
        /// </summary>
        public static string Index2PreviewCopySubTitles = "Index2Preview.CopySubTitles";
    }
}
