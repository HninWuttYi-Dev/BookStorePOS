using Amazon.S3;
using Amazon.S3.Model;

public class R2FileStorageService : IFileStorageService
{
    private readonly IConfiguration _config;
    private readonly ILogger<R2FileStorageService> _logger;

    public R2FileStorageService(IConfiguration config, ILogger<R2FileStorageService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task<string?> UploadImageAsync(IFormFile file, string folder = "books")
    {
        if (file == null || file.Length == 0)
            return null;

        var accountId = _config["CloudflareR2:AccountId"];
        var accessKey = _config["CloudflareR2:AccessKey"];
        var secretKey = _config["CloudflareR2:SecretKey"];
        var bucketName = _config["CloudflareR2:BucketName"];
        var publicUrl = _config["CloudflareR2:PublicUrl"]?.TrimEnd('/');

        if (string.IsNullOrWhiteSpace(accountId) ||
            string.IsNullOrWhiteSpace(accessKey) ||
            string.IsNullOrWhiteSpace(secretKey) ||
            string.IsNullOrWhiteSpace(bucketName) ||
            string.IsNullOrWhiteSpace(publicUrl))
        {
            _logger.LogWarning("R2 config is missing");
            return null;
        }

        var key = $"{folder}/{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";

        var s3Config = new AmazonS3Config
        {
            ServiceURL = $"https://{accountId}.r2.cloudflarestorage.com",
            ForcePathStyle = true,
            AuthenticationRegion = "auto"
        };

        using var client = new AmazonS3Client(accessKey, secretKey, s3Config);
        await using var stream = file.OpenReadStream();

        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = stream,
            DisablePayloadSigning = true,
            ContentType = string.IsNullOrWhiteSpace(file.ContentType)
                ? "application/octet-stream"
                : file.ContentType
        };

        await client.PutObjectAsync(request);

        var url = $"{publicUrl}/{key}";
        _logger.LogInformation("R2 upload success => {Url}", url);
        return url;
    }
}