using Email.Models;
using System.Net.Mail;
using System.Net;

namespace Email.API.SendMail;

public interface ISenderEmail
{
    Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHtml = false);
    Task SendEmailByEmailTemplate(EmailTemplate template, string to);
}

