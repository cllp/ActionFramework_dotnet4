using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Classes;
using ActionFramework.Context;
using ActionFramework.Enum;
using ActionFramework.Interfaces;
using ActionFramework.Extensions;
using System.ServiceProcess;
using System.ServiceModel.Web;
using System.Timers;
using System.Diagnostics;
using System.Threading;
using ActionFramework.Agent.Context;
using ActionFramework.Model;
using System.Diagnostics.Eventing.Reader;
using System.Xml.Linq;
using System.IO;
using ActionFramework.Agent.DataSource;
using ActionFramework.Logging;
using Microsoft.Owin.Hosting;
using RestSharp;
using ActionFramework.Helpers;

namespace ActionFramework.Agent
{
    class Program : ServiceBase
    {
        //public WebServiceHost webServiceHost = null;
        static FileSystemWatcher watcher;
        private static string agentUri;

        public Program()
        {
            ServiceName = AgentConfigurationContext.Current.ServiceName; //agentConfig.ServiceName;   
        }

        static void Main(string[] args)
        {
            //owin console
            if (Connect())
            {
                using (WebApp.Start<Startup>(agentUri))
                {
                    Console.WriteLine("url: " + agentUri);
                    //ActionFactory.InitializeLog();

                    InitializeTimer();
                   
                    InitializeWatcher(AgentConfigurationContext.Current.DropFolder);
                    Console.WriteLine("AgentId: " + AgentConfigurationContext.Current.AgentId);
                    Console.WriteLine("Default interval schedule: " + AgentConfigurationContext.Current.Interval.ToString());
                    Console.WriteLine("Initialized Watcher on folder: " + AgentConfigurationContext.Current.DropFolder);
                    Console.WriteLine("Runmode: " + AgentConfigurationContext.Current.Mode.ToString());
                    Console.WriteLine("ActionFile: " + AgentConfigurationContext.Current.ActionFile);
                    Console.WriteLine("Directory: " + AgentConfigurationContext.Current.DirectoryPath);

                    Console.ReadLine();
                }

                ServiceBase.Run(new Program());
            }
        }

        public static void InitializeTimer()
        {
            //actionTimer = new System.Threading.Timer(new TimerCallback(actionTimer_Elapsed), null, 0, interval); //every two minutes http://csharptips.wordpress.com/tag/system-threading-timer/
            //TimerContext.Initialize(new System.Threading.Timer(new TimerCallback(actionTimer_Elapsed), null, 0, interval));
            ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Information, "Initializing the timer. Interval " + AgentConfigurationContext.Current.Interval, Constants.EventLogId);
            TimerContext.Initialize(AgentConfigurationContext.Current.Interval);
        }

