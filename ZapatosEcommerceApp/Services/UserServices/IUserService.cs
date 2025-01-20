using ZapatosEcommerceApp.Models.UserModels.UserDTOs;

namespace ZapatosEcommerceApp.Services.UserServices
{
    public interface IUserService
    {
        Task<List<UserViewDTO>> GetUsers();
        Task<UserViewDTO> GetUserById(int userId);
        Task<BlockUnblockRes> BlockAndUnBlock(int userId);
    }
}
