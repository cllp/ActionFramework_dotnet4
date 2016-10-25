using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Interfaces;
using ActionFramework.Logging;

namespace ActionFramework.Model.LogModel
{
    public class ErrorLog: ActionLogBase, IExceptionLog
    {
        public string StackTrace { get; set; }
        public string ExceptionMessage { get; set; }
        public string Source { get; set; }
    }
}
