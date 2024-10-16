using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Business.CreateBusiness.CompleteBusinessCreation;

public record CompleteBusinessCreationCommand(
    string Token,
    CreateBusinessRequestDto Dto
) : IRequest<Result<Unit, Error>>;