using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Conference.UploadConferenceLogo;

public record UploadConferenceLogoCommand(IFormFile File) : IRequest<Result<UploadConferenceLogoResponse, Error>>;