using ActionFramework.Classes;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Logging
{
    public class XmlLog : ILogElement
    {
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public XmlLog(string message)
        {
            this.message = message;
        }

        public XmlLog(object obj)
        {
            var name = ActionHelper.RemoveSpecialCharacters(obj.GetType().Name);
            var json = SimpleJson.SerializeObject(obj);

            var rootJson = "{Object:" + json + "}";

            var xml = Newtonsoft.Json.JsonConvert.DeserializeXNode(rootJson, name).Root;
            this.message = xml.ToString();
        }
    }
}
