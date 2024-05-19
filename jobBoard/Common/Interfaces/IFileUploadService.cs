namespace JobBoard.Common.Interfaces;

public interface IFileUploadService
{
    Task<string> UploadFileAsync(string fileName, Stream file);
}