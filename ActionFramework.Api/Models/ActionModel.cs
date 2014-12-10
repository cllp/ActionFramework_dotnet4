using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ActionFramework.Api.Models
{
    
    public class ActionModel
    {
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int AppId { get; set; }
        public bool Enabled { get; set; }
        public bool ClientExecute { get; set; }
        public bool BreakOnError { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public AppModel App { get; set; }
    }
}