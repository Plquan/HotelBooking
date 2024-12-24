using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Services
{
    public class MailSettings
    {
        public string? Mail { get; set; }
        public string? DisplayName { get; set; }
        public string? Password { get; set; }
        public string? Host { get; set; }
        public int Port { get; set; }
    }
    public interface IMailService 
    {
       
        Task SendEmailAsync(string email, string subject, string message);      
    }

    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<MailService> _logger;

        public MailService(IOptions<MailSettings> mailSettings, ILogger<MailService> logger)
        {
            _mailSettings = mailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
            message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;


            var builder = new BodyBuilder();
            builder.HtmlBody = htmlMessage;
            message.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(message);
            }

            catch (Exception ex)
            {
                Directory.CreateDirectory("mailssave");
                var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
                await message.WriteToAsync(emailsavefile);

                _logger.LogInformation("Lỗi gửi mail, lưu tại - " + emailsavefile);
                _logger.LogError(ex.Message);
            }
            smtp.Disconnect(true);

            _logger.LogInformation("send mail to " + email);
        }
    



    }
}
