using Microsoft.EntityFrameworkCore;
using ZapatosEcommerceApp.Models.AddressModels;
using ZapatosEcommerceApp.Models.CartModels;
using ZapatosEcommerceApp.Models.CategoryModels;
using ZapatosEcommerceApp.Models.OrderModels;
using ZapatosEcommerceApp.Models.ProductModels;
using ZapatosEcommerceApp.Models.UserModels;
using ZapatosEcommerceApp.Models.WishListModels;

namespace ZapatosEcommerceApp.Datas
{

    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<WishList> WishLists { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<OrderMain> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Address> Addresses { get; set; }

        private readonly IConfiguration _configuration;
        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionstring = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionstring);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasDefaultValue("user");

            modelBuilder.Entity<User>()
                .Property(u => u.IsBlocked)
                .HasDefaultValue(false);

            //modelBuilder.Entity<Category>()
            //    .Property(u => u.Image)
            //    .HasDefaultValue(null);
            //Product to category relation
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(c => c.Category)
                .HasForeignKey(c => c.CategoryId);

            //WishList to product relation
            modelBuilder.Entity<WishList>()
                .HasOne(w => w.Products)
                .WithMany()
                .HasForeignKey(w => w.ProductId);

            //Wishlist to user relation
            modelBuilder.Entity<WishList>()
                .HasOne(w => w.Users)
                .WithMany(w => w.WishLists)
                .HasForeignKey(w => w.UserId);

            //user to cart relation
            modelBuilder.Entity<User>()
                .HasOne(u => u.Cart)
                .WithOne(u => u.User)
                .HasForeignKey<Cart>(x => x.UserId);

            //cart to cartitem relation
            modelBuilder.Entity<Cart>()
                .HasMany(x => x.CartItems)
                .WithOne(c => c.Cart)
                .HasForeignKey(i => i.CartId);

            //CartItem to product relation
            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Product)
                .WithMany(c => c.CartItems)
                .HasForeignKey(k => k.ProductId);


            modelBuilder.Entity<OrderMain>()
                .HasOne(u => u.User)
                .WithMany(o => o.Orders)
                .HasForeignKey(f => f.UserId);


            modelBuilder.Entity<OrderItem>()
                .HasOne(u => u.Order)
                .WithMany(oi => oi.OrderItems)
                .HasForeignKey(f => f.OrderId);


            modelBuilder.Entity<OrderItem>()
                .HasOne(p => p.Product)
                .WithMany()
                .HasForeignKey(p => p.productId);


            modelBuilder.Entity<OrderItem>()
                .Property(pr => pr.TotalPrice).
                HasPrecision(30, 2);

            modelBuilder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(u => u.UserId);

            modelBuilder.Entity<OrderMain>()
                .HasOne(o => o.Address)
                .WithMany(a => a.Orders)
                .HasForeignKey(u => u.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderMain>()
                .Property(o => o.OrderStatus)
                .HasDefaultValue("pending");



            modelBuilder.Entity<Category>().HasData(

                new Category { CategoryId = 1, CategoryName = "Sneakers" },
                new Category { CategoryId = 2, CategoryName = "Running Shoe" },
                new Category { CategoryId = 3, CategoryName = "Canvas" },
                new Category { CategoryId = 4, CategoryName = "Boot" },
                new Category { CategoryId = 5, CategoryName = "Athletic" },
                new Category { CategoryId = 6, CategoryName = "Casual " }
                );

        }
    }
}
