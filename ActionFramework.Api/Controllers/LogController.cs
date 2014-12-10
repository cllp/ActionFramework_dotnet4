using ActionFramework.Domain.Model;
using ActionFramework.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ActionFramework.Domain.Model.Api;

namespace ActionFramework.Api.Controllers
{
    [RoutePrefix("log")]
    public class LogController : BaseController
    {
        [Route("detail/{id}")]
        [HttpGet]
        public Log Detail(int id)
        {
            return logRep.GetById(id);
        }

        [Route("getlogswithpaging/{page}/{pagesize}")]
        [HttpGet]
        public IEnumerable<Log> GetLogsWithPaging(int page, int pagesize)
        {
            int total;
            var result = logRep.GetLogsWithPaging(page, pagesize, out total);
            return result;
        }

        [Route("write")]
        [AllowAnonymous]
        //[AcceptVerbs("POST", "PUT")]
        [HttpPost]
        public string WriteLog(LogModel logModel)
        {
            if (logModel == null)
                return "NULL";
            //actionRep.WriteLog(logModel.AgentCode, logModel.AgentSecret, logModel.Type, logModel.Description, logModel.Message);
            return "Message: " + logModel.Message;
        }

        [Route("getlogcount")]
        [HttpGet]
        public LogCounts GetLogCount()
        {
            return logRep.GetLogCounts();
        }

        [Route("test")]
        [HttpGet]
        public string test()
        {
            return "test";
        }


    }
}
