using ServerLibrary.Repositories.Contracts;
using BaseLibrary.Responses;
using BaseLibrary.DTOs;
using Microsoft.Extensions.Options;
using ServerLibrary.Data;
using ServerLibrary.Helpers;
using BaseLibrary.Entities;
using Microsoft.EntityFrameworkCore;
using Constants = ServerLibrary.Helpers.Constants;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace ServerLibrary.Repositories.Implementations
{
    public class UserAccountRepository : IUserAccount
    {
        private readonly IOptions<JwtSection> config;
        private readonly AppDbContext appDbContext;

        public UserAccountRepository(IOptions<JwtSection> config, AppDbContext appDbContext)
        {
            this.config = config;
            this.appDbContext = appDbContext;
        }

        public async Task<GeneralResponse> CreateAsync(Register user)
        {
            if (user == null) return new GeneralResponse(false, "Model is empty");
            var checkUser = await FindUserByEmail(user.Email!);
            if (checkUser != null) return new GeneralResponse(false, "User registered already");

            var applicationUser = await AddToDatabase(new ApplicationUser()
            {
                Fullname = user.Fullname,
                Email = user.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password)
            });

            var checkAdminRole = await appDbContext.SystemRoles.FirstOrDefaultAsync(_ => _.Name!.Equals(Constants.Admin));
            if (checkAdminRole == null)
            {
                var createAdminRole = await AddToDatabase(new SystemRole() { Name = Constants.Admin });
                await AddToDatabase(new UserRole() { RoleId = createAdminRole.Id, UserId = applicationUser.Id });
                return new GeneralResponse(true, "Account created!");
            }

            var checkUserRole = await appDbContext.SystemRoles.FirstOrDefaultAsync(_ => _.Name!.Equals(Constants.User));
            SystemRole response = new();
            if (checkUserRole is null)
            {
                response = await AddToDatabase(new SystemRole() { Name = Constants.User });
                await AddToDatabase(new UserRole() { RoleId = response.Id, UserId = applicationUser.Id });
            }
            else
            {
                await AddToDatabase(new UserRole() { RoleId = checkUserRole.Id, UserId = applicationUser.Id });
            }
            return new GeneralResponse(true, "Account created!");
        }

        public async Task<LoginResponse> SignInAsync(Login user)
        {
            if (user is null)
                return new LoginResponse(false, "Model is empty");

            var applicationUser = await FindUserByEmail(user.Email!);
            if (applicationUser is null)
                return new LoginResponse(false, "User not found");

            if (!BCrypt.Net.BCrypt.Verify(user.Password, applicationUser.Password))
                return new LoginResponse(false, "Email/Password not valid");

            // Check user role
            var getUserRole = await FindUserRole(applicationUser.Id);
            if (getUserRole is null)
                return new LoginResponse(false, "User role not found");

            // Get role name
            var getRoleName = await FindRoleName(getUserRole.RoleId);
            if (getRoleName is null)
                return new LoginResponse(false, "User role not found");

            // Generate JWT token
            string jwtToken = GenerateToken(applicationUser, getRoleName.Name!);
            string refreshToken = GenerateRefreshToken();

            var findUser = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(_ => _.UserId == applicationUser.Id);
            if (findUser is not null)
            {
                findUser!.Token = refreshToken;
                await appDbContext.SaveChangesAsync();
            }
            else
            {
                await AddToDatabase(new RefreshTokenInfo() { Token = refreshToken, UserId = applicationUser.Id });
            }

            return new LoginResponse(true, "Login Successfully", jwtToken, refreshToken);
        }

        private string GenerateToken(ApplicationUser user, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Value.Key!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Fullname!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, role!),
                new Claim("ImagePath", user.Image)
            };

            var token = new JwtSecurityToken(
                issuer: config.Value.Issuer,
                audience: config.Value.Audience,
                claims: userClaims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<UserRole> FindUserRole(int userId) => await appDbContext.UserRoles.FirstOrDefaultAsync(_ => _.UserId == userId);
        private async Task<SystemRole> FindRoleName(int roleId) => await appDbContext.SystemRoles.FirstOrDefaultAsync(_ => _.Id == roleId);

        private string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        private async Task<ApplicationUser> FindUserByEmail(string email) => await appDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Email!.ToLower() == email.ToLower());

        private async Task<T> AddToDatabase<T>(T model)
        {
            var result = appDbContext.Add(model!);
            await appDbContext.SaveChangesAsync();
            return (T)result.Entity;
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
        {
            if (token is null) return new LoginResponse(false, "Model is empty");

            var findToken = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(u => u.Token!.Equals(token.Token));
            if (findToken is null) return new LoginResponse(false, "Refresh token is required");

            var user = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(_ => _.Id == findToken.UserId);
            if (user is null) return new LoginResponse(false, "Refresh token could not be generated because user not found");

            var userRole = await FindUserRole(user.Id);
            var roleName = await FindRoleName(userRole.RoleId);
            string jwtToken = GenerateToken(user, roleName.Name!);
            string refreshToken = GenerateRefreshToken();

            var updateRefreshToken = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(_ => _.UserId == user.Id);
            if (updateRefreshToken is null) return new LoginResponse(false, "Refresh token could not be generated because user not found");

            updateRefreshToken.Token = refreshToken;
            await appDbContext.SaveChangesAsync();
            return new LoginResponse(true, "Token refreshed successfully", jwtToken, refreshToken);
        }

        public async Task<ClaimsResponse> GetClaimsAsync(RefreshToken refreshToken)
        {
            if (refreshToken == null || string.IsNullOrEmpty(refreshToken.Token))
            {
                return new ClaimsResponse(false, "Invalid refresh token");
            }

            var storedRefreshToken = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(u => u.Token!.Equals(refreshToken.Token));
            if (storedRefreshToken == null)
            {
                return new ClaimsResponse(false, "Invalid or expired refresh token");
            }

            var user = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == storedRefreshToken.UserId);
            if (user == null) 
            {
                return new ClaimsResponse(false, "User not found");
            }

            var userRole = await FindUserRole(user.Id);
            var roleName = await FindRoleName(userRole.RoleId);

            // Here you need to generate a new JWT or fetch the existing one with claims
            var token = GenerateToken(user, roleName.Name!); // Generate a new token for claims
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            
            var claims = jsonToken.Claims.Select(c => new ClaimDto { Type = c.Type, Value = c.Value }).ToList();

            return new ClaimsResponse(true, "Claims retrieved successfully", claims);
        }
        public async Task<List<ManageUser>> GetUsers()
        {
            var allUsers = await GetApplicationUsers();
            var allUserRoles = await UserRoles();
            var allRoles = await SystemRoles();
            if (allUsers.Count == 0 || allRoles.Count == 0) return null!;

            var users = new List<ManageUser>();
            foreach (var user in allUsers)
            {
                var userRole = allUserRoles.FirstOrDefault(u => u.UserId == user.Id);
                var roleName = allRoles.FirstOrDefault(r => r.Id == userRole!.RoleId);
                users.Add(new ManageUser() { UserId = user.Id, Name=user.Fullname!, Email = user.Email!, Role = roleName!.Name! });
            }
            return users;
        }
        public async Task<List<SystemRole>> GetRoles() => await SystemRoles();
        public async Task<GeneralResponse> DeleteUser(int id)
        {
            var user = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id ==id);
            appDbContext.ApplicationUsers.Remove(user!);
            await appDbContext.SaveChangesAsync();
            return new GeneralResponse(true, "User successfully deleted!");
        }
        public async Task<GeneralResponse> UpdateUser(ManageUser user)
        {
            var getRole = (await SystemRoles()).FirstOrDefault(r => r.Name!.Equals(user.Role));
            var userRole = await appDbContext.UserRoles.FirstOrDefaultAsync(u => u.UserId == user.UserId);
            userRole!.RoleId = getRole!.Id;
            await appDbContext.SaveChangesAsync();
            return new GeneralResponse(true, "User role updated successfully");
        }
        private async Task<List<SystemRole>> SystemRoles() => await appDbContext.SystemRoles.AsNoTracking().ToListAsync();
        private async Task<List<UserRole>> UserRoles() => await appDbContext.UserRoles.AsNoTracking().ToListAsync();
        private async Task<List<ApplicationUser>> GetApplicationUsers() => await appDbContext.ApplicationUsers.AsNoTracking().ToListAsync();

        public async Task<string> GetUserImage(int Id) => (await GetApplicationUsers()).FirstOrDefault(u => u.Id ==Id)!.Image;
        public async Task<bool> UpdateProfile(UserProfile profile)
        {
            var user = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == int.Parse(profile.Id));
            user!.Email = profile.Email;
            user.Fullname = profile.Name;
            user.Image = profile.Image;
            await appDbContext.SaveChangesAsync();
            return true;
        }
        public async Task<GeneralResponse> UploadImage(int userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return new GeneralResponse(false, "No file provided");

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "userImages", uniqueFileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var user = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return new GeneralResponse(false, "User not found");

            // Update User's Image property with the path to the newly uploaded image
            user.Image = "/userImages/" + uniqueFileName;
            await appDbContext.SaveChangesAsync();

            // Return the path in the Message field of GeneralResponse
            return new GeneralResponse(true, uniqueFileName); // Note: Here we return the filename in the Message field
        }
        
    }
}