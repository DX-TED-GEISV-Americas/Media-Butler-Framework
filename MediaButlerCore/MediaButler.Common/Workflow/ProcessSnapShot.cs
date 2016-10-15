using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaButler.Common.Workflow
{
    /// <summary>
    /// MBF instance process information snapshoot
    /// </summary>
    public class ProcessSnapShot : TableEntity
    {
        /// <summary>
        /// Current step number
        /// </summary>
        public int CurrentStep { get; set; }
        /// <summary>
        /// instance process information json serialized
        /// </summary>
        public string jsonContext { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pk">process name key</param>
        /// <param name="rk">process instance id</param>
        public ProcessSnapShot(string pk, string rk)
        {
            this.PartitionKey = pk;
            this.RowKey = rk;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public ProcessSnapShot()
        { }
    }
}
