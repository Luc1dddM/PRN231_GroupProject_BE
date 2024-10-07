using Email.Models;
using System.Net.Mail;
using System.Net;
using Email.API.Models;

namespace Email.API.Repository;

public interface ISenderEmail
{
    Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHtml = false);
}

