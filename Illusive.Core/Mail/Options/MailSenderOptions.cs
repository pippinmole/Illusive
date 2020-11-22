namespace Illusive.Illusive.Core.Mail.Options {
    public class MailSenderOptions {
        
        public const string Name = "MailSenderOptions";

        public string AddressFrom { get; set; }
        public string Password { get; set; }
        public string SmtpHostAddress { get; set; }
        public int Port { get; set; }
    }
}