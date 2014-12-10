using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Domain.Interface;

namespace ActionFramework.Domain.Model.LogModel
{
    public class ErrorLog: ActionLogBase, IExceptionLog
    {
        public string StackTrace { get; set; }
        public string ExceptionMessage { get; set; }
        public string Source { get; set; }
    }
}
