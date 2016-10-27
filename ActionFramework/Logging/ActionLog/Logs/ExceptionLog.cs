using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Logging
{
    public class ExceptionLog : ILogElement
    {
        private string exceptionMessage;
        private string message;
        private string stackTrace;
        private string source;
        private string innerException;

        public string ExceptionMessage
        {
            get { return exceptionMessage; }
            set { exceptionMessage = value; }
        }
      
        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public string StackTrace
        {
            get { return stackTrace; }
            set { stackTrace = value; }
        }

        public string Source
        {
            get { return source; }
            set { source = value; }
        }

        public string InnerException
        {
            get { return innerException; }
            set { innerException = value; }
        }

        public ExceptionLog(string message)
        {
            this.message = message;
        }

        public ExceptionLog(string message, Exception ex)
        {
            this.message = message;
            this.exceptionMessage = ex.Message;
            this.stackTrace = ex.StackTrace;
            this.source = ex.Source;

            if (ex.InnerException != null)
                this.innerException = ex.InnerException.Message;
        }

        public ExceptionLog(Exception ex)
        {
            this.exceptionMessage = ex.Message;
            this.stackTrace = ex.StackTrace;
            this.source = ex.Source;

            if (ex.InnerException != null)
                this.innerException = ex.InnerException.Message;
        }
    }
}
