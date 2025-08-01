using System.Net.Mail;
using System.Net;

namespace FlyTickets2025.web.Helpers
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string message, Attachment attachment = null)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            // Read the delivery method from appsettings.json
            var deliveryMethod = smtpSettings.GetValue<SmtpDeliveryMethod>("DeliveryMethod");

            var client = new SmtpClient();

            if (deliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory)
            {
                // Local development settings
                client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                client.PickupDirectoryLocation = smtpSettings["PickupDirectoryLocation"];
            }
            else
            {
                // Production settings for network-based email sending
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Host = smtpSettings["Host"];
                client.Port = int.Parse(smtpSettings["Port"]);
                client.EnableSsl = bool.Parse(smtpSettings["EnableSsl"]);
                client.Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]);
            }

            var mailMessage = new MailMessage(smtpSettings["FromAddress"], email, subject, message)
            {
                IsBodyHtml = true
            };

            if (attachment != null)
            {
                mailMessage.Attachments.Add(attachment);
            }

            return client.SendMailAsync(mailMessage);
        }
        //public Task SendEmailAsync(string email, string subject, string message, Attachment attachment = null)
        //{
        //    var host = _configuration["SmtpSettings:Host"];
        //    var port = _configuration.GetValue<int>("SmtpSettings:Port");
        //    var username = _configuration["SmtpSettings:Username"];
        //    var password = _configuration["SmtpSettings:Password"];

        //    var client = new SmtpClient(host, port)
        //    {
        //        Credentials = new NetworkCredential(username, password),
        //        EnableSsl = true
        //    };

        //    var mailMessage = new MailMessage(username, email, subject, message)
        //    {
        //        IsBodyHtml = true
        //    };

        //    if (attachment != null)
        //    {
        //        mailMessage.Attachments.Add(attachment);
        //    }

        //    return client.SendMailAsync(mailMessage);
        //}
    }
}
