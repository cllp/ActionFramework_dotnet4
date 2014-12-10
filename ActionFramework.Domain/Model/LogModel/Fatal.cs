using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Woxion.Utility.ActionFramework.Domain.Interface;

namespace Woxion.Utility.ActionFramework.Domain.Model.LogModel
{
    public class FatalLog: ActionLogBase, IExceptionLog
    {
        public string StackTrace { get; set; }
        public string ExceptionMessage { get; set; }
        public string Source { get; set; }
    }
}
