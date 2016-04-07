using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ActionFramework.Domain.EF;
using ActionFramework.Domain.Model;

namespace ActionFramework.Server.Data.Interface
{
    public interface ILogRepository : IRepository<Log>
    {
        Log GetLatestByAgentId(int agentId);

        LogCounts GetLogCounts();

        IEnumerable<Log> GetLogsWithPaging(int page, int pagesize, out int total);

    }
}
