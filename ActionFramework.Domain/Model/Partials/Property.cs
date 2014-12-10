using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Woxion.Utility.ActionFramework.Domain.Enum;
using Woxion.Utility.ActionFramework.Model.Attributes;

namespace Woxion.Utility.ActionFramework.Domain.Model
{
    public partial class Property
    {
        private PropertyType propertyType;

        [DapperIgnore]
        [DataMember]
        public PropertyType PropertyType
        {
            get { return (PropertyType)this.PropertyTypeId; }
            set { propertyType = value; }
        }
    }
}
