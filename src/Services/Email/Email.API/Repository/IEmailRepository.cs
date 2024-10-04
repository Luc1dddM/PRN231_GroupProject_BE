using Email.Models;

namespace Email.API.Repository
{
    public interface IEmailRepository
    {
        public Task SendEmailCoupon(EmailTemplate template, string to, string couponCode);
        Task<EmailTemplate> GetByIdAsync(int templateId);
        /* public Task SendCouponToAll(EmailTemplate emailTemplate, string coupon);*/
    }
}
