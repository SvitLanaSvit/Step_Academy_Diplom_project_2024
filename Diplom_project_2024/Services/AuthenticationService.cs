﻿using Diplom_project_2024.Data;
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
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var tokenValidationParametrs = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"])),
                ClockSkew = TimeSpan.Zero
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token,tokenValidationParametrs, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if(jwtSecurityToken is null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid Token");
            }
            return principal;
        }

        public async Task<TokenDTO> RefreshToken(TokenDTO tokenDTO)
        {
            var principal = GetPrincipalFromExpiredToken(tokenDTO.accessToken);
            var user = await userManager.FindByNameAsync(principal.Identity.Name);
            if(user is null|| user.RefreshToken!=tokenDTO.refreshToken||user.RefreshTokenExpiryTime<=DateTime.Now)
            {
                throw new ErrorException("Refresh token not valid");
            }
            _user = user;
            return await CreateToken(false);
        }

        public async Task<bool> RegisterUser(UserRegisterDTO user)
        {
            var createdUser = new User() { UserName = user.Email, Email = user.Email };
            var check = await userManager.FindByNameAsync(createdUser.Email);
            if (check != null)
            {
                throw new ErrorException("This Email already taken");
            }
            var res = await userManager.CreateAsync(createdUser, user.Password);
            if (res.Succeeded)
            {
                createdUser = await userManager.FindByNameAsync(user.Email);
                _user = createdUser;
                return true;
            }
            else
                throw new ErrorException(res.Errors.ToList());
        }
    }
}