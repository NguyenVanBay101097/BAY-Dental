using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Utilities
{
    public static class DateUtils
    {

        public static DateTime AbsoluteBeginOfDate(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }

        public static DateTime AbsoluteEndOfDate(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 999);
        }
    }
}
