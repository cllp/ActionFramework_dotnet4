using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Server.Data.Interface;
using ActionFramework.Server.Data.Repository;

namespace ActionFramework.Server.Data
{
    public static class DataFactory
    {
        public static IActionRepository ActionRepository
        {
            get { return new ActionRepository(); }
        }

        public static IUserRepository UserRepository
        {
            get { return new UserRepository(); }
        }

        public static IAppRepository AppRepository
        {
            get { return new AppRepository(); }
        }

        public static IAgentRepository AgentRepository
        {
            get { return new AgentRepository(); }
        }

        public static ILogRepository LogRepository
        {
            get { return new LogRepository(); }
        }
    }
}
