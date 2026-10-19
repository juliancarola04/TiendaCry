using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Options;
using TiendaCry.Backend.Repositories;

namespace TiendaCry.Backend.Services
{
    public class TokenService(IOptions<JwtSettings> jwtSettings)
        : ITokenService
    {
        private readonly JwtSettings _settings = jwtSettings.Value;

        public string CrearToken(Usuario usuario)
        {
            var expiracion = DateTime.UtcNow
                .AddMinutes(_settings.ExpiryMinutes);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString())
            };

            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.Key));

            var credentials = new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256);

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiracion,
                Issuer = _settings.Issuer,
                Audience = _settings.Audience,
                SigningCredentials = credentials
            };

            var handler = new JsonWebTokenHandler();
            var token = handler.CreateToken(descriptor);

            return token;
        }

        public string CrearRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public string CalcularHash(string token)
        {
            var bytes = Encoding.UTF8.GetBytes(token);
            return Convert.ToHexString(SHA256.HashData(bytes));
        }
    }
}
