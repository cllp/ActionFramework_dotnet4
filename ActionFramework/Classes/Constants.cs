using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ActionFramework.Classes
{
    public static class Constants
    {
        public const int EventLogId = 77;
        public const string SuccessMessage = "OK";
        public const string ExceptionMessage = "Exception";
        public const string LogFilePropertyName = "LogFile";
        public const string CustomAssemblyFolderName = "CustomAssemblyFolder";
        public const string ExecuteNotImplementedMessage = "The '{0}' returned NotImplementedException for object type: '{1}' Exception occured in Action with id: '{2}'. Check constructor.";
        public const string CountMessageText = "[Total:{0}][AgentExecute:{1}][InternalActionExecute:{2}][Failed:{3}]";
        public const string AssemblyFileName = "{0}.{1}.dll"; //Name.Version.dll

    }
}
