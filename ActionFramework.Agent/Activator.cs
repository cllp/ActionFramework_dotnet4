using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ActionFramework.Classes;
using ActionFramework.Context;
using ActionFramework.Enum;
using ActionFramework.Interfaces;
using ActionFramework.Agent.Context;
using ActionFramework.Logging;
using ActionFramework.Model;
using RestSharp;
using Newtonsoft.Json;
using ActionFramework.Agent.DataSource;
using ActionFramework.Helpers;

namespace ActionFramework.Agent
{
    public static class Activator
    {
        private static LogElements systemlog = null;
        private static IActionList actionList = null;
        private static ActionResultLog actionResult;
        private static string runtime = string.Empty;

        public static string RunActions(ActionListParameters par)
        {
            //initialize the log
            InitLog();

            try
            {
                actionList = ActionFactory.ActionList(par);
                if (AgentConfigurationContext.Current.Mode == RunMode.Remote)
                    systemlog.Add(new InformationLog(string.Format("ActionList Initiated. Using {0} server url.", AgentConfigurationContext.Current.ServerUrl)));
                else
                    systemlog.Add(new InformationLog(string.Format("ActionList Initiated. Using {0} local url.", AgentConfigurationContext.Current.LocalUrl)));

                //run all actions in the list
                actionResult = actionList.Run(out runtime);

                //add action result log to system log
                systemlog.Add(actionResult);
            }
            catch (Exception ex)
            {
                systemlog.Add(new ExceptionLog(ex));
                ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Error, "An error occured when runnign the actionlist. " + ex.Message + ". " + ex.StackTrace, Constants.EventLogId);
            }

            return WriteLog();
        }

        private static void InitLog()
        {
            if (!LogContext.IsInitialized)
            {
                //initiate the log
                ActionFactory.InitializeLog();
            }

            //create a new system log
            systemlog = new LogElements(LogType.Agent.ToString());

            // add assembly log to system log
            systemlog.Add(new AssemblyLog(LogType.Assembly.ToString()));
        }

        private static string WriteLog()
        {
            //add the system log to the current log
            ActionFactory.CurrentLog().Add(systemlog);

            var log = ActionFactory.CurrentLog().WriteXml;

            ActionFactory.CurrentLog().ClearElements();

            //string json = JsonConvert.SerializeXNode(xmllog.Root);

            //push the log to server
            var saveLogStatus = SaveActionLog(log);

            //log to eventlogger
            ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Information, log, Constants.EventLogId);

