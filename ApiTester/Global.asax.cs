using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using Microsoft.Win32;

namespace ApiTester
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication
    {
        private static Version _iisVersion;

        private List<string> _headersToRemove = new List<string>()
        {
            "X-AspNet-Version",
            "X-Powered-By",
            "Server"
        };

        protected void Application_Start()
        {
            WebApiConfig.Register(GlobalConfiguration.Configuration);
        }

        protected void Application_PreSendRequestHeaders()
        {
            _iisVersion = _iisVersion ?? GetIisVersion();
            if (_iisVersion.Major < 7) return;

            _headersToRemove.ForEach(s => Response.Headers.Remove(s));
        }

        public Version GetIisVersion()
        {
            using (RegistryKey componentsKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\InetStp", false))
            {
                if (componentsKey != null)
                {
                    int majorVersion = (int)componentsKey.GetValue("MajorVersion", -1);
                    int minorVersion = (int)componentsKey.GetValue("MinorVersion", -1);

                    if (majorVersion != -1 && minorVersion != -1)
                    {
                        return new Version(majorVersion, minorVersion);
                    }
                }

                return new Version(0, 0);
            }
        }
    }
}