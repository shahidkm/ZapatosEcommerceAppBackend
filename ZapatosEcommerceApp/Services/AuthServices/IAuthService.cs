using ZapatosEcommerceApp.DTOs;
using ZapatosEcommerceApp.Models.UserModels.UserDTOs;

namespace ZapatosEcommerceApp.Services.AuthServices
{
    public interface IAuthService
    {
        Task<bool> Register(UserRegisterDTO userRegisterDTO);
        Task<UserResDTO> Login(UserLoginDTO userDTO);
    }
}
