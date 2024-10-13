using Email.API.Events;
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
        await _emailRepository.SendEmailOrder(@event.OrderId, @event.UserEmail, @event.CouponCode);
    }
}
