using System.ComponentModel.DataAnnotations;
using ZapatosEcommerceApp.Models.ProductModels;

namespace ZapatosEcommerceApp.Models.CategoryModels
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Required]
        public string? CategoryName { get; set; }
        //public string Image { get; set; }
        public virtual ICollection<Product>? Products { get; set; }
    }
}
