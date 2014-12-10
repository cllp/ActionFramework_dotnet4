using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Interfaces;
using ActionFramework.Classes;

namespace ActionFramework.Logging
{
    public class AssemblyLog : ApplicationInfo, ILogElement
    {
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public AssemblyLog(string message)
        {
            this.message = message;
        }
    }
}
