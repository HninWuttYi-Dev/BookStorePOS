public interface IFileStorageService
{
    Task<string?> UploadImageAsync(IFormFile file, string folder = "books");
}