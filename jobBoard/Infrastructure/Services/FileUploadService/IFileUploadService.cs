namespace JobBoard.Infrastructure.Services.FileUploadService;

public interface IFileUploadService
{
    Task<string> UploadBusinessLogoAsync(string fileName, Stream file);
}