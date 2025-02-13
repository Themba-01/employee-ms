using BaseLibrary.DTOs;
using ClientLibrary.Services.Contracts;

namespace ClientLibrary.Helpers
{
    public class CustomHttpHandler(GetHttpClient getHttpClient, LocalStorageService localStorageService, IUserAccountService accountService) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            bool loginUrl = request.RequestUri!.AbsoluteUri.Contains("login");
            bool registerUrl = request.RequestUri!.AbsoluteUri.Contains("register");
            bool refreshTokenUrl = request.RequestUri!.AbsoluteUri.Contains("refresh-token");

            if (loginUrl || registerUrl || refreshTokenUrl) return await base.SendAsync(request, cancellationToken);

            var result = await base.SendAsync(request, cancellationToken);

            if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var userSession = await localStorageService.GetToken();
                if (userSession == null) return result;

                string token = string.Empty;
                try { token = request.Headers.Authorization!.Parameter!; }
                catch { }

                if (string.IsNullOrEmpty(token))
                {
                    if (userSession.Token != null)
                    {
                        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userSession.Token);
                        return await base.SendAsync(request, cancellationToken);
                    }
                    return result; // If no valid token, return the unauthorized response
                }

                var newJwtToken = await GetRefreshToken(userSession.RefreshToken!);
                if (!string.IsNullOrEmpty(newJwtToken))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", newJwtToken);
                    return await base.SendAsync(request, cancellationToken);
                }
            }
            return result;
        }

        private async Task<string> GetRefreshToken(string refreshToken)
        {
            var result = await accountService.RefreshTokenAsync(new RefreshToken() { Token = refreshToken });
            if (result.Flag)
            {
                // Update the stored token with the new one
                await localStorageService.SetToken(new UserSession { Token = result.Token, RefreshToken = result.RefreshToken });
                return result.Token; // Return the new JWT
            }
            return string.Empty; // If refresh fails, return empty string
        }
    }
}