using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ActionFramework.Model.EventLog;

namespace ActionFramework.Interfaces
{
    public interface ISystemLogger
    {
        void Write(EventLogEntryType logtype, string message, int eventId);

        void Write(EventLogEntryType logtype, string message);

        EventList GetEventLogs(string logName, string level, string eventId, string timeSpanStart, string timeSpanEnd, int max);
    }
}
