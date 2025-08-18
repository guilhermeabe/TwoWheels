namespace TwoWheels.Functions.Services.Storage.Interfaces
{
    public interface IStorageService
    {
        Task<string> SaveImageAsync(string base64Image, string fileName, string[] allowedExtensions);
        bool DeleteImageAsync(string filePath);
        bool ValidateImageFormat(string base64Image, string[] allowedExtensions);
    }
}