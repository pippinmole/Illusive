using System.Linq;

namespace Illusive.Utility {
    public static class StringUtilities {
        public static bool IsEmail(this string str) {
            try {
                var addr = new System.Net.Mail.MailAddress(str);
                return addr.Address == str;
            } catch {
                return false;
            }
        }

        public static string SafeSubstring(this string value, int startIndex, int length) {
            return new((value ?? string.Empty).Skip(startIndex).Take(length).ToArray());
        }
    }
}