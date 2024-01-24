using MediatR;

namespace JobBoard;

public record CreateJobPostCommand(
    int CategoryId,
    int CountryId,
    int EmploymentTypeId,
    string Title,
    string Description,
    string CompanyName,
    string ApplyUrl,
    bool IsRemote,
    bool IsFeatured,
    string? City,
    int? MinSalary,
    int? MaxSalary,
    string? Currency
) : IRequest<Result<Unit, Error>>;

public class CreateJobPostCommandHandler
    : IRequestHandler<CreateJobPostCommand, Result<Unit, Error>>
{
    public async Task<Result<Unit, Error>> Handle(
        CreateJobPostCommand request,
        CancellationToken cancellationToken
    )
    {
        return Unit.Value;
    }
}
