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
using System.Xml.Linq;
using ActionFramework.Domain.Model.LogModel;
using ActionFramework.Domain.Interface;
using System.Reflection;
using ActionFramework.Classes;
using ActionFramework.Extensions;

namespace ActionFramework.Server.Data.Repository
{
    internal class ActionRepository : Repository<Domain.Model.Action>, IActionRepository
    {
        public override Domain.Model.Action GetById(int id)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                var query = @"SELECT act.*, agt.*, app.* 
                              FROM [Action] act
                              INNER JOIN [Agent] agt ON act.AgentId = agt.Id
                              INNER JOIN [App] app ON act.AppId = app.Id
                              WHERE act.Id = @Id
                                
                              SELECT p.* FROM 
                              Property p
                              WHERE p.ActionId = @Id

                              SELECT r.* FROM 
                              Resource r
                              WHERE r.ActionId = @Id";

                using (var multi = cn.QueryMultiple(query, new { Id = id }))
                {
                    var action = multi.Read<Domain.Model.Action, Agent, App, Domain.Model.Action>((act, agt, app) =>
                    {
                        act.Agent = agt;
                        act.App = app;
                        return act;
                    }).FirstOrDefault();

                    if (action == null)
                        throw new Exception(string.Format("Query returned null for action id: '{0}', check related app in the database", id));

                    action.Properties = multi.Read<Property>();
                    action.Resources = multi.Read<Resource>();

                    action.App.FillActionsAndProperties();

                    return action;
                }
            }
        }

        public bool SaveActionProperty(int actionId, string property, string dataType, string value)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                try
                {
                    var query = @"
                            UPDATE [Property] SET [Value] = @Value
                            WHERE ActionId= @ActionId AND Name = @Name
                            IF @@ROWCOUNT=0
                            INSERT INTO [Property] (ActionId, Name, Value, DataType) 
                            VALUES (@ActionId, @Name, @Value, @DataType)";

                    var id = cn.Execute(query, new { ActionId = actionId, Name = property.Trim(), DataType = dataType.Trim(), Value = value.Trim() });
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public int SaveSetting(int agentId, int settingId, string name, string value, string dataType)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                try
                {
                    var query = String.Empty;

                    if (settingId > 0)
                    {
                        query = @"
                            UPDATE [Setting] SET Name = @Name, [Value] = @Value, [DataType] = @DataType
                            WHERE Id= @Id";
                    }
                    else
                    {
                        query = @"
                            INSERT INTO [Setting] (AgentId, Name, Value, DataType) 
                            VALUES (@AgentId, @Name, @Value, @DataType)";
                    }

                    var id = cn.Execute(query, new { Id = settingId, Name = name.Trim(), DataType = dataType.Trim(), Value = value.Trim() });
                    return id;
                }
                catch
                {
                    throw;
                }
            }
        }

        public int CreateSetting(int agentId, string name, string value, string dataType)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                var returnid = -1;

                try
                {
                    var query = String.Empty;

                    query = @"
                            IF NOT EXISTS (SELECT Id FROM [Setting] WHERE AgentId = @AgentId AND Name = @Name)
                            BEGIN
                            INSERT INTO [Setting] (AgentId, Name, Value, DataType) 
                            VALUES (@AgentId, @Name, @Value, @DataType)
                            END";

                    cn.Execute(query, new { AgentId = agentId, Name = name.Trim(), Value = value.Trim(), DataType = dataType.Trim() });
                    SqlHelper.SetIdentity<int>(cn, id => returnid = id);

                    return returnid;
                }
                catch
                {
                    //throw;
                    return returnid;
                }
            }
        }

        public Agent GetAgentActions(string id)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                //var id = cn.Query<int>(@"SELECT Id FROM Agent c WHERE c.agentCode = @agentCode AND c.agentSecret = @agentSecret", new { agentCode = agentCode, agentSecret = agentSecret }).FirstOrDefault();

                var query = @"SELECT * FROM Agent WHERE Id = @Id
                              SELECT * FROM Setting WHERE AgentId = @Id
                              
                              --not installed  --131218. take the installed instead 
                              SELECT App.* 
                              FROM App 
                              INNER JOIN AgentApp ON App.Id = AgentApp.AppId
                              INNER JOIN Agent ON Agent.Id = @Id
                              WHERE AgentApp.Installed = 1
                              
                              SELECT * FROM Schedule WHERE AgentId = @Id
                              --SELECT sch.* FROM Schedule sch 
							  --INNER JOIN [Action] a ON sch.ActionId = a.Id
							  --WHERE a.AgentId = @Id
                              
                              SELECT * FROM Action WHERE AgentId = @Id AND Enabled = 1
                              SELECT p.* FROM Property p INNER JOIN Action a ON p.ActionId = a.Id WHERE a.AgentId = @Id AND a.Enabled = 1
                              SELECT r.* FROM Resource r INNER JOIN Action a ON r.ActionId = a.Id WHERE a.AgentId = @Id AND a.Enabled = 1";

                using (var multi = cn.QueryMultiple(query, new { Id = id }))
                {
                    var agent = multi.Read<Agent>().FirstOrDefault();
                    agent.Settings = multi.Read<Setting>();
                    agent.Apps = multi.Read<App>();
                    agent.Schedules = multi.Read<Schedule>();
                    agent.Actions = multi.Read<ActionFramework.Domain.Model.Action>();
                    var properties = multi.Read<Property>();
                    var resources = multi.Read<Resource>();

                    foreach (var action in agent.Actions)
                    {
                        action.Properties = properties.Where(p => p.ActionId.Equals(action.Id));
                        action.Resources = resources.Where(r => r.ActionId.Equals(action.Id));
                    }

                    if (InSchedule(agent))
                        return agent;
                    else
                        return null;
                }
            }
        }

        public IEnumerable<Domain.Model.Action> GetActionsByAgentId(int agentId)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                var query = @"
                              SELECT * FROM Action WHERE AgentId = @Id
                              SELECT p.* FROM Property p INNER JOIN Action a ON p.ActionId = a.Id WHERE a.AgentId = @Id
                              SELECT r.* FROM Resource r INNER JOIN Action a ON r.ActionId = a.Id WHERE a.AgentId = @Id";

                using (var multi = cn.QueryMultiple(query, new { Id = agentId }))
                {
                    var actions = multi.Read<Domain.Model.Action>();
                    var properties = multi.Read<Property>();
                    var resources = multi.Read<Resource>();

                    foreach (var action in actions)
                    {
                        action.Properties = properties.Where(p => p.ActionId.Equals(action.Id));
                        action.Resources = resources.Where(r => r.ActionId.Equals(action.Id));
                    }

                    return actions;
                }
            }
        }

        private bool InSchedule(Agent agent)
        {
            //bool 
            foreach (var s in agent.Schedules)
            {
                //if(client.Schedules)
            }

            return true;
        }

        public bool UpdateInstallApp(string agentCode, string agentSecret, int appId, bool installed)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                var id = cn.Query<int>(@"SELECT Id FROM Agent c WHERE c.agentCode = @agentCode AND c.agentSecret = @agentSecret", new { agentCode = agentCode, agentSecret = agentSecret }).FirstOrDefault();

                var query = @"UPDATE [AgentApp] SET Installed = @Installed, InstallDate = @InstallDate WHERE AppId = @AppId AND AgentId = @AgentId";


                var par = new DynamicParameters();
                par.Add("Installed", installed);
                par.Add("InstallDate", DateTime.Now);
                par.Add("AppId", appId);
                par.Add("AgentId", id);

                int result = cn.Execute(query, par);
                return true;
            }
        }

        public bool WriteLog(string agentCode, string agentSecret, string type, string description, string message)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                var id = cn.Query<int>(@"SELECT Id FROM Agent c WHERE c.agentCode = @agentCode AND c.agentSecret = @agentSecret", new { agentCode = agentCode, agentSecret = agentSecret }).FirstOrDefault();

                var query = @"INSERT INTO [Log] (AgentId, Type, Description, Message) Values (@AgentId, @Type, @Description, @Message)";

                var decompressed = DecompressString(message);

                var par = new DynamicParameters();
                par.Add("AgentId", id);
                par.Add("Date", DateTime.Now);
                par.Add("Type", type);
                par.Add("Description", description);
                par.Add("Message", decompressed);

                int result = cn.Execute(query, par);
                return true;
            }
        }

        public bool AddResource(string agentCode, string agentSecret, int actionId, string name, string type, string format, string obj)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                var id = cn.Query<int>(@"SELECT Id FROM Agent c WHERE c.agentCode = @agentCode AND c.agentSecret = @agentSecret", new { agentCode = agentCode, agentSecret = agentSecret }).FirstOrDefault();

                var query = @"INSERT INTO [Resource] (ActionId, Name, Type, format, obj) Values (@ActionId, @Name, @Type, @Format, @Obj)";

                var par = new DynamicParameters();
                par.Add("ActionId", actionId);
                par.Add("Name", name);
                par.Add("Type", type);
                par.Add("Format", format);
                par.Add("Obj", obj);

                int result = cn.Execute(query, par);
                return true;
            }
        }

        public User GetUserData(User user)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {

                var agentIds = cn.Query<int>("SELECT Id FROM Agent WHERE OrganizationId = @OrganizationId", new { OrganizationId = user.OrganizationId }).ToList();

                var agentIdList = string.Join(", ", agentIds);

                var query = @"
                            SELECT a.*
                            FROM Agent a 
                            WHERE a.OrganizationId = @OrganizationId

                            SELECT s.*
                            FROM Schedule s
                            INNER JOIN Agent agt ON s.AgentId = agt.Id
                            WHERE agt.OrganizationId = @OrganizationId

                            SELECT 
                            p.*
                            FROM Property p
                            INNER JOIN [Action] act ON act.Id = p.ActionId
                            INNER JOIN Agent agt ON act.AgentId = agt.Id
                            WHERE agt.OrganizationId = @OrganizationId

                            SELECT 
                            r.*
                            FROM [Resource] r
                            INNER JOIN [Action] act ON act.Id = r.ActionId
                            INNER JOIN Agent agt ON act.AgentId = agt.Id
                            WHERE agt.OrganizationId = @OrganizationId

                            SELECT 
                            act.*,
                            App.*
                            FROM [Action] act
                            INNER JOIN App ON act.AppId = App.Id
                            INNER JOIN Agent agt ON act.AgentId = agt.Id
                            WHERE agt.OrganizationId = @OrganizationId

					        SELECT 
								TOP (10)l.*, a.*
								FROM Log l
								INNER JOIN Agent a ON l.AgentId = a.Id
								--INNER JOIN Organization o ON a.OrganizationId = o.Id
								WHERE a.Id IN (" + agentIdList + ") ORDER BY l.Date DESC";

                //SELECT Top(25)* FROM Log WHERE AgentId IN (" + agentIdList + ") ORDER BY [Date] DESC";

                using (var multi = cn.QueryMultiple(query, new { OrganizationId = user.OrganizationId }))
                {
                    var agents = multi.Read<Agent>();
                    var schedules = multi.Read<Schedule>();
                    var properties = multi.Read<Property>();
                    var resources = multi.Read<Resource>();

                    var actions = multi.Read<Domain.Model.Action, App, Domain.Model.Action>((act, app) =>
                    {
                        act.App = app;
                        return act;
                    }).ToList();

                    foreach (var act in actions)
                    {
                        act.Properties = properties.Where(a => a.ActionId.Equals(act.Id));
                        act.Resources = resources.Where(r => r.ActionId.Equals(act.Id));
                    }

                    var logs = multi.Read<Log, Agent, Log>((l, a) =>
                    {
                        l.Agent = a;
                        return l;
                    }).ToList();

                    foreach (var log in logs)
                    {
                        log.Agent = agents.Where(a => a.Id.Equals(log.AgentId)).FirstOrDefault();
                    }

                    foreach (var a in agents)
                    {
                        a.Logs = logs.Where(l => l.AgentId.Equals(a.Id));
                        a.Actions = actions.Where(act => act.AgentId.Equals(a.Id));
                        a.Schedules = schedules.Where(s => s.AgentId.Equals(a.Id));
                        a.Organization = user.Organization;
                    }

                    if (agents != null && agents.Count() > 0)
                        user.Organization.Agents = agents;

                    user.Organization.RecentLogs = logs;

                    return user;
                }
            }
        }

        public Organization GetOrganizationStats(Organization Organization)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                var query = @"
                                SELECT a.*
                                FROM Agent a 
                                WHERE a.OrganizationId = @OrganizationId

                                SELECT l.* 
                                FROM [Log] l 
                                INNER JOIN Agent a ON a.Id = l.AgentId
                                WHERE a.OrganizationId = @OrganizationId
                                ";

                //var result = cn.Query<Agent>(query, new { Id = userId }).ToList();
                using (var multi = cn.QueryMultiple(query, new { OrganizationId = Organization.Id }))
                {
                    var agents = multi.Read<Agent>();
                    var logs = multi.Read<Log>();

                    foreach (var a in agents)
                        a.Logs = logs.Where(l => l.AgentId.Equals(a.Id));

                    Organization.Agents = agents;

                    return Organization;
                }
            }
        }

        //public Log GetLog(int id)
        //{
        //    using (SqlConnection cn = SqlHelper.GetOpenConnection())
        //    {
        //        var log = cn.Query<Log>(@"SELECT * FROM Log WHERE Id = @Id", new { Id = id }).FirstOrDefault();

        //        SerializeLog(log);

        //        return log;
        //    }
        //}

        //private Log SerializeLog(Log log)
        //{
        //    var doc = XDocument.Parse(log.Message);

        //    foreach (var element in doc.Element("ActionLog").Element("Log").Elements())
        //    {
        //        switch (element.Name.ToString())
        //        {
        //            case "Error":
        //                {
        //                    var obj = new ErrorLog();
        //                    FillProperties(obj, element);
        //                    FillExceptionDetails(obj, element);
        //                    log.ErrorLogs.Add(obj);
        //                    break;
        //                }
        //            case "Warning":
        //                {
        //                    var obj = new WarningLog();
        //                    FillProperties(obj, element);
        //                    FillExceptionDetails(obj, element);
        //                    log.WarningLogs.Add(obj);
        //                    break;
        //                }
        //            case "Agent":
        //                {
        //                    var obj = new AgentLog();
        //                    obj.Message = element.Element("Message").Value;
        //                    obj.Created = Convert.ToDateTime(element.Attribute("Created").Value);

        //                    if (element.Element("Assembly") != null)
        //                        obj.AssemblyInfo = element.Element("Assembly").Value;

        //                    obj.Count = element.Element("Count").Value;

        //                    if (element.Element("DataSource") != null)
        //                        obj.DataSource = element.Element("DataSource").Value;

        //                    if (element.Element("Runtime") != null)
        //                        obj.Runtime = element.Element("Runtime").Value;

        //                    log.AgentLog = obj;
        //                    break;
        //                }
        //            case "Information":
        //                {
        //                    var obj = new InformationLog();
        //                    FillProperties(obj, element);
        //                    log.InformationLogs.Add(obj);
        //                    break;
        //                }
        //            default:
        //                {

        //                    break;
        //                }
        //        }

        //    }

        //    return log;
        //}

        //private IActionLog FillProperties(IActionLog obj, XElement element)
        //{
        //    if (element.Element("Message") != null)
        //        obj.Message = element.Element("Message").Value;

        //    if (element.Attribute("Created") != null)
        //        obj.Created = Convert.ToDateTime(element.Attribute("Created").Value);

        //    if (element.Attribute("ActionId") != null)
        //        obj.ActionId = element.Attribute("ActionId").Value;

        //    if (element.Attribute("ActionType") != null)
        //        obj.ActionType = element.Attribute("ActionType").Value;

        //    if (element.Attribute("Assembly") != null)
        //        obj.Assembly = element.Attribute("Assembly").Value;

        //    return obj;
        //}

        //private IExceptionLog FillExceptionDetails(IExceptionLog obj, XElement element)
        //{
        //    if (element.Element("Source") != null)
        //        obj.Source = element.Element("Source").Value;

        //    if (element.Element("StackTrace") != null)
        //        obj.StackTrace = element.Element("StackTrace").Value;

        //    if (element.Element("ExceptionMessage") != null)
        //        obj.ExceptionMessage = element.Element("ExceptionMessage").Value;

        //    return obj;
        //}

        //public List<Log> GetLog(string agentCode, string agentSecret)
        //{
        //    using (SqlConnection cn = SqlHelper.GetOpenConnection())
        //    {
        //        var id = cn.Query<int>(@"SELECT Id FROM Agent c WHERE c.agentCode = @agentCode AND c.agentSecret = @agentSecret", new { agentCode = agentCode, agentSecret = agentSecret }).FirstOrDefault();

        //        var query = @"SELECT * FROM Log WHERE AgentId = @AgentId";

        //        var par = new DynamicParameters();
        //        par.Add("AgentId", id);

        //        //par.Add("Date", DateTime.Now);
        //        //par.Add("Type", type);
        //        //par.Add("Description", description);
        //        //par.Add("Message", message);

        //        var result = cn.Query<Log>(query, par).ToList();
        //        return result;
        //    }
        //}

        //public List<Log> GetLog(string agentCode, string agentSecret, DateTime thisDate)
        //{
        //    throw new NotImplementedException();
        //}

        //public List<Log> GetLog(string agentCode, string agentSecret, DateTime fromDate, DateTime toDate)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Decompresses the string.
        /// </summary>
        /// <param name="compressedText">The compressed text.</param>
        /// <returns></returns>
        private string DecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }
    }
}
