namespace ZapatosEcommerceApp.Services.CloudinaryServices
{
    public interface ICloudinaryService
    {
        // Method now includes optional width and height parameters for image transformations
        Task<string> UploadImageAsync(IFormFile file, int? width = 500, int? height = 500);
    }
}
