using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Agent.Helpers
{
    public class EventLogHelper
    {
        public static string CreateQueryString(string logName, string level, string eventId, string timeSpanStart, string timeSpanEnd)
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
