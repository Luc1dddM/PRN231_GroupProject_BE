using System.ComponentModel.DataAnnotations;

namespace Email.API.DTOs;

public class OrderDto
{
    public string Id { get; set; } = null!;

    public string CustomerId { get; set; } = null!;

    [Required(ErrorMessage = "Shipping address is required.")]
    public ShippingAddressDto ShippingAddress { get; set; } = null!;

    [Required(ErrorMessage = "Payment information is required.")]
    public PaymentDto Payment { get; set; } = null!;

    public int Status { get; set; }

    [Required(ErrorMessage = "Order items are required.")]
    public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();

    public string? CouponId { get; set; }
    public decimal? TotalPrice { get; set; }
}

public class ShippingAddressDto
{
    [Required(ErrorMessage = "First name is required.")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last name is required.")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "The phone number must be numeric and exactly 10 digits.")]
    public string Phone { get; set; } = null!;

    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string? EmailAddress { get; set; }

    [Required(ErrorMessage = "Address line is required.")]
    public string AddressLine { get; set; } = null!;

    [Required(ErrorMessage = "City is required.")]
    public string City { get; set; } = null!;

    [Required(ErrorMessage = "District is required.")]
    public string District { get; set; } = null!;

    [Required(ErrorMessage = "Ward is required.")]
    public string Ward { get; set; } = null!;
    public string GetFullAddress()
    {
        return $"{AddressLine}, {Ward}, {District}, {City}";
    }
}

public class PaymentDto
{
    [Required(ErrorMessage = "Card name is required.")]
    public string CardName { get; set; } = null!;

    [Required(ErrorMessage = "Card number is required.")]
    [RegularExpression(@"^\d{16}$", ErrorMessage = "Card number must be 16 digits.")]
    public string CardNumber { get; set; } = null!;

    [Required(ErrorMessage = "Expiration date is required.")]
    public string Expiration { get; set; } = null!;

    [Required(ErrorMessage = "CVV is required.")]
    [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV must be 3 digits.")]
    public string Cvv { get; set; } = null!;

    [Required(ErrorMessage = "Payment method is required.")]
    public string PaymentMethod { get; set; } = null!;
}

public class OrderItemDto
{
    public string OrderId { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    [Required(ErrorMessage = "Quantity is required.")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Price is required.")]
    public double Price { get; set; }

    [Required(ErrorMessage = "Color is required.")]
    public string Color { get; set; } = null!;
}

public class OrdersResponseDto
{
    public List<OrderDto> Orders { get; set; }
}

