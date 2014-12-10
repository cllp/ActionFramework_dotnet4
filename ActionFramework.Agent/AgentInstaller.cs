using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Classes;
using ActionFramework.Context;
using ActionFramework.Interfaces;

namespace ActionFramework.Agent
{
    [RunInstaller(true)]
    public class AgentInstaller : Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;
        //private IAgentConfiguration agentConfig = Activator.GetConfiguration();

        public AgentInstaller()
        {
            //if (!AgentConfigurationContext.IsInitialized)
            //{
            //    ActionFactory.EventLogger().Write(System.Diagnostics.EventLogEntryType.Information, "Initialize AgentConfigurationContext from AgentInstaller", Constants.EventLogId);
            //    AgentConfigurationContext.Initialize(new AgentConfiguration());
            //}

            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
            service.ServiceName = AgentConfigurationContext.Current.ServiceName;
            service.DisplayName = AgentConfigurationContext.Current.DisplayName;
            service.Description = AgentConfigurationContext.Current.ServiceDescription;
            
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}
