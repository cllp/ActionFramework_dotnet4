using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Base;
using ActionFramework.Interfaces;
using System.Xml;
using System.Data;
using System.Xml.Linq;
using System.IO;
using ActionFramework.Classes;

namespace ActionFramework.Actions
{
    public class MultipleContentReplace : ActionBase
    {
        public override object Execute()
        {
            try
            {
                string rootFolder = Prop("RootFolder");
                string fileName = Prop("FileName");
                string patternToReplace = Prop("PatternToReplace");
                string patternToReplaceWith = Prop("PatternToReplaceWith");

                if (fileName.Contains("*"))
                {
                    DirectoryInfo di = new DirectoryInfo(rootFolder);
                    FileInfo[] rgFiles = di.GetFiles(fileName);

                    foreach (FileInfo fi in rgFiles)
                    {
                        string filePath = fi.FullName;
                        var fileContents = System.IO.File.ReadAllText(filePath);
                        fileContents = fileContents.Replace(patternToReplace, patternToReplaceWith);
                        System.IO.File.WriteAllText(filePath, fileContents);
                    }
                }
                else
                {
                    string filePath = rootFolder + fileName;
                    var fileContents = System.IO.File.ReadAllText(filePath);
                    fileContents = fileContents.Replace(patternToReplace, patternToReplaceWith);
                    System.IO.File.WriteAllText(filePath, fileContents);
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
    }
}
