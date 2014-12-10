using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ActionFramework.Api.Models
{
    public class ResourceModel
    {
        //string agentCode, string agentSecret, int actionId, string name, string type, string format, string obj
        public string AgentCode { get; set; }
        public string AgentSecret { get; set; }
        public int ActionId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        public string Obj { get; set; }
    }
}