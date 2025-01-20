//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace ZapatosEcommerceApp.Services.JwtServices
//{
//    public class JwtService
//    {
//        private readonly string _secret;
//        private readonly string _issuer;
//        private readonly string _audience;
//        private readonly int _expiryMinutes;

//        public JwtService(IConfiguration configuration)
//        {
//            var jwtSettings = configuration.GetSection("JwtSettings");
//            _secret = jwtSettings["Secret"];
//            _issuer = jwtSettings["Issuer"];
//            _audience = jwtSettings["Audience"];
//            _expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"]);
//        }

//        public string GenerateToken(string username)
//        {
//            var tokenHandler = new JwtSecurityTokenHandler();
//            var key = Encoding.ASCII.GetBytes(_secret);
//            var tokenDescriptor = new SecurityTokenDescriptor
//            {
//                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }),
//                Expires = DateTime.UtcNow.AddMinutes(_expiryMinutes),
//                Issuer = _issuer,
//                Audience = _audience,
//                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//            };
//            var token = tokenHandler.CreateToken(tokenDescriptor);
//            return tokenHandler.WriteToken(token);
//        }
//    }
//}
