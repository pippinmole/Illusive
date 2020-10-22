namespace Illusive.Illusive.Utilities {
    public static class StringUtilities {

        public static bool IsEmail(this string str) {
            try {
                var addr = new System.Net.Mail.MailAddress(str);
                return addr.Address == str;
            } catch {
                return false;
            }
        }
        
    }
}