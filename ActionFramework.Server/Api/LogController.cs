using ActionFramework.Classes;
using ActionFramework.Domain.Model;
using ActionFramework.Domain.Model.Api;
using ActionFramework.Server.LogWriter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Linq;

namespace ActionFramework.Server.Api
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
        public HttpResponseMessage Write(LogModel model)
        {
            ILogWriter xmlLogWriter = new XmlLogWriter(model);
            var saveToXmlResult = xmlLogWriter.Write;

            //ILogWriter dbLogWriter = new XmlLogWriter(model);
            //var saveToDbResult = dbLogWriter.Write;

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(saveToXmlResult, Encoding.UTF8, "text/html")
            };
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
