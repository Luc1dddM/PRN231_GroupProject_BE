namespace Email.Models;

public partial class EmailSend
{
    public int Id { get; set; }

    public string EmailSendId { get; set; } = null!;

    public int TemplateId { get; set; } = 0;

    public string SenderId { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime Sendate { get; set; }


    // Thêm thuộc tính Receiver (trước đây là To)
    public string Receiver { get; set; } = null!;

    public EmailTemplate Template { get; set; } = null!;
}

