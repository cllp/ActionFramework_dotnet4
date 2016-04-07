using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Domain.Model.Api;
using ActionFramework.Classes;
using System.IO;

namespace ActionFramework.Server.LogWriter
{
    internal class XmlLogWriter : ILogWriter
    {
        private LogModel model;

        public XmlLogWriter(LogModel model)
        {
            this.model = model;
        }

        public string Write
        {
            get
            {
                var path = string.Format(@"{0}\{1}\{2}\{3}\", AppDomain.CurrentDomain.BaseDirectory, "Agents", model.AgentId, "Logs");
                var xml = ActionFactory.Compression.DecompressString(model.XmlData);

                string file = new GlobalActionFunctions().GetCurrentFormatDateTimeString() + ".xml";
                File.WriteAllText(path + file, xml, Encoding.UTF8);

                return "Log saved successfully";
            }
        }

        LogModel ILogWriter.model
        {
            get { return model; }
            set { model = value; }
        }
    }
}
