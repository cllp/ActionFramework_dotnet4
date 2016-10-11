using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ActionFramework.Classes;
using ActionFramework.Context;
using ActionFramework.Interfaces;
using ActionFramework.Model;
using System.IO;
using ActionFramework.Agent.DataSource;

namespace ActionFramework.Agent.Context
{
    public class TimerContext : IDisposable
    {
        public static System.Threading.Timer actionTimer;
        private static int timerInterval = 0;

        public static void Initialize(int interval)
        {
            ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Information, "TimerContext Initialized. Interval: '" + interval.ToString() + "'", Constants.EventLogId);
            Activator.SetLastRunDate();
            timerInterval = interval;
            actionTimer = new System.Threading.Timer(new TimerCallback(actionTimer_Elapsed), null, 0, timerInterval);
        }

        public static void actionTimer_Elapsed(object sender)
        {
            try
            {
                var lastRun = Activator.GetLastRunDate().ToString("yyyy-MM-dd HH:mm");
                ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Information, "ActionTimer Elapsed. Previous " + lastRun + " Interval " + AgentConfigurationContext.Current.Interval.ToString(), Constants.EventLogId);
                var ds = Activator.GetActionDataSource();
                
                var log = Activator.RunActions(new ActionListParameters(ds));
                Activator.SetLastRunDate();
            }
            catch (Exception ex)
            {
                ActionFactory.EventLogger(AgentConfigurationContext.Current.ServiceName).Write(EventLogEntryType.Error, "ActionTimer Elapsed with 'RunActions' caused an error. '" + ex.Message + "'", Constants.EventLogId);
            }

            //TODO: Printout activity in colsole / app window
        }

        /// <summary>
        /// Returns a agent configuration context that is applicable for this context.
        /// </summary>
        public static System.Threading.Timer Current
        {
            get
            {
                EnsureInitialized();
                return actionTimer;
            }
        }

        public static bool IsInitialized
        {
            get { return actionTimer != null; }
        }

        public static int TimerInterval
        {
            get { return timerInterval; }
        }

        private static void EnsureInitialized()
        {
            if (actionTimer == null)
            {
                throw new InvalidOperationException("ActionTimer provider has not been initialized.");
            }
        }

        public void Dispose()
        {
            actionTimer = null;
            actionTimer.Dispose();
        }

        public static void StopTimer()
        {
            actionTimer = null;
            //actionTimer.Dispose();
        }
    }
}
