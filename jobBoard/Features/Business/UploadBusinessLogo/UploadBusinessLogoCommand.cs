using MediatR;

namespace JobBoard;

public record UploadBusinessLogoCommand(IFormFile File) : IRequest<Result<Unit, Error>>;
