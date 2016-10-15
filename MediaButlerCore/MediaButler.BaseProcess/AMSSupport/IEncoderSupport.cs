using MediaButler.Common.ResourceAccess;
using Microsoft.WindowsAzure.MediaServices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaButler.BaseProcess.AMSSupport
{
    /// <summary>
    /// Azure Media Services encoder support
    /// </summary>
    public interface IEncoderSupport
    {
        /// <summary>
        /// Get last Processor by name
        /// </summary>
        /// <param name="mediaProcessorName"></param>
        /// <returns></returns>
        IMediaProcessor GetLatestMediaProcessorByName(string mediaProcessorName);

        /// <summary>
        /// Event transcoding process change status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void StateChanged(object sender, JobStateChangedEventArgs e);

        /// <summary>
        /// Wait for transcoding porcess ends
        /// </summary>
        /// <param name="jobId">transcoding JOB ID</param>
        void WaitJobFinish(string jobId);

        /// <summary>
        /// Job update event call back
        /// </summary>
        event EventHandler JobUpdate;

        /// <summary>
        /// Job error event call back
        /// </summary>
        event EventHandler OnJobError;

        /// <summary>
        /// Get AMS Job by Job ID
        /// </summary>
        /// <param name="jobId">job id</param>
        /// <returns></returns>
        IJob GetJob(string jobId);

        /// <summary>
        /// Get AMS JOb by anme
        /// </summary>
        /// <param name="JobName">AMS job name</param>
        /// <returns></returns>
        IJob GetJobByName(string JobName);

        /// <summary>
        /// Create AMS asset
        /// </summary>
        /// <param name="AssetName">Name</param>
        /// <param name="blobUrl">blob video URL</param>
        /// <param name="MediaStorageConn">AMS connection</param>
        /// <param name="StorageConnectionString">AMs stoarge connection</param>
        /// <param name="WorkflowName">Process type ID</param>
        /// <returns>New Asset</returns>
        IAsset CreateAsset(string AssetName, string blobUrl, string MediaStorageConn, string StorageConnectionString, string WorkflowName);

        /// <summary>
        /// Load transcoding profile base on process type and process type configuration
        /// </summary>
        /// <param name="profileInfo">transcoding profile information</param>
        /// <param name="ProcessConfigConn">Process configuration key value</param>
        /// <returns></returns>
        string LoadEncodeProfile(string profileInfo, string ProcessConfigConn);

        /// <summary>
        /// Set primary asset file
        /// </summary>
        /// <param name="MyAsset">Asset</param>
        /// <param name="theAssetFile">file</param>
        void SetPrimaryFile(IAsset MyAsset, IAssetFile theAssetFile);
        /// <summary>
        /// Execute transcoding JOB in grid mode.
        /// </summary>
        /// <param name="OutputAssetsName">Output asset name</param>
        /// <param name="JobName">Job Name</param>
        /// <param name="MediaProcessorName">AMS Media processor Name</param>
        /// <param name="EncodingConfiguration">transcoding configuration list</param>
        /// <param name="TaskNameBase">prefixTask name</param>
        /// <param name="AssetId">AMS asset ID</param>
        /// <param name="OnJob_Error">Call back on error</param>
        /// <param name="OnJob_Update">Call back on update</param>
        /// <returns></returns>
        IJob ExecuteGridJob(string OutputAssetsName, string JobName, string MediaProcessorName, string[] EncodingConfiguration, string TaskNameBase, string AssetId, EventHandler OnJob_Error, EventHandler OnJob_Update);

        /// <summary>
        /// Load list transcoding profiles
        /// </summary>
        /// <param name="dotControlData">key value dot control configuration</param>
        /// <param name="processData">key value process type configuration</param>
        /// <param name="MezzanineFiles">file list</param>
        /// <param name="ProcessConfigConn">storage configuration conection</param>
        /// <param name="StepConfiguration">step configuration data</param>
        /// <returns></returns>
        string[] GetLoadEncodignProfiles(IjsonKeyValue dotControlData, IjsonKeyValue processData, List<string> MezzanineFiles, string ProcessConfigConn, string StepConfiguration);

        /// <summary>
        /// Get AMS processor by name base on key value configuration process or dot control level
        /// </summary>
        /// <param name="dotControlData">key value dot control configuration</param>
        /// <param name="processData">key value process configuration</param>
        /// <returns></returns>
        string GetMediaProcessorName(IjsonKeyValue dotControlData, IjsonKeyValue processData);
    }
}
