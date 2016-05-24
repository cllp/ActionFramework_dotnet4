using ActionFramework.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApps
{
    public class GetAgentUri : ActionBase
    {
        public override object Execute(object agentId)
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Agents", agentId.ToString());
            string uri = File.ReadAllText(Path.Combine(dir, "uri.txt"));
            return uri;
        }
    }
}
