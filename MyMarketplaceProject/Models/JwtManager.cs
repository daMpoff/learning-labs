using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyMarketplaceProject.Models
{
    public class JwtManager
    {
        private readonly IConfiguration configuration;
        public JwtManager(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GenerateJwtToken(Account account)
        {
            var secretKey = Encoding.ASCII.GetBytes(configuration["JwtSettings:SecretKey"]);
            var securityKey = new SymmetricSecurityKey(secretKey);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var issuer = configuration["JwtSettings:Issuer"];
            var audience = configuration["JwtSettings:Audience"];

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, account.Login), // Имя пользователя или уникальный идентификатор
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, account.Login),
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(120), // Указывается UTC времени для исключения путаницы с часовыми поясами
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
