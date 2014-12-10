using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ActionFramework.Api.Models
{
    public class AgentModel
    {
        public string CurrentStatus { get; set; }
        public string CurrentInterval { get; set; }
        public string NewInterval { get; set; }
        public int Id { get; set; }
        public string Application { get; set; }
        public string Type { get; set; }
        public string ServiceUrl { get; set; }
        public string Version { get; set; }
        public string Notes { get; set; }
        public IEnumerable<SettingModel> Settings { get; set; }
        public IEnumerable<AppModel> InstalledApps { get; set; }
        public IEnumerable<AppModel> AvailableApps { get; set; }
        public IEnumerable<AppModel> Apps { get; set; }
        public OrganizationModel Organization { get; set; }
        public SystemInformationModel SystemInformation { get; set; }

        public AgentModel()
        {
            //new interval default value
            NewInterval = "60";
        }
    }
}