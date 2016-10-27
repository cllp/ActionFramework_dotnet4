using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ActionFramework.Interfaces;
using ActionFramework.Classes;
using System.IO;

namespace ActionFramework.SystemLogger
{
    internal class FileLogger : ISystemLogger
    {
        private string source = "ActionFramework";
        //private string caller = "";
        string log = "Application";
        private int eventId = 77;

        public FileLogger()
        {
            if (!EventLog.SourceExists(source))
                EventLog.CreateEventSource(source, log);
        }

        public string GetLog()
        {
            throw new NotImplementedException();
        }

        public void Write(string logtype, string message)
        {
            EventLogEntryType type = EventLogEntryType.Information;

            switch (logtype)
            {
                case "Warning":
                    {
                        type = EventLogEntryType.Warning;
                        break;
                    }
                case "Error":
                    {
                        type = EventLogEntryType.Error;
                        break;
                    }
            }

            EventLog.WriteEntry(source, message, type, eventId);


            //var path = ActionHelper.GetDirectoryPath();
            //var dir = Path.Combine(path, "syslog");

            //if (!Directory.Exists(dir))
            //    Directory.CreateDirectory(dir);

            //string fullpath = Path.Combine(dir, DateTime.Now.ToString("yyyy-MM-dd") + ".txt");

            ////if (!File.Exists(fullpath))
            ////    File.Create(fullpath);

            ////TextWriter tw = new StreamWriter(fullpath, true);
            ////tw.WriteLine(DateTime.Now + " " + logtype);
            ////tw.WriteLine(message);
            ////tw.WriteLine(@"\n\n");
            ////tw.Close();

            //var content = logtype + " " + DateTime.Now;
            //content += Environment.NewLine;
            //content += message;
            //content += Environment.NewLine;
            //content += Environment.NewLine;

            //System.IO.File.AppendAllText(fullpath, content, Encoding.Default);
        }
    }
}
