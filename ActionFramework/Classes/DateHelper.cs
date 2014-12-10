using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionFramework.Classes
{
    public static class DateHelper
    {
        public static int ToOADate(DateTime date)
        {
            return Convert.ToInt32(date.ToOADate() + 36161);
        }

        public static DateTime FromOADate(int number)
        {
            number = number - 36161;
            return DateTime.FromOADate(number);
        }
    }
}
