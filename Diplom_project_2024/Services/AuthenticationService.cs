using Diplom_project_2024.Data;
using Diplom_project_2024.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Diplom_project_2024.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private User? _user;

        public AuthenticationService(UserManager<User> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }
        public async Task<TokenDTO> CreateToken(bool populateExp)
        {
            var claims = await GetClaims();
            var signinCredential = GetSigningCredentials();
            var tokenOptions = GenerateTokenOptions(signinCredential, claims);
            TokenDTO token = new TokenDTO()
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions),
                refreshToken = GenerateRefreshToken()
            };
            _user.RefreshToken = token.refreshToken;
            if (populateExp)
                _user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            _user.RefreshToken = token.refreshToken;
            await userManager.UpdateAsync(_user);
            return token;
        }
        public async Task<bool> ValidateUser(UserLoginDTO user)
        {
            var us = await userManager.FindByNameAsync(user.Email);
            if(us == null) return false;
            var res= await userManager.CheckPasswordAsync(us, user.Password);
            if (res)
                _user = us;
            return res;
        }
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3), // Срок действия токена (3 часа)
                signingCredentials: signingCredentials
            );
            return token;
        }
        private async Task<List<Claim>> GetClaims()
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Role, "User"));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, _user.Id));
            claims.Add(new Claim(ClaimTypes.Name, _user.UserName));
            if (await userManager.IsInRoleAsync(_user, "Admin"))
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            return claims;
        }
        private SigningCredentials GetSigningCredentials()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var signin = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            return signin;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);

                return Convert.ToBase64String(randomNumber);
            }
        }
        
    }
}
