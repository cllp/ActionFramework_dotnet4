using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Woxion.Utility.ActionFramework.Base;
using Woxion.Utility.ActionFramework.Interfaces;
using System.Xml;
using System.Data;
using System.Xml.Linq;
using System.IO;
using Woxion.Utility.ActionFramework.Classes;
using Microsoft.Win32;
//using WorkflowNetAlias = Workflow.NET;

namespace ActionFramework.Actions
{
    public class CleanWorkflowXmlPath : ActionBase, IAction
    {
        public override object Execute()
        {
            try
            {
                string pathSourcePatternUrl = Prop("PathSourcePatternUrl");
                string pathReplacePatternUrl = GetRegistrySkeltaRootFolder(); //Prop("PathReplacePatternUrl");
                string pathSourcePatternVir = ReplacePathPattern(pathSourcePatternUrl);//Prop("PathSourcePatternVir");
                string pathReplacePatternVir = ReplacePathPattern(pathReplacePatternUrl);//Prop("PathReplacePatternVir");
                string rootFolder = Prop("RootFolder");
                string cleanDirectory = Prop("CleanDirectory");

                try
                {
                    DirectoryInfo cleanDirInfo = new DirectoryInfo(cleanDirectory);
                    if (!cleanDirInfo.Exists)
                        cleanDirInfo.Create();
                }
                catch (Exception ex)
                {
                    Log.Error("Could not create clean directory", ex);
                }
                
                string fileName = Prop("FileName");

                if (fileName.Contains("*"))
                {
                    DirectoryInfo di = new DirectoryInfo(rootFolder);
                    FileInfo[] rgFiles = di.GetFiles(fileName);

                    foreach (FileInfo fi in rgFiles)
                    {
                        string cleanstring = "";
                        string filedata = ActionHelper.ReadTextFile(fi.FullName);
                        cleanstring = CleanDefinitionPaths(filedata, pathSourcePatternUrl, pathReplacePatternUrl);
                        cleanstring = CleanDefinitionPaths(cleanstring, pathSourcePatternVir, pathReplacePatternVir);

                        XDocument doc = XDocument.Parse(cleanstring);
                        doc.Save(cleanDirectory + fi.Name);
                    }
                }
                else
                {
                    string cleanstring = "";
                    string filedata = ActionHelper.ReadTextFile(rootFolder + fileName);
                    cleanstring = CleanDefinitionPaths(filedata, pathSourcePatternUrl, pathReplacePatternUrl);
                    cleanstring = CleanDefinitionPaths(cleanstring, pathSourcePatternVir, pathReplacePatternVir);

                    XDocument doc = XDocument.Parse(cleanstring);
                    doc.Save(cleanDirectory + fileName);
                }

                Status = HandleSuccess();
            }
            catch (System.Exception ex)
            {
                Status = HandleException(ex);

                if (BreakOnError)
                  throw;
            }

            return Status;
        }

        private string CleanDefinitionPaths(string filedata, string sourcePattern, string replacePattern)
        {
            try
            {
                return ActionHelper.RegExReplace(filedata, sourcePattern, replacePattern);
            }
            catch (System.Exception ex)
            {
                Log.Error("CleanDefinitionPaths threw an exception: " + ex.Message, ex);
                throw;
            }
        }

        private string ReplacePathPattern(string sourcePattern)
        {
          try
          {
            string virPattern = sourcePattern.Replace(@"\", "$");
            if (virPattern.EndsWith("$"))
              virPattern = virPattern.Substring(0, virPattern.Length - 1);
            return virPattern;
          }
          catch (System.Exception ex)
          {
            Log.Error("ReplacePathPattern threw an exception: " + ex.Message, ex);
            throw;
          }
        }

        private string GetRegistrySkeltaRootFolder()
        {
          try
          {
            string path = "SOFTWARE\\Skelta\\BPM.NET";
            string key = "ConfigFilePath";

            RegistryKey SUBKEY;
            RegistryKey TAWKAY;
            TAWKAY = RegistryKey.OpenRemoteBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, "");

            string subkey = path;
            SUBKEY = TAWKAY.OpenSubKey(subkey);
            return SUBKEY.GetValue(key).ToString();
          }
          catch (System.Exception ex)
          {
            Log.Error("GetRegistrySkeltaRootFolder threw an exception: " + ex.Message, ex);
            throw;
          }
        }
    }
}
