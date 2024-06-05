namespace JobBoard.Common.Interfaces;

public interface IFileUploadService
{
    Task<string> UploadBusinessLogoAsync(string fileName, Stream file);
}