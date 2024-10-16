using MediatR;
using WaferJobs.Common.Models;
using WaferJobs.Features.Business.UpdateMyBusinessLogo;

namespace WaferJobs.Features.JobPost.UploadJobPostLogo;

public record UploadJobPostLogoCommand(IFormFile File) : IRequest<Result<UpdateBusinessLogoResponse, Error>>;