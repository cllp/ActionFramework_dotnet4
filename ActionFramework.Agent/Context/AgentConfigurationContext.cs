using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Classes;
using ActionFramework.Agent.Configuration;

namespace ActionFramework.Agent.Context
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
                ActionFactory.SysLog().Write("Info", "Initialize AgentConfigurationContext");
            }
        }

        public void Dispose()
        {
            config = null;
        }
    }
}
