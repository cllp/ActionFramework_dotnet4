using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Woxion.Utility.ActionFramework.Model.Attributes;

namespace Woxion.Utility.ActionFramework.Domain.Model
{
    public partial class App
    {
        private IEnumerable<Action> actions;
        //private bool installed = false;

        //public bool Installed
        //{
        //    get { return installed; }
        //    set { installed = value; }
        //}
        

        [DapperIgnore]
        [DataMember]
        public IEnumerable<Action> Actions
        {
            get { return actions; }
            set { actions = value; }
        }

        [DapperIgnore]
        public AgentApp AgentApp { get; set; }

        //public DateTime? InstallDate { get; set; }
    }
}
