using CoreDomainBase.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CoreApiBase.Configurations;
using BCrypt.Net;

namespace CoreApiBase.Services
{
    public class AuthService
    {
        private readonly JwtSettings _jwtSettings;

        public AuthService(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                // Note: Role is NOT included in token - it will be fetched from DB on each request
                new Claim("SecurityStamp", user.SecurityStamp) // Add SecurityStamp for token validation
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // OAuth2 placeholders for future implementation
        public Task<string> LoginWithGoogleAsync(string googleToken)
        {
            // TODO: Implement Google OAuth2 login
            throw new NotImplementedException("Google OAuth2 not implemented yet");
        }

        public Task<string> LoginWithMicrosoftAsync(string microsoftToken)
        {
            // TODO: Implement Microsoft OAuth2 login
            throw new NotImplementedException("Microsoft OAuth2 not implemented yet");
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, 12);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
