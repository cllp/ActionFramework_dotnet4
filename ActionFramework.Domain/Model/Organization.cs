using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Domain.Model
{
    public partial class Organization
    {
        private int logCount = 0;
        private int errorCount = 0;
        private IEnumerable<Agent> agents;// = new IEnumerable<Setting>();

        public IEnumerable<Agent> Agents
        {
            get { return agents; }
            set { agents = value; }
        }

        public IEnumerable<Log> RecentLogs { get; set; }       

        public int LogCount
        {
            get
            {
                if (agents != null)
                {
                    logCount = 0;
                    foreach (var a in agents)
                        logCount += a.LogCount;

                    return logCount;
                }
                else
                    return 0;
            }
        }

        public int ErrorCount
        {
            get
            {
                if (agents != null)
                {
                    errorCount = 0;
                    foreach (var a in agents)
                        errorCount += a.ErrorCount;

                    return errorCount;
                }
                else
                    return 0;
            }
        }
    }
}
