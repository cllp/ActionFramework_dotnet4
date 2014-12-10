using ActionFramework.Agent.Context;
using ActionFramework.Classes;
using ActionFramework.Context;
using ActionFramework.DataSource;
using ActionFramework.Domain.Model;
using ActionFramework.Domain.Model.EventLog;
using ActionFramework.Interfaces;
using ActionFramework.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ActionFramework.Agent.Api
{
    [RoutePrefix("agent")]
    public class AgentController : ApiController
    {
        [HttpGet]
        [Route("run")]
        public HttpResponseMessage Run()
        {
            string result = "OK";
            try
            {
                IActionDataSource dataSource = Activator.GetActionDataSource();
                result = Activator.RunActions(new ActionListParameters(dataSource));
            }
            catch (Exception ex)
            {
                var msg = "RunActions() caused an exception" + " " + ex.Message;
                ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Error, msg, Constants.EventLogId);
                result = msg;
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(result, Encoding.UTF8, "text/html")
            };
        }

        [HttpGet]
        [Route("run/{name}")]
        public string RunConfiguration(string name)
        {
            try
            {
                //should call server with a specific file to then use as a parameter
                throw new NotImplementedException();

                //IActionDataSource dataSource;
                //dataSource = new XmlDataSource(configurationFile);

                //return Activator.RunActions(new ActionListParameters(dataSource));
            }
            catch (Exception ex)
            {
                var msg = "RunActions(configurationFile) caused an exception" + " " + ex.Message;
                ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Error, msg, Constants.EventLogId);
                return msg;
            }
        }

        [HttpGet]
        [Route("timer/stop")]
        public HttpResponseMessage StopTimer()
        {
            try
            {
                TimerContext.StopTimer();
                var msg = "Timer Stopped " + DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(msg, Encoding.UTF8, "text/xml")
                };
            }
            catch (Exception ex)
            {
                var msg = "StopTimer caused an exception" + " " + ex.Message;
                ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Error, msg, Constants.EventLogId);

                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(msg, Encoding.UTF8, "text/xml")
                };
            }
        }

        [HttpGet]
        [Route("timer/start/{interval}")]
        public string StartTimer(int interval)
        {
            try
            {
                AgentConfigurationContext.Current.UpdateSetting("Interval", interval);
                TimerContext.Initialize(interval);
                return "Timer Started " + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + ". Timer interval " + TimerContext.TimerInterval.ToString();
            }
            catch (Exception ex)
            {
                var msg = "StartTimer caused an exception" + " " + ex.Message;
                ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Error, msg, Constants.EventLogId);
                return msg;
            }
        }

        [HttpGet]
        [Route("timer/initializes")]
        public int IsInitialized()
        {
            if (!TimerContext.IsInitialized)
                return 0;
            else
                return TimerContext.TimerInterval;
        }

        [HttpGet]
        [Route("timer/info")]
        public string TimerInfo()
        {
            if (!TimerContext.IsInitialized)
                return "0";
            else
            {
                var minuteInterval = Activator.GetTimeString(TimeSpanUtil.ConvertMillisecondsToMinutes(TimerContext.TimerInterval));
                return "Runs every " + minuteInterval + ". Next " + Activator.GetNextRuntime();
            }
        }

        [HttpGet]
        [Route("host/info")]
        public SystemInformation GetSystemInformation()
        {
            SystemInformation info = new SystemInformation();

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

        [HttpGet]
        [Route("host/eventlog")]
        public EventList GetEventLogInfo(string logName, string level, string eventId, string timeSpanStart, string timeSpanEnd, int max)
        {
            return ActionFactory.EventLogger().GetEventLogs(logName, level, eventId, timeSpanStart, timeSpanEnd, max);
        }

        [HttpGet]
        [Route("install/{assembly}")]
        public string Install(string assembly)
        {
            var ass = ActionFactory.Compression.DecompressAssembly(assembly);
            GlobalActionFunctions gaf = new GlobalActionFunctions();

            ApplicationInfo assemblyInfo = new ApplicationInfo(ass);
            //var assemblyInfo = app.FillActionsAndProperties(assemblyFileData);

            var folder = gaf.GetExecutionRoot();
            var file = ActionFactory.Compression.DecompressFile(assembly);
            var filename = string.Format(Constants.AssemblyFileName, assemblyInfo.Title, assemblyInfo.Version);

            if (!File.Exists(folder + filename))
                File.WriteAllBytes(folder + filename, file);

            return "";
        }

        [HttpGet]
        [Route("service/stop")]
        public void StopService()
        {
            using (var controller = new ServiceController(AgentConfigurationContext.Current.ServiceName))
            {
                controller.Stop();
            }
        }

        [HttpGet]
        [Route("service/refresh")]
        public string RefreshService()
        {
            using (var controller = new ServiceController(AgentConfigurationContext.Current.ServiceName))
            {
                controller.Refresh();
                return controller.Status.ToString();
            }
        }

        [HttpGet]
        [Route("service/restart")]
        public string RestartService()
        {
            return RestartService(AgentConfigurationContext.Current.ServiceName);
        }

        [HttpGet]
        [Route("service/restart/{name}")]
        private string RestartService(string serviceName)
        {
            //var errormsg = string.Format("Could not pause service: '{0}'.", serviceName);
            var status = string.Empty;

            try
            {
                using (var controller = new ServiceController(serviceName))
                {
                    //controller.Pause();

                    //int counter = 0;
                    //while (controller.Status != ServiceControllerStatus.Paused)
                    //{
                    //    Thread.Sleep(100);
                    //    controller.Refresh();
                    //    counter++;
                    //    if (counter > 1000)
                    //    {
                    //        status = errormsg;
                    //    }
                    //}

                    controller.Start();
                }
            }
            catch (Exception ex)
            {
                status = ex.Message;
            }

            return status;
        }
    }
}
