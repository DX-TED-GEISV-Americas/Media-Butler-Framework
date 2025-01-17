﻿using MediaButler.Common.Workflow;
using MediaButler.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.MediaServices.Client;
using MediaButler.BaseProcess.AMSSupport;
using System.Diagnostics;

namespace MediaButler.BaseProcess
{
    /// <summary>
    /// Step to create the message back to watchfolder process, 
    /// and it will move files from porcessing to success container.
    /// </summary>
    class SendMessageBackStep : StepHandler
    {
        private ButlerProcessRequest myRequest;

        /// <summary>
        /// Serialize Asset info and send message to success queue
        /// </summary>
        private void SendMessage()
        {
            string qName = MBFConfiguration.ButlerSuccessQueue;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(myRequest.ProcessConfigConn);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            //TODO: queue info
            CloudQueue queue = queueClient.GetQueueReference(qName);
            ButlerResponse myButlerResponse = new ButlerResponse();

            myButlerResponse.MezzanineFiles = myRequest.ButlerRequest.MezzanineFiles;
            //Add to Mezzamine Files the control File URL if it exist
            //Becouse it is needed to move/delete the control file from processing to succes or fail
            if (!string.IsNullOrEmpty(myRequest.ButlerRequest.ControlFileUri))
            {
                myButlerResponse.MezzanineFiles.Add(myRequest.ButlerRequest.ControlFileUri);
            }
            myButlerResponse.TimeStampProcessingCompleted = DateTime.Now.ToString();
            myButlerResponse.TimeStampProcessingStarted = myRequest.TimeStampProcessingStarted.ToString();
            myButlerResponse.WorkflowName = myRequest.ProcessTypeId;
            myButlerResponse.MessageId = myRequest.ButlerRequest.MessageId;
            myButlerResponse.TimeStampRequestSubmitted = myRequest.ButlerRequest.TimeStampUTC;
            myButlerResponse.StorageConnectionString = myRequest.ButlerRequest.StorageConnectionString;




            CloudMediaContext _MediaServiceContext = new CloudMediaContext(myRequest.MediaAccountName, myRequest.MediaAccountKey);
            IAsset x = _MediaServiceContext.Assets.Where(xx => xx.Id == myRequest.AssetId).FirstOrDefault();
            AssetInfo ai = new AssetInfo(x);
            StringBuilder AssetInfoResume = ai.GetStatsTxt();

            AssetInfoResume.AppendLine("");
            AssetInfoResume.AppendLine("Media Butler Process LOG " + DateTime.Now.ToString());
            foreach (string txt in myRequest.Log)
            {
                AssetInfoResume.AppendLine(txt);

            }
            AssetInfoResume.AppendLine("-----------------------------");
            myButlerResponse.Log = AssetInfoResume.ToString();


            CloudQueueMessage responseMessae = new CloudQueueMessage(Newtonsoft.Json.JsonConvert.SerializeObject(myButlerResponse));
            queue.AddMessage(responseMessae);
            Trace.TraceInformation("Return Butler Message sent to queue");
        }

        /// <summary>
        /// Execute step
        /// </summary>
        /// <param name="request"></param>
        public override void HandleExecute(ChainRequest request)
        {
            myRequest = (ButlerProcessRequest)request;
            //Send output info back using ButlerResponse Message LOG
            SendMessage();
        }

        /// <summary>
        /// Step compensation on errror, delete asset
        /// </summary>
        /// <param name="request"></param>
        public override void HandleCompensation(ChainRequest request)
        {
            Trace.TraceWarning("{0} in process {1} processId {2} has not HandleCompensation", this.GetType().FullName, myRequest.ProcessTypeId, myRequest.ProcessInstanceId);


        }
    }
}
