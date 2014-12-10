using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Model.Attributes;

namespace ActionFramework.Domain.Model
{
    public partial class Action
    {
        private IEnumerable<Property> properties;// = new IEnumerable<Setting>();
        private IEnumerable<Resource> resources;// = new IEnumerable<Setting>();

        [DapperIgnore]
        [DataMember]
        public App App { get; set; }

        [DapperIgnore]
        [DataMember]
        public Agent Agent { get; set; }

        [DapperIgnore]
        //[DataMember]
        public string Status
        {
            get
            {
                if (this.Enabled)
                    return "Active";
                else
                    return "Disabled";
            }
        }

        [DapperIgnore]
        [DataMember]
        public IEnumerable<Property> Properties
        {
            get { return properties; }
            set { properties = value; }
        }

        [DapperIgnore]
        [DataMember]
        public IEnumerable<Resource> Resources
        {
            get { return resources; }
            set { resources = value; }
        }

        public IEnumerable<Property> GetDiscoveredProperties()
        {
            if (this.App != null)
            {
                var action = App.Actions.Where(a => a.Type.Equals(this.Type.Trim())).FirstOrDefault();
                if (action != null)
                { 
                    //var discovered = action.properties;

                    foreach (var d in action.properties) //discovered
                    {
                        var prop = this.properties.Where(p => p.Name.Trim().Equals(d.Name.Trim())).FirstOrDefault();
                        if (prop != null)
                        {
                            d.Value = prop.Value;
                        }
                    }

                    return action.properties;
                }
            }

            return null;
        }
    }
}
