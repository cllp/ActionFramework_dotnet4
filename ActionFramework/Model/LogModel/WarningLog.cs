using ActionFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Model.LogModel
{
    public class WarningLog : ActionLogBase, IExceptionLog
    {
        public string StackTrace { get; set; }
        public string ExceptionMessage { get; set; }
        public string Source { get; set; }
    }
}
