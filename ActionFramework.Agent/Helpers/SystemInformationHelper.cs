using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Agent.Context
{
    public class SystemInformationHelper
    {        
        private ManagementObjectSearcher query;
        public SystemInformationHelper(string win32Class)
        {
            query = new ManagementObjectSearcher("SELECT * FROM " + win32Class);
        }

        public string GetSysInfo(string property)
        {
            string reply = "";
            
            foreach(ManagementObject obj in query.Get())
            {
                reply = obj[property].ToString();
            }

            return reply;
        }

        public static string GetIP4Address()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return (from ip in host.AddressList
                    where ip.AddressFamily == AddressFamily.InterNetwork
                    select ip.ToString()).FirstOrDefault();
        }
    }
}
