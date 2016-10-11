using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Logging;

namespace TestApp
{
    public class DemoLog : ILogElement
    {
        private int count = 0;

        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

    }
}
