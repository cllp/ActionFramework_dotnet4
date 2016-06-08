using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Enum;

namespace ActionFramework.Model
{
    public partial class Property
    {
        private PropertyType propertyType;

        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int ActionId { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int PropertyTypeId { get; set; }
        [DataMember]
        public bool Enabled { get; set; }
        [DataMember]
        public string DataType { get; set; }
        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public PropertyType PropertyType
        {
            get { return PropertyType; }//(PropertyType)this.PropertyTypeId; }
            set { propertyType = value; }
        }
    }
}
