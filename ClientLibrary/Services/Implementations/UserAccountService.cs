using BaseLibrary.Responses;
using ClientLibrary.Services.Contracts;
using BaseLibrary.DTOs;
using ClientLibrary.Helpers;
using System.Net.Http.Json;
using BaseLibrary.Entities;

namespace ClientLibrary.Services.Implementatons
{
    public class UserAccountService(GetHttpClient getHttpClient) : IUserAccountService
    {
        public const string AuthUrl = "api/authentication";

        // Create User - Register
        public async Task<GeneralResponse> CreateAsync(Register user)
        {
            var httpClient = getHttpClient.GetPublicHttpClient();
            var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/register", user);
            if (!result.IsSuccessStatusCode) 
                return new GeneralResponse(false, "Error occurred");

            // Ensure the content is not null before attempting to read it
            var generalResponse = await result.Content.ReadFromJsonAsync<GeneralResponse>();
            return generalResponse ?? new GeneralResponse(false, "Failed to deserialize response");
        }

        // Sign in User - Login
        public async Task<LoginResponse> SignInAsync(Login user)
        {
            var httpClient = getHttpClient.GetPublicHttpClient();
            var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/login", user);
            if (!result.IsSuccessStatusCode) 
                return new LoginResponse(false, "Error occurred");

            // Ensure the content is not null before attempting to read it
            var loginResponse = await result.Content.ReadFromJsonAsync<LoginResponse>();
            return loginResponse ?? new LoginResponse(false, "Failed to deserialize response");
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
        {
            var httpClient = getHttpClient.GetPublicHttpClient();
            var result = await httpClient.PostAsJsonAsync($"{AuthUrl}/refresh-token", token);
            if (!result.IsSuccessStatusCode) return new LoginResponse(false, "Error occured");

            return await result.Content.ReadFromJsonAsync<LoginResponse>()!;   
        }
        public async Task<List<ManageUser>> GetUsers()
        {
            var httpClient = await getHttpClient.GetPrivateHttpClient();
            var result = await httpClient.GetFromJsonAsync<List<ManageUser>>($"{AuthUrl}/users");
            return result!;
        }
        public async Task<GeneralResponse> UpdateUser(ManageUser user)
        {
            var httpClient = getHttpClient.GetPublicHttpClient();
            var result = await httpClient.PutAsJsonAsync($"{AuthUrl}/update-user", user);
            if (!result.IsSuccessStatusCode) return new GeneralResponse(false, "Error occured");
            return await result.Content.ReadFromJsonAsync<GeneralResponse>()!;
        }
        public async Task<List<SystemRole>> GetRoles()
        {
            var httpClient = await getHttpClient.GetPrivateHttpClient();
            var result = await httpClient.GetFromJsonAsync<List<SystemRole>>($"{AuthUrl}/roles");
            return result!;
        }
        public async Task<GeneralResponse> DeleteUser(int id)
        {
            var httpClient =await getHttpClient.GetPrivateHttpClient();
            var result = await httpClient.DeleteAsync($"{AuthUrl}/delete-user/{id}");
            if (!result.IsSuccessStatusCode) return new GeneralResponse(false, "Error occured");
            return await result.Content.ReadFromJsonAsync<GeneralResponse>()!;
        }
    }
}
