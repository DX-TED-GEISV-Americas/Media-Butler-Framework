﻿using MediaButler.Common.Workflow;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediaButler.Common.Configuration;

namespace MediaButler.Common.WatcherWatcher
{
    /// <summary>
    /// MBF watcher JOB manager
    /// </summary>
    public static partial class JobManager
    {
        /// <summary>
        /// Create a job object for a multipart job -- one that consists of 
        /// multiple mezannine files.
        /// </summary>
        /// <param name="mediaAssets">The list of mezannine Assets</param>
        /// <param name="controlFile">Butler control file.</param>
        /// <returns>the new Butler job object</returns>
        public static ButlerJob CreateJob(IList<Uri> mediaAssets, Uri controlFile)
        {
            var b = new ButlerJob()
            {
                JobControlFile = controlFile,
                JobMediaFiles = mediaAssets,
            };
            return b;
        }

        /// <summary>
        /// Create a Butler job object for the case of a single mezannine
        /// file.  No control file provided.
        /// </summary>
        /// <param name="mediaAsset">The Uri of the single mezannine file (e.g.,
        /// for .../Incoming/myVid.mp4).</param>
        /// <returns></returns>
        public static ButlerJob CreateSimpleJob(Uri mediaAsset)
        {
            var b = new ButlerJob()
            {
                JobControlFile = null,
                JobMediaFiles = new List<Uri>() { mediaAsset }
            };
            return b;
        }

        /// <summary>
        /// Submit this job to the Butler Request queue
        /// </summary>
        /// <param name="j">Job to submit</param>
        /// <returns>the Guid of the JOB if submission was successful</returns>
        public static Guid Submit(ButlerJob j, string storageAccountString)
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(storageAccountString);
            CloudQueueClient sendQueueClient = account.CreateCloudQueueClient();
            try
            {
                CloudQueue sendQueue = sendQueueClient.GetQueueReference(Configuration.MBFConfiguration.ButlerSendQueue);
                sendQueue.CreateIfNotExists();

                var message = new ButlerRequest
                {
                    MessageId = j.JobId,
                    MezzanineFiles = new List<string> { },
                    StorageConnectionString = storageAccountString,
                    WorkflowName = "",
                    TimeStampUTC = String.Format("{0:o}", DateTime.Now.ToUniversalTime()),
                    ControlFileUri = ""
                };
                if (j.JobMediaFiles.Count > 0)
                {
                    var blob = new CloudBlockBlob(j.JobMediaFiles[0]);
                    message.WorkflowName = blob.Container.Name;
                }
                foreach (Uri blobUri in j.JobMediaFiles)
                {
                    message.MezzanineFiles.Add(blobUri.ToString());
                }

                if (j.JobControlFile != null)
                {
                    message.ControlFileUri = j.JobControlFile.ToString();
                    if (message.WorkflowName == "")
                    {
                        //Process with only ControlFile
                        try
                        {
                            Uri controlUri = new Uri(message.ControlFileUri);
                            message.WorkflowName = controlUri.Segments[1].Substring(0, controlUri.Segments[1].Length - 1);
                        }
                        catch (Exception)
                        {

                        }


                    }
                }

                CloudQueueMessage butlerRequestMessage = new CloudQueueMessage(JsonConvert.SerializeObject(message));
                sendQueue.AddMessageAsync(butlerRequestMessage);
            }
            catch (Exception)
            {

                throw;
            }

            return j.JobId;
        }

        /// <summary>
        /// Read response from message
        /// </summary>
        /// <param name="txtMsg">message text</param>
        /// <returns></returns>
        private static ButlerResponse DeserializeRsponseMessage(string txtMsg)
        {
            ButlerResponse messageResponse = null;
            try
            {
                messageResponse = JsonConvert.DeserializeObject<ButlerResponse>(txtMsg);
            }
            catch (Exception)
            {


            }

            return messageResponse;
        }
        
