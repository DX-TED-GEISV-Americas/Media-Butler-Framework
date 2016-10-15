using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaButler.Common.ResourceAccess
{
    /// <summary>
    /// Key value configuration data
    /// </summary>
    public interface IjsonKeyValue
    {
        /// <summary>
        /// read configuration value of specific key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string Read(string key);
        /// <summary>
        /// Read array data
        /// </summary>
        /// <param name="key"></param>
        /// <returns>array data</returns>
        JToken ReadArray(string key);
    }
}
