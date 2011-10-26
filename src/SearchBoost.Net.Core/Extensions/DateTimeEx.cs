using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SearchBoost.Net.Core.Extensions
{
    static public class DateTimeEx
    {
        public static int ToUnixTimestamp(this DateTime d)
        {
            TimeSpan _unixTimeSpan = (d - new DateTime(1970, 1, 1, 0, 0, 0));
            return (int)_unixTimeSpan.TotalSeconds;
        }

        public static DateTime FromUnixTimestamp(int timestamp)
        {
            DateTime d = new DateTime(1970, 1, 1, 0, 0, 0);
            d.AddSeconds((double)timestamp);
            return d;
        }

    }
}
