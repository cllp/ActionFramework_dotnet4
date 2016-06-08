using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Interfaces
{
    public interface IExceptionLog
    {
        string StackTrace { get; set; }
        string ExceptionMessage { get; set; }
        string Source { get; set; }
    }
}
