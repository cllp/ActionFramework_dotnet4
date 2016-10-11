using ActionFramework.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApp
{
    public class FileListener : ActionBase
    {

        public override object Execute()
        {
            foreach (var r in this.Resources)
            {
                Log.Info("Name: " + r.FileName);
                Log.Info("FilePath: " + r.FilePath);
                Log.Info("FileType: " + r.FileType);
                Log.Info("Origin: " + r.Origin);
                Log.Info("LoadDate: " + r.LoadDate);

                using (StreamReader sr = new StreamReader(r.FilePath))
                {
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();
                    Log.Info(line);
                }

                //Log.Info()

            }

            return HandleSuccess();
        }
    }
}
