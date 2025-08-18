using TwoWheels.Functions.Services.Storage.Interfaces;

namespace TwoWheels.Functions.Services.Storage
{
    public class LocalStorageService : IStorageService
    {
        private readonly string _basePath;

        public LocalStorageService()
        {
            _basePath = Path.Combine(Environment.CurrentDirectory, "uploads", "cnh-images");
            Directory.CreateDirectory(_basePath);
        }

        public async Task<string> SaveImageAsync(string base64Image, string fileName, string[] allowedExtensions)
        {
            if (!ValidateImageFormat(base64Image, allowedExtensions))
            {
                throw new ArgumentException("Invalid image format. Only PNG and BMP are allowed.");
            }

            var base64Data = base64Image;
            if (base64Image.Contains(','))
            {
                base64Data = base64Image.Split(',')[1];
            }

            var imageBytes = Convert.FromBase64String(base64Data);

            var extension = GetImageExtension(imageBytes);
            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException($"Image format {extension} not allowed.");
            }
            var uniqueFileName = $"{fileName}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_basePath, uniqueFileName);

            await File.WriteAllBytesAsync(filePath, imageBytes);

            return filePath;
        }

        public bool DeleteImageAsync(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public bool ValidateImageFormat(string base64Image, string[] allowedExtensions)
        {
            try
            {
                var base64Data = base64Image;
                if (base64Image.Contains(','))
                {
                    base64Data = base64Image.Split(',')[1];
                }

                var imageBytes = Convert.FromBase64String(base64Data);
                var extension = GetImageExtension(imageBytes);

                return allowedExtensions.Contains(extension);
            }
            catch
            {
                return false;
            }
        }

        private static string GetImageExtension(byte[] imageBytes)
        {
            // PNG: 89 50 4E 47
            if (imageBytes.Length >= 4 &&
                imageBytes[0] == 0x89 && imageBytes[1] == 0x50 &&
                imageBytes[2] == 0x4E && imageBytes[3] == 0x47)
            {
                return ".png";
            }

            // BMP: 42 4D
            if (imageBytes.Length >= 2 &&
                imageBytes[0] == 0x42 && imageBytes[1] == 0x4D)
            {
                return ".bmp";
            }

            throw new ArgumentException("Unsupported image format");
        }
    }
}