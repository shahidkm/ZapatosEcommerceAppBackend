﻿using System.ComponentModel.DataAnnotations;
using ZapatosEcommerceApp.Models.AddressModels;
using ZapatosEcommerceApp.Models.CartModels;
using ZapatosEcommerceApp.Models.OrderModels;
using ZapatosEcommerceApp.Models.WishListModels;

namespace ZapatosEcommerceApp.Models.UserModels
{
    public class User
    {

        public int Id { get; set; }
        [Required(ErrorMessage = "name is required")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [RegularExpression(@"^(?=.[A-Za-z])(?=.\d)(?=.[@$!%?&])[A-Za-z\d@$!%*?&]{8,}$",
         ErrorMessage = "Password must contain at least one letter, one number, and one special character.")]
        public string Password { get; set; }
        public string? Role { get; set; }
        public bool IsBlocked { get; set; }
        public virtual Cart? Cart { get; set; }
        public virtual List<OrderMain> Orders { get; set; }
        public List<WishList> WishLists { get; set; }
        public ICollection<Address> Addresses { get; set; }


    }
}

