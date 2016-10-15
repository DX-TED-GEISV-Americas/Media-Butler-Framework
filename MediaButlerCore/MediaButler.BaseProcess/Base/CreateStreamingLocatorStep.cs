using MediaButler.Common.Workflow;
using Microsoft.WindowsAzure.MediaServices.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaButler.BaseProcess
{
    /// <summary>
    /// Create AMS Streaming locaator step.
    /// </summary>
    class CreateStreamingLocatorStep : StepHandler
    {
        private ButlerProcessRequest myRequest;
        private CloudMediaContext _MediaServiceContext;

        /// <summary>
        /// Create Streminf locatior for Asset
        /// </summary>
        /// <param name="outputAssetid">AMS asset ID</param>
        /// <returns></returns>
        private ILocator CreateStreamingLocator(string outputAssetid)
        {
            IAssetFile assetFile = null;
            ILocator locator = null;

            var daysForWhichStreamingUrlIsActive = 365;
            var outputAsset = _MediaServiceContext.Assets.Where(a => a.Id == outputAssetid).FirstOrDefault();

            var accessPolicy = _MediaServiceContext.AccessPolicies.Create(
                outputAsset.Name
                , TimeSpan.FromDays(daysForWhichStreamingUrlIsActive)
                , AccessPermissions.Read);
            var assetFiles = outputAsset.AssetFiles.ToList();

            assetFile = assetFiles.Where(f => f.Name.ToLower().EndsWith(".ism")).FirstOrDefault();
            locator = _MediaServiceContext.Locators.CreateLocator(LocatorType.OnDemandOrigin, outputAsset, accessPolicy, DateTime.UtcNow.AddMinutes(-5));
            //Add Smooth URL to Metadata
            if (!myRequest.MetaData.ContainsKey("smoothurl"))
            {
                myRequest.MetaData.Add("smoothurl", locator.Path + assetFile.Name + "/manifest");
            }
            return locator;
        }

        /// <summary>
        /// Step execution to create AMS locator for current asset
        /// </summary>
        /// <param name="request"></param>
        public override void HandleExecute(ChainRequest request)
        {
            myRequest = (ButlerProcessRequest)request;
            _MediaServiceContext = new CloudMediaContext(myRequest.MediaAccountName, myRequest.MediaAccountKey);

            var locator = CreateStreamingLocator(myRequest.AssetId);
        }

        /// <summary>
        /// Compesantion step, no action only trace
        /// </summary>
        /// <param name="request"></param>
        public override void HandleCompensation(ChainRequest request)
        {
            Trace.TraceWarning("{0} in process {1} processId {2} has not HandleCompensation", this.GetType().FullName, myRequest.ProcessTypeId, myRequest.ProcessInstanceId);
        }
    }
}
