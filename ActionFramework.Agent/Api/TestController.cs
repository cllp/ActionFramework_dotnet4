using ActionFramework.Agent.Context;
using ActionFramework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ActionFramework.Agent.Api
{
    [RoutePrefix("test")]
    public class TestController : ApiController
    {
        // GET api/values 
        [Route("get")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [Route("systeminfo")]
        public Model.SystemInformation GetSystemInformation()
        {
            Model.SystemInformation info = new Model.SystemInformation();

            SystemInformationHelper computerInfo = new SystemInformationHelper("win32_computersystem");

            info.ComputerName = computerInfo.GetSysInfo("Name");
            info.Domain = computerInfo.GetSysInfo("Domain");
            info.CurrentTimeZone = computerInfo.GetSysInfo("CurrentTimeZone");
            info.EnableDaylightSavingsTime = computerInfo.GetSysInfo("EnableDaylightSavingsTime");
            info.DaylightInEffect = computerInfo.GetSysInfo("DaylightInEffect");
            info.DnsHostName = computerInfo.GetSysInfo("DnsHostName");
            info.Caption = computerInfo.GetSysInfo("Caption");
            info.Manufacturer = computerInfo.GetSysInfo("Manufacturer");
            info.Model = computerInfo.GetSysInfo("Model");
            info.PartOfDomain = computerInfo.GetSysInfo("PartOfDomain");
            info.PrimaryOwnerName = computerInfo.GetSysInfo("PrimaryOwnerName");
            info.SystemType = computerInfo.GetSysInfo("SystemType");
            info.UserName = computerInfo.GetSysInfo("UserName");
            info.TotalPhysicalMemory = computerInfo.GetSysInfo("TotalPhysicalMemory");

            SystemInformationHelper operatingSystemInfo = new SystemInformationHelper("win32_operatingsystem");

            info.OperatingSystem = operatingSystemInfo.GetSysInfo("Caption");
            info.FreePhysicalMemory = operatingSystemInfo.GetSysInfo("FreePhysicalMemory");
            info.FreeSpaceInPagingFiles = operatingSystemInfo.GetSysInfo("FreeSpaceInPagingFiles");
            info.FreeVirtualMemory = operatingSystemInfo.GetSysInfo("FreeVirtualMemory");
            info.InstallDate = operatingSystemInfo.GetSysInfo("InstallDate");
            info.LastBootUpTime = operatingSystemInfo.GetSysInfo("LastBootUpTime");
            info.LocalDateTime = operatingSystemInfo.GetSysInfo("LocalDateTime");
            info.OSArchitecture = operatingSystemInfo.GetSysInfo("OSArchitecture");
            info.OSLanguage = operatingSystemInfo.GetSysInfo("OSLanguage");
            info.Status = operatingSystemInfo.GetSysInfo("Status");
            info.RegisteredUser = operatingSystemInfo.GetSysInfo("RegisteredUser");
            info.SerialNumber = operatingSystemInfo.GetSysInfo("SerialNumber");

            //SystemInformationHelper processorInfo = new SystemInformationHelper("win32_processor");

            //info.CPU = processorInfo.GetSysInfo("Name");
            //info.NumberOfCores = processorInfo.GetSysInfo("NumberOfCores");
            //info.NumberOfLogicalProcessors = processorInfo.GetSysInfo("NumberOfLogicalProcessors");

            info.LocalIP = SystemInformationHelper.GetIP4Address();

            return info;
        }
    }
}
