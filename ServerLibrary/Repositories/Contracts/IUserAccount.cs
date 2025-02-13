
using BaseLibrary.DTOs;
using BaseLibrary.Responses;
using BaseLibrary.Entities;
using Microsoft.AspNetCore.Http;

namespace ServerLibrary.Repositories.Contracts
{
    public interface IUserAccount
    {
        Task<GeneralResponse> CreateAsync(Register user);
        Task<LoginResponse> SignInAsync(Login user);
        Task<LoginResponse> RefreshTokenAsync(RefreshToken token);
        Task<ClaimsResponse> GetClaimsAsync(RefreshToken refreshToken);
        Task<List<ManageUser>>GetUsers();
        Task<GeneralResponse> UpdateUser(ManageUser user);
        Task<List<SystemRole>> GetRoles();
        Task<GeneralResponse> DeleteUser(int id);
        Task<string>GetUserImage(int id);
        Task<bool>UpdateProfile(UserProfile profile);
        Task<GeneralResponse> UploadImage(int userId, IFormFile file);
    }
}