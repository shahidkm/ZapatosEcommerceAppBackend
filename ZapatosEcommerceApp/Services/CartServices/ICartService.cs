using ZapatosEcommerceApp.Models.ApiResponsesModels;
using ZapatosEcommerceApp.Models.CartModels.CartDTO;
using ZapatosEcommerceApp.Models.CartModels;

namespace ZapatosEcommerceApp.Services.CartServices
{
    public interface ICartService
    {
        Task<CartResDTO> GetCartItems(int userId);
        Task<ApiResponses<CartItem>> AddToCart(int userId, int productId);
        Task<bool> RemoveFromCart(int userId, int productId);
        Task<ApiResponses<CartItem>> IncreaseQty(int userId, int productId);
        Task<bool> DecreaseQty(int userId, int productId);
        Task<bool> RemoveAllItems(int userId);
    }
}
