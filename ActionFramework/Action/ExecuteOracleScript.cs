using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Woxion.Utility.ActionFramework.Base;
using Woxion.Utility.ActionFramework.Interfaces;

namespace Woxion.Utility.ActionFramework.Action
{
    public class ExecuteOracleScript : ActionBase, IAction
    {
        public override object Execute()
        {
            try
            {
                string scriptFile = Prop("ScriptFile");

                Status = HandleSuccess();
            }
            catch (System.Exception ex)
            {
                Status = HandleException(ex);
            }

            return Status;
        }
           
    }
}
