using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;

namespace ActionFramework.Server.Filters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            var ex = context.Exception;
            //log.Error(string.Format("Exception ({0})", ex.GetType().Name), context.Exception);
            var statusCode = GetStatusCode(ex);
            var msg = GetMessage(ex);
            Console.WriteLine("");
            Console.WriteLine("Error: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine("Uri: " + context.Request.RequestUri.AbsoluteUri);
            Console.WriteLine("Method: " + context.Request.Method.Method);
            Console.WriteLine("StatusCode: " + statusCode);
            Console.WriteLine("Message: " + msg);
            Console.WriteLine("");

            ActionFactory.EventLogger("Server").Write(System.Diagnostics.EventLogEntryType.Error, msg, 77);
            
            context.Response = context.Request.CreateResponse(GetStatusCode(ex), msg);
        }

        private HttpStatusCode GetStatusCode(Exception ex)
        {
            switch (ex.GetType().Name)
            {
                case "NotImplementedException":
                        return HttpStatusCode.NotImplemented;
                case "AuthenticationException":
                        return HttpStatusCode.Unauthorized;
                case "UnauthorizedAccessException":
                        return HttpStatusCode.Unauthorized;
                default:
                    return HttpStatusCode.InternalServerError;
            }
        }

        private string GetMessage(Exception ex)
        {
            if (ex.InnerException != null)
                return ex.Message + ". " + ex.InnerException;
            else
                return ex.Message;
        }
    }
}