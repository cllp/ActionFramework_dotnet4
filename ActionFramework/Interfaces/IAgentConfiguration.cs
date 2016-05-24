using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Entities;
using ActionFramework.Enum;

namespace ActionFramework.Interfaces
{
    public interface IAgentConfiguration
    {
        string ConfigurationFile { get; }

        string ConfigurationPath { get; }

        string AgentId { get; }
        string ServiceName { get; }
        string ServiceDescription { get; }
        string DisplayName { get; }
        //string WebApiUrl { get; set; }
        //string AgentUrl { get; set; }
        //string AgentUrl { get; set; }
        string ServerUrl { get; set; }
        string LocalUrl { get; set; }

        string DropFolder { get; set; }
        //string AgentCode { get; set; }
        //string AgentSecret { get; set; }
        //DataSourceLocation DataSourceLocation { get; set; }
        //DataSourceFormat DataSourceFormat { get; set; }
        string ActionFile { get; set; }
        //bool LogToDisk { get; set; }
        //bool LogRemote { get; set; }
        bool Debug { get; set; }
        RunMode Mode { get; set; }
        //DateTime LastRunDate { get; set; }
        int Interval { get; set; }

        bool UpdateSetting(string key, object value);

        string DirectoryPath { get; }
    }
}