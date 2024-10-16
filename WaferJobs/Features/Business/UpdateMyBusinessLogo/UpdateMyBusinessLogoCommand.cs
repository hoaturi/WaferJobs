using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Business.UpdateMyBusinessLogo;

public record UpdateMyBusinessLogoCommand(IFormFile File) : IRequest<Result<UpdateBusinessLogoResponse, Error>>;