using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Lookup.ValidatePublicDomain;

public record ValidatePublicDomainCommand(string Domain) : IRequest<Result<ValidatePublicDomainResponse, Error>>;