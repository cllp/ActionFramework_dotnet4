using ActionFramework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public class DemoActionReturnValue : ActionBase
    {

        public override object Execute()
        {
            return "output from DemoActionReturnValue";
        }
    }
}
