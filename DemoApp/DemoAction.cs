using ActionFramework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TestApp
{
    public class DemoAction : ActionBase
    {
        public string DemoProperty { get; set; }

        public string AnotherProperty { get; set; }

        public override object Execute()
        {
            Log.Info(DemoProperty);

            //the property value is the return value of another action
            Log.Info(AnotherProperty);
            
            //find another action and log the results
            Log.Info("Output from other action: " + FindActionById("5").Execute().ToString());

            //get customer xml
            var customers = FindActionByType("GetCustomers").Execute();
            Log.Info(customers.ToString());

            Log.Custom(new DemoLog() {
                Message = "Custom Message",
                Count = 25
            });

            return HandleSuccess(); 
        }
    }
}
