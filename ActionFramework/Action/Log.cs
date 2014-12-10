using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Base;
using ActionFramework.Interfaces;
using System.IO;

namespace ActionFramework.Actions
{
    public class Log : ActionBase
    {

        public override object Execute()
        {
            try
            {
                return HandleSuccess(" - " + Prop("Message"));
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

    }
}
