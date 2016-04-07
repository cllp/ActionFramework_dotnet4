using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Server.Data.Interface;
using ActionFramework.Domain.Model;
using Dapper;
using ActionFramework.Server.Data.Helpers;
using System.Data;
using System.IO;
using System.IO.Compression;
using ActionFramework.Extensions;

namespace ActionFramework.Server.Data.Repository
{
    internal class AgentRepository : Repository<Agent>, IAgentRepository
    {
        public IEnumerable<Agent> GetAgents(int userId)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {

                var query = @"SELECT a.* 
                              FROM [Agent] a
                              INNER JOIN Organization o ON o.Id = a.OrganizationId
                              INNER JOIN [User] u ON o.Id = @UserId";

                return cn.Query<Agent>(query, new { UserId = userId });
            }
        }

        public override Agent GetById(int id)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                var query = @"SELECT a.*, o.* 
                              FROM [Agent] a
                              INNER JOIN Organization o ON o.Id = a.OrganizationId
                              INNER JOIN [User] u ON o.Id = u.OrganizationId
                              WHERE a.Id = @Id

                                --gets the action and the actions app
                                SELECT a.*, app.* 
                                FROM Action a 
                                LEFT JOIN App app ON a.AppId = app.Id
                                WHERE a.AgentId = @Id

                               --gets installed apps on the agent
                                SELECT app.*, aa.*
                                FROM App app 
                                INNER JOIN AgentApp aa ON aa.AppId = app.Id
                                WHERE aa.AgentId = @Id-- AND Installed = 1
                                UNION
                                --gets all available apps that is not currently installed on the agent
                                SELECT app.*, aa.*
                                FROM App app 
                                LEFT JOIN AgentApp aa ON aa.AppId = app.Id
                                WHERE aa.Id IS NULL-- OR Installed = 0

                                SELECT s.* 
                                FROM Setting s 
                                WHERE s.AgentId = @Id

                                --SELECT Top(25)* 
                                --FROM Log 
                                --WHERE AgentId = @Id 
                                --ORDER BY [Date] DESC";

                using (var multi = cn.QueryMultiple(query, new { Id = id }))
                {
                    var agent = multi.Read<Agent, Organization, Agent>((a, o) =>
                    {
                        a.Organization = o;
                        return a;
                    }).FirstOrDefault();

                    var actions = multi.Read<Domain.Model.Action, App, Domain.Model.Action>((act, app) =>
                    {
                        act.App = app;
                        return act;
                    }).ToList();

                    var apps = multi.Read<App, AgentApp, App>((a, aa) =>
                    {
                        a.AgentApp = aa;
                        return a;
                    }).ToList();

                    agent.Actions = actions;
                    agent.Apps = apps; //multi.Read<App>();
                    agent.Settings = multi.Read<Setting>();
                    
                    //agent.Logs = multi.Read<Log>();

                    //foreach (var log in agent.Logs)
                    //    log.Agent = agent;

                    //fill the available actions for all the installed apps of the agent
                    foreach (var app in agent.Apps.Where(a => a.AgentApp != null && a.AgentApp.Installed))
                    {
                        app.FillActionsAndProperties();

                        foreach(var act in app.Actions)
                        {
                            agent.AvailableActions.Add(act);
                        }
                    }

                    return agent;
                }
            }

        }
    }
}
