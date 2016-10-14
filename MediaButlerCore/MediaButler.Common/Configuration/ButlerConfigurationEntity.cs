using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaButler.Common.Configuration
{
    /// <summary>
    /// Medua Butler configuration Entity
    /// </summary>
    public class ButlerConfigurationEntity : TableEntity
    {
        /// <summary>
        /// Cofiguration entity constructor
        /// </summary>
        /// <param name="configKey">Configuration Key</param>
        /// <param name="processKey">Process Key</param>
        public ButlerConfigurationEntity(string configKey, string processKey)
        {
            this.PartitionKey = processKey;
            this.RowKey = configKey;
        }
        /// <summary>
        /// Construtor
        /// </summary>
        public ButlerConfigurationEntity() { }
        /// <summary>
        /// Configuration Value
        /// </summary>
        public string ConfigurationValue { get; set; }

    }
}
