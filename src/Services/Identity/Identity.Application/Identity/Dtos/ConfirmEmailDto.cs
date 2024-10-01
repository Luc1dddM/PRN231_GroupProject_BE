namespace Identity.Application.Identity.Dtos
{
    public class ConfirmEmailDto
    {
        public string UserId { get; set; }
        public string Token { get; set; }
    }
}
