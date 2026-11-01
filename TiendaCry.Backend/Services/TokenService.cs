using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Options;
using TiendaCry.Backend.Repositories;
using TiendaCry.Shared.Auth;

namespace TiendaCry.Backend.Services
{
    public class TokenService(IOptions<JwtSettings> jwtSettings)
        : ITokenService
    {
        private readonly JwtSettings _settings = jwtSettings.Value;

        public string CrearToken(Usuario usuario, List<string> roles)
        {
            DateTime expiracion = DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes);

            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(ClaimsUsuario.EmailVerificado, usuario.EmailVerificado ? "true" : "false", ClaimValueTypes.Boolean)
            };

            foreach (string rol in roles.Distinct())
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));

            SigningCredentials credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiracion,
                Issuer = _settings.Issuer,
                Audience = _settings.Audience,
                SigningCredentials = credentials
            };

            JsonWebTokenHandler handler = new JsonWebTokenHandler();

            return handler.CreateToken(descriptor);
        }

        public string CrearRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public string CalcularHash(string token)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(token);
            return Convert.ToHexString(SHA256.HashData(bytes));
        }
    }
}
