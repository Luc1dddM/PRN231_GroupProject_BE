namespace Ordering.Domain.Models
{
    public class Order : Aggrerate<OrderId>
    {
        private readonly List<OrderItem> _orderItems = new();
        //private decimal _dicountAmount = 0m; // Add a private discount field
        private decimal _totalPrice; //Backing field to store the base total before applying the discount.

        public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();
        public CustomerId CustomerId { get; private set; } = default!; //"private set" mean it can only be modified within the Order class itself.
        public Address ShippingAddress { get; private set; } = default!;
        public Payment Payment { get; private set; } = default!;
        public OrderStatus Status { get; private set; } = OrderStatus.Pending;
        public string? CouponId { get; set; } = default!;
        public decimal TotalPrice
        {
            get => _totalPrice;
            set => _totalPrice = value;
        }



        public static Order Create(OrderId id, CustomerId customerId, Address shippingAddress, Payment payment, string couponId)
        {
            var order = new Order
            {
                Id = id,
                CustomerId = customerId,
                ShippingAddress = shippingAddress,
                Payment = payment,
                Status = OrderStatus.Pending,
                CouponId = couponId
            };

            order.AddDomainEvent(new OrderCreatedEvent(order));

            return order;
        }

        public void Update(Address shippingAddress, OrderStatus status)
        {

            ShippingAddress = shippingAddress;
            Status = status;

            AddDomainEvent(new OrderUpdatedEvent(this));
        }


        //this method for adding OrderItem in the Order
        public void Add(ProductId productId, int quantity, decimal price, string color)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

            var orderItem = new OrderItem(Id, productId, quantity, price, color);
            _orderItems.Add(orderItem); //this Add() method belong to the List<>, like: List<OrderItems> list => list.Add(newOrderItems)

            // Update total price when adding an item
            _totalPrice = OrderItems.Sum(x => x.Price * x.Quantity);
        }


        public void ApplyCoupon(decimal discountAmount)
        {
            if (discountAmount > 0 && discountAmount <= TotalPrice)
            {
                _totalPrice -= discountAmount;
            }
        }
    }
}
