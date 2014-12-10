using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Agent.Service
{
    public class AgentServiceAuthorizationManager : ServiceAuthorizationManager
    {
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            HttpResponseMessageProperty prop = new HttpResponseMessageProperty();
            prop.Headers.Add("Access-Control-Allow-Origin", "*");
            operationContext.OutgoingMessageProperties.Add(HttpResponseMessageProperty.Name, prop);
            return true;
        }
    }
}
