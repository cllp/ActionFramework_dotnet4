using ActionFramework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Logging
{
    public class ActionResultLog : ILogElement
    {
        private string message;
        private IActionDataSource dataSource;
        private int total;
        private int agentExecute;
        private int internalActionExecute;
        private int failed;
        private string runtime;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public IActionDataSource DataSource
        {
            get { return dataSource; }
            set { dataSource = value; }
        }

        public int Total
        {
            get { return total; }
            set { total = value; }
        }

        public int AgentExecute
        {
            get { return agentExecute; }
            set { agentExecute = value; }
        }

        public int InternalActionExecute
        {
            get { return internalActionExecute; }
            set { internalActionExecute = value; }
        }

        public int Failed
        {
            get { return failed; }
            set { failed = value; }
        }

        public string Runtime
        {
            get { return runtime; }
            set { runtime = value; }
        }

        public ActionResultLog(string message)
        {
            this.message = message;
        }

    }
}
