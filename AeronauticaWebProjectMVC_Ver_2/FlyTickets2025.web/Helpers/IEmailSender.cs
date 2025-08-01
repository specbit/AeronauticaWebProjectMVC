using System.Net.Mail;

namespace FlyTickets2025.web.Helpers
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message, Attachment attachment = null);
    }
}
