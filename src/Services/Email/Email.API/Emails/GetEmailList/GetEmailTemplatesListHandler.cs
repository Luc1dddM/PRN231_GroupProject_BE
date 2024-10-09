using BuildingBlocks.CQRS;
using Email.API.Models;
using Email.Models;
using Microsoft.EntityFrameworkCore;

namespace Email.API.Emails.GetEmailList;

public record GetEmailsQuery(string? Category = null, string? Name = null, string? Subject = null, bool? Status = null) : IQuery<GetEmailsResult>;
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
        IQueryable<EmailTemplate> emailQuery = _context.EmailTemplates;

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            emailQuery = emailQuery.Where(email => email.Category.Contains(query.Category));
        }

       /* if (!string.IsNullOrWhiteSpace(query.Name))
        {
            emailQuery = emailQuery.Where(email => email.Name.Contains(query.Name));
        }*/

       /* if (!string.IsNullOrWhiteSpace(query.Subject))
        {
            emailQuery = emailQuery.Where(email => email.Subject.Contains(query.Subject));
        }
*/
        if (query.Status.HasValue)
        {
            emailQuery = emailQuery.Where(email => email.Active == query.Status.Value);
        }

        var emails = await emailQuery.ToListAsync(cancellationToken);

        return new GetEmailsResult(emails);
    }
}

