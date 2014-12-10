using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Classes;
using ActionFramework.Interfaces;

namespace ActionFramework.Context
{
    public class AgentConfigurationContext : IDisposable
    {
        private static IAgentConfiguration config;

        public static void Initialize(IAgentConfiguration configProvider)
        {
            config = configProvider;
        }

        /// <summary>
        /// Returns a agent configuration context that is applicable for this context.
        /// </summary>
        public static IAgentConfiguration Current
        {
            get
            {
                EnsureInitialized();

                return config;
            }
        }

        public static bool IsInitialized
        {
            get { return config != null; }
        }

        private static void EnsureInitialized()
        {
            if (config == null)
            {
                
                config = new AgentConfiguration();
                ActionFactory.EventLogger(config.ServiceName).Write(System.Diagnostics.EventLogEntryType.Information, "Initialize AgentConfigurationContext", Constants.EventLogId);
            }
        }

        public void Dispose()
        {
            config = null;
        }
    }
}
