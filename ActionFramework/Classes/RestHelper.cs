using ActionFramework.Context;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Classes
{
    public class RestHelper
    {
        private string url;
        private RestSharp.DataFormat format = RestSharp.DataFormat.Json;
        private Method method = Method.POST;
        private RestRequest request = new RestRequest();

        public RestRequest Request
        {
            get { return request; }
            set { request = value; }
        }
        
        public Method Method
        {
            get { return method; }
            set { method = value; }
        }
        
        public RestSharp.DataFormat Format
        {
            get { return format; }
            set { format = value; }
        }
        
        public RestHelper(string url)
        {
            this.url = url;
            //request.AddHeader("Accept", "application/json");
            request.Method = method;
            request.RequestFormat = format;
        }

        public RestHelper(string url, Method method)
        {
            this.url = url;
            this.method = method;
            request.AddHeader("Accept", "application/json");
            request.Method = method;
            request.RequestFormat = format;
            
        }

        public RestHelper(string url, string method)
        {

            this.url = url;
            this.method = (Method)System.Enum.Parse(typeof(Method), method);
            //request.AddHeader("Accept", "application/json");
            request.Method = this.method;
            request.RequestFormat = format;
        }

        public void AddHeader(string name, string value)
        {
            this.request.AddHeader(name, value);
        }

        public void AddParameter(string name, object value)
        {
            this.request.AddParameter(name, value);
        }

        public void AddBody(object value)
        {
            this.request.AddBody(value);
        }

        public IRestResponse Execute()
        {
            var apiurl = url;
            var client = new RestClient
            {
                BaseUrl = apiurl
            };

            var response = client.Execute(request);

            //if (response.StatusCode != System.Net.HttpStatusCode.OK)
            //    throw new Exception("REST Response Exception. Statuscode: '" + response.StatusCode + "'. Message: " + response.Content + ". Url: " + apiurl);

            return response;
        }

        //public HttpResponse Execute()
        //{
        //    var apiurl = url;
        //    var client = new RestClient
        //    {
        //        BaseUrl = apiurl
        //    };

        //    var response = client.Execute(request);

        //    if (response.StatusCode != System.Net.HttpStatusCode.OK)
        //        throw new Exception("REST Response Exception. Statuscode: '" + response.StatusCode + "'. Message: " + response.Content + ". Url: " + apiurl);

        //    return (HttpResponse)response;
        //}
    }
}
