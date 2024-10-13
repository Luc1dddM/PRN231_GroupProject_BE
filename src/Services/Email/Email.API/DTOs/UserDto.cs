namespace Email.API.DTOs
{
    public class UsersResponseDTO
    {
        public bool IsSuccess { get; set; }
        public List<UserDto> Result { get; set; } = new List<UserDto>();
        public string? Message { get; set; }
    }

    public class UserDto
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public bool IsActive { get; set; }
        public DateTime BirthDay { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
