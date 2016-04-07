using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Domain.Model.Api;
using ActionFramework.Classes;
using System.IO;
using ActionFramework.Logging;
using Newtonsoft.Json;

namespace ActionFramework.Server.LogWriter
{
    internal class DBLogWriter : ILogWriter
    {
        private LogModel model;

        public DBLogWriter(LogModel model)
        {
            this.model = model;
        }

        public string Write
        {
            get
            {
                //deserialize model.Data into object and insert into DB.
                List<ILogElement> elements = (List<ILogElement>)JsonConvert.DeserializeObject(model.JsonData);
                //todo loop the elements and insert into database
                return "not implemented";
                
            }
        }

        LogModel ILogWriter.model
        {
            get { return model; }
            set { model = value; }
        }
    }
}
