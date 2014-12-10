using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionFramework.Model
{
    public class ActionProperty
    {
        private string name;

        public string Name
        {
            get { return name.Trim(); }
            set { name = value; }
        }

        private string value;

        public string Value
        {
            get { return this.value.Trim(); }
            set { this.value = value; }
        }

        public ActionProperty(string name, string value)
        {
            this.name = name;
            this.value = value;
        }
    }
}
