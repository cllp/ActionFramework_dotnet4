using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Classes;
//using ActionFramework.Context;
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
using ActionFramework.Agent.Sync;

namespace ActionFramework.Agent
{
    class Program : ServiceBase
    {
        static FileSystemWatcher watcher;

        public Program()
        {
            ServiceName = AgentConfigurationContext.Current.ServiceName;
        }

        static void Main(string[] args)
        {
            using (WebApp.Start<Startup>(AgentConfigurationContext.Current.LocalUrl))
            {
                Console.WriteLine("Local url: " + AgentConfigurationContext.Current.LocalUrl);

                InitializeTimer();

                InitializeDropWatcher(AgentConfigurationContext.Current.DropFolder);
                InitializeSyncWatcher();

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

        public static void InitializeTimer()
        {
            ActionFactory.SysLog().Write("Info", "Initializing the timer. Interval " + AgentConfigurationContext.Current.Interval);
            TimerContext.Initialize(AgentConfigurationContext.Current.Interval);
        }

        private static void InitializeDropWatcher(string path)
        {
            ActionFactory.SysLog().Write("Info", "Initialize Watcher on path: " + path);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                   | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;
            watcher.Filter = "*.*";
            watcher.Changed += new FileSystemEventHandler(OnDropWatcherChanged);
            watcher.EnableRaisingEvents = true;
        }

        private static void InitializeSyncWatcher()
        {
            var path = Path.Combine(AgentConfigurationContext.Current.DirectoryPath) + @"\Agents\" + AgentConfigurationContext.Current.AgentId + @"\Logs\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            ActionFactory.SysLog().Write("Info", "Initialize Watcher on path: " + path);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                   | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size;
            watcher.Filter = "*.xml";
            watcher.Changed += new FileSystemEventHandler(OnSyncWatcherChanged);
            watcher.EnableRaisingEvents = true;
        }

        private static void OnDropWatcherChanged(object source, FileSystemEventArgs e)
        {
            ActionFactory.SysLog().Write("Info", "Filewatcher Event. file: " + e.FullPath);

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
                    ActionFactory.SysLog().Write("Error", "Filewatcher Event Error. Could not delete file " + e.FullPath + ". Message: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                ActionFactory.SysLog().Write("Error", "Filewatcher Event Error. Fullpath: " + e.FullPath + ". Message: " + ex.Message);
            }
            finally
            {
                //this is set for not firing dublicate events
                watcher.EnableRaisingEvents = true;
            }
        }

        private static void OnSyncWatcherChanged(object source, FileSystemEventArgs e)
        {
            if (AgentConfigurationContext.Current.Sync)
            {
                //log to eventlogger
                ActionFactory.SysLog().Write("Info", "Syncwatcher Event. file: " + e.FullPath);

                ISync watcherSync = new WatcherSync();

                var status = watcherSync.SyncLog(e.FullPath);

                Console.WriteLine("Syncstatus OK. " + e.FullPath);
                
            }
            else
            {
                ActionFactory.SysLog().Write("Info", "Syncwatcher Disabled");
                Console.WriteLine("Sync failed. " + e.FullPath);
            }
        }


        private static bool IsActionFile()
        {
            return false;
        }

        protected override void OnStart(string[] args)
        {
            WebApp.Start<Startup>(AgentConfigurationContext.Current.LocalUrl);
            ActionFactory.SysLog().Write("Info", "Started and opened servicehost");

            InitializeTimer();
            InitializeDropWatcher(AgentConfigurationContext.Current.DropFolder);
            //TODO: InitializeSyncWatcher(AgentConfigurationContext.Current.DropFolder);
        }

        protected override void OnStop()
        {
            //TODO? do we need/want to add a stop action
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
            //if actionfile, we might want to run that specific actionfile?
            return false;
        }


    }
}
