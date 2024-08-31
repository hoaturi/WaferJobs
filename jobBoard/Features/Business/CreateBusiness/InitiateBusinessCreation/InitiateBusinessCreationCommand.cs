using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.CreateBusiness.InitiateBusinessCreation;

public record InitiateBusinessCreationCommand(
    string Name,
    string WebsiteUrl
) : IRequest<Result<Unit, Error>>;