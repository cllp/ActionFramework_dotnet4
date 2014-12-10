using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Domain.Interface
{
    public interface ILogBase
    {
        DateTime Created { get; set; }
        string Message { get; set; }
    }
}
