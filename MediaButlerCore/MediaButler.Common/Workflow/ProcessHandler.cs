using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaButler.Common.Workflow
{
    /// <summary>
    /// MBF process handler
    /// Manage the process instance
    /// </summary>
    public class ProcessHandler
    {
        /// <summary>
        /// Multi threading control lock
        /// </summary>
        private object myLock = new object();

        /// <summary>
        /// Internal # of process running
        /// </summary>
        private int currentProcessRunning = 0;
        
        /// <summary>
        /// Process configuration storage connection
        /// </summary>
        private string myProcessConfigConn;

        /// <summary>
        /// get # current process running
        /// </summary>
        public int CurrentProcessRunning
        {
            get
            {
                lock (myLock)
                {
                    return currentProcessRunning;
                }
            }
        }

       
        /// <summary>
        /// Read configuration for configuration table but return "" if the row don't exist
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string ReadConfigOrDefault(string Key)
        {
            string config = "";
            try
            {
                config = Configuration.MBFConfiguration.GetConfigurationValue(Key, MediaButler.Common.Configuration.MBFConfiguration.ProcessHandlerConfigKey, myProcessConfigConn);
            }
            catch (Exception)
            {
                string txt = string.Format("ProcessHandler try to read {0} but it is not in configuration table! at {1} ", Key, DateTime.Now.ToString());
                Trace.TraceInformation(txt);
            }
            return config;
        }
        
        /// <summary>
        /// Load Process request base on configuration
        /// </summary>
        /// <param name="processTypeId">process type ID</param>
        /// <returns>process request</returns>
        private ProcessRequest GetCurrentContext(string processTypeId)
        {
            //TODO> defaul request 
            ProcessRequest currentContext;
            stepTypeInfo x = null;
            string jsonContext = "";// = ReadConfigOrDefault(processTypeId + ".Context");
            if (string.IsNullOrEmpty(jsonContext))
            {
                //default Context
                jsonContext = "{\"AssemblyName\":\"MediaButler.Common.dll\",\"TypeName\":\"MediaButler.Common.Workflow.ButlerProcessRequest\",\"ConfigKey\":\"\"}";
            }
            x = Newtonsoft.Json.JsonConvert.DeserializeObject<stepTypeInfo>(jsonContext);
            currentContext = (ProcessRequest)Activator.CreateComInstanceFrom(x.AssemblyName, x.TypeName).Unwrap();
            if ((x.ConfigKey != null) && (x.ConfigKey != ""))
            {
                //loadXMLConfig Step
                currentContext.ConfigData = ReadConfigOrDefault(x.ConfigKey);
            }
            return currentContext;
        }

        /// <summary>
        /// Build process step chaing base on process type configuration
        /// </summary>
        /// <param name="processTypeId"></param>
        /// <returns></returns>
        private List<StepHandler> BuildChain(string processTypeId)
        {
            StepHandler prevStep = null;

            List<StepHandler> auxSteps = new List<StepHandler>();
            string jsonTxt;
            try
            {
                jsonTxt = ReadConfigOrDefault(processTypeId + ".ChainConfig");
                if (string.IsNullOrEmpty(jsonTxt))
                    throw new Exception(processTypeId + " Not Found, check ButlerConfiguration Table");
            }
            catch (Exception X)
            {

                throw new Exception("[Error at BuildChain] Process " + X.Message);
            }

            //Sensible config manually
            List<stepTypeInfo> StepList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<stepTypeInfo>>(jsonTxt);

            foreach (stepTypeInfo item in StepList)
            {
                //Build the chain
                //1. is the Assembly in bin?
                if (!File.Exists(item.AssemblyName))
                {
                    //try to download from storage
                    try
                    {

                        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(myProcessConfigConn);
                        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                        CloudBlobContainer container = blobClient.GetContainerReference("mediabutlerbin");

                        //TODO: fix this is DLL exis and is it on use
                        foreach (IListBlobItem dll in container.ListBlobs(null, false))
                        {

                            Uri myUri = dll.Uri;
                            int seg = myUri.Segments.Length - 1;
                            string name = myUri.Segments[seg];
                            CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);
                            using (var fileStream = System.IO.File.OpenWrite(@".\" + name))
                            {
                                blockBlob.DownloadToStream(fileStream);
                            }

                        }
                        if (!File.Exists(item.AssemblyName))
                        {
                            throw new Exception(item.AssemblyName + " don't exist");
                        }
                    }
                    catch (Exception X)
                    {
                        string txt = string.Format("[{0}] Error BuildChain  Assembly {1} error: {2}", this.GetType().FullName, item.AssemblyName, X.Message);

                        Trace.TraceError(txt);
                        throw X;
                    }

                }

                StepHandler obj = (StepHandler)Activator.CreateComInstanceFrom(item.AssemblyName, item.TypeName).Unwrap();
                if ((item.ConfigKey != null) && (item.ConfigKey != ""))
                {
                    //LOAD STRING CONFIGURATION FOR CONFIG TABLE

                    obj.StepConfiguration = this.ReadConfigOrDefault(item.ConfigKey + ".StepConfig");
                }
                auxSteps.Add(obj);

                if (prevStep != null)
                {
                    prevStep.SetSuccessor(obj);
                }
                prevStep = obj;

            }
            return auxSteps;
        }
        
        /// <summary>
        /// Get proccess instance configuration storage connection
        /// </summary>
        /// <param name="ProcessConfigConn"></param>
        public ProcessHandler(string ProcessConfigConn)
        {
            myProcessConfigConn = ProcessConfigConn;
        }
        
        /// <summary>
        /// Get process instance ID base on control file or message ID
        /// </summary>
        /// <param name="ContolFileUri">control file URL</param>
        /// <param name="messageID">message ID</param>
        /// <returns></returns>
        public string getProcessId(string ContolFileUri, string messageID)
        {
            string xID = null;
            if (string.IsNullOrEmpty(ContolFileUri))
            {
                xID = messageID;
            }
            else
            {
                //get Blob conatier guid
                Uri xFile = new Uri(ContolFileUri);
                string aux = xFile.Segments[3];
                xID = aux.Substring(0, aux.Length - 1);
            }
            return xID;
        }
       
        /// <summary>
        /// Load or create process request from snapshot table
        /// </summary>
        /// <param name="currentRequest">current reequest</param>
        /// <param name="currentSteps">Steps list</param>
        /// <returns></returns>
        private ProcessRequest restoreRequestOrCreateMetadata(ProcessRequest currentRequest, List<StepHandler> currentSteps)
        {
            Common.ResourceAccess.IButlerStorageManager storageManager = Common.ResourceAccess.BlobManagerFactory.CreateBlobManager(currentRequest.ProcessConfigConn);
            var processSnap = storageManager.readProcessSanpShot(currentRequest.ProcessTypeId, currentRequest.ProcessInstanceId);
            if (processSnap == null)
            {
                //First time Execution
                string workflowStepListData = Newtonsoft.Json.JsonConvert.SerializeObject(currentSteps);
                currentRequest.MetaData.Add(Configuration.MBFConfiguration.workflowStepListKey, workflowStepListData);
                currentRequest.MetaData.Add(Configuration.MBFConfiguration.workflowStepLength, currentSteps.Count.ToString());
            }
            else
            {
                //Second or other time execution
                //load Metadata
                dynamic dynObj = Newtonsoft.Json.JsonConvert.DeserializeObject((processSnap).jsonContext);
                //Dictionary<string, string> dynMetaData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>((dynObj.MetaData.ToString()));
                currentRequest.MetaData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>((dynObj.MetaData.ToString()));
                //load Errors
                currentRequest.Exceptions = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>((dynObj.Exceptions.ToString()));
                //Load LOGS Log
                currentRequest.Log = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>((dynObj.Log.ToString()));
            }
            return currentRequest;
        }
       
        /// <summary>
        /// Execute process
        /// </summary>
        /// <param name="currentMessage">information message</param>
        private void execute(CloudQueueMessage currentMessage)
        {
            ProcessRequest myRequest = null;
            string txt;
            Common.ResourceAccess.IButlerStorageManager storageManager = null;

            try
            {
                lock (myLock)
                {
                    currentProcessRunning += 1;
                }
                ButlerRequest watcherRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<ButlerRequest>(currentMessage.AsString);
                //Load Workflow's steps
                List<StepHandler> mysteps = BuildChain(watcherRequest.WorkflowName);

                myRequest = GetCurrentContext(watcherRequest.WorkflowName);
                
                myRequest.CurrentMessage = currentMessage;
                myRequest.ProcessTypeId = watcherRequest.WorkflowName;
                //ProcessInstanceId:
                //Single File: MessageID Guid (random)
                //multiFile package: Container folder guid ID (set for client)
                myRequest.ProcessInstanceId = this.getProcessId(watcherRequest.ControlFileUri, watcherRequest.MessageId.ToString());

                myRequest.ProcessConfigConn = this.myProcessConfigConn;
                myRequest.IsResumeable = (this.ReadConfigOrDefault(myRequest.ProcessTypeId + ".IsResumeable") == "1");

                //Restore Status
                storageManager = Common.ResourceAccess.BlobManagerFactory.CreateBlobManager(myRequest.ProcessConfigConn);
                myRequest = restoreRequestOrCreateMetadata(myRequest, mysteps);

                //2.Execute Chain
                txt = string.Format("[{0}] Starting new Process, type {1} and ID {2}", this.GetType().FullName, myRequest.ProcessTypeId, myRequest.ProcessInstanceId);
                Trace.TraceInformation(txt);
                mysteps.FirstOrDefault().HandleRequest(myRequest);
                //FinishProcess();
                txt = string.Format("[{0}] Finish Process, type {1} and ID {2}", this.GetType().FullName, myRequest.ProcessTypeId, myRequest.ProcessInstanceId);
                Trace.TraceInformation(txt);
                //Finish Status
                myRequest.CurrentStepIndex = Configuration.MBFConfiguration.successFinishProcessStep;
                storageManager.PersistProcessStatus(myRequest);

                lock (myLock)
                {
                    currentProcessRunning -= 1;
                }
            }
            catch (Exception xxx)
            {
                if (myRequest != null)
                {
                    //foreach (Exception item in myRequest.Exceptions)
                    foreach (string errorTxt in myRequest.Exceptions)
                    {
                        //Full Rollback?
                        txt = string.Format("[{0}] Error list process {1} intance {2} error: {3}", this.GetType().FullName, myRequest.ProcessTypeId, myRequest.ProcessInstanceId, errorTxt);
                        Trace.TraceError(txt);
                    }
                    txt = string.Format("[{0}] Error list process {1} intance {2} error: {3}", this.GetType().FullName, myRequest.ProcessTypeId, myRequest.ProcessInstanceId, xxx.Message);
                    myRequest.Exceptions.Add(txt);

                    //Update Status
                    myRequest.CurrentStepIndex = Configuration.MBFConfiguration.failFinishProcessStep;
                    storageManager.PersistProcessStatus(myRequest);
                }
                else
                {
                    txt = string.Format("[{0}] Error {1} without context Request yet", this.GetType().FullName, xxx.Message);
                    Trace.TraceError(xxx.Message);
                }
                //Exception no Managed

                lock (myLock)
                {
                    currentProcessRunning -= 1;
                }

            }
            //3.return control
            if (myRequest != null)
            {
                myRequest.DisposeRequest();
            }
            else
            {
                Trace.TraceError("myRequest is null raw message " + currentMessage.AsString);

            }
            myRequest = null;

            Trace.Flush();
        }
        
        /// <summary>
        /// Execute process single or multi thread 
        /// </summary>
        /// <param name="currentMessage">information message</param>
        public void Execute(CloudQueueMessage currentMessage)
        {
            if (Configuration.MBFConfiguration.GetConfigurationValue("IsMultiTask", "MediaButler.Common.workflow.ProcessHandler", myProcessConfigConn) == "1")
            {
                Task xTask = Task.Factory.StartNew(() =>
                {
                    this.execute(currentMessage);
                }
                 );
            }
            else
            {
                this.execute(currentMessage);
            }
        }
    }

}
