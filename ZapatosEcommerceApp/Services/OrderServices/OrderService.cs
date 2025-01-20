using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using ZapatosEcommerceApp.Datas;
using ZapatosEcommerceApp.Models.CartModels.CartDTO;
using ZapatosEcommerceApp.Models.OrderModels.OrderDTOs;
using ZapatosEcommerceApp.Models.OrderModels;
using System.Security.Cryptography;
using System.Text;

namespace ZapatosEcommerceApp.Services.OrderServices
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        // Razorpay Order Creation (hardcoded credentials)
        public async Task<string> RazorOrderCreate(long price)
        {
            try
            {
                Dictionary<string, object> input = new Dictionary<string, object>
                {
                    { "amount", price * 100 }, // Razorpay expects price in paise (multiply by 100)
                    { "currency", "INR" }, // Currency is INR
                    { "receipt", Guid.NewGuid().ToString() } // Unique receipt ID for the order
                };

                // Hardcoded Razorpay credentials
                string key = "rzp_test_iA2stFg1qD86OQ"; // Replace with your Razorpay KeyId
                string secret = "B442j5qkUCP0WrsGGgHBG6F8"; // Replace with your Razorpay KeySecret

                RazorpayClient client = new RazorpayClient(key, secret);
                Razorpay.Api.Order order = client.Order.Create(input);
                string orderId = order["id"].ToString();

                return orderId;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while creating Razorpay order: " + ex.Message);
            }
        }

        // Razorpay Payment Verification
        public bool RazorPayment(PaymentDTO payment)
        {
            if (payment == null ||
                string.IsNullOrEmpty(payment.razorpay_payment_id) ||
                string.IsNullOrEmpty(payment.razorpay_orderId) ||
                string.IsNullOrEmpty(payment.razorpay_signature))
            {
                return false;
            }

            try
            {
                // Razorpay credentials
                string key = "rzp_test_iA2stFg1qD86OQ"; // Replace with your Razorpay KeyId
                string secret = "B442j5qkUCP0WrsGGgHBG6F8"; // Replace with your Razorpay KeySecret

                // Prepare the data required for signature verification
                string paymentId = payment.razorpay_payment_id;
                string orderId = payment.razorpay_orderId;
                string signature = payment.razorpay_signature;

                // Generate expected signature
                string generatedSignature = GenerateSignature(paymentId, orderId, secret);

                // Verify if the signature matches
                if (signature == generatedSignature)
                {
                    return true; // Signature is valid
                }
                else
                {
                    throw new Exception("Invalid signature passed");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while verifying Razorpay payment: " + ex.Message);
            }
        }

        private string GenerateSignature(string paymentId, string orderId, string secret)
        {
            // Razorpay signature generation formula
            string stringToSign = orderId + "|" + paymentId;
            var hmac = new HMACSHA256();
            hmac.Key = Encoding.ASCII.GetBytes(secret);
            var hashBytes = hmac.ComputeHash(Encoding.ASCII.GetBytes(stringToSign));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower(); // Return the lowercase signature
        }

        // Create Order from Cart
        public async Task<bool> CreateOrder(int userId, CreateOrderDTO createOrderDTO)
        {
            try
            {
                var cart = await _context.Carts.Include(c => c.CartItems).ThenInclude(c => c.Product).FirstOrDefaultAsync(x => x.UserId == userId);
                if (cart == null)
                {
                    throw new Exception("Cart is empty.");
                }

                var order = new OrderMain
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    OrderStatus = "pending",
                    AddressId = createOrderDTO.AddressId,
                    TotalAmount = createOrderDTO.Totalamount,
                    OrderString = createOrderDTO.OrderString,
                    TransactionId = createOrderDTO.TransactionId,
                    OrderItems = cart.CartItems.Select(c => new OrderItem
                    {
                        productId = c.ProductId,
                        Quantity = c.Quantity,
                        TotalPrice = c.Quantity * c.Product.ProductPrice
                    }).ToList(),
                };

                foreach (var cartItem in cart.CartItems)
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == cartItem.ProductId);
                    if (product != null && product.Stock < cartItem.Quantity)
                    {
                        return false; // Insufficient stock available
                    }
                    product.Stock -= cartItem.Quantity;
                }

                await _context.Orders.AddAsync(order);
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Database update failed: " + ex.InnerException?.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating order: " + ex.Message);
            }
        }

        // Get Order Details for Admin
        public async Task<List<ViewOrderAdminDTO>> GetOrderDetailsAdmin()
        {
            var orderdetails = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .Select(o => new ViewOrderAdminDTO
                {
                    id = o.Id,
                    OrderDate = o.OrderDate,
                    orderId = o.OrderString,
                    CustomerEmail = o.User.Email,
                    OrderStatus = o.OrderStatus,
                    CustomerName = o.User.Name,
                    TransactionId = o.TransactionId
                })
                .ToListAsync();

            return orderdetails;
        }

        // Get Order Details for a User
        public async Task<List<ViewOrderUserDetailDTO>> GetOrderDetails(int userId)
        {
            try
            {
                var orders = await _context.Orders.Include(i => i.OrderItems)
                                .ThenInclude(i => i.Product)
                                .Where(i => i.UserId == userId)
                                .ToListAsync();

                if (orders == null || !orders.Any())
                {
                    return new List<ViewOrderUserDetailDTO>();
                }

                var orderdetails = orders.Select(i => new ViewOrderUserDetailDTO
                {
                    Id = i.Id,
                    OrderId = i.OrderString,
                    OrderDate = i.OrderDate,
                    OrderStatus = i.OrderStatus,
                    TransactionId = i.TransactionId,
                    OrderProducts = i.OrderItems.Select(o => new CartViewDTO
                    {
                        ProductId = o.productId,
                        ProductName = o.Product.ProductName,
                        Price = o.Product.ProductPrice,
                        TotalAmount = o.TotalPrice,
                        Quantity = o.Quantity,
                        Image = o.Product.Image
                    }).ToList(),

                }).ToList();

                return orderdetails;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving order details: " + ex.Message);
            }
        }

        // Get Orders by User ID
        public async Task<List<ViewOrderUserDetailDTO>> GetOrdersByUserId(int userId)
        {
            try
            {
                var orders = await _context.Orders.Include(i => i.OrderItems)
                                .ThenInclude(i => i.Product)
                                .Where(i => i.UserId == userId)
                                .ToListAsync();

                if (orders == null || !orders.Any())
                {
                    return new List<ViewOrderUserDetailDTO>();
                }

                var orderdetails = orders.Select(i => new ViewOrderUserDetailDTO
                {
                    Id = i.Id,
                    OrderId = i.OrderString,
                    OrderDate = i.OrderDate,
                    OrderStatus = i.OrderStatus,
                    TransactionId = i.TransactionId,
                    OrderProducts = i.OrderItems.Select(o => new CartViewDTO
                    {
                        ProductId = o.productId,
                        ProductName = o.Product.ProductName,
                        Price = o.Product.ProductPrice,
                        TotalAmount = o.TotalPrice,
                        Quantity = o.Quantity,
                        Image = o.Product.Image
                    }).ToList(),

                }).ToList();

                return orderdetails;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving orders by user ID: " + ex.Message);
            }
        }

        // Calculate Total Revenue
        public async Task<decimal> TotalRevenue()
        {
            try
            {
                var total = await _context.OrderItems.SumAsync(p => p.TotalPrice);
                return total;
            }
            catch (Exception ex)
            {
                throw new Exception("Error calculating total revenue: " + ex.Message);
            }
        }

        // Get Total Products Purchased
        public async Task<int> TotalProductsPurchased()
        {
            try
            {
                var totalProduct = await _context.OrderItems.SumAsync(p => p.Quantity);
                return totalProduct;
            }
            catch (Exception ex)
            {
                throw new Exception("Error calculating total products purchased: " + ex.Message);
            }
        }

        // Update Order Status
        public async Task UpdateOrder(string orderId, string orderStatus)
        {
            try
            {
                var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderString == orderId);
                if (order != null)
                {
                    order.OrderStatus = orderStatus;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Order with this order Id not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating order: " + ex.Message);
            }
        }
    }
}
