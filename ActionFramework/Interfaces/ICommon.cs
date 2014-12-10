using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ActionFramework.Interfaces
{
    public interface ICommon
    {
        object ReadRegistry(string path, string key, Microsoft.Win32.RegistryHive registryHive);

        object ReadRegistry(string path, string key, string registryHive);

        object InvokeMethod(object instance, string methodName, object[] parameters);

        object InvokeMethod(object instance, string methodName);

        MethodInfo[] InvokeMethod(Type type);

        string Build(string solutionFile, string solutionConfig);

        List<string> GetVariables(string value);

        List<string> GetVariables(string pattern, string value);

    }
}
