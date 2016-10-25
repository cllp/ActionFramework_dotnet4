using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ActionFramework.Model.EventLog;

namespace ActionFramework.SystemLogger
{
    public interface ISystemLogger
    {
        void Write(string logtype, string message);

        string GetLog();
    }
}
