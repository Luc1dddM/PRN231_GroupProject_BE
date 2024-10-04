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

        public async Task SendEmailByEmailTemplate(EmailTemplate template, string receiver)
        {
            try
            {
                // Kiểm tra xem template có tồn tại không bằng EmailTemplateId
                var existingTemplate = await _context.EmailTemplates
                    .FirstOrDefaultAsync(t => t.EmailTemplateId == template.EmailTemplateId);

                if (existingTemplate == null)
                {
                    throw new Exception($"Template with ID {template.EmailTemplateId} does not exist.");
                }

                var body = existingTemplate.Body;

                // Gửi email
                await SendEmailAsync(receiver, existingTemplate.Subject, body, true);

                // Tạo đối tượng EmailSend để lưu thông tin email đã gửi
                var emailSent = new EmailSend
                {
                    EmailSendId = Guid.NewGuid().ToString(),
                    TemplateId = existingTemplate.Id,
                    SenderId = _configuration["EmailSettings:FromEmail"],
                    Content = body,
                    Receiver = receiver,
                    Sendate = DateTime.UtcNow
                };

                _context.EmailSends.Add(emailSent);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"Failed to save email send record: {dbEx.InnerException?.Message}", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send email to {receiver}: {ex.Message}", ex);
            }
        }


        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }


        // Hàm mới để gửi email kèm mã giảm giá qua gRPC
        public async Task SendEmailCoupon(EmailTemplate template, string to, string couponCode)
        {
            try
            {
                // Gọi gRPC để lấy thông tin coupon theo mã
                var couponResponse = await _couponClient.GetCouponAsync(new GetCouponRequest { CouponCode = couponCode });

                if (couponResponse == null || couponResponse.CouponCode == "No Coupon")
                {
                    throw new Exception("Coupon not found.");
                }

                // Tạo nội dung email với coupon
                var body = template.Body;
                body += $"<br> <p>Coupon Code: {couponResponse.CouponCode}</p>";
                body += $"<p>Discount Amount: {couponResponse.DiscountAmount}</p>";
                body += $"<p>Min Purchase Amount: {couponResponse.MinAmount}</p>";

                // Gửi email
                await SendEmailAsync(to, template.Subject, body, true);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to send email: {ex.Message}");
            }
        }

        // Triển khai phương thức IsValidCouponCode
        public async Task<string> GetValidCouponCodeAsync(string couponCode)
        {
            var couponResponse = await _couponClient.GetCouponAsync(new GetCouponRequest { CouponCode = couponCode });

            if (couponResponse == null || couponResponse.CouponCode == "No Coupon")
            {
                throw new Exception("Coupon not found.");
            }

            return couponResponse.CouponCode; // Trả về mã giảm giá hợp lệ
        }
    }
}

