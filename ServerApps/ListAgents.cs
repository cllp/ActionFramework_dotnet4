using ActionFramework.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApps
{
    public class ListAgents : ActionBase
    {
        public override object Execute()
        {
            List<object> agents = new List<object>();
            foreach (var dir in Directory.GetDirectories(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Agents")))
            {
                string uri = File.ReadAllText(Path.Combine(dir, "uri.txt"));
                DirectoryInfo di = new DirectoryInfo(dir);
                agents.Add(new
                {
                    Name = di.Name,
                    Uri = uri
                });
            }

            return agents;
        }
    }
}
