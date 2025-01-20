
using AutoMapper;
using ZapatosEcommerceApp.Models.AddressModels.AddressDto;
using ZapatosEcommerceApp.Models.AddressModels;
using ZapatosEcommerceApp.Models.CategoryModels.CategoryDTO;
using ZapatosEcommerceApp.Models.CategoryModels;
using ZapatosEcommerceApp.Models.ProductModels.ProductDTOs;
using ZapatosEcommerceApp.Models.ProductModels;
using ZapatosEcommerceApp.Models.UserModels.UserDTOs;
using ZapatosEcommerceApp.Models.UserModels;
using ZapatosEcommerceApp.Models.WishListModels.WishListDTOs;
using ZapatosEcommerceApp.Models.WishListModels;

namespace ZapatosEcommerceApp.Mapper
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<User, UserRegisterDTO>().ReverseMap();
            CreateMap<Category, CategoryResDTO>().ReverseMap();
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Product, AddProductDTO>().ReverseMap();
            CreateMap<WishList, WishListDTO>().ReverseMap();
            CreateMap<User, UserViewDTO>().ReverseMap();
            CreateMap<Address, AddressResDTO>().ReverseMap();
        }
    }
}
