using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

public class R2FileStorageService : IFileStorageService
{
    private readonly IConfiguration _config;
    private readonly ILogger<R2FileStorageService> _logger;
    private readonly IWebHostEnvironment _environment;

    public R2FileStorageService(IConfiguration config, ILogger<R2FileStorageService> logger, IWebHostEnvironment environment)
    {
        _config = config;
        _logger = logger;
        _environment = environment;
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

        var ext = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var key = $"{folder}/{fileName}";

        // 1. Try Cloudflare R2 if configured
        if (!string.IsNullOrWhiteSpace(accountId) &&
            !string.IsNullOrWhiteSpace(accessKey) &&
            !string.IsNullOrWhiteSpace(secretKey) &&
            !string.IsNullOrWhiteSpace(bucketName) &&
            !string.IsNullOrWhiteSpace(publicUrl))
        {
            try
            {
                var s3Config = new AmazonS3Config
                {
                    ServiceURL = $"https://{accountId}.r2.cloudflarestorage.com",
                    ForcePathStyle = true,
                    AuthenticationRegion = "auto",
                    Timeout = TimeSpan.FromSeconds(10),
                    MaxErrorRetry = 1
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
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "R2 upload failed or timed out: {Message}. Falling back to local storage.", ex.Message);
            }
        }
        else
        {
            _logger.LogInformation("R2 config is missing, saving file locally.");
        }

        // 2. Resilient Fallback: Save locally in wwwroot/uploads/{folder}
        try
        {
            var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsDir = Path.Combine(webRoot, "uploads", folder);
            if (!Directory.Exists(uploadsDir))
            {
                Directory.CreateDirectory(uploadsDir);
            }

            var filePath = Path.Combine(uploadsDir, fileName);
            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(fileStream);

            var localUrl = $"/uploads/{folder}/{fileName}";
            _logger.LogInformation("Local file upload success => {Url}", localUrl);
            return localUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save file locally: {Message}", ex.Message);
            return null;
        }
    }
}