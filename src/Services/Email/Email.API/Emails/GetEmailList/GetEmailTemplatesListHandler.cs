using BuildingBlocks.CQRS;
using Email.API.Models;
using Email.Models;
using Microsoft.EntityFrameworkCore;

namespace Email.API.Emails.GetEmailList;

public record GetEmailsQuery() : IQuery<GetEmailsResult>; // gửi request query lấy từ dtb
public record GetEmailsResult(IEnumerable<EmailTemplate> Emails);

internal class GetEmailTemplatesListHandler : IQueryHandler<GetEmailsQuery, GetEmailsResult>
{
    private readonly Prn231GroupProjectContext _context;

    public GetEmailTemplatesListHandler(Prn231GroupProjectContext context)
    {
        _context = context;
    }

    public async Task<GetEmailsResult> Handle(GetEmailsQuery query, CancellationToken cancellationToken)
    {
        // get all list from dtb
        var emails = await _context.EmailTemplates.ToListAsync(cancellationToken);

        /*var emails = await _context.EmailTemplates
            .Skip((query.PageNumber ?? 1 - 1) * (query.PageSize ?? 10))
            .Take(query.PageSize ?? 10)
            .ToListAsync(cancellationToken);*/

        return new GetEmailsResult(emails);
    }
}

