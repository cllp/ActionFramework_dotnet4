using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Base;
using ActionFramework.Interfaces;

namespace ActionFramework.Action
{
    public class ExecuteFile : ActionBase
    {
        public override object  Execute()
        {
            System.Diagnostics.Process useprocess;

            useprocess = new System.Diagnostics.Process();
            useprocess.StartInfo.FileName = Prop("FileName");

            if (!string.IsNullOrEmpty(Prop("Parameters")))
                useprocess.StartInfo.Arguments = Prop("Parameters");
            
            useprocess.Start();
            return base.Execute();
        }
           
    }
}
