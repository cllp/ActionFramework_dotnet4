using ActionFramework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApp
{
    public class OnDemandAction : ActionBase
    {
        public override object Execute()
        {
            return "Ondemand: " + DateTime.Now.ToLongTimeString();
        }
    }
}
