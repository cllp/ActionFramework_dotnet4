using ActionFramework.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DemoApp
{
    public class GetCustomers : ActionBase
    {
        public override object Execute()
        {
            try
            {
                //return XDocument.Parse(@"<Root><Child>Content</Child></Root>");

                string str =
                @"<?xml version=""1.0""?>
                <!-- comment at the root level -->
                <Root>
                    <Child>Content</Child>
                </Root>";

                XDocument doc = XDocument.Parse(str);
                return doc;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return ex.Message;
            }
        }
    }
}
