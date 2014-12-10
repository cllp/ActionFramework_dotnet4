using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Base;
using ActionFramework.Interfaces;
using System.IO;
using ActionFramework.Events;

namespace ActionFramework.Action
{
    public class CopyFile : ActionBase
    {
        public override object Execute()
        {
            Initialize += new InitEventHandler(CopyFile_Initialize);
            CopyFile_Initialize(this, new InitEventArgs(this));

            try
            {
                string sourceFolder = Prop("FileFolder");
                string destinationFolder = Prop("DestinationFolder");
                string fileName = Prop("FileName");

                if (!Directory.Exists(destinationFolder))
                    Directory.CreateDirectory(destinationFolder);

                if (fileName.Contains("*"))
                {
                    DirectoryInfo di = new DirectoryInfo(sourceFolder);
                    FileInfo[] rgFiles = di.GetFiles(fileName);

                    foreach (FileInfo fi in rgFiles)
                    {
                        File.Copy(fi.FullName, destinationFolder + fi.Name, true);
                    }
                }
                else
                {
                    File.Copy(sourceFolder + fileName, destinationFolder + fileName, true);
                }

                Status = HandleSuccess();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }

            //output = "This is output from function";
            return null;
        }

        void CopyFile_Initialize(object sender, InitEventArgs e)
        {
            CopyFile copyFile = (CopyFile)e.Action;
            //Log.Info(copyFile.Assembly.FullName);
        }

    }
}
