using BuildingBlocks.Exceptions;
using BuildingBlocks.Models;
using BuildingBlocks.Validation;
using ClosedXML.Excel;
using Coupon.Grpc;
using Email.API.DTOs;
using Email.API.Models;
using Email.DTOs;
using Email.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Data;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;

namespace Email.API.Repository;

public class EmailRepository : IEmailRepository
{
    private IWebHostEnvironment environment;
    private readonly ISenderEmail _emailSender;
    private readonly HttpClient _httpClient;
    private readonly Prn231GroupProjectContext _context;
    private readonly CouponProtoService.CouponProtoServiceClient _couponServiceClient;
    public EmailRepository(ISenderEmail emailSender, Prn231GroupProjectContext context, CouponProtoService.CouponProtoServiceClient couponServiceClient, HttpClient httpClient, IWebHostEnvironment env)
    {
        environment = env;
        _emailSender = emailSender;
        _context = context;
        _couponServiceClient = couponServiceClient;
        _httpClient = httpClient;
    }

    private async Task<List<UserDto>> GetAllUsersAsync()
    {
        var response = await _httpClient.GetAsync("https://localhost:7183/api/User");

        if (response.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            string strResult = await response.Content.ReadAsStringAsync();
            var results = JsonSerializer.Deserialize<UsersResponseDTO>(strResult, options);
            return results.Result;
        }
        else
        {
            throw new Exception("Failed to fetch results from the external service.");
        }
    }
    private async Task<List<OrderDto>> GetAllOrdersAsync()
    {
        var response = await _httpClient.GetAsync("https://localhost:5053/orders");

        if (response.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            string strResult = await response.Content.ReadAsStringAsync();
            var results = JsonSerializer.Deserialize<OrdersResponseDto>(strResult, options);
            return results.Orders;
        }
        else
        {
            throw new Exception("Failed to fetch results from the external service.");
        }
    }

    private async Task<ProductDto> GetProductById(string productId)
    {
        var response = await _httpClient.GetAsync($"https://localhost:5050/products/{productId}");

        if (response.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            string strResult = await response.Content.ReadAsStringAsync();
            var results = JsonSerializer.Deserialize<ProductResponse>(strResult, options);
            return results.Product;
        }
        else
        {
            throw new Exception("Failed to fetch results from the external service.");
        }
    }

