using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ActionFramework.Server.Models
{
    public class AgentAppModel
    {
        public int AppId { get; set; }
        public int AgentId { get; set; }
        public bool Installed { get; set; }
        public DateTime InstallDate { get; set; }
    }
}