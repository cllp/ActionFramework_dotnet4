using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ActionFramework.Domain.EF;
using ActionFramework.Domain.Model;

namespace ActionFramework.Server.Data.Interface
{
    public interface IAppRepository : IRepository<App>
    {
        int AppExists(string name, string version);

        int Install(int agentId, int appId, bool installed);

        IEnumerable<App> GetByAgentId(int agentId);

        IEnumerable<Domain.Model.Action> GetAvailableActionsByAgentId(int agentId);
    }
}
