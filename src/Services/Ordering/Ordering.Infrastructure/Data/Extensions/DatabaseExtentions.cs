using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ordering.Infrastructure.Data.Extensions //for auto migration and seed data
{
    public static class DatabaseExtentions
    {
        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            //auto migrate
            using var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.Database.MigrateAsync().GetAwaiter().GetResult();

            //seed data
            await SeedAsync(context);
        }


        private static async Task SeedAsync(ApplicationDbContext context)
        {
            await SeedOrdersWithItemsAsync(context);
        }

        private static async Task SeedOrdersWithItemsAsync(ApplicationDbContext context)
        {
            if (!await context.Orders.AnyAsync())
            {
                await context.Orders.AddRangeAsync(InitialData.OrdersWithItems);
                await context.SaveChangesAsync();
            }
        }
    }
}
