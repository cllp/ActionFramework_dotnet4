using ActionFramework.Server.Data;
using ActionFramework.Server.Data.Interface;
using ActionFramework.Server.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ActionFramework.Server.Api
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
