using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ActionFramework.Model.EventLog;
using ActionFramework.Interfaces;

namespace ActionFramework.Classes
{
    internal class EventLogger : IEventLogger
    {
        private string source = "ActionFramework";
        private string caller = "";
        string log = "Application";

        public EventLogger()
        {
            if (!EventLog.SourceExists(source))
                EventLog.CreateEventSource(source, log);
        }

        public EventLogger(string caller)
        {
            this.caller = caller;

            if (!EventLog.SourceExists(source + " " + caller))
                EventLog.CreateEventSource(source + " " + caller, log);
        }

        public void Write(EventLogEntryType logtype, string message, int eventId)
        {
            EventLog.WriteEntry(source + " " + caller, message, logtype, eventId);
        }

        public EventList GetEventLogs(string logName, string level, string eventId, string timeSpanStart, string timeSpanEnd, int max)
        {

            //TEST: GetEventLogInfo?logname=Application&level={1,2,3}&eventId=77&timeSpanStart=2013-12-10T15:30:30.000Z&timeSpanEnd=2013-12-11T15:30:30.000Z
            // Disclaimer: Code NOT optimized, jsut want it to work...
            // TODO: Optimize code god damn it!!

            Model.EventLog.EventList events = new Model.EventLog.EventList();

            // Create the query string
            string queryString = CreateQueryString(logName, level, eventId, timeSpanStart, timeSpanEnd);

            // Create the log query?
            EventLogQuery eventsQuery = new EventLogQuery(logName,
            PathType.LogName, queryString);


            EventLogReader logReader;
            try
            {
                // Query the log
                logReader = new EventLogReader(eventsQuery);
            }
            catch (EventLogNotFoundException e)
            {
                var f = e;
                throw;
            }

            //IEnumerable<String> xPathEnum = xPathRefs;
            // Create the property selection context using the XPath reference
            EventLogPropertySelector logPropertyContext = new EventLogPropertySelector(new string[] { "Event/EventData/Data" }.AsEnumerable());
            int count = 0;
            TimeSpan readTimeout = new TimeSpan(0, 0, 2); //max two seconds

            for (EventRecord eventInstance = logReader.ReadEvent(readTimeout);
                eventInstance != null;
                eventInstance = logReader.ReadEvent(readTimeout))
            {
                IList<object> logEventProps;
                logEventProps = ((EventLogRecord)eventInstance).GetPropertyValues(logPropertyContext);

                var log = new Model.EventLog.Event();
                log.EventId = eventInstance.Id.ToString();
                log.Level = eventInstance.LevelDisplayName;
                log.Task = eventInstance.TaskDisplayName;
                log.TimeCreated = eventInstance.TimeCreated.ToString();
                log.EventRecordId = eventInstance.RecordId.ToString();
                log.Channel = eventInstance.LogName; //rätt?
                log.Computer = eventInstance.MachineName;
                string[] evdata = (string[])logEventProps[0];
                foreach (string s in evdata)
                    log.EventData += (s + " ").Trim();

                events.Add(log);
                count++;

                if(count == max)
                    return events;
            }

            return events;
        }

        public void Write(EventLogEntryType logtype, string message)
        {
            EventLog.WriteEntry(source + " " + caller, message, logtype);
        }

        private string CreateQueryString(string logName, string level, string eventId, string timeSpanStart, string timeSpanEnd)
        {
            string[] levelarray = level.Split(',');
            string levels = "(";

            for (int i = 0; i < levelarray.Length; i++)
            {
                if (i + 1 == levelarray.Length)
                {
                    levels += "Level=" + levelarray[i].Trim() + ")";
                }
                else
                {
                    levels += "Level=" + levelarray[i].Trim() + " or ";
                }
            }
            string queryString = string.Format(
                "<QueryList>" +
                "   <Query Id=\"0\" Path=\"{0}\">" +
                "       <Select Path=\"{0}\">*[System[{1} and" +
                "        (EventID={2}) and TimeCreated[@SystemTime&gt;='{3}' and @SystemTime&lt;='{4}']]] and" +
                "           *[EventData[Data]] " +
                "       </Select>" +
                "   </Query>" +
                "</QueryList>", logName, levels, eventId, timeSpanStart, timeSpanEnd);

            return queryString;
        }
    }
}
