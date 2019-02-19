using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generics.Utils.Date
{
    public static class DateParser
    {
        public static DateTime ParseTime(string date)
        {
            return DateTime.ParseExact(date, @"yyyy-MM-dd\THH:mm:ss.fff\Z", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}