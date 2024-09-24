using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Conference.UploadConferenceLogo;

public record UploadConferenceLogoCommand(IFormFile File) : IRequest<Result<UploadConferenceLogoResponse, Error>>;