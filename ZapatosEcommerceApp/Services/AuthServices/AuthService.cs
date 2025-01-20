using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ZapatosEcommerceApp.Datas;
using ZapatosEcommerceApp.Models.UserModels.UserDTOs;
using ZapatosEcommerceApp.Models.UserModels;

namespace ZapatosEcommerceApp.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IMapper mapper, ILogger<AuthService> logger, AppDbContext context, IConfiguration configuration)
        {
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        public async Task<bool> Register(UserRegisterDTO newUser)
        {
            try
            {
                if (newUser == null)
                {
                    throw new ArgumentNullException("User data cannot be null");
                }
                var IsUserExist = await _context.Users.SingleOrDefaultAsync(u => u.Email == newUser.Email);
                if (IsUserExist != null)
                {
                    return false;
                }

                newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);

                var user = _mapper.Map<User>(newUser);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (ArgumentNullException ex)
            {
                throw new Exception($"Validation error: {ex.Message}", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserResDTO> Login(UserLoginDTO userdto)
        {
            try
            {
                _logger.LogInformation("Logging into the user");

                var usr = await _context.Users.FirstOrDefaultAsync(u => u.Email == userdto.Email);
                if (usr == null)
                {
                    _logger.LogWarning("User not found");
                    return new UserResDTO { Error = "Not Found" };
                }

                _logger.LogInformation("Validating email...");
                var pass = ValidatePassword(userdto.Password, usr.Password);

                if (!pass)
                {
                    _logger.LogWarning("Invalid password");
                    return new UserResDTO { Error = "Invalid Password" };
                }

                if (usr.IsBlocked)
                {
                    _logger.LogWarning("User is blocked");
                    return new UserResDTO { Error = "User Blocked" };
                }

                _logger.LogInformation("Generating token");
                var token = GenerateToken(usr);
                return new UserResDTO
                {
                    Token = token,
                    Role = usr.Role,
                    Email = usr.Email,
                    Id = usr.Id,
                    Name = usr.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in login: {ex.Message}");
                throw;
            }
        }

        private string GenerateToken(User user)
        {
            // Ensure proper retrieval of the JWT Secret Key from configuration
            var secretKey = _configuration["JwtSettings:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT Secret Key is missing in the configuration.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claim = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                claims: claim,
                signingCredentials: credentials,
                expires: DateTime.UtcNow.AddDays(1) // Adjust expiry time as needed
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidatePassword(string password, string hashpassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashpassword);
        }
    }
}
