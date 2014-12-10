using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Woxion.Utility.ActionFramework.Model.Attributes;
using Woxion.Utility.ActionFramework.Domain.Extensions;

namespace Woxion.Utility.ActionFramework.Domain.Model
{
    public partial class Agent
    {
        private IEnumerable<Setting> settings;// = new IEnumerable<Setting>();
        private IEnumerable<Action> actions;// = new List<Action>();
        private List<Action> availableActions = new List<Action>();
        private IEnumerable<Schedule> schedules;// = new List<Schedule>();
        private IEnumerable<Log> logs;// = new List<Schedule>();
        private IEnumerable<App> apps;// = new List<Schedule>();  
        //private IEnumerable<App> availableApps;// = new List<Schedule>();  
        private Organization organization;// = new List<Schedule>();
        

        [DataMember]
        public IEnumerable<Setting> Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        [DataMember]
        public IEnumerable<Action> Actions
        {
            get { return actions; }
            set { actions = value; }
        }

        public List<Action> AvailableActions
        {
            get { return availableActions; }
            set { availableActions = value; }
        }

        [DataMember]
        public IEnumerable<Schedule> Schedules
        {
            get { return schedules; }
            set { schedules = value; }
        }

        public IEnumerable<Log> Logs
        {
            get { return logs; }
            set { logs = value; }
        }

        [DataMember]
        public IEnumerable<App> Apps
        {
            get { return apps; }
            set { apps = value; }
        }

        //[DapperIgnore]
        //public IEnumerable<App> AvailableApps
        //{
        //    get { return availableApps; }
        //    set { availableApps = value; }
        //}

        public IEnumerable<App> AvailableApps
        {
            get
            {
                return apps.Where(a => a.AgentApp == null || a.AgentApp.Installed.Equals(false));
            }
        }

        [DapperIgnore]
        //[DataMember]
        public IEnumerable<App> InstalledApps
        {
            get
            {
                return apps.Where(a => a.AgentApp != null && a.AgentApp.Installed);
            }
        }

        public Organization Organization
        {
            get { return organization; }
            set { organization = value; }
        }

        [DapperIgnore]
        public int LogCount
        {
            get 
            {
                if (logs != null)
                    return logs.Count();
                else
                    return 0;
            }
        }

        [DapperIgnore]
        public int ErrorCount
        {
            get 
            {
                var elogs = logs.Where(l => l.Type.Contains("Error"));
                if (elogs != null && elogs.Count() > 0)
                    return elogs.Count();
                else
                    return 0;
            }
        }

        //[DapperIgnore]
        //public SystemInformation SystemInformation
        //{
        //    get
        //    {
        //        if (this.SystemInfo != null)
        //            return this.SystemInfo.Deserialize<SystemInformation>();
        //        else
        //            return null;
        //    }
        //}
    }
}
