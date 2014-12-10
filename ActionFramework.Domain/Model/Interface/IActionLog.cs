using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Woxion.Utility.ActionFramework.Domain.Interface
{
    public interface IActionLog : ILogBase
    {
        string ActionId { get; set; }
        string ActionType { get; set; }
        string Assembly { get; set; }
    }
}
