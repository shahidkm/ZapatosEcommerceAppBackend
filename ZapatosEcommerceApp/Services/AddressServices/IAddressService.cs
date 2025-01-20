﻿using ZapatosEcommerceApp.Models.AddressModels.AddressDto;

namespace ZapatosEcommerceApp.Services.AddressServices
{

    public interface IAddressService
    {
        Task<bool> AddAddress(int userId, AddressCreateDTO newAddress);
        Task<bool> RemoveAddress(int userId, int addressId);
        Task<List<AddressResDTO>> GetAddresses(int userId);
    }
}
