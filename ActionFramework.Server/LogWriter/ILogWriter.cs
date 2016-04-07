using ActionFramework.Domain.Model.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Server.LogWriter
{
    public interface ILogWriter
    {
        LogModel model
        {
            get;
            set;
        }
        string Write
        {
            get;
        }
    }
}
