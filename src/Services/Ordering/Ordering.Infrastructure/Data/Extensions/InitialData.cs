using Ordering.Domain.Models;
using Ordering.Domain.ValueObjects;

namespace Ordering.Infrastructure.Data.Extensions
{
    internal class InitialData
    {
        public static IEnumerable<Order> OrdersWithItems
        {
            get
            {
                var address1 = Address.Of("valeria", "ozkaya", "0912345678", "valeria@gmail.com", "Duong 15", "Tp.HCM", "Tp.Thu Duc", "Linh Trung");
                var address2 = Address.Of("john", "doe", "0702148775", "", "600 NVC", "CanTho", "Kinh Kieu", "An Binh");

                var payment1 = Payment.Of("mehmet", "5555555555554444", "12/28", "355", "Paypal");
                var payment2 = Payment.Of("john", "8885555555554444", "06/30", "222", "Credit card");

                var order1 = Order.Create(
                                OrderId.Of(Guid.NewGuid()),
                                CustomerId.Of(new Guid("58c49479-ec65-4de2-86e7-033c546291aa")),
                                shippingAddress: address1,
                                "Online Banking",
                                null);
                order1.Add(ProductId.Of(new Guid("5334c996-8457-4cf0-815c-ed2b77c4ff61")), "6885EBEB-5104-4D75-AEF6-52044131632F", 2, 500, "Red");
                order1.Add(ProductId.Of(new Guid("c67d6323-e8b1-4bdf-9a75-b0d0d2e7e914")), "3B9CEBB9-305B-4C29-B1D3-B29D2C82622D", 1, 400, "Green");

                var order2 = Order.Create(
                                OrderId.Of(Guid.NewGuid()),
                                CustomerId.Of(new Guid("189dc8dc-990f-48e0-a37b-e6f2b60b9d7d")),
                                shippingAddress: address2,
                                "Cash On Delivery",
                                "68cc65ca-0829-4032-9374-80db70631f65");
                order2.Add(ProductId.Of(new Guid("4f136e9f-ff8c-4c1f-9a33-d12f689bdab8")), "36CF519B-52CA-4D8C-BC89-581021126D42", 1, 650, "Black");
                order2.Add(ProductId.Of(new Guid("6ec1297b-ec0a-4aa1-be25-6726e3b51a27")), "5F5BE5EB-DD39-477D-A56D-40D28EDEF44A", 2, 450, "Royal Blue");

                return new List<Order> { order1, order2 };
            }
        }
    }
}
