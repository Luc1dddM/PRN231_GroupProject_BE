using Email.Models;
using System.Net.Mail;
using System.Net;
using Email.API.Models;
using Microsoft.EntityFrameworkCore;
using Coupon.Grpc;

namespace Email.API.Repository
{
    public class SenderEmail : ISenderEmail
    {
        private readonly IConfiguration _configuration;
        private readonly Prn231GroupProjectContext _context;

        private readonly CouponProtoService.CouponProtoServiceClient _couponClient;

        public SenderEmail(IConfiguration configuration, Prn231GroupProjectContext context, CouponProtoService.CouponProtoServiceClient couponClient)
        {
            _configuration = configuration;
            _context = context;

            _couponClient = couponClient; // Đảm bảo đã inject couponClient
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
    }
}

