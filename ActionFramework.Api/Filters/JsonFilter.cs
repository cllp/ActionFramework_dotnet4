using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace ActionFramework.Api.Filters
{
    public class JsonFilter: System.Web.Http.Filters.ActionFilterAttribute
    {
        Type _type;
        string _queryStringKey;
        public JsonFilter(Type type, string queryStringKey)
        {
            _type = type;
            _queryStringKey = queryStringKey;
        }
        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var json = HttpUtility.ParseQueryString(actionContext.Request.RequestUri.Query)[_queryStringKey];
            //var json = actionContext.Request.RequestUri.ParseQueryString()[_queryStringKey];
            var serializer = new JavaScriptSerializer();
            actionContext.ActionArguments[_queryStringKey] = serializer.Deserialize(json, _type);
        }
    }
}