namespace Chat.API.Model.DTO
{
    public class UpdateUserConsumer
    {
        public string UserId { get; set; } = default!;
        public bool IsChat { get; set; }
        public string Name { get; set; } = default!;
    }
}
