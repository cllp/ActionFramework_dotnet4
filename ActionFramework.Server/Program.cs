using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Server
{
    class Program : ServiceBase
    {
        private static string serverurl = ConfigurationManager.AppSettings["url"];

        public Program()
        {
            ServiceName = "ActionFramework Server";   
        }

        static void Main(string[] args)
        {
            using (WebApp.Start<Startup>(serverurl))
            {
                Console.WriteLine("Server Online: " + serverurl);
                Console.ReadLine();
            }

            ServiceBase.Run(new Program());
        }

        protected override void OnStart(string[] args)
        {
            WebApp.Start<Startup>(serverurl);
        }

        protected override void OnStop()
        {
            
        }
    }
}
