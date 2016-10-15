using MediaButler.Common.Workflow;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaButler.BaseProcess
{
    /// <summary>
    /// Control Step to keep inbound message hiden until process instance finish
    /// </summary>
    class MessageHiddeControlStep : StepHandler
    {
        int timeSpanMessage;
        int sleepSeconds;

        /// <summary>
        /// Constant setup
        /// </summary>
        public void Setup()
        {
            timeSpanMessage = 30;
            sleepSeconds = 10;

        }

        /// <summary>
        /// Hidden message on process start or realease message at process ends
        /// </summary>
        /// <param name="request"></param>
        public override void HandleExecute(ChainRequest request)
        {
            ButlerProcessRequest myRequest = (ButlerProcessRequest)request;
            Setup();
            if (myRequest.MessageHiddenTaskStatus == TaskStatus.WaitingToRun)
            {
                //Start to hidden the Process trgigger Message
                myRequest.StartMessageHidden(timeSpanMessage, sleepSeconds);
                Trace.TraceInformation("{0}Process type {1} instance {2} start hidden Message {3}",
                    this.GetType().FullName,
                    myRequest.ProcessTypeId,
                    myRequest.ProcessInstanceId,
                    myRequest.ButlerRequest.MessageId);
            }
            else
            {
                //Hidden is running, now Stop it
                myRequest.StopMessageHidden();
                do
                {
                    System.Threading.Thread.Sleep(1 * 1000);

                } while (myRequest.MessageHiddenTaskStatus != TaskStatus.RanToCompletion);


                Trace.TraceInformation("{0}Process type {1} instance {2} stop hidden Message {3}",
                    this.GetType().FullName,
                    myRequest.ProcessTypeId,
                    myRequest.ProcessInstanceId,
                    myRequest.ButlerRequest.MessageId);

                //DELETE MESSAGE Butler request
                myRequest.DeleteCurrentMessage();

                Trace.TraceInformation("{0}Process type {1} instance {2} delete Message {3}",
                    this.GetType().FullName,
                    myRequest.ProcessTypeId,
                    myRequest.ProcessInstanceId,
                    myRequest.ButlerRequest.MessageId);

            }


        }

        /// <summary>
        /// No error compensation 
        /// </summary>
        /// <param name="request"></param>
        public override void HandleCompensation(ChainRequest request)
        {
            ButlerProcessRequest myRequest = (ButlerProcessRequest)request;
            Trace.TraceWarning("{0} in process {1} processId {2} has not HandleCompensation", this.GetType().FullName, myRequest.ProcessTypeId, myRequest.ProcessInstanceId);
        }
    }
}
