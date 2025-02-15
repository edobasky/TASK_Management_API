using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.API.Model;

namespace TaskManagement.API.Utility
{
    public interface IJWTService
    {
        public (string userToken, int tokenExpiryTime) CreateToken(AppUser user);
        public (string userToken, int tokenExpiryTime) GenerateToken(AppUser user);
    }
    public class JWTService : IJWTService
    {
        private readonly IConfiguration _config;

        public JWTService(IConfiguration configuration)
        {
            _config = configuration;
        }

        public (string userToken,int tokenExpiryTime) CreateToken(AppUser user)
        {
            var issuer = _config["JwtConfig:Issuer"];
            var audience = _config["JwtConfig:Audience"];
            var Key = _config["JwtConfig:Key"];
            var tokenValidityMins = _config.GetValue<int>("JwtConfig:TokenValidityMins");
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Name, user.Username!),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("Role",user.Role!)
                }),
                Expires = tokenExpiryTimeStamp,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)),SecurityAlgorithms.HmacSha256),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);

            return (accessToken, (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalMinutes);
        }



        public (string userToken, int tokenExpiryTime) GenerateToken(AppUser user)
        {
            var issuer = _config["JwtConfig:Issuer"];
            var audience = _config["JwtConfig:Audience"];
            var assettingKey = _config["JwtConfig:Key"];
            var subject = _config["JwtConfig:Subject"];
            var tokenValidityMins = _config.GetValue<int>("JwtConfig:TokenValidityMins");
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, subject),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim("Email",user.Email.ToString()),
                new Claim("Role", user.Role.ToString())
           };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(assettingKey));
            var signIn = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                tokenExpiryTimeStamp,
                signingCredentials : signIn
                );

            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return (tokenValue, (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalMinutes);
        }

    }
}
