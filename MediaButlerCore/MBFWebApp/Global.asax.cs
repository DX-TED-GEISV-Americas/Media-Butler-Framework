using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace MBFWebApp
{
    public class Global : System.Web.HttpApplication
    {
        /// <summary>
        /// On Web Application start, check if Web Job exist.
        /// If Web Job not exist, install it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Start(object sender, EventArgs e)
        {
            string zipPath = HttpRuntime.AppDomainAppPath + "\\App_data\\jobs\\continuous\\";
            if (!Directory.Exists(zipPath))
            {
                try
                {
                    string packageURI = System.Web.Configuration.WebConfigurationManager.AppSettings["webjobURI"];
                    Directory.CreateDirectory(zipPath);
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(packageURI, zipPath + "X.zip");
                        System.IO.Compression.ZipFile.ExtractToDirectory(zipPath + "X.zip", HttpRuntime.AppDomainAppPath + "\\App_data\\jobs\\continuous\\");
                    }
                    File.Delete(zipPath + "X.zip");
                }
                catch (Exception)
                {
                    Directory.Delete(zipPath);
                }

            }

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}