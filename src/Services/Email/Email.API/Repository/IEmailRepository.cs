using BuildingBlocks.Models;
using Email.API.Models;
using Email.DTOs;
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

        // crud email
        public Task<EmailListDTO> GetList(string[] statusesParam, string[] categoriesParam, string searchterm, string sortBy, string sortOrder, int? pageNumberParam, int? pageSizeParam);
        public Task<List<EmailTemplate>> GetList();
        public Task<EmailTemplate> AddEmailTemplate(EmailTemplate newEmailTemplate);
        public Task<EmailTemplate> UpdateEmailTemplate(EmailTemplate newEmailTemplate);
        public Task<EmailTemplate> GetEmailTemplateById(string id);

        public Task<BaseResponse<MemoryStream>> ImportEmailTemplate(IFormFile file, string userId);
        public Task<byte[]> ExportEmailFilter(string[] statusesParam, string[] categoriesParam, string searchterm, int pageNumberParam, int pageSizeParam);
    }
}
