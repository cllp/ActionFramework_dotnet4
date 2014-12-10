using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Base;
using ActionFramework.Interfaces;
using System.IO;
using ActionFramework.Events;

namespace ActionFramework.Actions
{
    public class CreateFolder : ActionBase
    {

        public override object Execute()
        {
            try
            {
                string path = Prop("Path");

                if (!Directory.Exists(path))
                  Directory.CreateDirectory(path);

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