        private static void InitializeWatcher(string path)
        {
            ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Information, "Initialize Watcher on path: " + path, Constants.EventLogId);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                   | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;
            watcher.Filter = "*.*";
            watcher.Changed += new FileSystemEventHandler(OnWatcherChanged);
            watcher.EnableRaisingEvents = true;
        }

        private static void OnWatcherChanged(object source, FileSystemEventArgs e)
        {
            ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Information, "Filewatcher Event. file: " + e.FullPath, Constants.EventLogId);

            try
            {
                //this is set for not firing dublicate events
                watcher.EnableRaisingEvents = false;

                IActionDataSource dataSource;

                var par = new ActionListParameters();

                //this is an action list dropped to file listener, thus this is going to be executed
                if (IsActionFile()) //todo: more accurate check if action list
                {
                    dataSource = new XmlDataSource(XDocument.Load(e.FullPath).Root.ToString());
                }
                else
                {
                    //the file is a resourcefile, get the datasource from server
                    dataSource = Activator.GetActionDataSource();

                    var resource = new ResourceParameter();

                    var ext = Path.GetExtension(e.FullPath).Replace(".", "");

                    resource.FileType = (ResourceFileType)System.Enum.Parse(typeof(ResourceFileType), ext, true);
                    resource.FileName = Path.GetFileName(e.FullPath);
                    resource.FileExtension = Path.GetExtension(e.FullPath);
                    resource.FilePath = e.FullPath;
                    resource.Origin = ResourceOrigin.FileWatcher;
                    resource.CompressedFile = ActionFactory.Compression.CompressFile(File.ReadAllBytes(resource.FilePath));
                    resource.Description = "Resource added from ";
                    resource.FileContent = System.IO.File.ReadAllText(e.FullPath);
                    resource.LoadDate = DateTime.Now;
                    par.Add(resource);
                }

                par.DataSource = dataSource;

                Activator.RunActions(par);

                try
                {
                    ArchiveWatcherFile(e.FullPath);
                }
                catch (Exception ex)
                {
                    ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Error, "Filewatcher Event Error. Could not delete file " + e.FullPath + ". Message: " + ex.Message, Constants.EventLogId);
                }
            }
            catch (Exception ex)
            {
                ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Error, "Filewatcher Event Error. Fullpath: " + e.FullPath + ". Message: " + ex.Message, Constants.EventLogId);
            }
            finally
            {
                //this is set for not firing dublicate events
                watcher.EnableRaisingEvents = true;
            }
        }

        private static bool IsActionFile()
        {
            return false;
        }

        protected override void OnStart(string[] args)
        {
            //initiera service host
            //var uri = AgentConfigurationContext.Current.AgentUrl;
            //webServiceHost = new WebServiceHost(typeof(AgentService), new Uri(uri));
            //agentUri = GetAgentUri();
            if (Connect())
            {
                WebApp.Start<Startup>(agentUri);

                //var serviceEndPoint = new System.ServiceModel.Description.ServiceEndpoint()
                //webServiceHost.AddServiceEndpoint(serviceEndPoint);
                //webServiceHost.Open();
                ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Information, "Started and opened servicehost", Constants.EventLogId);

                InitializeTimer();
                //InitializeWatcher(Path.Combine(AgentConfigurationContext.Current.ConfigurationPath, "Drop"));
                InitializeWatcher(AgentConfigurationContext.Current.DropFolder);
            }
        }

        protected override void OnStop()
        {
            //webServiceHost.Close();
            //webServiceHost = null;
        }

        private static void ArchiveWatcherFile(string path)
        {
            string archivePath = Path.Combine(Path.GetDirectoryName(path), "Archive");
            if (!Directory.Exists(archivePath))
                Directory.CreateDirectory(archivePath);

            //add the current datetime to the filename.
            string fileprefix = new GlobalActionFunctions().GetCurrentFormatDateTimeString();
            string filename = fileprefix + "_" + Path.GetFileName(path);

            File.Move(path, Path.Combine(archivePath, filename));
        }

        private static bool IsActionFile(string path)
        {
            //todo compare with schema if the file is an action file.
            return false;
        }

        private static bool Connect()
        {
            //always start an agent on the local uri from own configuration
            agentUri = AgentConfigurationContext.Current.LocalUrl;
            return true;

            if (AgentConfigurationContext.Current.Mode == RunMode.Remote)
            {
                //var uri = AgentConfigurationContext.Current.ServerUrl + "/api/agent/uri/" + AgentConfigurationContext.Current.AgentId;
                //RestHelper rh = new RestHelper(uri, Method.GET);
                //var response = rh.Execute();
                //RestSharp.ResponseStatus status = response.ResponseStatus;

                //GET:
                var client = new RestClient(AgentConfigurationContext.Current.ServerUrl);
                var request = new RestRequest("/api/agent/runaction/getagenturi", Method.GET);
                var response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    agentUri = response.Content;
                    return true;
                }
                else
                {
                    var msg = "An error occured while connecting to the server '" + AgentConfigurationContext.Current.ServerUrl + "'. Status: '" + response.ResponseStatus + "'. Code: '" + response.StatusCode + "'. AgentId: '" + AgentConfigurationContext.Current.AgentId + "'.";
                    ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Error, msg, Constants.EventLogId);
                    Console.WriteLine("Error: " + msg);
                    Console.ReadLine();
                    return false;
                }
            }
            else
            {
                agentUri = AgentConfigurationContext.Current.LocalUrl;
                return true;
            }
        }
    }
}
