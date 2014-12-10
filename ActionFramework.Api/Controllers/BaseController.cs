using ActionFramework.Api.Filters;
using ActionFramework.Data;
using ActionFramework.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ActionFramework.Api.Controllers
{
    [ApiExceptionFilter]
    public class BaseController : ApiController
    {
        public static IActionRepository actionRep = DataFactory.ActionRepository;
        public static IUserRepository userRep = DataFactory.UserRepository;
        public static IAppRepository appRep = DataFactory.AppRepository;
        public static IAgentRepository agentRep = DataFactory.AgentRepository;
        public static ILogRepository logRep = DataFactory.LogRepository;
    }
}