            //log status of save to eventlogger
            ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Information, "SaveLogStatus: " + saveLogStatus, Constants.EventLogId);

            return log;
        }

        public static object RunAction(IAction action)
        {
            return action.Execute();
        }

        public static string GetNextRuntime()
        {
            int interval = TimerContext.TimerInterval;

            DateTime lastRun = Activator.GetLastRunDate();//AgentConfigurationContext.Current.LastRunDate;

            if (interval > 0)
            {
                TimeSpan span = DateTime.Now - lastRun;

                var minutesSinceLast = span.TotalMinutes;
                var minutesInterval = TimeSpanUtil.ConvertMillisecondsToMinutes(interval);
                var minutesLeft = Math.Round(minutesInterval - minutesSinceLast, 2);

                return GetTimeString(minutesLeft);
            }
            else
            {
                return "0";
            }
        }

        public static string GetTimeString(double minuteinterval)
        {
            if (minuteinterval < 1) //less than a minute
                return Convert.ToInt32(TimeSpanUtil.ConvertMinutesToSeconds(minuteinterval)).ToString() + " sec";
            else if (minuteinterval < 120) //2hours
                return Convert.ToInt32(minuteinterval).ToString() + " min";
            else
                return Convert.ToInt32(TimeSpanUtil.ConvertMinutesToHours(minuteinterval)) + " hrs";
        }

        public static DateTime GetLastRunDate()
        {
            ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Information, "GetLastRunDate from text file", Constants.EventLogId);
            var file = Path.Combine(ActionHelper.GetDirectoryPath(), "lastrun.txt");

            if (!File.Exists(file))
                SetLastRunDate();

            using (StreamReader sr = new StreamReader(file))
            {
                return DateTime.Parse(sr.ReadToEnd());
            }
        }

        public static void SetLastRunDate()
        {
            ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Information, "SetLastRunDate in text file", Constants.EventLogId);
            System.IO.File.WriteAllText(Path.Combine(ActionHelper.GetDirectoryPath(), "lastrun.txt"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        /// <summary>
        /// TODO: Get the "action file" be synced in a nother process?
        /// </summary>
        /// <returns></returns>
        public static IActionDataSource GetActionDataSource()
        {

            var actionFile = AgentConfigurationContext.Current.ActionFile;
            var agentId = AgentConfigurationContext.Current.AgentId;
            var xml = string.Empty;

            if (AgentConfigurationContext.Current.Mode == RunMode.Remote)
            {
                var postclient = new RestClient(AgentConfigurationContext.Current.ServerUrl);
                var postrequest = new RestRequest("api/agent/runaction?name=getconfig", Method.POST);
                postrequest.RequestFormat = DataFormat.Json;

                var body = new object[2];
                body[0] = agentId;
                body[1] = actionFile; //with extension

                postrequest.AddBody(body);

                var postresponse = postclient.Execute(postrequest);

                //TODO parse JSON Result to string in order to decompress

                var compressed_xml = JsonConvert.DeserializeObject<dynamic>(postresponse.Content);
                
                
                //string fromjson = JsonConvert.DeserializeObject(compressed_xml);

                //string compressed = JsonConvert.DeserializeObject(postresponse.Content).ToString();
                
                xml = ActionFactory.Compression.DecompressString(compressed_xml);

            }
            else
            {
                //read xml from disk
                var path = string.Format("{0}/{1}/{2}/{3}/{4}", AppDomain.CurrentDomain.BaseDirectory, "Agents", agentId, "Configuration", actionFile);
                var doc = XDocument.Load(path);
                xml = doc.ToString();
            }

            return new XmlDataSource(xml);
        }

        public static IAction FindAction(string name)
        {
            IActionDataSource dataSource = Activator.GetActionDataSource();
            IActionList actionList = new ActionList(dataSource);
            dataSource.FillActions(actionList, Enum.ActionStatus.Enabled);
            IAction action = actionList.Where(a => a.Type.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

            if (action != null)
            {
                return action;
            }
            else
            {
                throw new Exception(string.Format("Could not find action {0}", name));
            }
        }

        /// <summary>
        /// NEW TODO
        /// </summary>
        /// <returns></returns>
        public static string SaveActionLog(string xml)
        {
            //var model = new ActionFramework.Domain.Model.Api.LogModel();
            //model.AgentId = AgentConfigurationContext.Current.AgentId;
            //model.ActionFile = AgentConfigurationContext.Current.ActionFile;
            //model.Description = "";
            //model.XmlData = ActionFactory.Compression.CompressString(xml);

            if (AgentConfigurationContext.Current.Mode == RunMode.Remote)
            {
                //var uri = AgentConfigurationContext.Current.ServerUrl + "/api/log/write/";
                var postclient = new RestClient(AgentConfigurationContext.Current.ServerUrl);
                var postrequest = new RestRequest("api/agent/runaction?name=writelog", Method.POST);
                postrequest.RequestFormat = DataFormat.Json;

                var body = new object[2];
                body[0] = ActionFactory.Compression.CompressString(xml); //compressed xml
                body[1] = AgentConfigurationContext.Current.AgentId; //agentId

                postrequest.AddBody(body);

                var response = postclient.Execute(postrequest);
                return response.StatusCode.ToString();
            }
            else
            {
                //ILogWriter xmlLogWriter = new XmlLogWriter();
                //xmlLogWriter.model = model;
                //xmlLogWriter.Path = Path.Combine(AgentConfigurationContext.Current.DirectoryPath) + @"\Logs\";
                //return xmlLogWriter.Write;

                string file = new GlobalActionFunctions().GetCurrentFormatDateTimeString() + ".xml";
                var path = Path.Combine(AgentConfigurationContext.Current.DirectoryPath) + @"\Agents\" + AgentConfigurationContext.Current.AgentId + @"\Logs\";
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                File.WriteAllText(path + file, xml, Encoding.UTF8);
                return "OK";
            }
        }
    }
}
