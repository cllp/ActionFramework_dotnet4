using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Server.Data.Helpers;
using ActionFramework.Server.Data.Interface;
using Dapper;
using ActionFramework.Domain.Model;
using System.Data;
using ActionFramework.Extensions;

namespace ActionFramework.Server.Data.Repository
{
    internal class AppRepository : Repository<App>, IAppRepository
    {
        public int AppExists(string name, string version)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                var query = @"IF EXISTS (SELECT Id FROM [App] WHERE Name = @Name AND Version = @Version)
                    SELECT Id FROM [App] WHERE Name = @Name AND Version = @Version
                ELSE
                    SELECT -1";

                var par = new DynamicParameters();
                par.Add("Name", name);
                par.Add("Version", version);

                return cn.Query<int>(query, par).FirstOrDefault();
            }
        }

        public override int Insert(App item)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                return cn.Insert(item);
            }
        }

        public int Install(int agentId, int appId, bool installed)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                var par = new DynamicParameters();
                par.Add("AgentId", agentId);
                par.Add("AppId", appId);

                int result = cn.Execute("[InstallApp]", par, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        public IEnumerable<App> GetByAgentId(int agentId)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                var query = @"SELECT 
                            app.Id, 
                            app.OrganizationId, 
                            app.Name, 
                            app.[Version], 
                            app.AssemblyInfo, 
                            app.Icon,
                            aa.*
                            FROM App app
                            INNER JOIN Agent a ON app.OrganizationId = a.OrganizationId 
                            LEFT JOIN AgentApp aa ON app.Id = aa.AppId 
                            WHERE a.Id = @AgentId
                            ORDER BY app.Id, aa.AppId";

                using (var multi = cn.QueryMultiple(query, new { AgentId = agentId }))
                {
                    var apps = multi.Read<App, AgentApp, App>((app, aa) =>
                    {
                        app.AgentApp = aa;
                        return app;
                    });

                    return apps;
                }
            }
        }

        public IEnumerable<Domain.Model.Action> GetAvailableActionsByAgentId(int agentId)
        {
            
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                List<Domain.Model.Action> actions = new List<Domain.Model.Action>();

                var query = @"SELECT DISTINCT
                            app.*
                            FROM App app
                            INNER JOIN Agent a ON app.OrganizationId = a.OrganizationId 
                            INNER JOIN AgentApp aa ON app.Id = aa.AppId
                            WHERE a.Id = @AgentId";

                var apps = cn.Query<App>(query, new { AgentId = agentId });

                foreach(var app in apps)
                {
                    //1. get the actions from app assembly
                    //2. foreach action in assembly -> create a new action

                    var ass = ActionFactory.Compression.DecompressAssembly(app.Assembly);
                    var assemblyInfo = app.FillActionsAndProperties();
                    app.AssemblyInfo = assemblyInfo.ToString();
                    
                    foreach(var action in ass.GetActionAndProperties(app))
                    {
                        action.AgentId = agentId;
                        action.AppId = app.Id;
                        action.App = app;

                        //dont add the action if te type is already added
                        if(actions.Find(a => a.Type.Equals(action.Type)) == null)
                        {
                            actions.Add(action);
                        }
                    }
                }

                return actions;
            }
        }
    }
}
