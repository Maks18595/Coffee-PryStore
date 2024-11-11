using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Coffee_PryStore.Models;
using Coffee_PryStore.Models.Configurations; 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Coffee_PryStore.Models.Configurations
{
    public class JwtSettings
    {
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required string Key { get; set; }
        public required string Secret { get; set; }
        public int ExpirationInMinutes { get; set; }
    }

    public class TokenService(IOptions<JwtSettings> jwtSettings)
    {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Email) 
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.ExpirationInMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}