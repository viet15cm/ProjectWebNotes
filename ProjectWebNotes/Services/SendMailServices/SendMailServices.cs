using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace ProjectWebNotes.Services.MailServices
{
    public class SendMailServices : ISendMailServices
    {
        private readonly MailSetting mailSetting;
        private readonly ILogger<SendMailServices> logger;
        public SendMailServices(IOptions<MailSetting> options, ILogger<SendMailServices> logger)
        {
            mailSetting = options.Value;
            this.logger = logger;
        }

        public Task SendMail(MailContent content)
        {
            throw new NotImplementedException();
        }

     
        public async Task<bool> SendMailAsync( MailContent content)
        {
            var task = new Task<bool>(() =>
            {

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(mailSetting.Mail));
                email.To.Add(MailboxAddress.Parse(content.To));
                email.Subject = content.Subject;
                email.Body = new TextPart(TextFormat.Plain) { Text = content.Body };

                // send email

                try
                {
                    using var smtp = new SmtpClient();
                    smtp.Connect(mailSetting.Host, mailSetting.Port, SecureSocketOptions.Auto);
                    smtp.Authenticate(mailSetting.Mail, mailSetting.Password);
                    smtp.Send(email);
                    smtp.Disconnect(true);
                    return true;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                    logger.LogError(content.Body);
                    return false;
                }

            });

            task.Start();

            return await task;

        }

    }
}
