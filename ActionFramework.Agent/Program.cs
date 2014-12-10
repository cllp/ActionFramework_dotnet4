using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Classes;
using ActionFramework.Context;
using ActionFramework.Entities;
using ActionFramework.Enum;
using ActionFramework.Interfaces;
using ActionFramework.Extensions;
using System.ServiceProcess;
using System.ServiceModel.Web;
using System.Timers;
using System.Diagnostics;
using System.Threading;
using ActionFramework.Agent.Context;
using ActionFramework.Domain.Model;
using System.Diagnostics.Eventing.Reader;
using System.Xml.Linq;
using System.IO;
using ActionFramework.Model;
using ActionFramework.DataSource;
using ActionFramework.Logging;
using Microsoft.Owin.Hosting;

namespace ActionFramework.Agent
{
    class Program : ServiceBase
    {
        //public WebServiceHost webServiceHost = null;
        FileSystemWatcher watcher;

        public Program()
        {
            ServiceName = AgentConfigurationContext.Current.ServiceName; //agentConfig.ServiceName;   
        }

        static void Main(string[] args)
        {
            //owin console
            using (WebApp.Start<Startup>(AgentConfigurationContext.Current.AgentUrl))
            {
                Console.WriteLine("url: " + AgentConfigurationContext.Current.AgentUrl);
                Console.ReadLine();
            }

            ServiceBase.Run(new Program());
        }

        public void InitializeTimer()
        {
            //actionTimer = new System.Threading.Timer(new TimerCallback(actionTimer_Elapsed), null, 0, interval); //every two minutes http://csharptips.wordpress.com/tag/system-threading-timer/
            //TimerContext.Initialize(new System.Threading.Timer(new TimerCallback(actionTimer_Elapsed), null, 0, interval));
            ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Information, "Initializing the timer. Interval " + AgentConfigurationContext.Current.Interval, Constants.EventLogId);
            TimerContext.Initialize(AgentConfigurationContext.Current.Interval);
        }

        private void InitializeWatcher(string path)
        {
            watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                   | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;
            watcher.Filter = "*.*";
            watcher.Changed += new FileSystemEventHandler(OnWatcherChanged);
            watcher.EnableRaisingEvents = true;
        }

        private void OnWatcherChanged(object source, FileSystemEventArgs e)
        {
            ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Information, "Filewatcher Event. file: " + e.FullPath, Constants.EventLogId);

            try
            {
                //this is set for not firing dublicate events
                watcher.EnableRaisingEvents = false;

                IActionDataSource dataSource;

                var par = new ActionListParameters();

                //this is an action list dropped to file listener, thus this is going to be executed
                if (e.FullPath.EndsWith(".xml")) //todo: more accurate check if action list
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

        protected override void OnStart(string[] args)
        {
            //initiera service host
            //var uri = AgentConfigurationContext.Current.AgentUrl;
            //webServiceHost = new WebServiceHost(typeof(AgentService), new Uri(uri));

            WebApp.Start<Startup>(AgentConfigurationContext.Current.AgentUrl);

            //var serviceEndPoint = new System.ServiceModel.Description.ServiceEndpoint()
            //webServiceHost.AddServiceEndpoint(serviceEndPoint);
            //webServiceHost.Open();
            ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Information, "Started and opened servicehost", Constants.EventLogId);

            InitializeTimer();
            InitializeWatcher(Path.Combine(AgentConfigurationContext.Current.ConfigurationPath, "Drop"));
        }

        protected override void OnStop()
        {
            //webServiceHost.Close();
            //webServiceHost = null;
        }

        private void ArchiveWatcherFile(string path)
        {
            string archivePath = Path.Combine(Path.GetDirectoryName(path), "Archive");
            if (!Directory.Exists(archivePath))
                Directory.CreateDirectory(archivePath);

            File.Move(path, Path.Combine(archivePath, Path.GetFileName(path)));
        }
    }
}
