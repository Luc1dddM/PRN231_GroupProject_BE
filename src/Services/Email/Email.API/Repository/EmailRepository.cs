using Email.API.Models;
using Email.Models;
using Microsoft.EntityFrameworkCore;

namespace Email.API.Repository;

public class EmailRepository : IEmailRepository
{
    private readonly ISenderEmail _emailSender;
    private readonly Prn231GroupProjectContext _context;

    public EmailRepository(ISenderEmail emailSender, Prn231GroupProjectContext context)
    {
        _emailSender = emailSender;
        _context = context;

    }

    public async Task SendEmailCoupon(EmailTemplate template, string to, string couponCode)
    {
        try
        {
            var body = template.Body;
            body += "<br> <p>Coupon: " + couponCode + "</p>";
            await _emailSender.SendEmailAsync(to, template.Subject, body, true);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }

    public async Task<EmailTemplate> GetByIdAsync(int templateId)
    {
        // Giả định bạn sử dụng Entity Framework, bạn có thể viết như sau:
        return await _context.EmailTemplates.FindAsync(templateId);
    }

    /* public async Task SendCouponToAll(EmailTemplate emailTemplate, string coupon)
     {
         var users = _userRepo.GetUsers();
         foreach (var user in users)
         {
             await SendEmailCoupon(emailTemplate, user?.Email, coupon);
         }
     }*/
}


