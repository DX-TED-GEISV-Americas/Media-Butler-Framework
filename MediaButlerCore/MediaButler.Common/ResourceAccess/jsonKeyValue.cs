using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaButler.Common.ResourceAccess
{
    /// <summary>
    /// Key value data from json 
    /// </summary>
    public class jsonKeyValue : IjsonKeyValue
    {
        /// <summary>
        /// configuration data
        /// </summary>
        private dynamic data;
        /// <summary>
        /// Create Key value data from json 
        /// </summary>
        /// <param name="jsonTxt">json key value data</param>
        public jsonKeyValue(string jsonTxt)
        {
            try
            {
                //Check if json is well formed
                if (jsonTxt[0] != '{')
                {
                    jsonTxt = jsonTxt.Substring(1, jsonTxt.Length - 1);
                }
                data = JObject.Parse(jsonTxt);
            }
            catch (Exception X)
            {
                Trace.TraceError("jsonKeyValue error: " + jsonTxt + " is not valid json file. Error  " + X.Message);
            }
        }
       /// <summary>
       /// 
       /// </summary>
       /// <param name="key"></param>
       /// <returns></returns>
        public string Read(string key)
        {
            string aux = "";
            try
            {
                //Explicit TOStirng
                aux = data[key].ToString();

            }
            catch (Exception X)
            {
                Trace.TraceWarning("Configuration key " + key + "was not availale. " + X.Message);
            }
            return aux;
        }
        /// <summary>
        /// Read array data
        /// </summary>
        /// <param name="key"></param>
        /// <returns>array data</returns>
        public JToken ReadArray(string key)
        {
            JToken aux = null;
            try
            {
                aux = (JArray)data[key];

            }
            catch (Exception X)
            {
                Trace.TraceWarning("Configuration key " + key + "was not availale. " + X.Message);
            }
            return aux;
        }

    }
}
