using System;

namespace Illusive.Utility {
    public static class DateTimeUtilities {

        public static TimeSpan TimeSince(this DateTime date) {
            return DateTime.Now - date;
        }
        
    }
}