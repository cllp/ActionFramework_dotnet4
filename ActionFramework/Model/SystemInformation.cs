using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Model
{
    [DataContract]
    public class SystemInformation
    {
        // from win32_ComputerSystem
        [DataMember]
        public string ComputerName { get; set; }
        [DataMember]
        public string Domain { get; set; }
        [DataMember]
        public string CurrentTimeZone { get; set; }
        [DataMember]
        public string EnableDaylightSavingsTime { get; set; }
        [DataMember]
        public string DaylightInEffect { get; set; }
        [DataMember]
        public string DnsHostName { get; set; }
        [DataMember]
        public string Caption { get; set; }
        [DataMember]
        public string Manufacturer { get; set; }
        [DataMember]
        public string Model { get; set; }
        [DataMember]
        public string PartOfDomain { get; set; }
        [DataMember]
        public string PrimaryOwnerName { get; set; }
        [DataMember]
        public string SystemType { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string TotalPhysicalMemory { get; set; }

        // from win32_OperatingSystem
        [DataMember]
        public string OperatingSystem { get; set; }
        [DataMember]
        public string FreePhysicalMemory { get; set; }
        [DataMember]
        public string FreeSpaceInPagingFiles { get; set; }
        [DataMember]
        public string FreeVirtualMemory { get; set; }
        [DataMember]
        public string InstallDate { get; set; }
        [DataMember]
        public string LastBootUpTime { get; set; }
        [DataMember]
        public string LocalDateTime { get; set; }
        [DataMember]
        public string OSArchitecture { get; set; }
        [DataMember]
        public string OSLanguage { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string RegisteredUser { get; set; }
        [DataMember]
        public string SerialNumber { get; set; }


        //from win32_Processor
        //public string CPU { get; set; }
        //public string NumberOfCores { get; set; }
        //public string NumberOfLogicalProcessors { get; set; }

        // IP
        [DataMember]
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

        public double ConvertKilobytesToMegabytes(long kilobytes)
        {
            return kilobytes / 1024f;
        }

        public DateTime LastUpdate { get; set; }
    }
}
