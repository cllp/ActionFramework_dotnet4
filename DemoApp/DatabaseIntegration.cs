using ActionFramework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TestApp
{
    public class DatabaseIntegration : ActionBase
    {
        public string ConnectionString { get; set; }

        public string EmailReceiver { get; set; }

        public string ReportFormat { get; set; }

        public override object Execute()
        {

            //pseudocode.
            return HandleSuccess(); 
        }
    }
}
