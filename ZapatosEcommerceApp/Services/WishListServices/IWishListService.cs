using ZapatosEcommerceApp.Models.WishListModels.WishListDTOs;

namespace ZapatosEcommerceApp.Services.WishListServices
{
    public interface IWishListService
    {
        Task<string> AddorRemove(int userId, int productId);
        Task<List<WishListResDTO>> GetWishList(int userId);
    }
}
