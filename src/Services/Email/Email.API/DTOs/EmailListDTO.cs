using Email.Models;

namespace Email.DTOs
{
    public class EmailListDTO
    {
        public List<EmailTemplate> listEmail;
        public int totalPages;
        public int totalElements;
    }
}
