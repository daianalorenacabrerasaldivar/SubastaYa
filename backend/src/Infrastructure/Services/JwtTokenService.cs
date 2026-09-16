using Application.Interfaces.Services;
using Domain.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{
    public sealed class JwtTokenService : IJwtTokenService
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiresHours;

        public JwtTokenService(IConfiguration configuration)
        {
            _key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key no configurado.");
            _issuer = configuration["Jwt:Issuer"] ?? "SubastaYaApi";
            _audience = configuration["Jwt:Audience"] ?? "SubastaYaFrontend";
            _expiresHours = int.TryParse(configuration["Jwt:ExpiresInHours"], out var h) ? h : 24;
        }

        public string GenerarToken(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,   usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim("nombre",                      usuario.Nombre),
                new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer:   _issuer,
                audience: _audience,
                claims:   claims,
                expires:  DateTime.UtcNow.AddHours(_expiresHours),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
