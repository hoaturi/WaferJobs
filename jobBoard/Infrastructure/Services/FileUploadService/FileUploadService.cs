using Azure;
using Azure.Storage.Blobs;
using JobBoard.Common.Interfaces;
using JobBoard.Common.Options;
using Microsoft.Extensions.Options;

namespace JobBoard.Infrastructure.Services.FileUploadService;

public class FileUploadService : IFileUploadService
{
    private readonly AzureOptions _azureOptions;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<FileUploadService> _logger;

    public FileUploadService(IOptions<AzureOptions> azureOptions, ILogger<FileUploadService> logger)
    {
        _azureOptions = azureOptions.Value;
        _blobServiceClient = new BlobServiceClient(_azureOptions.StorageConnectionString);
        _logger = logger;
    }

    public async Task<string> UploadFileAsync(string fileName, Stream fileStream)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_azureOptions.BusinessLogoContainer);
        var blobClient = blobContainerClient.GetBlobClient(fileName);

        try
        {
            await blobClient.UploadAsync(fileStream, true);
            _logger.LogInformation("Business logo uploaded successfully: {FileName}", fileName);
            return blobClient.Uri.AbsoluteUri;
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError("Failed to upload business logo. Error Code: {ErrorCode}", ex.ErrorCode);
            throw new BusinessLogoUploadFailedException();
        }
    }
}