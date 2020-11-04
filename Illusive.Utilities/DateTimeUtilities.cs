using System;

namespace Illusive.Illusive.Utilities {
    public static class DateTimeUtilities {

        public static TimeSpan TimeSince(this DateTime date) {
            return DateTime.Now - date;
        }
        
    }
}