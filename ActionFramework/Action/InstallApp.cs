using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Base;
using ActionFramework.Interfaces;
using System.IO;

namespace ActionFramework.Actions
{
    public class InstallApp : ActionBase
    {
        public override object Execute()
        {
            try
            {
                //string assembly = Resource("");
                string destinationFile = Prop("DestinationFile");
                string backupFile = destinationFile + "_BCK_" + DateTime.Now.ToString("yyMMdd.hhmm");

                //FileInfo origFile = new FileInfo(sourceFile);
                //FileInfo destFile = new FileInfo(File.Replace(origDir, destDir));
                //copy the file, use the OverWrite overload to overwrite
                //destination file if it exists
                //System.IO.File.Copy(sourceFile, destinationFile, true);

                //File.Replace(sourceFile, destinationFile, backupFile, true);
                Status = HandleSuccess();
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
