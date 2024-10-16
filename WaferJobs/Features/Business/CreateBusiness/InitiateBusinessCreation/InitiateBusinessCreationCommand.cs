using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Business.CreateBusiness.InitiateBusinessCreation;

public record InitiateBusinessCreationCommand(
    string Name,
    string WebsiteUrl
) : IRequest<Result<Unit, Error>>;