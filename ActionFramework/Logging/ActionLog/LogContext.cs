using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Interfaces;

namespace ActionFramework.Logging
{
    public class LogContext : IDisposable
    {
        private static ILogger logger;

        public static void Initialize(ILogger loggingProvider)
        {
            logger = loggingProvider;
        }

        ///// <summary>
        ///// Returns a log context that is applicable for this context.
        ///// </summary>
        //public static ILogger Current
        //{
        //    get
        //    {
        //        EnsureInitialized();

        //        return logger;
        //    }
        //}

        /// <summary>
        /// Returns a log context that is applicable for this context.
        /// </summary>
        public static ILogger Current()
        {
            EnsureInitialized();
            logger.Action = null; //set default to null TODO: if this is not current calling assembly, it is ok to set it to null
            return logger;
        }

        /// <summary>
        /// Returns a log context that is applicable for this context.
        /// </summary>
        public static ILogger Current(IAction action)
        {
            EnsureInitialized();
            logger.Action = action;
            return logger;
        }

        public static bool IsInitialized
        {
            get { return logger != null; }
        }

        private static void EnsureInitialized()
        {
            if (logger == null)
            {
                throw new InvalidOperationException("Logging provider has not been initialized.");
            }
        }

        public void Dispose()
        {
            logger = null;
        }
    }
}
