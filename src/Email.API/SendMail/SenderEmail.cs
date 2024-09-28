using Email.Models;
using System.Net.Mail;
using System.Net;

namespace Email.API.SendMail
{
    public class SenderEmail : ISenderEmail
    {
        private readonly IConfiguration _configuration;

        public SenderEmail(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHtml = false)
        {
            string mailServer = _configuration["EmailSettings:MailServer"];
            string fromEmail = _configuration["EmailSettings:FromEmail"];
            string password = _configuration["EmailSettings:Password"];
            int port = int.Parse(_configuration["EmailSettings:MailPort"]);

            var client = new SmtpClient(mailServer, port)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true,
            };

            MailMessage mailMessage = new MailMessage(fromEmail, toEmail, subject, body)
            {
                IsBodyHtml = isBodyHtml
            };

            return client.SendMailAsync(mailMessage);
        }

        public async Task SendEmailByEmailTemplate(EmailTemplate template, string receiver)
        {
            try
            {
                var body = template.Body;
                await SendEmailAsync(receiver, template.Subject, body, true);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
