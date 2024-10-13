namespace Email.API.Events
{
    public class SendMailOrderEvent
    {
        public string OrderId { get; set; }
        public string UserEmail { get; set; }
        public string CouponCode { get; set; }
    }
}
