namespace Chat.API.Model.DTO
{
    public class UnReadNotifyDTO
    {
        public string groupId { get; set; } = default!;
        public int Count { get; set; }
    }
}
