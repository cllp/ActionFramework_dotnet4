using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ActionFramework.Server.Models
{
    public class SystemInformationModel
    {
   
        public string ComputerName { get; set; }
        
        public string Domain { get; set; }
        
        public string CurrentTimeZone { get; set; }
        
        public string EnableDaylightSavingsTime { get; set; }
        
        public string DaylightInEffect { get; set; }
        
        public string DnsHostName { get; set; }
        
        public string Caption { get; set; }
        
        public string Manufacturer { get; set; }
        
        public string Model { get; set; }
        
        public string PartOfDomain { get; set; }
        
        public string PrimaryOwnerName { get; set; }
        
        public string SystemType { get; set; }
        
        public string UserName { get; set; }
        
        public string TotalPhysicalMemory { get; set; }

        // from win32_OperatingSystem
        
        public string OperatingSystem { get; set; }
        
        public string FreePhysicalMemory { get; set; }
        
        public string FreeSpaceInPagingFiles { get; set; }
        
        public string FreeVirtualMemory { get; set; }
        
        public string InstallDate { get; set; }
        
        public string LastBootUpTime { get; set; }
        
        public string LocalDateTime { get; set; }
        
        public string OSArchitecture { get; set; }
        
        public string OSLanguage { get; set; }
        
        public string Status { get; set; }
        
        public string RegisteredUser { get; set; }
        
        public string SerialNumber { get; set; }

        public string LocalIP { get; set; }

        public string Memory
        {
            get
            {
                var freemb = (Convert.ToInt64(FreePhysicalMemory) / 1024);
                var totkb = (Convert.ToInt64(TotalPhysicalMemory) / 1024);
                var totmb = (totkb / 1024);
                return string.Format("{0}mb free of total {1}mb", freemb, totmb);
            }
            
        }

        public DateTime LastUpdate { get; set; }
    }
}