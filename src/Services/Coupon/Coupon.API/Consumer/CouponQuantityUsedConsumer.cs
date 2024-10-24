using BuildingBlocks.Messaging.Events;
using Coupon.API.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

public class CouponQuantityUsedConsumer : IConsumer<CouponQuantityUsedEvent>
{
    private readonly Prn231GroupProjectContext _dbContext;

    public CouponQuantityUsedConsumer(Prn231GroupProjectContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CouponQuantityUsedEvent> context)
    {
        var message = context.Message;

        var coupon = await _dbContext.Coupons.FirstOrDefaultAsync(c => c.CouponCode == message.CouponCode);

        if (coupon != null && coupon.Quantity >= message.QuantityUsed)
        {
            coupon.Quantity -= message.QuantityUsed;
            coupon.UpdatedDate = DateTime.Now;
            await _dbContext.SaveChangesAsync();

            await _dbContext.UpdateCouponStatusIfNeeded(coupon.CouponCode);
        }
        else
        {
  
            Console.WriteLine($"Not enough quantity for Coupon Code: {message.CouponCode}");
        }
    }
}
