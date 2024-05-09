using Diplom_project_2024.Models;
using Diplom_project_2024.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Diplom_project_2024.Services
{
    public interface IAuthenticationService
    {
        public Task<TokenDTO> CreateToken(string? oldRefreshToken = null);
        public Task<bool> ValidateUser(UserLoginDTO user);
        public Task<TokenDTO> RefreshAccessToken(string refreshToken);
        public Task<bool> RegisterUser(UserRegisterDTO user);
        public void LoginOrCreateUser(ClaimsPrincipal user);
        public Task<bool> AuthorizeGoogle(string token);
    }
}
