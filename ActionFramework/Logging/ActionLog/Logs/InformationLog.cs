using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Interfaces;

namespace ActionFramework.Logging
{
    public class InformationLog : ILogElement
    {
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public InformationLog(string message)
        {
            this.message = message;
        }
    }
}
