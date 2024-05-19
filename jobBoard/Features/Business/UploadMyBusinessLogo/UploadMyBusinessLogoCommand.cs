using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Business.UploadMyBusinessLogo;

public record UploadMyBusinessLogoCommand(IFormFile File) : IRequest<Result<Unit, Error>>;