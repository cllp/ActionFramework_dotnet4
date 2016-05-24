using ActionFramework;
using ActionFramework.Base;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServerApps
{
    public class GetConfig : ActionBase
    {
        public override object Execute(object input)
        {
            //if I dont want to use specific model?
            //how do I know this is an object[] array?
            //convert to object array.
            //ConfigModel model = (ConfigModel)input;
            //var path = string.Format("{0}/{1}/{2}/{3}/{4}", AppDomain.CurrentDomain.BaseDirectory, "Agents", model.Agent, "Configuration", model.File);

            //Message = "Unable to cast object of type 'Newtonsoft.Json.Linq.JArray' to type 'System.Object[]'."
            //object[] obj = (object[])input;
            JArray obj = (JArray)input;

            

            var path = string.Format("{0}/{1}/{2}/{3}/{4}", AppDomain.CurrentDomain.BaseDirectory, "Agents", obj[0], "Configuration", obj[1]);

            var doc = XDocument.Load(path);
            string compact = ActionFactory.Compression.CompressString(doc.Root.ToString());

            return compact;
        }
    }
}
