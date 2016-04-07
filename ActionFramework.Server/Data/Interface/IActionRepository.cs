using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ActionFramework.Domain.EF;
using ActionFramework.Domain.Model;

namespace ActionFramework.Server.Data.Interface
{
    public interface IActionRepository : IRepository<Domain.Model.Action>
    {
        Agent GetAgentActions(string id);

        IEnumerable<Domain.Model.Action> GetActionsByAgentId(int agentId);

        bool SaveActionProperty(int actionId, string property, string dataType, string value);

        int SaveSetting(int agentId, int settingId, string name, string value, string dataType);

        int CreateSetting(int agentId, string name, string value, string dataType);

        bool WriteLog(string agentCode, string agentSecret, string type, string description, string message);

        bool UpdateInstallApp(string agentCode, string agentSecret, int appId, bool installed);

        bool AddResource(string agentCode, string agentSecret, int actionId, string name, string type, string format, string obj);

        ////bool AddApp(int OrganizationId, string assembly, string documentation, string documentationExtension, bool isPrivate);
        //bool AddApp(App app);

        ////bool UpdateApp(int appId, string releaeNotes, string documentation, string documentationExtension, string icon, bool isPrivate);
        //bool UpdateApp(App app);

        //List<Log> GetLog(string agentCode, string agentSecret);

        //List<Log> GetLog(string agentCode, string agentSecret, DateTime thisDate);

        //List<Log> GetLog(string agentCode, string agentSecret, DateTime fromDate, DateTime toDate);

        //Log GetLog(int id);

        User GetUserData(User user);

        Organization GetOrganizationStats(Organization Organization);
    }
}
