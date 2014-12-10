using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Base;
using ActionFramework.Interfaces;

namespace ActionFramework.Action
{
    public class XmlReplace : ActionBase
    {
        public override object Execute()
        {
            return "MergeFile: " + this.Prop("RootFilePath");
        }
    }
}
