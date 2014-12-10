using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Domain.Enum
{
    public enum PropertyType
    {
        /// <summary>
        /// Built in property in the action class
        /// </summary>
        Static = 1,
        /// <summary>
        /// Added dynamic property after type is uploaded
        /// </summary>
        Dynamic = 2

    }
}
