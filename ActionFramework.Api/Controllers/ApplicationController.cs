using ActionFramework.Api.Attributes;
using ActionFramework.Api.Controllers;
using ActionFramework.Api.Mapper;
using ActionFramework.Api.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Http;

namespace ActionFramework.Api.Controllers
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
