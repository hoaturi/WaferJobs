namespace JobBoard;

public interface IFileUploadService
{
    Task<string> UploadBusinessLogoAsync(string fileName, Stream file);
}
