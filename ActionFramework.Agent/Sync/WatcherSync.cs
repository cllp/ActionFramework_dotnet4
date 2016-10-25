using ActionFramework.Classes;
using ActionFramework.Context;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Agent.Sync
{
    public class WatcherSync : ISync
    {
        public bool SyncLog(string filePath)
        {
            try
            {
                //get the extension
                var fileName = Path.GetFileName(filePath);
                var fileExtension = Path.GetExtension(filePath);
                var compressedFile = ActionFactory.Compression.CompressFile(File.ReadAllBytes(filePath));

                //todo: sync the file
                //ActionFactory.SysLog().Write("Info", "Syncwatcher Event. compressed file: " + compressedFile);

                var postclient = new RestClient(AgentConfigurationContext.Current.ServerUrl);
                var postrequest = new RestRequest("api/agent/runaction?name=writelog", Method.POST);
                postrequest.RequestFormat = DataFormat.Json;

                var body = new object[2];
                body[0] = compressedFile;
                body[1] = AgentConfigurationContext.Current.AgentId; //agentId

                postrequest.AddBody(body);

                var response = postclient.Execute(postrequest);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ActionFactory.SysLog().Write("Info", "Syncwatcher Event. Response: " + response.StatusCode.ToString());
                    return true;
                }
                else
                {
                    ActionFactory.SysLog().Write("Warning", string.Format("Syncwatcher Event. Statuscode: '{0}', {1}", response.StatusCode.ToString(), response.ErrorMessage));
                    return false;
                }
            }
            catch (Exception ex)
            {
                ActionFactory.SysLog().Write("Error", string.Format("Syncwatcher Event failed. Uri '{0}' {1} ", AgentConfigurationContext.Current.ServerUrl, ex.Message));
                return false;
            }
        }
    }
}
