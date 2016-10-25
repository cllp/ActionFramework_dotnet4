using ActionFramework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Interfaces
{
    public interface IActionLog : ILogBase
    {
        string ActionId { get; set; }
        string ActionType { get; set; }
        string Assembly { get; set; }
    }
}
