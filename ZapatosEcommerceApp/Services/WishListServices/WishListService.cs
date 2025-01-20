﻿using AutoMapper;
using ZapatosEcommerceApp.Datas;
using ZapatosEcommerceApp.Models.WishListModels.WishListDTOs;
using ZapatosEcommerceApp.Models.WishListModels;
using Microsoft.EntityFrameworkCore;
namespace ZapatosEcommerceApp.Services.WishListServices
{
    public class WishListService : IWishListService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public WishListService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<string> AddorRemove(int userId, int productId)
        {
            try
            {
                var productExists = await _context.Products.AnyAsync(p => p.ProductId == productId);
                if (!productExists)
                {
                    return "Product does not exist.";
                }
                var isExist = await _context.WishLists.Include(x => x.Products).FirstOrDefaultAsync(w => w.ProductId == productId && w.UserId == userId);
                if (isExist == null)
                {
                    WishListDTO wishlistdto = new WishListDTO
                    {
                        UserId = userId,
                        ProductId = productId,
                    };

                    var wishlist = _mapper.Map<WishList>(wishlistdto);
                    _context.WishLists.Add(wishlist);
                    await _context.SaveChangesAsync();
                    return "Item added to wishList";
                }
                else
                {
                    _context.WishLists.Remove(isExist);
                    await _context.SaveChangesAsync();
                    return "Item removed from WishList";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<List<WishListResDTO>> GetWishList(int userId)
        {
            try
            {
                var items = await _context.WishLists.Include(x => x.Products).ThenInclude(x => x.Category)
                            .Where(x => x.UserId == userId).ToListAsync();
                if (items != null)
                {
                    var products = items.Select(p => new WishListResDTO
                    {
                        Id = p.Id,
                        ProductId = p.Products.ProductId,
                        ProductName = p.Products.ProductName,
                        ProductDescription = p.Products.ProductDescription,
                        Material = p.Products.Material,
                        Image = p.Products.Image,
                        Price = p.Products.ProductPrice,
                        Category = p.Products.Category.CategoryName

                    }).ToList();
                    return products;
                }
                else
                {
                    return new List<WishListResDTO>();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
