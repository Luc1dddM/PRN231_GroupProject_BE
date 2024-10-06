using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Ordering.Application.Data
{
    public interface IApplicationDbContext
    {
        //these interface provide a contract for database operations, ensuring that the application layer
        //remains independent of specific database implementation and also not including any infrastructure implementation
        //like SQL server, database provider and so on.

        DbSet<Order> Orders { get; }
        DbSet<OrderItem> OrderItems { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
