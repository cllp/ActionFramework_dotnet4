using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Model
{
    public partial class App
    {
        private IEnumerable<Action> actions;

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int OrganizationId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public bool Private { get; set; }
        [DataMember]
        public bool Active { get; set; }
        [DataMember]
        public string Assembly { get; set; }
        [DataMember]
        public string AssemblyInfo { get; set; }
        [DataMember]
        public string ReleaseNotes { get; set; }
        [DataMember]
        public string Documentation { get; set; }
        [DataMember]
        public string DocumentationExtension { get; set; }
        [DataMember]
        public string Icon { get; set; }

        [DataMember]
        public IEnumerable<Action> Actions
        {
            get { return actions; }
            set { actions = value; }
        }
    }
}
