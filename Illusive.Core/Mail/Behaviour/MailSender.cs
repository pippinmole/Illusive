using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Illusive.Illusive.Core.Mail.Interfaces;
using Illusive.Illusive.Core.Mail.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Illusive.Illusive.Core.Mail.Behaviour {
    public class MailSender : IMailSender {
        private readonly ILogger<MailSender> _logger;
        private readonly IOptionsMonitor<MailSenderOptions> _options;

        public MailSender(ILogger<MailSender> logger) {
            this._logger = logger;
            
        }
        
        public MailSender(ILogger<MailSender> logger, IOptionsMonitor<MailSenderOptions> options) {
            this._logger = logger;
            this._options = options;
        }

        public async Task SendEmailAsync(IEnumerable<string> recipients, string subject, string body, string sender) {
            var options = this._options.CurrentValue;

            var msg = new MailMessage {
                Subject = subject, 
                Body = body, 
                IsBodyHtml = true, 
                From = new MailAddress(options.AddressFrom)
            };
            
            foreach ( var recipient in recipients ) {
                msg.To.Add(recipient);
            }
            
            // Build SMTP Server
            using var client = new SmtpClient {
                Host = options.SmtpHostAddress,
                Port = options.Port,
                EnableSsl = true, 
                Credentials = new NetworkCredential(options.AddressFrom, options.Password)
            };

            await client.SendMailAsync(msg);
        }
    }
}