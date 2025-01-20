using ZapatosEcommerceApp.Models.OrderModels.OrderDTOs;

namespace ZapatosEcommerceApp.Services.OrderServices
{
    public interface IOrderService
    {
        Task<string> RazorOrderCreate(long price);  // Razorpay Order Creation
        bool RazorPayment(PaymentDTO payment);  // Razorpay Payment Verification
        Task<bool> CreateOrder(int userId, CreateOrderDTO createOrderDTO);  // Create an order
        Task UpdateOrder(string orderId, string orderStatus);  // Update the order status
        Task<List<ViewOrderAdminDTO>> GetOrderDetailsAdmin();  // Get order details for admin
        Task<List<ViewOrderUserDetailDTO>> GetOrderDetails(int userId);  // Get order details for a user
        Task<List<ViewOrderUserDetailDTO>> GetOrdersByUserId(int userId);  // Get orders by user ID
        Task<decimal> TotalRevenue();  // Get total revenue
        Task<int> TotalProductsPurchased();  // Get total number of products purchased
    }
}
