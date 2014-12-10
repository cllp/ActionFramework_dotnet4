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
    public class FileContentReplace : ActionBase
    {
        public override object Execute()
        {
            try
            {
                string filePath = Prop("FilePath");
                string patternToReplace = Prop("PatternToReplace");
                string patternToReplaceWith = Prop("PatternToReplaceWith");

                var fileContents = System.IO.File.ReadAllText(filePath);
                fileContents = fileContents.Replace(patternToReplace, patternToReplaceWith);
                System.IO.File.WriteAllText(filePath, fileContents);

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
