using MediatR;

namespace JobBoard;

public record UploadMyBusinessLogoCommand(IFormFile File) : IRequest<Result<Unit, Error>>;
