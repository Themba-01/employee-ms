using System.Security.Claims;
using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;

namespace ClientLibrary.Helpers
{
    public class CustomAuthenticationStateProvider(LocalStorageService localStorageService, ILogger<CustomAuthenticationStateProvider> logger, HttpClient httpClient) : AuthenticationStateProvider
    {
        private readonly ClaimsPrincipal anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        private readonly ILogger<CustomAuthenticationStateProvider> _logger = logger;

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            _logger.LogInformation("Getting authentication state...");
            var userSession = await localStorageService.GetToken();
            
            if (userSession == null || string.IsNullOrEmpty(userSession.RefreshToken))
            {
                _logger.LogWarning("No refresh token found in local storage.");
                return new AuthenticationState(anonymous);
            }

            var claims = await FetchClaimsFromServer(userSession.RefreshToken);
            if (claims.Count == 0)
            {
                // No claims means authentication failed or server couldn't be reached
                _logger.LogWarning("Authentication failed: No valid claims returned.");
                return new AuthenticationState(anonymous);
            }

            var identity = new ClaimsIdentity(claims, "CustomAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _logger.LogInformation("Authentication state updated with claims from server.");
            return new AuthenticationState(claimsPrincipal);
        }

        public async Task UpdateAuthenticationState(UserSession userSession)
        {
            _logger.LogInformation("Updating authentication state...");
            if (!string.IsNullOrEmpty(userSession?.RefreshToken))
            {
                await localStorageService.SetToken(userSession);
                _logger.LogInformation("Refresh token set in local storage.");

                var claims = await FetchClaimsFromServer(userSession.RefreshToken);
                if (claims.Count == 0)
                {
                    // If claims fetch fails, revert to anonymous
                    _logger.LogWarning("Authentication failed: No valid claims returned.");
                    await localStorageService.RemoveToken();
                    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
                    return;
                }

                var identity = new ClaimsIdentity(claims, "CustomAuth");
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
                _logger.LogInformation("Authentication state updated with claims from server.");
            }
            else
            {
                await localStorageService.RemoveToken();
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
                _logger.LogInformation("Refresh token removed from local storage. User logged out.");
            }
        }

        private async Task<List<Claim>> FetchClaimsFromServer(string refreshToken)
        {
            try
            {
                var claimsResponse = await httpClient.PostAsJsonAsync("api/authentication/get-claims", new RefreshToken { Token = refreshToken });
                if (claimsResponse.IsSuccessStatusCode)
                {
                    var response = await claimsResponse.Content.ReadFromJsonAsync<ClaimsResponse>();
                    if (response != null && response.Flag)
                    {
                        // Convert ClaimDto to Claim
                        return response.Claims.Select(c => new Claim(c.Type, c.Value)).ToList();
                    }
                    else
                    {
                        _logger.LogWarning("Claims fetch failed: " + (response?.Message ?? "Unknown error"));
                    }
                }
                else
                {
                    if (claimsResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        _logger.LogWarning("Authentication failed: Token might be invalid or expired.");
                        await localStorageService.RemoveToken(); // Clear invalid token
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to fetch claims. Status code: {claimsResponse.StatusCode}");
                    }
                }
                return new List<Claim>(); // Return empty list if there's an error or no claims
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching claims from server.");
                return new List<Claim>(); // Return empty list on exception
            }
        }
    }
}