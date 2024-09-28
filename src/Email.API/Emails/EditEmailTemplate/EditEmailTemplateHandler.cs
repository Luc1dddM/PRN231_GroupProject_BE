using BuildingBlocks.CQRS;
using Carter;
using Email.API.Models;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Email.API.Emails.EditEmailTemplate;

public record EditEmailTemplateCommand(string EmailTemplateId, string Name, string Description, string Subject, string Body, bool Active, string Category)
    : ICommand<EditEmailTemplateResult>;

public record EditEmailTemplateResult(bool IsSuccess);

public class EditEmailTemplateCommandValidator : AbstractValidator<EditEmailTemplateCommand>
{
    public EditEmailTemplateCommandValidator()
    {
        RuleFor(command => command.EmailTemplateId)
            .NotEmpty().WithMessage("Email Template ID is required");

        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Name is required");

        RuleFor(command => command.Subject)
            .NotEmpty().WithMessage("Subject is required");

        RuleFor(command => command.Body)
            .NotEmpty().WithMessage("Body is required");

        RuleFor(command => command.Category)
            .NotEmpty().WithMessage("Category is required");
    }
}

internal class EditEmailTemplateHandler : ICommandHandler<EditEmailTemplateCommand, EditEmailTemplateResult>
{
    private readonly Prn231GroupProjectContext _context;

    public EditEmailTemplateHandler(Prn231GroupProjectContext context)
    {
        _context = context;
    }

    public async Task<EditEmailTemplateResult> Handle(EditEmailTemplateCommand command, CancellationToken cancellationToken)
    {
        var emailTemplate = await _context.EmailTemplates
            .FirstOrDefaultAsync(e => e.EmailTemplateId == command.EmailTemplateId, cancellationToken);

        if (emailTemplate is null)
        {
            throw new KeyNotFoundException($"Email Template with ID {command.EmailTemplateId} not found");
        }

        // Update email template properties
        emailTemplate.Name = command.Name;
        emailTemplate.Description = command.Description;
        emailTemplate.Subject = command.Subject;
        emailTemplate.Body = command.Body;
        emailTemplate.Active = command.Active;
        emailTemplate.Category = command.Category;
        emailTemplate.UpdatedBy = "System";
        emailTemplate.UpdatedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new EditEmailTemplateResult(true);
    }
}
