using System.ComponentModel.DataAnnotations;

namespace ZapatosEcommerceApp.Models.CategoryModels.CategoryDTO
{
    public class CategoryDTO
    {

        [Required]
        public string? CategoryName { get; set; }
    }
}
