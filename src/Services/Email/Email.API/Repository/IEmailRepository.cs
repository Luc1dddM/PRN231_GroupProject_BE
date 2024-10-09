using Email.API.Models;
using Email.Models;

namespace Email.API.Repository
{
    public interface IEmailRepository
    {
        public void SaveEmailSend(EmailTemplate template, string senderId, string? body, string receiver, string? couponCode);
        // send once
        public Task SendEmailCoupon(EmailTemplate template, string senderId, string to, string coupon);

        // send all
        public Task SendEmailToAll(EmailTemplate emailTemplate, string senderId);
        public Task SendCouponToAll(EmailTemplate emailTemplate, string senderId, string coupon);

        // send by template
        public Task SendEmailByEmailTemplate(EmailTemplate template, string senderId, string to);

        // send email by order
        public Task SendEmailOrder(string orderId, string userEmail, string couponCode);

        public Task<EmailTemplate> GetEmailTemplateById(string id);
    }
}
