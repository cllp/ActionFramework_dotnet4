using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Interfaces;
using ActionFramework.Base;
using Microsoft.Win32;
using ActionFramework.Enum;

namespace ActionFramework.Actions.Actions
{
  public class ReadRegistry : ActionBase
  {

    public override object Execute()
    {
      try
      {
        string path = Prop("RegistryPath");
        string key = Prop("RegistryKey");
        string root = Prop("RegistryRoot");

        RegistryKey SUBKEY;
        RegistryKey TAWKAY;
        switch (root)
        {
          case "LocalMachine":
            {
              TAWKAY = RegistryKey.OpenRemoteBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, "");
              break;
            }
          default:
            TAWKAY = RegistryKey.OpenRemoteBaseKey(Microsoft.Win32.RegistryHive.CurrentUser, "");
            break;
        }
        
        string subkey = path;
        SUBKEY = TAWKAY.OpenSubKey(subkey);

        if (SUBKEY.GetValue(key) == null)
          this.Status = "Could not find Key: " + key;
        else
          this.Status = SUBKEY.GetValue(key).ToString();

        Log.Info(Status);

        Status = HandleSuccess(" " + Status);
      }
      catch (Exception ex)
      {
        Status = HandleException(ex);

        if (BreakOnError)
          throw;
      }

      return Status;
    }
  }
}
