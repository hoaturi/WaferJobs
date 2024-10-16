using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Lookup.ValidatePublicDomain;

public record ValidatePublicDomainCommand(string Domain) : IRequest<Result<ValidatePublicDomainResponse, Error>>;