using ActionFramework.Server.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml.Linq;

namespace ActionFramework.Server.Api
{
    [RoutePrefix("test")]
    public class TestController : ApiController
    {
        [HttpGet]
        [Route("connect")]
        public HttpResponseMessage Connect()
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<object>(new
                {
                    Content = new StringContent("Connected to ActionFramework Server", Encoding.UTF8, "text/html")
                }, Configuration.Formatters.JsonFormatter)
            };
        }

        [HttpGet]
        [Route("profile")]
        //[Authorize]
        [ApiExceptionFilter]
        public HttpResponseMessage Profile()
        {
            throw new Exception("Exxxxxxl");

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<object>(new
                {
                    IsAuthenticated = true,
                    User = "Claes-Philip Staiger",
                    Email = "Claes-Philip@Staiger.se",
                }, Configuration.Formatters.JsonFormatter)
            };
        }

        [HttpGet]
        [Route("config/{agent}/{config}")]
        //[Authorize]
        [ApiExceptionFilter]
        public HttpResponseMessage Config(string agent, string config)
        {
            var doc = XDocument.Load(@"C:\Users\cllp\Dev\ActionFramework\ActionFramework.Agent\Config\config.xml");

            string compact = ActionFactory.Compression.CompressString(doc.Root.ToString());

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(compact, Encoding.UTF8, "text/html")
                //Content = new ObjectContent<string>(compact, Configuration.Formatters.JsonFormatter)
            };
        }
    }
}
