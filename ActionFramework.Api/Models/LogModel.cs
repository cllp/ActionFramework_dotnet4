using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ActionFramework.Api.Models
{
    public class LogModel
    {
        public string AgentCode { get; set; }
        public string AgentSecret { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Message { get; set; }
    }
}