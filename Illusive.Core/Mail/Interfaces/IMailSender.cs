using System.Collections.Generic;
using System.Threading.Tasks;

namespace Illusive.Illusive.Core.Mail.Interfaces {
    public interface IMailSender {
        Task SendEmailAsync(IEnumerable<string> recipients, string subject, string body, string sender);
    }
}