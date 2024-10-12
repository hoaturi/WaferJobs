using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using JobBoard.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace JobBoard.Infrastructure.Services.FileUploadService;

public class FileUploadService : IFileUploadService
{
    private const string CompanyLogoFolder = "companies";
    private const string ConferenceLogoFolder = "conferences";
    private readonly CloudFlareOptions _cloudFlareOptions;
    private readonly ILogger<FileUploadService> _logger;
    private readonly AmazonS3Client _s3Client;

    public FileUploadService(IOptions<CloudFlareOptions> cloudFlareOptions, ILogger<FileUploadService> logger)
    {
        _cloudFlareOptions = cloudFlareOptions.Value;
        var credentials = new BasicAWSCredentials(_cloudFlareOptions.AccessKeyId, _cloudFlareOptions.SecretAccessKey);
        _s3Client = new AmazonS3Client(credentials, new AmazonS3Config
        {
            ServiceURL = _cloudFlareOptions.S3Endpoint
        });

        _logger = logger;
    }

    public async Task<string> UploadLogoAsync(string fileName, Stream fileStream, LogoTypes logoTypes)
    {
        var folder = logoTypes == LogoTypes.Company ? CompanyLogoFolder : ConferenceLogoFolder;

        var request = new PutObjectRequest
        {
            BucketName = _cloudFlareOptions.S3BucketName,
            Key = $"{folder}/{fileName}",
            InputStream = fileStream,
            DisablePayloadSigning = true
        };

        await _s3Client.PutObjectAsync(request);

        _logger.LogInformation("Uploaded {Folder} logo file {FileName} to bucket {BucketName}", folder, fileName
            , _cloudFlareOptions.S3BucketName);

        return $"{_cloudFlareOptions.ImageBaseUrl}/{folder}/{fileName}";
    }
}