using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Enum;
using ActionFramework.Interfaces;
using ActionFramework.Logging;

namespace ActionFramework.Logging
{
    public class LogElements : List<ILogElement>
    {
        private string type;
        //private string message;
        private DateTime created = DateTime.Now;
        private IAction action;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        //public string Message
        //{
        //    get { return message; }
        //    set { message = value; }
        //}

        public DateTime Created
        {
            get { return created; }
            set { created = value; }
        }

        public IAction Action
        {
            get { return action; }
            set { action = value; }
        }

        public LogElements(string type)
        {
            this.type = type;
            //this.message = message;
        }

        public LogElements(string type, DateTime created)
        {
            this.type = type;
            //this.message = message;
            this.created = created;
        }

        public LogElements(string type, DateTime created, IAction action)
        {
            this.type = type;
            //this.message = message;
            this.created = created;
            this.action = action;
        }

        public LogElements()
        {
        }
    }
}
