using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Domain.Enum;
using ActionFramework.Model.Attributes;

namespace ActionFramework.Domain.Model
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
