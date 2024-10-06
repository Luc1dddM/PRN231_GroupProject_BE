using Coupon.Grpc;
using Email.API.Models;
using Email.Models;
using Microsoft.EntityFrameworkCore;

namespace Email.API.Repository;

public class EmailRepository : IEmailRepository
{
    private readonly ISenderEmail _emailSender;
    private readonly HttpClient _httpClient;
    private readonly Prn231GroupProjectContext _context;
    private readonly CouponProtoService.CouponProtoServiceClient _couponServiceClient;
    public EmailRepository(ISenderEmail emailSender, Prn231GroupProjectContext context, CouponProtoService.CouponProtoServiceClient couponServiceClient, HttpClient httpClient)
    {
        _emailSender = emailSender;
        _context = context;
        _couponServiceClient = couponServiceClient;
        _httpClient = httpClient;
    }

    private async Task<List<User>> GetAllUsersAsync()
    {
        var response = await _httpClient.GetAsync("https://yourapi.com/api/users");

        if (response.IsSuccessStatusCode)
        {
            var users = await response.Content.ReadFromJsonAsync<List<User>>();
            return users;
        }
        else
        {
            throw new Exception("Failed to fetch users from the external service.");
        }
    }
    public void SaveEmailSend(EmailTemplate template, string senderId, string? body, string receiver, string? couponCode)
    {
        var enailSend = new EmailSend
        {
            EmailSendId = Guid.NewGuid().ToString(),
            TemplateId = template.Id,
            SenderId = senderId,
            Content = body,
            Sendate = DateTime.UtcNow,
            Receiver = receiver,
            CouponCode = couponCode
        };


        _context.Add(enailSend);
        _context.SaveChanges();

    }

    //========================================//

    public async Task SendEmailCoupon(EmailTemplate template, string senderId, string to, string coupon)
    {
        var request = new GetCouponRequest { CouponCode = coupon };
        CouponModel couponData = await _couponServiceClient.GetCouponAsync(request);

        if (couponData == null || couponData.CouponCode.Equals("No Coupon"))
        {
            throw new Exception($"Coupon with code '{coupon}' does not exist.");
        }


        var body = template.Body;
        body += "<br> <p>Coupon: " + couponData.CouponCode + "</p>";
        body += "<br> <p>DiscountAmount: " + couponData.DiscountAmount + "</p>";
        body += "<br> <p>MinAmount: " + couponData.MinAmount + "</p>";
        body += "<br> <p>MaxAmount: " + couponData.MaxAmount + "</p>";
        SaveEmailSend(template, senderId, body, to, couponData.CouponCode);
        await _emailSender.SendEmailAsync(to, template.Subject, body, true);
    }

    public async Task SendEmailToAll(EmailTemplate emailTemplate, string senderId)
    {
        List<User> users = await GetAllUsersAsync();

        foreach (var user in users)
        {

            await SendEmailByEmailTemplate(emailTemplate, senderId, user?.Email);
        }
    }
    public async Task SendCouponToAll(EmailTemplate emailTemplate, string senderId, string coupon)
    {
        List<User> users = await GetAllUsersAsync();

        foreach (var user in users)
        {
            await SendEmailCoupon(emailTemplate, senderId, user?.Email, coupon);
        }
    }
    public async Task SendEmailByEmailTemplate(EmailTemplate template, string senderId, string to)
    {
        var body = template.Body;
        SaveEmailSend(template, senderId, body, to, null);
        await _emailSender.SendEmailAsync(to, template.Subject, body, true);
    }



    public async Task<EmailTemplate> GetEmailTemplateById(string id)
    {
        return await _context.EmailTemplates.FirstOrDefaultAsync(e => e.EmailTemplateId.Equals(id));
    }
}


