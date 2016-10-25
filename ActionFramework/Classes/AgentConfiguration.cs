using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ActionFramework.Enum;
using ActionFramework.Interfaces;
using RestSharp;

namespace ActionFramework.Classes
{
    public class AgentConfiguration : IAgentConfiguration
    {

        private string configurationFile = "AgentConfig";
        private string dropFolder = Path.Combine(GetDirectoryPath(), "Drop");
        private string configurationPath = GetDirectoryPath();
        //private DataSourceLocation dataSourceLocation = DataSourceLocation.Local;
        //private DataSourceFormat dataSourceFormat = DataSourceFormat.Simple;
        private bool debug = false;
        private bool sync = false;
        private RunMode mode = RunMode.Local;

        public string ConfigurationFile
        {
            get { return configurationFile; }
            set { configurationFile = value; }
        }

        public string DropFolder
        {
            get { return dropFolder; }
            set { dropFolder = value; }
        }

        public string ConfigurationPath
        {
            get { return configurationPath; }
        }

        public string AgentId { get; set; }
        public string ServiceName
        {
            get
            {
                return "Agent " + AgentId;
            }
        }
        public string ServiceDescription
        {
            get
            {
                return "ActionFramework Agent";
            }
        }

        public string DisplayName
        {
            get
            {
                return "Agent " + AgentId;
            }
        }

        public string ServerUrl { get; set; }
        public string LocalUrl { get; set; }
        //public string AgentUrl { get; set; }
        //public string AgentUrl
        //{
        //    get
        //    {
        //        var uri = ServerUrl + "/api/agent/uri/" + AgentId;
        //        RestHelper rh = new RestHelper(uri, Method.GET);
        //        var response = rh.Execute();
        //        return response.Content;
        //    }
        //}
        //public string AgentCode { get; set; }
        //public string AgentSecret { get; set; }
        public string ActionFile { get; set; }
        //public bool LogToDisk { get; set; }
        //public bool LogRemote { get; set; }
        public int Interval { get; set; }


        public string DirectoryPath { get { return GetDirectoryPath(); } }
        

        //public DataSourceLocation DataSourceLocation
        //{
        //    get { return dataSourceLocation; }
        //    set { dataSourceLocation = value; }
        //}

        //public DataSourceFormat DataSourceFormat
        //{
        //    get { return dataSourceFormat; }
        //    set { dataSourceFormat = value; }
        //}

        public bool Debug
        {
            get { return debug; }
            set { debug = value; }
        }

        public bool Sync
        {
            get { return sync; }
            set { sync = value; }
        }

        public RunMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        public AgentConfiguration()
        {
            SetConfiguration();
        }

        public AgentConfiguration(string configurationFile)
        {
            this.configurationFile = configurationFile;
            SetConfiguration();
        }

        public bool UpdateSetting(string key, object value)
        {
            try
            {
                string path = Path.Combine(configurationPath, configurationFile + ".xml");
                XDocument xDoc = XDocument.Load(path);

                var setting = (from x in xDoc.Descendants("add")
                               where x.Attribute("key").Value.Equals("LastRunDate")
                               select x).FirstOrDefault();

                setting.Attribute("value").Value = value.ToString();
                xDoc.Save(path);

                //ActionFactory.EventLogger(this.ServiceName).Write(EventLogEntryType.Information, string.Format("Agent configuration updated '{0}'. Key '{1}' Value '{2}'", path, key, value));
                ActionFactory.SysLog().Write("Info", string.Format("Agent configuration updated '{0}'. Key '{1}' Value '{2}'", path, key, value));

                return true;
            }
            catch (Exception ex)
            {
                var msg = "Could not update agent setting file '" + configurationFile + "' with key '" + key + "' and value '" + value.ToString() + "'. Message '" + ex.Message + "'";
                ActionFactory.SysLog().Write("Error", msg);
                throw;
            }
        }

        private void SetConfiguration()
        {
            string path = Path.Combine(configurationPath, configurationFile + ".xml");
            XDocument xDoc = XDocument.Load(path);

            var settings = (from x in xDoc.Descendants("add") select x);

            AgentId = GetElementValue(settings, "AgentId");
            //ServiceName =  //GetElementValue(settings, "ServiceName");
            //ServiceDescription = "" + "";  GetElementValue(settings, "ServiceDescription");
            //DisplayName = GetElementValue(settings, "DisplayName");
            //LogRemote = Convert.ToBoolean(GetElementValue(settings, "LogRemote"));
            //LogToDisk = Convert.ToBoolean(GetElementValue(settings, "LogToDisk"));
            //AgentCode = GetElementValue(settings, "AgentCode");
            //AgentSecret = GetElementValue(settings, "AgentSecret");
            ActionFile = GetElementValue(settings, "ActionFile");
            //WebApiUrl = GetElementValue(settings, "WebApiUrl");
            //AgentUrl = GetAgentUrl();

            if (!string.IsNullOrEmpty(GetElementValue(settings, "DropFolder")))
                DropFolder = GetElementValue(settings, "DropFolder");

            ServerUrl = GetElementValue(settings, "ServerUrl");
            LocalUrl = GetElementValue(settings, "LocalUrl");
            Mode = (RunMode)System.Enum.Parse(typeof(RunMode), GetElementValue(settings, "RunMode"), true);
            Interval = Convert.ToInt32(GetElementValue(settings, "Interval"));
            //DataSourceLocation = (DataSourceLocation)System.Enum.Parse(typeof(DataSourceLocation), GetElementValue(settings, "DataSourceLocation"));
            //DataSourceFormat = (DataSourceFormat)System.Enum.Parse(typeof(DataSourceFormat), GetElementValue(settings, "DataSourceFormat"));
            Debug = Convert.ToBoolean(GetElementValue(settings, "Debug"));
            Sync = Convert.ToBoolean(GetElementValue(settings, "Sync"));

            ActionFactory.SysLog().Write("Info", "Loaded agent configuration: " + path);
        }

        //private string GetAgentUrl()
        //{
        //    var uri = ServerUrl + "/api/agent/uri/" + AgentId;
        //    RestHelper rh = new RestHelper(uri, Method.GET);
        //    var response = rh.Execute();
        //    return response.Content;
        //}

        private static string GetElementValue(IEnumerable<XElement> elements, string key)
        {
            return elements.Where(s => s.Attribute("key").Value.Equals(key)).First().Attribute("value").Value;
        }

        private static string GetDirectoryPath()
        {
            FileInfo fileInfo = new FileInfo(ReflectionHelper.GetAssemblyLocation());
            string dir = fileInfo.DirectoryName;
            return dir;
        }
    }
}
