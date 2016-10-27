using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Logging
{
    public interface ILogElement
    {
        string Message { get; set; }
        //object Value { get; set; }
    }
}
