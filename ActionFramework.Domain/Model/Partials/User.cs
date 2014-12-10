using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Woxion.Utility.ActionFramework.Domain.Enum;

namespace Woxion.Utility.ActionFramework.Domain.Model
{
    public partial class User
    {
        private Role role;

         [DataMember]
        public Role Role
        {
            get { return (Role)this.RoleId; }
            set { role = value; }
        }

        public bool RememberMe { get; set; }

        [DataMember]
        public Organization Organization { get; set; }

    }
}
