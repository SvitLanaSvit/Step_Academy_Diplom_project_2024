using Diplom_project_2024.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace Diplom_project_2024.Services
{
    public interface IAuthenticationService
    {
        public Task<TokenDTO> CreateToken(string? oldRefreshToken = null);
        public Task<bool> ValidateUser(UserLoginDTO user);
        public Task<TokenDTO> RefreshAccessToken(string refreshToken);
        public Task<bool> RegisterUser(UserRegisterDTO user);
    }
}
