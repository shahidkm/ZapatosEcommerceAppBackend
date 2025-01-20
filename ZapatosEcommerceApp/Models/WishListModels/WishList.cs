using System.ComponentModel.DataAnnotations;
using ZapatosEcommerceApp.Models.ProductModels;
using ZapatosEcommerceApp.Models.UserModels;

namespace ZapatosEcommerceApp.Models.WishListModels
{
    public class WishList
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int ProductId { get; set; }
        public virtual User? Users { get; set; }
        public virtual Product? Products { get; set; }
    }
}