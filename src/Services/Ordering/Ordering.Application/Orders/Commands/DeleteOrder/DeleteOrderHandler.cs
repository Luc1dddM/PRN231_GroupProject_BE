using BuildingBlocks.Models;

namespace Ordering.Application.Orders.Commands.DeleteOrder
{
    public class DeleteOrderHandler : ICommandHandler<DeleteOrderCommand, DeleteOrderResult>
    {
        private readonly IApplicationDbContext _context;

        public DeleteOrderHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DeleteOrderResult> Handle(DeleteOrderCommand command, CancellationToken cancellationToken)
        {
            //Delete Order entity from command object
            //save to database
            //return result
            try
            {
                //find order ot delete
                var orderId = OrderId.Of(command.EntityId);

                //when we are passing the orderId, orderId is a primary key and the type is OrderId strongly type ID
                //That's why Entity Framework core expecting this strongly type ID
                //When seeking the table, it has conversion definition into configurations that is getting the great value and seek table with the great value.
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.EntityId == orderId, cancellationToken: cancellationToken);

                if (order is null)
                {
                    throw new NotFoundException($"Order with Id {orderId.Value} does not exist.");
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync(cancellationToken);

                return new DeleteOrderResult(new BaseResponse<object>
                {
                    IsSuccess = true,
                    Message = "Order Delete Successful."
                });
            }
            catch (NotFoundException e)
            {
                throw new NotFoundException(e.Message, e);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }
    }
}
