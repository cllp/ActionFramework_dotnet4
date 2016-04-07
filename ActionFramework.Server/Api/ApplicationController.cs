using ActionFramework.Server.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ActionFramework.Server.Api
{
    [RoutePrefix("application")]
    [ApiAuthorize]
    public class ApplicationController : BaseController
    {   
        [HttpGet]
        [Route("version")]
        [AllowAnonymous]
        public string Version()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
