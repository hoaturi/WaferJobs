using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobPost.UploadJobPostLogo;

public record UploadJobPostLogoCommand(IFormFile File) : IRequest<Result<UploadJobPostLogoResponse, Error>>;