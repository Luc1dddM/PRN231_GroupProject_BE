using BuildingBlocks.CQRS;
using Email.API.Models;
using Email.Models;
using Microsoft.EntityFrameworkCore;

namespace Email.API.Emails.GetEmailTemplateById;

public record GetEmailByIdQuery(string Id) : IQuery<GetEmailByIdResult>;
public record GetEmailByIdResult(EmailTemplate EmailTemplate);

internal class GetEmailByIdHandler : IQueryHandler<GetEmailByIdQuery, GetEmailByIdResult>
{
    private readonly Prn231GroupProjectContext _context;

    public GetEmailByIdHandler(Prn231GroupProjectContext context)
    {
        _context = context;
    }

    public async Task<GetEmailByIdResult> Handle(GetEmailByIdQuery query, CancellationToken cancellationToken)
    {
       // đổi string thành int
        if (!int.TryParse(query.Id, out int id)) // Kiểm tra ID có thể chuyển đổi thành int ko
        {
            throw new ArgumentException("Invalid Id format", nameof(query.Id));
        }

        var emailTemplate = await _context.EmailTemplates
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);


        if (emailTemplate is null)
        {
            throw new Exception(query.Id); 
        }

        return new GetEmailByIdResult(emailTemplate);
    }
}
