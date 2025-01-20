using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using ZapatosEcommerceApp.Datas;
using ZapatosEcommerceApp.Mapper;
using ZapatosEcommerceApp.Middlewares;
using ZapatosEcommerceApp.Models.ApiResponsesModels;
using ZapatosEcommerceApp.Services.AddressServices;
using ZapatosEcommerceApp.Services.AuthServices;
using ZapatosEcommerceApp.Services.CartServices;
//using ZapatosEcommerceApp.Services.CategoryServices;
using ZapatosEcommerceApp.Services.CloudinaryServices;
using ZapatosEcommerceApp.Services.OrderServices;
using ZapatosEcommerceApp.Services.ProductServices;
using ZapatosEcommerceApp.Services.UserServices;
using ZapatosEcommerceApp.Services.WishListServices;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers()
      .ConfigureApiBehaviorOptions(options =>
      {
          options.InvalidModelStateResponseFactory = context =>
          {
              var errors = context.ModelState.Values
                  .SelectMany(v => v.Errors)
                  .Select(e => e.ErrorMessage)
                  .ToList();

              var response = new ApiResponses<string>(400, "Validation error occurred", null, string.Join("; ", errors));
              return new BadRequestObjectResult(response);
          };
      });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "CasaAuraAPI", Version = "v1" });


            // Add JWT Authentication in Swagger
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' [space] and then your token"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
        });

        builder.Services.AddScoped<AppDbContext>();
        builder.Services.AddAutoMapper(typeof(MapperProfile));


        builder.Services.AddScoped<IAuthService, AuthService>();
        //builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IWishListService, WishListService>();
        builder.Services.AddScoped<ICartService, CartService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
        builder.Services.AddScoped<IAddressService, AddressService>();



        // Jwt authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
  .AddJwtBearer(o =>
  {
      // Ensure correct configuration path for SecretKey
      var secretKey = builder.Configuration["JwtSettings:SecretKey"];
      if (string.IsNullOrEmpty(secretKey))
      {
          throw new InvalidOperationException("JWT Secret Key is missing from configuration.");
      }

      o.TokenValidationParameters = new TokenValidationParameters
      {
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
          ValidateIssuer = false,        // Consider setting to true for security in production
          ValidateAudience = false,      // Consider setting to true for security in production
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true
      };
  });


        builder.Services.AddCors(options =>
        {
            options.AddPolicy("ReactPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("ReactPolicy");

        app.UseStaticFiles();
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<GetUserIdMiddleware>();

        app.MapControllers();

        app.Run();
    }
}