        /// <summary>
        /// Send message to dead letter queue 
        /// </summary>
        /// <param name="errorFrendlyMessage">Error message for LOG</param>
        /// <param name="deadLetterQueueClient">Dead letter Queue </param>
        /// <param name="poisonMessage">the message with problem</param>
        private static void SendMessageToDeadLetterQueue(string errorFrendlyMessage, CloudQueueClient deadLetterQueueClient, CloudQueueMessage poisonMessage)
        {
            CloudQueue ButlerSuccessQueueDeadLetter = deadLetterQueueClient.GetQueueReference(Configuration.MBFConfiguration.ButlerResponseDeadLetter);
            ButlerSuccessQueueDeadLetter.CreateIfNotExists();
            ButlerSuccessQueueDeadLetter.AddMessage(poisonMessage);
            Trace.TraceError(errorFrendlyMessage);
        }
       
        /// <summary>
        /// Process message Back fro success and Fial queues
        /// </summary>
        /// <param name="storageAccountString">Storage account</param>
        /// <param name="queueName">Queue Name</param>
        /// <param name="status">Workflow status</param>
        private static void processMessageBack(string storageAccountString, string queueName, Configuration.MBFConfiguration.WorkflowStatus status)
        {
            string erroMsg;
            //Trace.TraceInformation("reading from workflow results queue: " + status.ToString());
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountString);
            CloudQueueClient inQueueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue inQueue = inQueueClient.GetQueueReference(queueName);
            inQueue.CreateIfNotExists();
            CloudQueueMessage retrievedMessage = inQueue.GetMessage();
            if (retrievedMessage != null)
            {
                try
                {
                    if (retrievedMessage.DequeueCount > Configuration.MBFConfiguration.maxDequeueCount)
                    {
                        //Poison Message
                        erroMsg = String.Format("[0]Error: Max DequeueCount Messsage : {1}", "getWorkflowOperations_" + status.ToString(), retrievedMessage.AsString);
                        SendMessageToDeadLetterQueue(erroMsg, inQueueClient, retrievedMessage);
                    }
                    else
                    {
                        ButlerResponse messageResponse = DeserializeRsponseMessage(retrievedMessage.AsString);
                        if (messageResponse != null)
                        {
                            //Process the message
                            Configuration.MBFConfiguration.WorkflowStatus messageStatus = status;
                            processFilesFromResponse(messageResponse, messageStatus, storageAccount);
                        }
                        else
                        {
                            //Message bad format ccopu to deadletter 
                            erroMsg = String.Format("[0]Error: invalid Response message : {1}", "getWorkflowOperations_" + status.ToString(), retrievedMessage.AsString);
                            SendMessageToDeadLetterQueue(erroMsg, inQueueClient, retrievedMessage);
                        }
                    }
                    // TODO: updateQueuemessage if it is taking longer
                    //Process the message in less than 30 seconds, and then delete the message
                    inQueue.DeleteMessage(retrievedMessage);
                }
                catch (Exception X)
                {
                    //Trace the error but don't break the proccess or delete the message.
                    //dequeue control is running too.
                    Trace.TraceError(string.Format("[{0}] Error: {1}", "getWorkflowOperations_" + status.ToString(), X.Message));
                }
            }


        }

        /// <summary>
        /// reading sucess messages task
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="storageAccountString"></param>
        /// <returns></returns>
        public static async Task getWorkflowSuccessOperations(CancellationToken ct, string storageAccountString)
        {
            var pollingInterval = TimeSpan.FromSeconds(Configuration.MBFConfiguration.SuccessQueuePollingInterval);
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    // Wake up and do some background processing if not canceled.      
                    processMessageBack(storageAccountString, Configuration.MBFConfiguration.ButlerSuccessQueue, Configuration.MBFConfiguration.WorkflowStatus.Finished);
                    // Go back to sleep for a period of time unless asked to cancel.      
                    // Task.Delay will throw an OperationCanceledException when canceled.      
                    await Task.Delay(pollingInterval, ct);
                }
            }
            catch (OperationCanceledException ocEx)
            {
                // Expect this exception to be thrown in normal circumstances or check    
                // the cancellation token. If the role instances are shutting down, a    
                // cancellation request will be signaled.    
                Trace.TraceInformation("Stopping service, cancellation requested");

                // Re-throw the Operation cancellation exception    
                throw ocEx;
            }
        }

        /// <summary>
        /// reading faild messahe task
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="storageAccountString"></param>
        /// <returns></returns>
        public static async Task getWorkflowFailedOperations(CancellationToken ct, string storageAccountString)
        {
            ButlerJob job = new ButlerJob();
            var pollingInterval = TimeSpan.FromSeconds(MBFConfiguration.FailedQueuePollingInterval);
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    // Wake up and do some background processing if not canceled.      
                    processMessageBack(storageAccountString, MBFConfiguration.ButlerFailedQueue, Configuration.MBFConfiguration.WorkflowStatus.Failed);
                    // Go back to sleep for a period of time unless asked to cancel.      
                    // Task.Delay will throw an OperationCanceledException when canceled.      
                    await Task.Delay(pollingInterval, ct);
                }
            }
            catch (OperationCanceledException ocEx)
            {
                // Expect this exception to be thrown in normal circumstances or check    
                // the cancellation token. If the role instances are shutting down, a    
                // cancellation request will be signaled.    
                Trace.TraceInformation("Stopping service, cancellation requested");

                // Re-throw the Operation cancellation exception    
                throw ocEx;
            }
        }

        /// <summary>
        /// process mezzamine files after process finish on stoarge 
        /// </summary>
        /// <param name="jobResponse"></param>
        /// <param name="jobStatus"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        private static bool processFilesFromResponse(ButlerResponse jobResponse, MBFConfiguration.WorkflowStatus jobStatus, CloudStorageAccount account)
        {
            bool returnValue = true;
            CloudBlockBlob baseBlob = new CloudBlockBlob(new Uri(jobResponse.MezzanineFiles[0]), account.Credentials);
            CloudBlobContainer container = baseBlob.Container;
            string directoryTo = (jobStatus == Configuration.MBFConfiguration.WorkflowStatus.Failed) ? Configuration.MBFConfiguration.DirectoryFailed : Configuration.MBFConfiguration.DirectoryCompleted;
            string timestampFileAppend = (string.IsNullOrEmpty(jobResponse.TimeStampProcessingStarted)) ? "" : jobResponse.TimeStampProcessingStarted.Replace(':', '-');
            // fix: substitute / with - in date to avoid file being treated as series of dirs
            timestampFileAppend = timestampFileAppend.Replace('/', '-');
            try
            {
                foreach (string videoUri in jobResponse.MezzanineFiles)
                {
                    CloudBlockBlob fileToMove = new CloudBlockBlob(new Uri(videoUri), account.Credentials);
                    var blobContinuationToken = new BlobContinuationToken();

                    string blobTarget = BlobUtilities.AdjustPath(fileToMove, directoryTo);
                    int trimEnd = blobTarget.LastIndexOf('.');
                    string blobTargetFileExt = blobTarget.Substring(trimEnd, blobTarget.Length - trimEnd);
                    blobTarget = string.Concat(blobTarget, ".", timestampFileAppend, ".", blobTargetFileExt);
                    BlobUtilities.RenameBlobWithinContainer(container, BlobUtilities.ExtractBlobPath(fileToMove), blobTarget);
                }


                // write log file
                string blobUriString = BlobUtilities.AdjustPath(baseBlob, directoryTo);
                // remove file ext
                // append .log
                blobUriString = string.Concat(blobUriString, ".", timestampFileAppend, ".log");
                CloudBlockBlob logBlob = container.GetBlockBlobReference(blobUriString);
                logBlob.Properties.ContentType = "text/plain";
                logBlob.UploadText(jobResponse.Log);

            }
            catch (Exception)
            {
                throw;
            }


            return returnValue;
        }
    }
}
