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
using ActionFramework.Domain.Interface;
using System.Xml.Linq;
using ActionFramework.Domain.Model.LogModel;

namespace ActionFramework.Server.Data.Repository
{
    internal class LogRepository : Repository<Log>, ILogRepository
    {
        public LogCounts GetLogCounts()
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                var logCounts = new LogCounts();
                logCounts.AddRange(cn.Query<LogCount>("LogCount", commandType: CommandType.StoredProcedure).ToList());
                return logCounts;
            }
        }

        public IEnumerable<Log> GetLogsWithPaging(int page, int pagesize, out int total)
        {
            //set the first page to start from 1 instead of 0
            if(page > 0)
                page--;

            /*
                declare @page int, @pagesize int
                    set @pagesize = 10
                    set @page = (@pagesize * 0) 
             */
            string query = @"SELECT Id, [Date], [Type], [Description] FROM [Log] ORDER BY id DESC OFFSET @Page ROWS FETCH NEXT @PageSize ROWS ONLY;";
            
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                total = cn.Query<int>("SELECT Count(Id) FROM [Log]").FirstOrDefault();
                return cn.Query<Log>(query, new {Page = page, PageSize = pagesize, });
            }
        }

        public Log GetLatestByAgentId(int agentId)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                var query = @"SELECT TOP(1) l.*, a.* 
                              FROM [Log] l 
                              INNER JOIN Agent a ON l.AgentId = a.Id
                              WHERE a.Id = @AgentId
                              ORDER BY l.Date DESC";

                using (var multi = cn.QueryMultiple(query, new { AgentId = agentId }))
                {

                    var log = multi.Read<Domain.Model.Log, Agent, Domain.Model.Log>((l, a) =>
                    {
                        l.Agent = a;
                        return l;
                    }).FirstOrDefault();

                    log.Serialize();

                    return log; //SerializeLog(log, log.Message);
                }
            }
        }

        public override Log GetById(int id)
        {
            using (SqlConnection cn = SqlHelper.GetOpenConnection())
            {
                var query = @"SELECT l.*, a.* 
                              FROM Log l
                              INNER JOIN [Agent] a ON l.AgentId = a.Id
                              WHERE l.Id = @Id";

                using (var multi = cn.QueryMultiple(query, new { Id = id }))
                {
                    return multi.Read<Log, Agent, Log>((l, a) =>
                    {
                        l.Agent = a;
                        return l;
                    }).FirstOrDefault();
                }
            }

        }

        //public Log SerializeLog(Log log, string message)
        //{
        //    if (log == null)
        //        return null;

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
        //                    obj.Created = element.Attribute("Created").Value;

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
        //        obj.Created = element.Attribute("Created").Value;

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
    }
}
