using Carter;
using Coupon.Grpc;
using Email.API.Repository;
using Email.Models;
using FluentValidation;

public class SendCouponEmailEndpoint : ICarterModule
{
    private readonly CouponProtoService.CouponProtoServiceClient _couponServiceClient;

    public SendCouponEmailEndpoint(CouponProtoService.CouponProtoServiceClient couponServiceClient)
    {
        _couponServiceClient = couponServiceClient;
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/send-coupon-email", async (HttpContext httpContext, EmailSend emailSend, ISenderEmail emailSender, IEmailRepository emailTemplateRepository) =>
        {
            try
            {
                // Lấy UserId từ headers
                var userId = httpContext.Request.Headers["UserId"].ToString();
                emailSend.SenderId = userId;

                // Lấy mẫu email từ cơ sở dữ liệu
                var emailTemplate = await emailTemplateRepository.GetByIdAsync(emailSend.TemplateId);
                if (emailTemplate == null)
                {
                    return Results.Problem("Email template not found.", statusCode: StatusCodes.Status404NotFound);
                }
                emailSend.Template = emailTemplate;

                // Kiểm tra loại mẫu email
                if (emailSend.TemplateType == "Notification")
                {
                    emailSend.Content = "This is a notification email.";
                }
                else if (emailSend.TemplateType == "Coupon")
                {
                    var couponsResponse = await _couponServiceClient.GetCouponsAsync(new Empty());
                    if (couponsResponse == null)
                    {
                        return Results.Problem("Failed to retrieve coupons.", statusCode: StatusCodes.Status500InternalServerError);
                    }

                    var couponList = couponsResponse.Coupons.Select(c => c.CouponCode).ToList();
                    if (!string.IsNullOrEmpty(emailSend.CouponCode) && couponList.Contains(emailSend.CouponCode))
                    {
                        var coupon = await _couponServiceClient.GetCouponAsync(new GetCouponRequest { CouponCode = emailSend.CouponCode });
                        emailSend.Content = $"You have received a coupon: {coupon.CouponCode} with a discount of {coupon.DiscountAmount}.";
                    }
                    else
                    {
                        return Results.Problem("Invalid or missing coupon code for the Coupon template.", statusCode: StatusCodes.Status400BadRequest);
                    }
                }
                else
                {
                    return Results.Problem("Invalid template type.", statusCode: StatusCodes.Status400BadRequest);
                }

                // Kiểm tra giá trị Receiver và Content
                if (string.IsNullOrEmpty(emailSend.Receiver) || string.IsNullOrEmpty(emailSend.Content))
                {
                    return Results.Problem("Receiver and Content must be provided.", statusCode: StatusCodes.Status400BadRequest);
                }

                // Gửi email
                await emailSender.SendEmailByEmailTemplate(emailSend.Template, emailSend.Receiver);

                // Tạo bản ghi EmailSend
                emailSend.EmailSendId = Guid.NewGuid().ToString();
                emailSend.Sendate = DateTime.UtcNow;

                return Results.Ok("Email sent successfully!");
            }
            catch (ValidationException ex)
            {
                return Results.Problem("Validation failed: " + ex.Message, statusCode: StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return Results.Problem("An unexpected error occurred: " + ex.Message);
            }
        })
        .WithName("SendCouponEmail")
        .Produces(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Send Coupon Email")
        .WithDescription("Sends an email based on the provided template type (Notification or Coupon).");
    }

}
