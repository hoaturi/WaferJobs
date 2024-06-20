using JobBoard.Common.Models;
using JobBoard.Features.Business.UpdateMyBusinessLogo;
using MediatR;

namespace JobBoard.Features.JobPost.UploadJobPostLogo;

public record UploadJobPostLogoCommand(IFormFile File) : IRequest<Result<UpdateBusinessLogoResponse, Error>>;