using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Interfaces;

namespace ActionFramework.Events
{
    public class InitEventArgs : System.EventArgs
    {
        public readonly IAction Action;

        public InitEventArgs(IAction arg)
        {
            Action = arg;
        }

    }

    public class StartEventArgs : System.EventArgs
    {
        // Provide one or more constructors, as well as fields and

        // accessors for the arguments.

    }

}
