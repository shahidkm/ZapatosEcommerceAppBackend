using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ZapatosEcommerceApp.Services.CloudinaryServices
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryService> _logger;
        private readonly long _maxFileSize;
        private readonly string[] _allowedExtensions;

        public CloudinaryService(ILogger<CloudinaryService> logger)
        {
            _logger = logger;

            // Hardcoded Cloudinary Configuration
            var cloudName = "dx0snkuft"; // Your Cloudinary CloudName
            var apiKey = "313131327483978"; // Your Cloudinary ApiKey
            var apiSecret = "m6oU8iNBI41sEQeRGpDLinAuyvg"; // Your Cloudinary ApiSecret

            if (string.IsNullOrEmpty(cloudName) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
            {
                throw new CloudinaryConfigurationException("Cloudinary configuration is missing or incomplete.");
            }

            // Max file size and allowed extensions (hardcoded)
            _maxFileSize = 5 * 1024 * 1024; // 5 MB
            _allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file, int? width = 500, int? height = 500)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogError("File is null or empty.");
                throw new ArgumentException("File is null or empty.");
            }

            if (file.Length > _maxFileSize)
            {
                _logger.LogError("File size exceeds the limit.");
                throw new ArgumentException($"File size exceeds the limit of {_maxFileSize / (1024 * 1024)}MB.");
            }

            if (!_allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
            {
                _logger.LogError("Unsupported file type.");
                throw new ArgumentException("Unsupported file type.");
            }

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation()
                        .Width(width ?? 500)
                        .Height(height ?? 500)
                        .Crop("fill")
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {

                    return null;
                }

                _logger.LogInformation($"Cloudinary upload successful. URL: {uploadResult.SecureUrl}");
                return uploadResult.SecureUrl?.ToString();
            }
        }
    }

    public class CloudinaryUploadException : Exception
    {
        public CloudinaryUploadException(string message) : base(message) { }
    }

    public class CloudinaryConfigurationException : Exception
    {
        public CloudinaryConfigurationException(string message) : base(message) { }
    }
}
