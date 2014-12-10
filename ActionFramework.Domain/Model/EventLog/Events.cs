using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Domain.Model.EventLog
{
    [CollectionDataContract(Name = "Events")]
    public class EventList : List<Event>
    {

    }

    [DataContract]
    public class Event
    {
        [DataMember]
        public string EventId { get; set; }

        [DataMember]
        public string Level { get; set; }

        [DataMember]
        public string Task { get; set; }

        [DataMember]
        public string Keywords { get; set; }

        [DataMember]
        public string TimeCreated { get; set; }

        [DataMember]
        public string EventRecordId { get; set; }

        [DataMember]
        public string Channel { get; set; }

        [DataMember]
        public string Computer { get; set; }

        [DataMember]
        public string EventData { get; set; }
    }


    //  <Events>
    //<Event xmlns="http://schemas.microsoft.com/win/2004/08/events/event">
    //  <System>
    //    <Provider Name="ActionFramework Agent 01" />
    //    <EventID Qualifiers="0">77</EventID>
    //    <Level>4</Level>
    //    <Task>0</Task>
    //    <Keywords>0x80000000000000</Keywords>
    //    <TimeCreated SystemTime="2013-12-11T16:47:56.000000000Z" />
    //    <EventRecordID>76663</EventRecordID>
    //    <Channel>Application</Channel>
    //    <Computer>CLAES-PHILI0912</Computer>
    //    <Security />
    //  </System>
    //  <EventData>
    //    <Data>GetLastRunDate from text file</Data>
    //  </EventData>
    //</Event>
}
