using Diplom_project_2024.Data;
using Diplom_project_2024.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Diplom_project_2024.CustomErrors;
using Diplom_project_2024.Configs;
using Diplom_project_2024.Models;
using Google.Apis.Auth;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Diplom_project_2024.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;
        private readonly HousesDBContext context;
        private User? _user;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthenticationService(UserManager<User> userManager, IConfiguration configuration, HousesDBContext context, IHttpClientFactory httpClientFactory)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<TokenDTO> CreateToken(string? oldToken = null)
        {
            var claims = await GetClaims();
            var signinCredential = GetSigningCredentials();
            var tokenOptions = GenerateTokenOptions(signinCredential, claims);
            TokenDTO token = new TokenDTO()
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions),
                refreshToken = GenerateRefreshToken()
            };
            if(oldToken ==null)
            {
                RefreshToken refToken = new RefreshToken()
                {
                    UserId = _user.Id,
                    User = _user,
                    refreshToken = token.refreshToken,
                    RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30)
                };
                await context.RefreshTokens.AddAsync(refToken);
                await context.SaveChangesAsync();
            }
            else
            {
                var tok = await context.RefreshTokens.Include(t=>t.User).FirstOrDefaultAsync(t=>t.refreshToken==oldToken);
                if (tok == null) throw new ErrorException("Your token is invalid");
                tok.refreshToken = token.refreshToken;
                context.Update(tok);
                await context.SaveChangesAsync();
            }
            //_user.RefreshToken = token.refreshToken;
            //if (populateExp)
            //    _user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            //await userManager.UpdateAsync(_user);
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
                expires: DateTime.UtcNow.AddDays(1), // Срок действия токена (1 день)
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
                throw new ErrorException("Invalid Token");
            }
            return principal;
        }

        public async Task<TokenDTO> RefreshAccessToken(string refreshToken)
        {
            var token = context.RefreshTokens.Include(t => t.User).FirstOrDefault(t => t.refreshToken==refreshToken);
            if (token == null) throw new ErrorException("Refresh token is invalid");
            var user = token.User;
            if (user is null || token.RefreshTokenExpiryTime <= DateTime.UtcNow) throw new ErrorException("Refresh token is invalid");
            //var principal = GetPrincipalFromExpiredToken(tokenDTO.accessToken);
            //var user = await userManager.FindByNameAsync(principal.Identity.Name);
            //if(user is null|| user.RefreshToken!=tokenDTO.refreshToken||user.RefreshTokenExpiryTime<=DateTime.Now)
            //{
            //    throw new ErrorException("Refresh token not valid");
            //}
            _user = user;
            return await CreateToken(token.refreshToken);
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
            {
                throw new ErrorException(res.Errors.ToList()); 
            }
        }

        public async void LoginOrCreateUser(ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var existingUser = await userManager.FindByIdAsync(userId);
            if (existingUser == null)
            {
                existingUser = new User
                {
                    Id = userId,
                    // Дополнительные данные о пользователе
                    FirstName = user.FindFirst(ClaimTypes.GivenName)?.Value,
                    Surname = user.FindFirst(ClaimTypes.Surname)?.Value,
                    Email = user.FindFirst(ClaimTypes.Email)?.Value,
                    ImagePath = user.FindFirst("picture")?.Value,
                    // Другие поля, которые вы хотите сохранить
                };
                var res = await userManager.CreateAsync(existingUser);
                if (res.Succeeded)
                {
                    existingUser = await userManager.FindByNameAsync(existingUser.Email);
                    _user = existingUser;
                }
                else
                {
                    throw new ErrorException(res.Errors.ToList());
                }
            }
            else
                _user = existingUser;
        }
        public async Task<bool> AuthorizeGoogle(string token)
        {
            var googleClient = _httpClientFactory.CreateClient("GoogleClient");
            googleClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await googleClient.GetAsync("");
            if (response != null && response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var userProfile = JsonConvert.DeserializeObject<GoogleUserDTO>(content);
   
                var isExistUser = await userManager.FindByEmailAsync(userProfile.email);
                if (isExistUser != null && isExistUser.PasswordHash == "")
                {
                    _user = isExistUser;
                    return true;
                }
                else if (isExistUser == null)
                {
                    var user = new User()
                    {
                        FirstName = userProfile.given_name,
                        Surname = userProfile.family_name,
                        Email = userProfile.email,
                        UserName = userProfile.email,
                        ImagePath = userProfile.picture,
                        PasswordHash = ""
                    };
                    await userManager.CreateAsync(user);
                    _user = await userManager.FindByEmailAsync(user.Email);
                    return true;
                }
                else
                    throw new ErrorException("You already register your email with simple authorization");

            }
            return false;
        }

        public async Task<bool> AuthorizeFacebook(FacebookUserDTO userDTO)
        {
            var ExistingUser = await userManager.FindByEmailAsync(userDTO.email);
            if (ExistingUser != null&& ExistingUser.PasswordHash=="") 
            {
                _user = ExistingUser;
                return true;
            }
            else if (ExistingUser == null)
            {
                var user = new User()
                {
                    FirstName = userDTO.firstName,
                    Surname = userDTO.surName,
                    Email = userDTO.email,
                    UserName = userDTO.email,
                    ImagePath = userDTO.picture,
                    PasswordHash = ""
                };
                await userManager.CreateAsync(user);
                _user = await userManager.FindByEmailAsync(user.Email);
                return true;
            }
            else
                throw new ErrorException("You already register your email with simple authorization");
        }
    }
}
