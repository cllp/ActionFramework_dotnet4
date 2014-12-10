using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ActionFramework.Api.Models
{
    public class AppModel
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string AssemblyInfo { get; set; }
        public string Icon { get; set; }
        public AgentAppModel AgentApp { get; set; }

    }
}