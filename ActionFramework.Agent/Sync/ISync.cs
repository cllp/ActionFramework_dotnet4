using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Agent.Sync
{
    public interface ISync
    {
        bool SyncLog(string filePath);
    }
}
