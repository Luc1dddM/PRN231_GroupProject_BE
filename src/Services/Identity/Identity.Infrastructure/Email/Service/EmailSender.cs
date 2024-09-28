using Identity.Application.Email.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Identity.Infrastructure.Email.Configuration;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Email.Service
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string ToEmail, string Subject, string Body, bool IsBodyHtml = false)
        {
            try
            {
                using var client = new SmtpClient(_emailSettings.MailServer, _emailSettings.MailPort)
                {
                    Credentials = new NetworkCredential(_emailSettings.FromEmail, _emailSettings.Password),
                    EnableSsl = true,
                };

                using var mailMessage = new MailMessage(_emailSettings.FromEmail, ToEmail, Subject, Body)
                {
                    IsBodyHtml = IsBodyHtml
                };

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
