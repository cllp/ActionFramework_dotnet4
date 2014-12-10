using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using ActionFramework.Interfaces;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ActionFramework.Classes
{
  public class Common : ICommon
  {
    public object ReadRegistry(string path, string key, Microsoft.Win32.RegistryHive registryHive)
    {
      #region
      RegistryKey SUBKEY;
      RegistryKey TAWKAY = RegistryKey.OpenRemoteBaseKey(registryHive, "");

      string subkey = path;
      SUBKEY = TAWKAY.OpenSubKey(subkey);

      if (SUBKEY.GetValue(key) == null)
        throw new Exception("Common.ReadRegistry caused an exception. Could not find Key: " + key + " in RegistryHive: " + registryHive.ToString() + " path: " + path + ".");
      else
        return SUBKEY.GetValue(key);
      #endregion
    }

    public object ReadRegistry(string path, string key, string registryHive)
    {
      #region
      RegistryKey SUBKEY;
      RegistryKey TAWKAY;
      Microsoft.Win32.RegistryHive hive;
      
      switch(registryHive)
      {
        case "LocalMachine":
          {
            hive = RegistryHive.LocalMachine;
            break;
          }
        case "CurrentUser":
          {
            hive = RegistryHive.CurrentUser;
            break;
          }
        default:
          {
            hive = RegistryHive.CurrentConfig;
            break;
          }
      }

      TAWKAY = RegistryKey.OpenRemoteBaseKey(hive, "");

      string subkey = path;
      SUBKEY = TAWKAY.OpenSubKey(subkey);

      if (SUBKEY.GetValue(key) == null)
        throw new Exception("Common.ReadRegistry caused an exception. Could not find Key: " + key + " in RegistryHive: " + registryHive.ToString() + " path: " + path + ".");
      else
        return SUBKEY.GetValue(key);
      #endregion
    }

    public string Build(string solutionFile, string solutionConfig)
    {
        string errorLog = "";
        // get temp logfile path
        //string logFileName = System.IO.Path.GetTempFileName();
        string logFileName = @"C:\temp\log.log";//System.IO.Path.GetTempFileName();
        // populate process environment
        System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
        psi.FileName = @"devenv.exe";
        psi.ErrorDialog = true;
        psi.Arguments = "\"" + solutionFile + "\"" + @" /rebuild " + solutionConfig + " /out " + logFileName;
        // start process
        System.Diagnostics.Process p = System.Diagnostics.Process.Start(psi);
        // instruct process to wait for exit
        p.WaitForExit();
        // get return code
        int exitCode = p.ExitCode;
        // free process resources
        p.Close();
        // if there was a build error, display build log to console			
        if (exitCode != 0)
        {
            System.IO.TextReader reader = System.IO.File.OpenText(logFileName);
            errorLog = reader.ReadToEnd();
            reader.Close();
            //
            //System.Console.WriteLine(errorLog);
            //System.Console.WriteLine("Hit enter to abort...");
            //System.Console.Read();
        }
        // delete temp logfile
        System.IO.File.Delete(logFileName);
        // return process exit code
        return "Exitcode: " + exitCode.ToString() + ". Log: " + errorLog;
    }

    public object InvokeMethod(object instance, string methodName, object[] parameters)
    {
        //Getting the method information using the method info class
        MethodInfo mi = instance.GetType().GetMethod(methodName);

        //invoing the method
        //null- no parameter for the function [or] we can pass the array of parameters
        return mi.Invoke(instance, parameters);
    }

    public object InvokeMethod(object instance, string methodName)
    {
      //Getting the method information using the method info class
      MethodInfo mi = instance.GetType().GetMethod(methodName);

      //invoing the method
      //null- no parameter for the function [or] we can pass the array of parameters
      return mi.Invoke(instance, null);
    }

    public MethodInfo[] InvokeMethod(Type type)
    {
        return type.GetMethods(BindingFlags.Public | BindingFlags.Static);
    }

    public List<string> GetVariables(string value)
    {
        List<string> variables = new List<string>();
        var pattern = @"\{(.*?)\}";

        var matches = Regex.Matches(value, pattern);

        foreach (Match m in matches)
        {
            if (!string.IsNullOrEmpty(m.Groups[1].Value))
                variables.Add(m.Groups[1].Value);
        }

        return variables;
    }

    public List<string> GetVariables(string pattern, string value)
    {
        List<string> variables = new List<string>();
        //var pattern = @"\{(.*?)\}";

        var matches = Regex.Matches(value, pattern);

        foreach (Match m in matches)
        {
            if (!string.IsNullOrEmpty(m.Groups[1].Value))
                variables.Add(m.Groups[1].Value);
        }

        return variables;
    }
    
  }
}
