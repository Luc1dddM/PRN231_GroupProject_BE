namespace Ordering.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 1, //customer just placed an order
        Approved = 2, //admin check whether product in-stock or not and approved
        Shipping = 3, //package on delivery
        Completed = 4, //once customer receive the package
        Cancelled = 5 //admin or customer can canncel the order
    }
}
