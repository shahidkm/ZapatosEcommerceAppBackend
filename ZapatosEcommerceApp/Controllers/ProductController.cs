using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZapatosEcommerceApp.Models.ApiResponsesModels;
using ZapatosEcommerceApp.Models.ProductModels.ProductDTOs;
using ZapatosEcommerceApp.Services.ProductServices;

namespace ZapatosEcommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _service.GetAllProducts();
                return Ok(new ApiResponses<List<ProductDTO>>(200, "Products fetched successfully", products));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponses<string>(500, "Internal server error", ex.Message));
            }
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _service.GetProductById(id);
                if (product == null)
                {
                    return NotFound(new ApiResponses<string>(404, $"Product with ID {id} not found"));
                }
                return Ok(new ApiResponses<ProductDTO>(200, "Product fetched successfully", product));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponses<string>(500, "An unexpected error occurred", null, ex.Message));
            }
        }

        [HttpGet("GetByCategory")]
        public async Task<IActionResult> GetProductByCategory(int CategoryId)
        {
            try
            {
                var products = await _service.GetProductbyCategory(CategoryId);
                if (products.Count == 0)
                {
                    return NotFound(new ApiResponses<string>(404, $"No products found for Category ID {CategoryId}"));
                }
                return Ok(new ApiResponses<List<ProductDTO>>(200, "Products fetched successfully", products));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponses<string>(500, "An unexpected error occurred", null, ex.Message));
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost("Add")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddProducts([FromForm] AddProductDTO newProduct, IFormFile image)
        {
            try
            {
                await _service.AddProduct(newProduct, image);
                return Ok(new ApiResponses<string>(200, "Product added successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponses<string>(500, "An unexpected error occurred", null, ex.Message));
            }
        }

        [HttpGet("search-item")]
        public async Task<IActionResult> SearchProduct(string search)
        {
            try
            {
                var res = await _service.SearchProduct(search);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponses<string>(500, "An unexpected error occurred", null, ex.Message));
            }
        }

        [HttpGet("paginated-products")]
        public async Task<IActionResult> pagination([FromQuery] int pageNumber = 1, [FromQuery] int size = 10)
        {
            try
            {
                var res = await _service.ProductPagination(pageNumber, size);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponses<string>(500, "An unexpected error occurred", null, ex.Message));
            }
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> RemoveDelete(int id)
        {
            try
            {
                bool res = await _service.RemoveProduct(id);
                if (res)
                {
                    return Ok(new ApiResponses<bool>(200, "Product removed successfully", res));
                }
                return NotFound(new ApiResponses<bool>(404, "Product not found", res));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponses<string>(500, "An unexpected error occurred", null, ex.Message));
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] AddProductDTO productdto, IFormFile image = null)
        {
            try
            {
                await _service.UpdateProduct(id, productdto, image);
                return Ok(new ApiResponses<string>(200, "Product updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponses<string>(500, "An unexpected error occurred", null, ex.Message));
            }
        }

        [HttpGet("GetByType")]
        public async Task<IActionResult> GetProductsByType(string type)
        {
            try
            {
                var res = await _service.GetProductsByType(type);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponses<string>(500, "An unexpected error occurred", null, ex.Message));
            }
        }
    }
}
