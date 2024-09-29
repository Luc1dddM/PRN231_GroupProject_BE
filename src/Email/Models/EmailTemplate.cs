namespace Email.Models;

public partial class EmailTemplate
{
    public int Id { get; set; }

    public string EmailTemplateId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string Body { get; set; } = null!;

    public bool Active { get; set; }

    public string Category { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? UpdatedDate { get; set; }

    public string? UpdatedBy { get; set; }

   /* public virtual ApplicationUser CreatedByNavigation { get; set; } = null!;*/
}
