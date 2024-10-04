namespace Ordering.Domain.Enums
{
    public enum OrderStatus
    {
        /*Draft = 1,
        Pending = 2,
        Completed = 3,
        Cancelled = 4,*/
        Pending = 1, //customer just placed an order
        Approved = 2, //admin check whether product in-stock or not and approved
        Shipping = 3, //package on delivery
        Completed = 4, //once customer receive the package
        Cancelled = 5, //admin or customer can canncel the order
        Refunded = 6 //if any product damage during shipping stage
    }
}
