using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ActionFramework.Entities
{
    public class EventLogs
    {
        private List<string> eventLogsItems;

        public List<string> EventLogsItems
        {
            get { return eventLogsItems; }
            set { eventLogsItems = value; }
        }
    }
}