    private async Task<CouponModel> GetCoupon(string couponCode)
    {
        var request = new GetCouponRequest { CouponCode = couponCode };
        CouponModel couponData = await _couponServiceClient.GetCouponAsync(request);
        return couponData;
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


    public async Task SendEmailCoupon(EmailTemplate template, string senderId, string to, string coupon)
    {
        CouponModel couponData = await GetCoupon(coupon);

        if (couponData == null || couponData.CouponCode.Equals("No Coupon"))
        {
            throw new Exception($"Coupon with code '{coupon}' does not exist.");
        }


        var body = template.Body;
        body += "<br> <p>Coupon: " + couponData.CouponCode + "</p>";
        body += "<br> <p>DiscountAmount: " + couponData.DiscountAmount + "</p>";
        /*  body += "<br> <p>MinAmount: " + couponData.MinAmount + "</p>";
          body += "<br> <p>MaxAmount: " + couponData.MaxAmount + "</p>";
          SaveEmailSend(template, senderId, body, to, couponData.CouponCode);*/
        await _emailSender.SendEmailAsync(to, template.Subject, body, true);
    }

    public async Task SendEmailToAll(EmailTemplate emailTemplate, string senderId)
    {
        List<UserDto> users = await GetAllUsersAsync();

        foreach (var user in users)
        {

            await SendEmailByEmailTemplate(emailTemplate, senderId, user?.Email);
        }
    }
    public async Task SendCouponToAll(EmailTemplate emailTemplate, string senderId, string coupon)
    {
        List<UserDto> users = await GetAllUsersAsync();

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


    public async Task SendEmailOrder(string orderId, string userEmail, string couponCode)
    {
        try
        {
            var orders = await GetAllOrdersAsync();
            string subject = "Notice of successful order from ToolZone";
            var order = orders.FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");
            }

            CouponModel coupon = await GetCoupon(couponCode);

            double TotalBefore = 0;
            double MoneyDiscountAmount = 0;

            if (coupon != null)
            {
                TotalBefore = (double)order.TotalPrice / coupon.DiscountAmount * 100;
                MoneyDiscountAmount = TotalBefore - (double)order.TotalPrice;
            }
            var body = "<table style=\"table-layout:fixed;vertical-align:top;min-width:320px;margin:0 auto;border-spacing:0;border-collapse:collapse;background-color:#ffffff;width:100%\" role=\"presentation\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#FFFFFF\">\r\n<tbody>\r\n<tr style=\"vertical-align:top\" valign=\"top\">\r\n<td style=\"word-break:break-word;vertical-align:top;border-collapse:collapse\" valign=\"top\">\r\n<div style=\"background-color:transparent\">\r\n<div style=\"margin:0 auto;min-width:320px;max-width:640px;word-wrap:break-word;word-break:break-word;background-color:#313130\">\r\n<div style=\"border-collapse:collapse;display:table;width:100%;background-color:#313130\">\r\n\r\n\r\n\r\n<div style=\"min-width:320px;max-width:640px;display:table-cell;vertical-align:top\">\r\n<div style=\"width:100%!important\">\r\n\r\n\r\n<div style=\"border:0px solid transparent;padding:5px 0px 5px 0px\">\r\n\r\n\r\n<div style=\"padding-right:10px;padding-left:10px\" align=\"center\">\r\n\r\n\r\n<div style=\"font-size:1px;line-height:10px\"></div>\r\n<img style=\"outline:none;text-decoration:none;clear:both;border:0;height:auto;float:none;width:100%;max-width:43px;display:block\" title=\"Image\" src=\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRAHXPluq6GtTRPDIHRv5kJPy86uFjp5sO7hg&s\" alt=\"Image\" width=\"43\" align=\"center\" border=\"0\" class=\"CToWUd\" data-bit=\"iit\">\r\n<div style=\"font-size:1px;line-height:10px\"></div>\r\n\r\n\r\n</div>\r\n\r\n\r\n</div>\r\n\r\n\r\n</div>\r\n</div>\r\n\r\n\r\n\r\n</div>\r\n</div>\r\n</div>\r\n<div style=\"background-color:transparent\">\r\n<div style=\"margin:0 auto;min-width:320px;max-width:640px;word-wrap:break-word;word-break:break-word;background-color:#cccccb\">\r\n<div style=\"border-collapse:collapse;display:table;width:100%;background-color:#cccccb\">\r\n\r\n\r\n\r\n<div style=\"min-width:320px;max-width:640px;display:table-cell;vertical-align:top\">\r\n<div style=\"width:100%!important\">\r\n\r\n\r\n<div style=\"border:0px solid transparent;padding:5px 0px 5px 0px\">\r\n\r\n<div style=\"color:#555555;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif;line-height:120%;padding:5px\">\r\n<div style=\"font-size:12px;line-height:14px;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif;color:#555555\">\r\n<p style=\"font-size:14px;line-height:13px;text-align:center;margin:0\"><span style=\"font-size:11px\"><em><span style=\"line-height:13px;font-size:11px\">Cuộc sống\r\nvốn có rất nhiều lựa chọn, cảm ơn vì bạn đã chọn Cick&Clack.</span></em>\r\n</span></p>\r\n\r\n</div>\r\n</div>\r\n\r\n\r\n\r\n</div>\r\n\r\n\r\n</div>\r\n</div>\r\n\r\n\r\n\r\n</div>\r\n</div>\r\n</div>";

            //Greeting User
            body += $"<div style=\"background-color:transparent\">\r\n<div style=\"margin:0 auto;min-width:320px;max-width:640px;word-wrap:break-word;word-break:break-word;background-color:#fff\">\r\n<div style=\"border-collapse:collapse;display:table;width:100%;background-color:#fff\">\r\n\r\n\r\n\r\n<div style=\"min-width:320px;max-width:640px;display:table-cell;vertical-align:top\">\r\n<div style=\"width:100%!important\">\r\n\r\n\r\n<div style=\"border:0px solid transparent;padding:5px 0px 5px 0px\">\r\n\r\n<div style=\"color:#000;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif;line-height:150%;padding:30px 50px 10px 50px\">\r\n<div style=\"font-size:12px;line-height:18px;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif;color:#000\">\r\n<p style=\"font-size:12px;line-height:16px;margin:0\"><span style=\"font-size:11px\">Xin\r\nchào <strong>{order.ShippingAddress.FirstName + " " + order.ShippingAddress.LastName}</strong> , </span></p>\r\n<p style=\"font-size:12px;line-height:16px;margin:0\"><span style=\"font-size:11px\">ToolZone xin thông báo đã nhận được đơn đặt hàng mang mã số <span style=\"color:#f15f2e;line-height:16px;font-size:11px\"><strong><a style=\"text-decoration:underline;color:#f15f2e\" rel=\"noopener noreferrer\">{order.Id}</a></strong></span> của bạn. </span></p>\r\n<p style=\"font-size:12px;line-height:16px;margin:0\"><span style=\"font-size:11px\">Đơn\r\nhàng của bạn bạn được tiếp nhận và trong quá trình xử lí. Dưới\r\nđây là thông tin đơn hàng, bạn cũng có thể theo dõi trạng thái đơn hàng bất cứ lúc\r\nnào bạn muốn.</span></p>\r\n\r\n</div>\r\n</div>\r\n\r\n<div style=\"padding:0px 10px 30px 10px\" align=\"center\">\r\n\r\n\r\n\r\n\r\n</div>\r\n\r\n\r\n</div>\r\n\r\n\r\n</div>\r\n</div>\r\n\r\n\r\n\r\n</div>\r\n</div>\r\n</div>";

            //Table Head of Order Detail
            body += "<div style=\"background-color:transparent\">\r\n    <div style=\"Margin:0 auto;width:640px;word-wrap:break-word;word-break:break-word;background-color:#fff\">\r\n            <div style=\"border-collapse:collapse;display:table;width:100%;background-color:#fff\">\r\n                \r\n                \r\n\r\n\r\n                <div style=\"width:640px;display:table-cell;vertical-align:top\">\r\n                    <div style=\"width:100%!important\">\r\n                        \r\n                        <div style=\"border-top:0px solid transparent;border-left:0px solid transparent;border-bottom:0px solid transparent;border-right:0px solid transparent;padding-top:5px;padding-bottom:5px;padding-right:50px;padding-left:50px\">\r\n                            \r\n                            \r\n                            <div style=\"color:#231f20;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif;line-height:120%;padding-top:0px;padding-right:50px;padding-bottom:0px;padding-left:50px\">\r\n                                <div style=\"font-size:12px;line-height:14px;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif;color:#231f20\">\r\n                                    <p style=\"font-size:14px;line-height:16px;margin:0\"><span style=\"color:#000000;font-size:14px;line-height:16px\"><strong>CHI TIẾT ĐƠN\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tHÀNG</strong></span></p>\r\n                                </div>\r\n                            </div>\r\n                                                        \r\n                            \r\n                            <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"table-layout:fixed;vertical-align:top;border-spacing:0;border-collapse:collapse;min-width:100%\" valign=\"top\" width=\"100%\">\r\n                                <tbody>\r\n                                <tr style=\"vertical-align:top\" valign=\"top\">\r\n                                    <td style=\"word-break:break-word;vertical-align:top;min-width:100%;padding-top:10px;padding-right:50px;padding-bottom:10px;padding-left:50px;border-collapse:collapse\" valign=\"top\">\r\n                                        <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" height=\"0\" role=\"presentation\" style=\"table-layout:fixed;vertical-align:top;border-spacing:0;border-collapse:collapse;width:100%;border-top:2px solid #000;height:0px\" valign=\"top\" width=\"100%\">\r\n                                            <tbody>\r\n                                            <tr style=\"vertical-align:top\" valign=\"top\">\r\n                                                <td height=\"0\" style=\"word-break:break-word;vertical-align:top;border-collapse:collapse\" valign=\"top\"><span></span></td>\r\n                                            </tr>\r\n                                            </tbody>\r\n                                        </table>\r\n                                    </td>\r\n                                </tr>\r\n                                </tbody>\r\n                            </table>";


            var format = System.Globalization.CultureInfo.GetCultureInfo("vi-VN");
            //Order Detail
            var productTask = order.OrderItems.Select(async detail =>
            {
                var product = await GetProductById(detail.ProductId);

                return new
                {
                    Product = product.Product,
                    Detail = detail
                };
            });
            var products = await Task.WhenAll(productTask);
            foreach (var item in products)
            {
                var product = item.Product;
                var detail = item.Detail;
                var productName = product.Name;
                body += $"  <div style=\"font-size:16px;text-align:center;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif\">\r\n                                    <div style=\"padding:10px 50px\">\r\n                                        <table style=\"width:100%;height:90px;font-size:10px;color:#939598;border-spacing:0px;border-collapse:collapse\">\r\n                                            <tbody><tr>\r\n                                                <td style=\"width:100px;text-align:left\">\r\n</td>\r\n<td style=\"text-align:left\">\r\n<table style=\"width:100%;height:100%;border-spacing:0px;border-collapse:collapse\">\r\n                                                        <tbody><tr>\r\n                                                            <td style=\"font-size:14px;font-weight:bold;vertical-align:top\">\r\n                                                                {productName}<br>                                                            </td>\r\n                                                        </tr>\r\n                                                                                                                  <tr>\r\n                                                            <td style=\"height:10px\"><b>Số\r\n                                                                    lượng:</b>&nbsp;&nbsp;{detail.Quantity}</td>\r\n                                                        </tr>\r\n                                                        <tr>\r\n                                                                                                                            <td style=\"height:10px\">\r\n                                                                    <b>Giá:</b> {String.Format(format, "{0:c0}", detail.Price)} \r\n                                                                </td>\r\n                                                                                                                    </tr>\r\n                                                    </tbody></table>\r\n                                                </td>\r\n                                                <td style=\"width:100px;text-align:right;vertical-align:top;font-size:11px;font-weight:bold;padding-top:4px\">\r\n                                                    {String.Format(format, "{0:c0}", detail.Price)}\r\n                                                </td>\r\n                                            </tr>\r\n                                        </tbody></table>\r\n                                    </div>\r\n                                </div>";
            }

            //Table Tail
            body += "<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"table-layout:fixed;vertical-align:top;border-spacing:0;border-collapse:collapse;min-width:100%\" valign=\"top\" width=\"100%\">\r\n                                <tbody>\r\n                                <tr style=\"vertical-align:top\" valign=\"top\">\r\n                                    <td style=\"word-break:break-word;vertical-align:top;min-width:100%;padding-top:10px;padding-right:50px;padding-bottom:10px;padding-left:50px;border-collapse:collapse\" valign=\"top\">\r\n                                        <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" height=\"0\" role=\"presentation\" style=\"table-layout:fixed;vertical-align:top;border-spacing:0;border-collapse:collapse;width:100%;border-top:2px solid #000;height:0px\" valign=\"top\" width=\"100%\">\r\n                                            <tbody>\r\n                                            <tr style=\"vertical-align:top\" valign=\"top\">\r\n                                                <td height=\"0\" style=\"word-break:break-word;vertical-align:top;border-collapse:collapse\" valign=\"top\"><span></span></td>\r\n                                            </tr>\r\n                                            </tbody>\r\n                                        </table>\r\n                                    </td>\r\n                                </tr>\r\n                                </tbody>\r\n                            </table>\r\n                            \r\n\r\n                            \r\n                        </div>\r\n                        \r\n                    </div>\r\n                </div>\r\n                \r\n                \r\n            </div>\r\n        </div>\r\n    </div>";

            //Order Total 
            body += $"<div style=\"background-color:transparent\">\r\n        <div style=\"Margin:0 auto;width:640px;word-wrap:break-word;word-break:break-word;background-color:#fff\">\r\n            <div style=\"border-collapse:collapse;display:table;width:100%;background-color:#fff\">\r\n                \r\n                \r\n                <div style=\"min-width:320px;max-width:320px;display:table-cell;vertical-align:top\">\r\n                    <div style=\"width:100%!important\">\r\n                        \r\n                        <div style=\"border-top:0px solid transparent;border-left:0px solid transparent;border-bottom:0px solid transparent;border-right:0px solid transparent;padding-top:0px;padding-bottom:30px;padding-right:0px;padding-left:50px\">\r\n                            \r\n                            \r\n\r\n                            <div style=\"color:#555555;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif;line-height:150%;padding-top:0px;padding-right:0px;padding-bottom:0px;padding-left:50px\">\r\n                                <div style=\"font-size:12px;line-height:18px;color:#555555;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif\">\r\n                                    <p style=\"font-size:14px;line-height:16px;margin:0\"><span style=\"font-size:11px\"><strong><span style=\"line-height:16px;font-size:11px\">Tổng\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tgiá trị đơn hàng: </span></strong></span></p>\r\n                                    <p style=\"font-size:14px;line-height:16px;margin:0\"><span style=\"font-size:11px\"><strong><span style=\"line-height:16px;font-size:11px\">Khuyến\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tmãi: </span></strong></span></p>\r\n                                    <p style=\"font-size:14px;line-height:16px;margin:0\"><span style=\"font-size:11px\"><strong><span style=\"line-height:16px;font-size:11px\">Phí vận\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tchuyển: </span></strong></span></p>\r\n                                    <p style=\"font-size:14px;line-height:16px;margin:0\"><span style=\"font-size:11px\"><strong><span style=\"line-height:16px;font-size:11px\">Phí thanh toán: </span></strong></span></p>\r\n                                    <p style=\"font-size:14px;line-height:21px;margin:0\"><span style=\"color:#f15f2e;font-size:14px;line-height:21px\"><strong>Tổng thanh\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\ttoán:</strong></span></p>\r\n                                </div>\r\n                            </div>\r\n                            \r\n                            \r\n                        </div>\r\n                        \r\n                    </div>\r\n                </div>\r\n                \r\n                \r\n                <div style=\"min-width:320px;max-width:320px;display:table-cell;vertical-align:top\">\r\n                    <div style=\"width:100%!important\">\r\n                        \r\n                        <div style=\"border-top:0px solid transparent;border-left:0px solid transparent;border-bottom:0px solid transparent;border-right:0px solid transparent;padding-top:0px;padding-bottom:0px;padding-right:50px;padding-left:0px\">\r\n                            \r\n                            \r\n                                                        <div style=\"color:#555555;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif;line-height:150%;padding-top:0px;padding-right:50px;padding-bottom:0px;padding-left:0px\">\r\n                                <div style=\"font-size:12px;line-height:18px;color:#555555;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif\">\r\n                                    <p style=\"font-size:14px;line-height:16px;text-align:right;margin:0\"><span style=\"font-size:11px\"><strong><span style=\"line-height:16px;font-size:11px\">{String.Format(format, "{0:c0}", TotalBefore)}</span></strong></span>\r\n                                    </p>\r\n                                    <p style=\"font-size:14px;line-height:16px;text-align:right;margin:0\"><span style=\"font-size:11px\"><strong><span style=\"line-height:16px;font-size:11px\"> {String.Format(format, "{0:c0}", MoneyDiscountAmount)}\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</span></strong></span></p>\r\n                                    <p style=\"font-size:14px;line-height:16px;text-align:right;margin:0\"><span style=\"font-size:11px\"><strong><span style=\"line-height:16px;font-size:11px\">0\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</span></strong></span></p>\r\n                                    <p style=\"font-size:14px;line-height:16px;text-align:right;margin:0\"><span style=\"font-size:11px\"><strong><span style=\"line-height:16px;font-size:11px\"> 0                                                   </span></strong></span></p>\r\n                                    <p style=\"font-size:14px;line-height:21px;text-align:right;margin:0\"><span style=\"color:#f15f2e;font-size:14px;line-height:21px\"><strong> {@String.Format(format, "{0:c0}", order.TotalPrice)}\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t</strong></span></p>\r\n                                </div>\r\n                            </div>\r\n                            \r\n                            \r\n                        </div>\r\n                        \r\n                    </div>\r\n                </div>\r\n                \r\n                \r\n            </div>\r\n        </div>\r\n    </div>";

            //Shipping Infor
            body += $" <div style=\"background-color:transparent\">\r\n        <div style=\"Margin:0 auto;width:640px;word-wrap:break-word;word-break:break-word;background-color:#fff\">\r\n            <div style=\"border-collapse:collapse;display:table;width:100%;background-color:#fff\">\r\n                \r\n                \r\n                <div style=\"width:640px;display:table-cell;vertical-align:top\">\r\n                    <div style=\"width:100%!important\">\r\n                        \r\n                        <div style=\"border-top:0px solid transparent;border-left:0px solid transparent;border-bottom:0px solid transparent;border-right:0px solid transparent;padding-top:0px;padding-bottom:0px;padding-right:50px;padding-left:50px\">\r\n                            \r\n                            \r\n                            <div style=\"color:#231f20;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif;line-height:120%;padding-top:0px;padding-right:50px;padding-bottom:0px;padding-left:50px\">\r\n                                <div style=\"font-size:12px;line-height:14px;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif;color:#231f20\">\r\n                                    <p style=\"font-size:14px;line-height:16px;margin:0\"><strong>THÔNG TIN GIAO\r\n                                            NHẬN</strong>\r\n                                    </p>\r\n                                </div>\r\n                            </div>\r\n                            \r\n                            <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"table-layout:fixed;vertical-align:top;border-spacing:0;border-collapse:collapse;min-width:100%\" valign=\"top\" width=\"100%\">\r\n                                <tbody>\r\n                                <tr style=\"vertical-align:top\" valign=\"top\">\r\n                                    <td style=\"word-break:break-word;vertical-align:top;min-width:100%;padding-top:10px;padding-right:50px;padding-bottom:10px;padding-left:50px;border-collapse:collapse\" valign=\"top\">\r\n                                        <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" height=\"0\" role=\"presentation\" style=\"table-layout:fixed;vertical-align:top;border-spacing:0;border-collapse:collapse;width:100%;border-top:2px solid #000;height:0px\" valign=\"top\" width=\"100%\">\r\n                                            <tbody>\r\n                                            <tr style=\"vertical-align:top\" valign=\"top\">\r\n                                                <td height=\"0\" style=\"word-break:break-word;vertical-align:top;border-collapse:collapse\" valign=\"top\"><span></span></td>\r\n                                            </tr>\r\n                                            </tbody>\r\n                                        </table>\r\n                                    </td>\r\n                                </tr>\r\n                                </tbody>\r\n                            </table>\r\n                            \r\n                        </div>\r\n                        \r\n                    </div>\r\n                </div>\r\n                \r\n                \r\n            </div>\r\n        </div>\r\n    </div>\r\n    <div style=\"background-color:transparent\">\r\n        <div style=\"Margin:0 auto;width:640px;word-wrap:break-word;word-break:break-word;background-color:#fff\">\r\n            <div style=\"border-collapse:collapse;display:table;width:100%;background-color:#fff\">\r\n                \r\n                \r\n                <div style=\"min-width:320px;max-width:320px;display:table-cell;vertical-align:top\">\r\n                    <div style=\"width:100%!important\">\r\n                        \r\n                        <div style=\"border-top:0px solid transparent;border-left:0px solid transparent;border-bottom:0px solid transparent;border-right:0px solid transparent;padding-top:0px;padding-bottom:40px;padding-right:0px;padding-left:50px\">\r\n                            \r\n                            \r\n                            <div style=\"color:#555555;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif;line-height:120%;padding-top:0px;padding-right:0px;padding-bottom:0px;padding-left:50px\">\r\n                                <div style=\"font-size:12px;line-height:14px;color:#555555;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif\">\r\n                                    <p style=\"font-size:14px;line-height:13px;margin:0\"><span style=\"font-size:11px\">Họ\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\ttên: {order.ShippingAddress.FirstName + " " + order.ShippingAddress.LastName}</span></p>\r\n                                    <p style=\"font-size:14px;line-height:13px;margin:0\"><span style=\"font-size:11px\"> Điện\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tthoại: {order.ShippingAddress.Phone} </span></p>\r\n                                    <p style=\"font-size:14px;line-height:13px;margin:0\"><span style=\"font-size:11px\">Email: <a href=\"mailto:buingocminhtam2k3@gmail.com\" target=\"_blank\">{order.ShippingAddress.EmailAddress}</a> </span></p>\r\n                                    <p style=\"font-size:14px;line-height:13px;margin:0\"><span style=\"font-size:11px\">Địa\r\n                                            chỉ: {order.ShippingAddress.AddressLine} </span></p>\r\n                                    <p style=\"font-size:14px;line-height:13px;margin:0\"><span style=\"font-size:11px\">Phường/xã: {order.ShippingAddress.Ward}</span></p>\r\n                                    <p style=\"font-size:14px;line-height:13px;margin:0\"><span style=\"font-size:11px\">Quận/Huyện: {order.ShippingAddress.District}</span></p>\r\n                                    <p style=\"font-size:14px;line-height:13px;margin:0\"><span style=\"font-size:11px\">Thành\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tphố/Tỉnh: {order.ShippingAddress.City}</span></p>\r\n                                </div>\r\n                            </div>\r\n                            \r\n                            \r\n                        </div>\r\n                        \r\n                    </div>\r\n                </div>\r\n                \r\n                \r\n                <div style=\"min-width:320px;max-width:320px;display:table-cell;vertical-align:top\">\r\n                    <div style=\"width:100%!important\">\r\n                        \r\n                        <div style=\"border-top:0px solid transparent;border-left:0px solid transparent;border-bottom:0px solid transparent;border-right:0px solid transparent;padding-top:0px;padding-bottom:0px;padding-right:50px;padding-left:0px\">\r\n                            \r\n                            \r\n                            <div style=\"color:#555555;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif;line-height:120%;padding-top:0px;padding-right:50px;padding-bottom:0px;padding-left:0px\">\r\n                                <div style=\"font-size:12px;line-height:14px;color:#555555;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif\">\r\n                                    <p style=\"font-size:14px;line-height:13px;margin:0\"><span style=\"font-size:11px\"><strong><span style=\"line-height:13px;font-size:11px\">Phương\r\n\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t\tthức thanh toán:</span></strong></span></p>\r\n                                    <p style=\"font-size:14px;line-height:13px;margin:0\"><span style=\"font-size:11px\">Thanh toán bằng tiền mặt khi nhận hàng (COD)</span></p>\r\n                                    <p style=\"font-size:14px;line-height:13px;margin:0\"><span style=\"font-size:11px\">&nbsp;</span></p>\r\n                                </div>\r\n                            </div>\r\n                            \r\n                            \r\n                        </div>\r\n                        \r\n                    </div>\r\n                </div>\r\n                \r\n                \r\n            </div>\r\n        </div>\r\n    </div>";

            //Footer
            body += "\r\n<div style=\"background-color:transparent\">\r\n<div style=\"margin:0 auto;min-width:320px;max-width:640px;word-wrap:break-word;word-break:break-word;background-color:#fff\">\r\n<div style=\"border-collapse:collapse;display:table;width:100%;background-color:#fff\">\r\n\r\n\r\n\r\n<div style=\"min-width:320px;max-width:640px;display:table-cell;vertical-align:top\">\r\n<div style=\"width:100%!important\">\r\n\r\n\r\n\r\n<table style=\"table-layout:fixed;vertical-align:top;border-spacing:0;border-collapse:collapse;min-width:100%\" role=\"presentation\" border=\"0\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\">\r\n<tbody>\r\n<tr style=\"vertical-align:top\" valign=\"top\">\r\n<td style=\"word-break:break-word;vertical-align:top;min-width:100%;border-collapse:collapse;padding:10px 50px 10px 50px\" valign=\"top\">\r\n<table style=\"table-layout:fixed;vertical-align:top;border-spacing:0;border-collapse:collapse;width:100%;border-top:1px dashed #cccccb;height:0px\" role=\"presentation\" border=\"0\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" align=\"center\">\r\n<tbody>\r\n<tr style=\"vertical-align:top\" valign=\"top\">\r\n<td style=\"word-break:break-word;vertical-align:top;border-collapse:collapse\" valign=\"top\" height=\"0\"></td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n\r\n<div style=\"color:#555555;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif;line-height:120%;padding:0px 50px 20px 50px\">\r\n<div style=\"font-size:12px;line-height:14px;color:#555555;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif\">\r\n<p style=\"font-size:14px;line-height:12px;margin:0\"><span style=\"color:#939598;font-size:10px\"><em><span style=\"line-height:12px;font-size:10px\">Đây là email được gửi tự động, vui lòng\r\nkhông phản hồi email này. Để tìm hiểu thêm các quy định về đơn hàng hay các chính sách\r\nsau bán hàng của ToolZone, vui lòng truy cập <span style=\"color:#f15f2e;line-height:12px;font-size:10px\"><strong><a style=\"text-decoration:none;color:#f15f2e\" href=\"https://ananas.vn/faqs\" rel=\"noopener noreferrer\" target=\"_blank\" data-saferedirecturl=\"https://www.google.com/url?q=https://ananas.vn/faqs&amp;source=gmail&amp;ust=1718110141220000&amp;usg=AOvVaw2XzuS4endcAcO0J1piRWg2\">tại\r\nlink</a></strong></span> hoặc gọi đến<strong> 0914468405</strong> (trong giờ\r\nhành chính) để được hướng dẫn.</span>\r\n</em>\r\n</span></p>\r\n\r\n</div>\r\n</div>\r\n\r\n\r\n\r\n</div>\r\n\r\n\r\n</div>\r\n</div>\r\n\r\n\r\n\r\n</div>\r\n</div>\r\n</div>\r\n<div style=\"background-color:transparent\">\r\n<div style=\"margin:0 auto;min-width:320px;max-width:640px;word-wrap:break-word;word-break:break-word;background-color:#4d4d4d\">\r\n<div style=\"border-collapse:collapse;display:table;width:100%;background-color:#4d4d4d\">\r\n\r\n\r\n\r\n<div style=\"min-width:320px;max-width:320px;display:table-cell;vertical-align:top\">\r\n<div style=\"width:100%!important\">\r\n\r\n\r\n<div style=\"border:0px solid transparent;padding:30px 0px 30px 50px\">\r\n\r\n\r\n<div style=\"padding-right:0px;padding-left:0px\" align=\"left\">\r\n\r\n\r\n</div>\r\n\r\n<div style=\"color:#555555;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif;line-height:120%;padding:20px 0px 0px 0px\">\r\n<div style=\"font-size:12px;line-height:14px;color:#555555;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif\">\r\n<p style=\"font-size:14px;line-height:13px;margin:0\"><span style=\"font-size:11px\"><strong><span style=\"color:#939598;line-height:13px;font-size:11px\">ToolZone Online Team\r\n</span></strong>\r\n</span></p>\r\n<p style=\"font-size:14px;line-height:13px;margin:0\"><span style=\"color:#939598;font-size:11px\"><strong>Phone:</strong> 0914468405 </span></p>\r\n<p style=\"font-size:14px;line-height:16px;margin:0\"><span style=\"color:#939598;line-height:16px;font-size:14px\"><span style=\"font-size:11px;line-height:13px\"><strong>Add:</strong>95/27, hẻm 95, đường Đ. Mậu Thân, An Phú, Ninh Kiều, Cần Thơ, Việt Nam</span> </span></p>\r\n\r\n</div>\r\n</div>\r\n\r\n\r\n\r\n</div>\r\n\r\n\r\n</div>\r\n</div>\r\n\r\n\r\n<div style=\"min-width:320px;max-width:320px;display:table-cell;vertical-align:top\">\r\n<div style=\"width:100%!important\">\r\n\r\n\r\n<div style=\"border:0px solid transparent;padding:0px 50px 0px 0px\">\r\n\r\n\r\n<table style=\"table-layout:fixed;vertical-align:top;border-spacing:0;border-collapse:collapse\" role=\"presentation\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\">\r\n<tbody>\r\n<tr style=\"vertical-align:top\" valign=\"top\">\r\n<td style=\"word-break:break-word;vertical-align:top;border-collapse:collapse;padding:35px 0px 0px 0px\" valign=\"top\">\r\n<table style=\"table-layout:fixed;vertical-align:top;border-spacing:0;border-collapse:undefined\" role=\"presentation\" cellspacing=\"0\" cellpadding=\"0\" align=\"left\">\r\n<tbody>\r\n<tr style=\"vertical-align:top;display:inline-block;text-align:left\" align=\"left\" valign=\"top\">\r\n<td style=\"word-break:break-word;vertical-align:top;padding-bottom:5px;padding-right:20px;padding-left:0px;border-collapse:collapse\" valign=\"top\"><a href=\"https://www.facebook.com/Ananas.vietnam/\" rel=\"noopener noreferrer\" target=\"_blank\" data-saferedirecturl=\"https://www.google.com/url?q=https://www.facebook.com/Ananas.vietnam/&amp;source=gmail&amp;ust=1718110141220000&amp;usg=AOvVaw2jWX1FZZ7LST3oNJa22ySH\"></a></td>\r\n<td style=\"word-break:break-word;vertical-align:top;padding-bottom:5px;padding-right:20px;padding-left:0px;border-collapse:collapse\" valign=\"top\"><a href=\"https://www.instagram.com/ananasvn/\" rel=\"noopener noreferrer\" target=\"_blank\" data-saferedirecturl=\"https://www.google.com/url?q=https://www.instagram.com/ananasvn/&amp;source=gmail&amp;ust=1718110141220000&amp;usg=AOvVaw3wwQdce83phIu_9EeyZ9HK\"><img style=\"outline:none;text-decoration:none;clear:both;height:auto;float:none;border:none;display:block\" title=\"Instagram\" src=\"https://ci3.googleusercontent.com/meips/ADKq_NYhfHeHeVL404fHhznJ-K8p7l22cQvbJnSpaCxqEGJZFrEM3BbXuGt0qUKp-HocK9ZzkakHTLMqQ6Znmw6hVyodmeXShOuCXDrb9Oxn=s0-d-e1-ft#https://ananas.vn/wp-content/uploads/icon_instagram.png\" alt=\"Instagram\" width=\"26\" height=\"25\" class=\"CToWUd\" data-bit=\"iit\"></a></td>\r\n<td style=\"word-break:break-word;vertical-align:top;padding-bottom:5px;padding-right:20px;padding-left:0px;border-collapse:collapse\" valign=\"top\"><a href=\"https://www.youtube.com/discoveryou\" rel=\"noopener noreferrer\" target=\"_blank\" data-saferedirecturl=\"https://www.google.com/url?q=https://www.youtube.com/discoveryou&amp;source=gmail&amp;ust=1718110141220000&amp;usg=AOvVaw3OzYlIUH6DALXofaLvh2bq\"><img style=\"outline:none;text-decoration:none;clear:both;height:auto;float:none;border:none;display:block\" title=\"YouTube\" src=\"https://ci3.googleusercontent.com/meips/ADKq_NZsyYGgtFg8RbBr7zukwlwHGRS1HQSldC3PeWNgTfNxrzThzKnrJs1lkc0flhCZhMO7IKEbVYB5qY8nkIYD8UQyn07EdmbBda40PA=s0-d-e1-ft#https://ananas.vn/wp-content/uploads/icon_youtube.png\" alt=\"YouTube\" width=\"26\" height=\"25\" class=\"CToWUd\" data-bit=\"iit\"></a></td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n<div style=\"padding:20px 0px 0px 0px\" align=\"left\">\r\n\r\n\r\n\r\n</div>\r\n\r\n<div style=\"color:#555555;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif;line-height:120%;padding:28px 0px 0px 0px\">\r\n<div style=\"font-size:12px;line-height:14px;color:#555555;font-family:Arial,'Helvetica Neue',Helvetica,sans-serif\">\r\n<p style=\"font-size:14px;line-height:13px;margin:0\"><span style=\"font-size:11px\"><em><span style=\"color:#808080;line-height:13px;font-size:11px\"><span style=\"color:#939598;line-height:13px;font-size:11px\">Copyright © 2024 ToolZone.\r\nAll rights reserved.</span> </span>\r\n</em>\r\n</span></p>\r\n\r\n</div>\r\n</div>\r\n\r\n\r\n\r\n</div>\r\n\r\n\r\n</div>\r\n</div>\r\n\r\n\r\n\r\n</div>\r\n</div>\r\n</div>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>";

            await _emailSender.SendEmailAsync(userEmail, subject, body, true);
        }
        catch (Exception ex)
        {
            // Log exception and rethrow or handle accordingly
            throw new Exception("An error occurred while sending the email order: " + ex.Message);
        }
    }

    public async Task<EmailTemplate> AddEmailTemplate(EmailTemplate newEmailTemplate)
    {
        try
        {
            await _context.AddAsync(newEmailTemplate);
            await _context.SaveChangesAsync();

            return newEmailTemplate;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<EmailTemplate> UpdateEmailTemplate(EmailTemplate updatedEmailTemplate)
    {
        try
        {
            var newEmailTemplate = _context.EmailTemplates.FirstOrDefault(e => e.EmailTemplateId.Equals(updatedEmailTemplate.EmailTemplateId));

            if (newEmailTemplate == null)
            {
                throw new KeyNotFoundException("Email template does not exist.");
            }

            newEmailTemplate.Active = updatedEmailTemplate.Active;
            newEmailTemplate.Subject = updatedEmailTemplate.Subject;
            newEmailTemplate.Description = updatedEmailTemplate.Description;
            newEmailTemplate.Category = updatedEmailTemplate.Category;
            newEmailTemplate.Body = updatedEmailTemplate.Body;
            newEmailTemplate.Name = updatedEmailTemplate.Name;
            newEmailTemplate.UpdatedBy = updatedEmailTemplate.UpdatedBy;
            newEmailTemplate.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return newEmailTemplate;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<EmailTemplate> GetEmailTemplateById(string id)
    {
        var emailTemplate = await _context.EmailTemplates
        .FirstOrDefaultAsync(e => e.EmailTemplateId.Equals(id));

        if (emailTemplate == null)
        {
            throw new KeyNotFoundException($"Email template with ID '{id}' not found.");
        }

        return emailTemplate;
    }

    public async Task<EmailListDTO> GetList(string[] statusesParam, string[] categoriesParam, string searchterm, string sortBy, string sortOrder, int? pageNumberParam, int? pageSizeParam)
    {
        //Get List from db
        var result = await _context.EmailTemplates.ToListAsync();

        //Call filter function 
        result = Filter(statusesParam, categoriesParam, result);
        result = Search(result, searchterm);
        result = Sort(sortBy, sortOrder, result);

        if(!pageNumberParam.HasValue && !pageSizeParam.HasValue)
        {
            return new EmailListDTO()
            {
                listEmail = result,
                totalPages = 1
            };
        }
        //Calculate pagination
        int pageNumber = pageNumberParam ?? 1;
        int pageSize = pageSizeParam ?? 10;
        var totalItems = result.Count();
        var TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

        //Get final result base on page size and page number 
        result = result.Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

        return new EmailListDTO()
        {
            listEmail = result,
            totalElements = totalItems,
            totalPages = TotalPages
        };
    }

    public async Task<List<EmailTemplate>> GetList()
    {
        return await _context.EmailTemplates.Where(e => e.Active).OrderByDescending(e => e.Id).ToListAsync();
    }

    public async Task SendEmailByEmailTemplate(EmailTemplate template, string to)
    {
        try
        {
            var body = template.Body;
            await _emailSender.SendEmailAsync(to, template.Subject, body, true);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }

    private List<EmailTemplate> Filter(string[] statuses, string[] categories, List<EmailTemplate> list)
    {
        if (categories != null && categories.Length > 0)
        {
            list = list.Where(e => categories.Contains(e.Category.Trim())).ToList();
        }

        if (statuses != null && statuses.Length > 0)
        {
            list = list.Where(e => statuses.Contains(e.Active.ToString())).ToList();
        }

        return list;
    }

    private List<EmailTemplate> Sort(string sortBy, string sortOrder, List<EmailTemplate> list)
    {
        switch (sortBy)
        {
            case "name":
                list = sortOrder == "ascend" ? list.OrderBy(e => e.Name).ToList() : list.OrderByDescending(e => e.Name).ToList();
                break;
            case "description":
                list = sortOrder == "ascend" ? list.OrderBy(e => e.Description).ToList() : list.OrderByDescending(e => e.Description).ToList();
                break;
            case "subject":
                list = sortOrder == "ascend" ? list.OrderBy(e => e.Subject).ToList() : list.OrderByDescending(e => e.Subject).ToList();
                break;
            case "body":
                list = sortOrder == "ascend" ? list.OrderBy(e => e.Body).ToList() : list.OrderByDescending(e => e.Body).ToList();
                break;
            case "active":
                list = sortOrder == "ascend" ? list.OrderBy(e => e.Active).ToList() : list.OrderByDescending(e => e.Active).ToList();
                break;
            case "category":
                list = sortOrder == "ascend" ? list.OrderBy(e => e.Category).ToList() : list.OrderByDescending(e => e.Category).ToList();
                break;
            case "createdBy":
                list = sortOrder == "ascend" ? list.OrderBy(e => e.CreatedBy).ToList() : list.OrderByDescending(e => e.CreatedBy).ToList();
                break;
            case "createdDate":
                list = sortOrder == "ascend" ? list.OrderBy(e => e.CreatedDate).ToList() : list.OrderByDescending(e => e.CreatedDate).ToList();
                break;
            default:
                list = list.OrderByDescending(e => e.Id).ToList();
                break;
        }
        return list;
    }

    private List<EmailTemplate> Search(List<EmailTemplate> list, string searchtearm)
    {
        if (!string.IsNullOrEmpty(searchtearm))
        {
            list = list.Where(p =>
                        p.Name.Contains(searchtearm, StringComparison.OrdinalIgnoreCase) ||
                        p.Description.Contains(searchtearm, StringComparison.OrdinalIgnoreCase) ||
                        p.CreatedBy.Contains(searchtearm, StringComparison.OrdinalIgnoreCase))
                        .ToList();
        }
        return list;
    }

    public async Task<BaseResponse<MemoryStream>> ImportEmailTemplate(IFormFile excelFile, string userId)
    {
        try
        {
            var contentPath = environment.ContentRootPath;
            var path = Path.Combine(contentPath, "Uploads\\Templates");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var filePath = Path.Combine(path, excelFile.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await excelFile.CopyToAsync(stream);
            }

            List<EmailTemplate> templates = new List<EmailTemplate>();
            List<(int RowIndex, List<string> Errors)> errorDetails = new List<(int, List<string>)>();


            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    bool isHeaderSkipped = false;
                    int rowIndex = 0;

                    while (reader.Read())
                    {
                        rowIndex++;
                        List<string> validationErrors = new List<string>();

                        if (!isHeaderSkipped)
                        {
                            isHeaderSkipped = true; // Skip header row
                            continue;
                        }

                        /* === Validation  ===*/
                        var error = "";

                        //Validation Status
                        if (!ImportFieldValidation.IsValidateBoolean(reader.GetValue(4)?.ToString(), out error))
                        {
                            validationErrors.Add(error);
                        }

                        //Skip row 
                        if (validationErrors.Count > 0)
                        {
                            errorDetails.Add((rowIndex, validationErrors));
                            continue;
                        }


                        var user = new EmailTemplate()
                        {
                            Name = reader.GetValue(0).ToString() ?? "Error Name!",
                            Description = reader.GetValue(1).ToString() ?? "Error Description!",
                            Subject = reader.GetValue(2).ToString() ?? "Error Subject!",
                            Body = reader.GetValue(3).ToString() ?? "Error Body!",
                            Active = bool.Parse(reader.GetValue(4).ToString() ?? "False"),
                            Category = reader.GetValue(5).ToString() ?? "Error Category!",
                            CreatedBy = userId,
                            CreatedDate = DateTime.Now
                        };
                        templates.Add(user);
                    }
                }
            }

            if (errorDetails.Any())
            {
                using (var errorWorkbook = new XLWorkbook())
                {
                    var worksheet = errorWorkbook.Worksheets.Add("Errors Report");

                    // Add header
                    worksheet.Cell(1, 1).Value = "Row Index";
                    worksheet.Cell(1, 2).Value = "Error Messages";

                    int errorRowIndex = 2; // Start from the second row, assuming the first row is for headers

                    foreach (var (rowIndex, errors) in errorDetails)
                    {
                        // Split errors to handle multiple errors for the same row
                        foreach (var error in errors)
                        {
                            worksheet.Cell(errorRowIndex, 1).Value = rowIndex; // Row index from the original data
                            worksheet.Cell(errorRowIndex, 2).Value = error;    // Individual error message
                            errorRowIndex++; // Move to the next row for the next error
                        }
                    }

                    using (var errorMemoryStream = new MemoryStream())
                    {
                        errorWorkbook.SaveAs(errorMemoryStream);
                        errorMemoryStream.Position = 0; // Reset stream position for reading
                        return new BaseResponse<MemoryStream>(errorMemoryStream); // Return the MemoryStream
                    }
                }
            }

            // Save valid templates to the database
            if (templates.Any())
            {
                await _context.EmailTemplates.AddRangeAsync(templates);
                await _context.SaveChangesAsync();
            }

        }
        catch (IndexOutOfRangeException ex)
        {
            throw new BadRequestException("File Import error");
        }

        return null;
    }

    public async Task<byte[]> ExportEmailFilter(string[] statusesParam, string[] categoriesParam, string searchterm, int pageNumberParam, int pageSizeParam)
    {
        try
        {
            //Get List from db
            var result = await _context.EmailTemplates.ToListAsync();

            //Call filter function 
            result = Filter(statusesParam, categoriesParam, result);
            result = Search(result, searchterm);

            DataTable dt = new DataTable();
            dt.Columns.Add("Template Name", typeof(string));
            dt.Columns.Add("Active", typeof(bool));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("Category", typeof(string));
            dt.Columns.Add("Created By", typeof(string));
            dt.Columns.Add("Created Date", typeof(DateTime));
            dt.Columns.Add("Updated By", typeof(string));
            dt.Columns.Add("Updated Date", typeof(string));

            foreach (var item in result)
            {
                DataRow row = dt.NewRow();
                row[0] = item.Name;
                row[1] = item.Active;
                row[2] = item.Description;
                row[3] = item.Category;
                row[4] = item.CreatedBy;
                row[5] = item.CreatedDate;
                row[6] = item.UpdatedBy;
                row[7] = item.UpdatedDate;
                dt.Rows.Add(row);
            }

            var memory = new MemoryStream();
            using (var excel = new ExcelPackage(memory))
            {
                var worksheet = excel.Workbook.Worksheets.Add("Sheet1");

                worksheet.Cells["A1"].LoadFromDataTable(dt, true);
                worksheet.Cells["A1:AN1"].Style.Font.Bold = true;
                worksheet.DefaultRowHeight = 25;


                return excel.GetAsByteArray();
            }

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}


