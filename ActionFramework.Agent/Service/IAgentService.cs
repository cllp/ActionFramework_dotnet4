using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ActionFramework.Domain.Model;
using ActionFramework.Entities;

namespace ActionFramework.Agent
{
    [ServiceContract]
    public interface IAgentService
    {
        [OperationContract]
        //[WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json, UriTemplate = "/EchoWithGet?s(={value}")]
        [WebGet(ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Xml, UriTemplate = "/EchoWithGet?s={s}")]
        //[WebGet(ResponseFormat = WebMessageFormat.Json)]
        string EchoWithGet(string s);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Xml)]
        string Run();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Xml)]
        string RunConfiguration(string configurationFile);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Xml)]
        string StopTimer();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Xml)]
        string StartTimer(int interval);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Xml)]
        int IsInitialized();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Xml)]
        string TimerInfo();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Xml)]
        SystemInformation GetSystemInformation();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Xml)]
        string Install(string assembly);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Xml)]
        Domain.Model.EventLog.EventList GetEventLogInfo(string logName, string level, string eventId, string timeSpanStart, string timeSpanEnd, int max);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Xml)]
        void StopService();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Xml)]
        string RefreshService();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Xml)]
        string RestartService();
    }
}

