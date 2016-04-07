using CMT.Api.Filters;
using CMT.Core;
using CMT.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace CMT.Api.Controllers
{
    [ApiExceptionFilter]
    public class BaseController : ApiController
    {
        public IAppRepository appRepository = CoreFactory.AppRepository;
        public IUploadRepository uploadRepository = CoreFactory.UploadRepository;
        public IInputRepository inputRepository = CoreFactory.InputRepository;
        public ILog log = CoreFactory.Log;
    }
}