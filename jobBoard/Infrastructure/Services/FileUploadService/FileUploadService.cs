using Azure;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;

namespace JobBoard;

public class FileUploadService : IFileUploadService
{
    private readonly AzureOptions _azureOptions;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<FileUploadService> _logger;

    public FileUploadService(IOptions<AzureOptions> options, ILogger<FileUploadService> logger)
    {
        _azureOptions = options.Value;
        _logger = logger;
        _blobServiceClient = new BlobServiceClient(_azureOptions.StorageConnectionString);
    }

    public async Task<string> UploadBusinessLogoAsync(string fileName, Stream fileStream)
    {
        try
        {
            var _blobContainerClient = _blobServiceClient.GetBlobContainerClient(
                _azureOptions.BusinessLogoContainer
            );
            var blobClient = _blobContainerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, true);

            _logger.LogInformation("Successfully uploaded business logo: {FileName}", fileName);

            return blobClient.Uri.AbsoluteUri;
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError("Failed to upload business logo: {errorCode}", ex.ErrorCode);
            throw new BusinessLogoUploadFailedException();
        }
    }
}
