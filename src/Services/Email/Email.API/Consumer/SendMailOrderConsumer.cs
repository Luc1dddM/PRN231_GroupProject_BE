using BuildingBlocks.Messaging.Events;
using Email.API.Repository;
using MassTransit;

namespace Email.API.Consumer;

public class SendMailOrderConsumer : IConsumer<SendMailOrderEvent>
{
    private readonly IEmailRepository _emailRepository;

    public SendMailOrderConsumer(IEmailRepository emailRepository)
    {
        _emailRepository = emailRepository;
    }

    public async Task Consume(ConsumeContext<SendMailOrderEvent> context)
    {
        var @event = context.Message;
        Console.WriteLine($"Received SendMailOrderEvent: OrderId={@event.OrderId}, UserEmail={@event.UserEmail}, CouponCode={@event.CouponCode}");

        await _emailRepository.SendEmailOrder(@event.OrderId, @event.UserEmail, @event.CouponCode);
    }
}
