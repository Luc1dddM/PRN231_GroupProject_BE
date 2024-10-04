using Email.Models;
using System.Net.Mail;
using System.Net;
using Email.API.Models;

namespace Email.API.Repository;

public interface ISenderEmail
{
    Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHtml = false);
    Task SendEmailByEmailTemplate(EmailTemplate template, string to);


    // Gửi email kèm mã giảm giá
    Task SendEmailCoupon(EmailTemplate template, string to, string couponCode);
    // Phương thức kiểm tra tính hợp lệ của mã giảm giá
    Task<string> GetValidCouponCodeAsync(string couponCode);


    // Hàm mới để lấy danh sách người dùng
    Task<List<User>> GetUsersAsync();
}

