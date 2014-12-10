using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Model.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class DapperIgnore : System.Attribute
    {
        public DapperIgnore()
        {
        }
    }
}
