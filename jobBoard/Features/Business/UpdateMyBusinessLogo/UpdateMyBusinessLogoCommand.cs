using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.UpdateMyBusinessLogo;

public record UpdateMyBusinessLogoCommand(IFormFile File) : IRequest<Result<UpdateBusinessLogoResponse, Error>>;