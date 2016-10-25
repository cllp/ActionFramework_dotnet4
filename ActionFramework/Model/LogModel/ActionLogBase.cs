using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Interfaces;
using ActionFramework.Logging;

namespace ActionFramework.Model.LogModel
{
    public class ActionLogBase : IActionLog, ILogBase
    {
        private string message = String.Empty;
        private DateTime created;
        private string actionId = String.Empty;
        private string actionType = String.Empty;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public DateTime Created
        {
            get { return created; }
            set { created = value; }
        }
        public string ActionId
        {
            get { return actionId; }
            set { actionId = value; }
        }

        public string ActionType
        {
            get { return actionType; }
            set { actionType = value; }
        }

        private string assembly;

        public string Assembly
        {
            get { return assembly; }
            set { assembly = value; }
        }
    }
}
