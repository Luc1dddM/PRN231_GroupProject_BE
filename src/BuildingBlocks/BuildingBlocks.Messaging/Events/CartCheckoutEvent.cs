namespace BuildingBlocks.Messaging.Events
{
    public record CartCheckoutEvent : IntegrationEvent
    {
        public Guid CustomerId { get; set; } = default!;
        public decimal TotalPrice { get; set; } = default!;

        //CartItems
        public IEnumerable<CartDetailEvent> CartItems { get; set; }

        public string CouponCode { get; set; } = default!;

        // ShippingAddress
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Phone {  get; set; } = default!;
        public string EmailAddress { get; set; } = default!;
        public string AddressLine { get; set; } = default!;
        public string City { get; set; } = default!;
        public string District { get; set; } = default!;
        public string Ward { get; set; } = default!;

        // Payment
        public string CardName { get; set; } = default!;
        public string CardNumber { get; set; } = default!;
        public string Expiration { get; set; } = default!;
        public string CVV { get; set; } = default!;
        public string PaymentMethod { get; set; } = default!;
    }

    public record CartDetailEvent
    {
        public string ProductId { get; set; } = default!;
        public string ProductName { get; set; } = default!;
        public int Quantity { get; set; } = default!;
        public string Color { get; set; } = default!;
        public decimal Price { get; set; } = default!;
    }
}
