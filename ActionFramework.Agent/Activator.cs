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
using ActionFramework.Entities;
using ActionFramework.Enum;
using ActionFramework.Interfaces;
using ActionFramework.Agent.Context;
using ActionFramework.Logging;
using ActionFramework.Model;
using FirebaseSharp.Portable;
using RestSharp;
using Newtonsoft.Json;
using ActionFramework.DataSource;

namespace ActionFramework.Agent
{
    public static class Activator
    {
        private static LogElements systemlog = null;
        private static IActionList actionList = null;
        private static ActionResultLog actionResult;
        private static string runtime = string.Empty;
        private static XDocument xmllog; 

        public static string RunActions(ActionListParameters par)
        {
            ActionFactory.InitializeLog();

            systemlog = new LogElements(LogType.Agent.ToString());
            systemlog.Add(new AssemblyLog(LogType.Assembly.ToString()));

            try
            {
                actionList = ActionFactory.ActionList(par);
                systemlog.Add(new InformationLog(string.Format("ActionList Initiated. Using {0} server url.", AgentConfigurationContext.Current.ServerUrl)));
                actionResult = actionList.Run(out runtime);
            }
            catch (Exception ex)
            {
                ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Error, "An error occured when runnign the actionlist. " + ex.Message + ". " + ex.StackTrace, Constants.EventLogId);
            }
            
            WriteLog();
            return xmllog.Root.ToString();
        }

        private static void WriteLog()
        {
            systemlog.Add(actionResult);
            ActionFactory.CurrentLog().Add(systemlog);

            xmllog = ActionFactory.CurrentLog().ToXml;

            string json = JsonConvert.SerializeXNode(xmllog.Root);

            //push the log to server
            SaveActionLog(xmllog.Root.ToString());
            
            //Firebase
            //string rootUri = "https://woxion01.firebaseio.com/";
            //Firebase fb = new Firebase(rootUri);
            //string path = "/logs";//"/path";
            //string data = json;
            //fb.Post(path, data);

            ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Information, xmllog.Root.ToString(), Constants.EventLogId);
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
        /// NEW TODO
        /// </summary>
        /// <returns></returns>
        public static IActionDataSource GetActionDataSource()
        {
            var configId = AgentConfigurationContext.Current.ActionFile;
            var agentId = AgentConfigurationContext.Current.AgentId;
            var uri = string.Format("{0}/{1}/{2}/{3}", AgentConfigurationContext.Current.ServerUrl, "/api/agent/config/", agentId, configId);

            RestHelper client = new RestHelper(uri, RestSharp.Method.GET);
            var result = client.Execute();
            var xml = ActionFactory.Compression.DecompressString(result.Content);
            return new XmlDataSource(xml);
            //var par = new ActionListParameters(ds);

            //return ActionFactory.ActionList(par);
        }

        /// <summary>
        /// NEW TODO
        /// </summary>
        /// <returns></returns>
        public static string SaveActionLog(string xml)
        {
            var model = new ActionFramework.Domain.Model.Api.LogModel();
            model.AgentId = AgentConfigurationContext.Current.AgentId;
            model.ActionFile = AgentConfigurationContext.Current.ActionFile;
            model.Description = "";
            model.Data = ActionFactory.Compression.CompressString(xml);

            var uri = AgentConfigurationContext.Current.ServerUrl + "/api/log/write/";
            
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json; // Or DataFormat.Xml, if you prefer
            request.AddObject(model);

            var client = new RestClient(uri);
            var response = client.Execute(request);

            return response.StatusCode.ToString();

            //RestHelper req = new RestHelper(AgentConfigurationContext.Current.ServerUrl + "/api/log/write/", Method.POST);
            //req.RequestFormat = DataFormat.Json;
            //req.AddBody(model);
            //req.AddBody(new 
            //        {
            //            ActionFile = AgentConfigurationContext.Current.ActionFile,
            //            AgentId = AgentConfigurationContext.Current.AgentId,
            //            Data = ActionFactory.Compression.CompressString(xml),
            //            Description = ""
            //        }
            //    );
            //request.AddBody(new { A = "foo", B = "bar" })); // uses JsonSerializer
            //return req.Execute().StatusCode.ToString();
        }
    }
}
