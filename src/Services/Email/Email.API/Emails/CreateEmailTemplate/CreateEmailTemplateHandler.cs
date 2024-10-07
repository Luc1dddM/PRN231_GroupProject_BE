using BuildingBlocks.CQRS;
using Email.API.Models;
using Email.Models;
using FluentValidation;

namespace Email.API.Emails.CreateEmailTemplate;

// chứa thông tin về email sẽ được create
public record CreateEmailTemplateCommand(string Name, string Description, string Subject, string Body, bool Active, string Category, string CreatedBy)
    : ICommand<CreateEmailTemplateResult>;

public record CreateEmailTemplateResult(int Id);

public class CreateEmailTemplateCommandValidator : AbstractValidator<CreateEmailTemplateCommand>
{
    // validate đã nhập
    public CreateEmailTemplateCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required");
        RuleFor(x => x.Subject).NotEmpty().WithMessage("Subject is required");
        RuleFor(x => x.Body).NotEmpty().WithMessage("Body is required");
        RuleFor(x => x.Category).NotEmpty().WithMessage("Category is required");
    }
}

internal class CreateEmailTemplateHandler : ICommandHandler<CreateEmailTemplateCommand, CreateEmailTemplateResult>
{
    private readonly Prn231GroupProjectContext _context;

    public CreateEmailTemplateHandler(Prn231GroupProjectContext context)
    {
        _context = context;
    }

    public async Task<CreateEmailTemplateResult> Handle(CreateEmailTemplateCommand command, CancellationToken cancellationToken)
    {

        // Kiểm tra hợp lệ
        var validator = new CreateEmailTemplateCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // hợp lệ tiếp tục create
        var emailTemplate = new EmailTemplate
        {
            // Tạo GUID ngẫu nhiên cho EmailTemplateId
            EmailTemplateId = Guid.NewGuid().ToString(),
            Name = command.Name,
            Description = command.Description,
            Subject = command.Subject,
            Body = command.Body,
            Active = command.Active,
            Category = command.Category,
            CreatedDate = DateTime.UtcNow,
            CreatedBy = command.CreatedBy // Sử dụng CreatedBy từ command
        };

        await _context.EmailTemplates.AddAsync(emailTemplate, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateEmailTemplateResult(emailTemplate.Id);
    }
}