using BaseLibrary.DTOs;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ClientLibrary.Helpers
{
    public class GetHttpClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly LocalStorageService _localStorageService;
        private const string AuthorizationHeaderKey = "Authorization";

        public GetHttpClient(IHttpClientFactory httpClientFactory, LocalStorageService localStorageService)
        {
            _httpClientFactory = httpClientFactory;
            _localStorageService = localStorageService;
        }

        /// <summary>
        /// Configures and returns an HttpClient for authenticated requests.
        /// Adds the Bearer token to the Authorization header if available.
        /// </summary>
        public async Task<HttpClient> GetPrivateHttpClient()
        {
            var client = _httpClientFactory.CreateClient("SystemApiClient");

            try
            {
                var userSession = await _localStorageService.GetToken();
                if (userSession != null && !string.IsNullOrEmpty(userSession.Token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userSession.Token);
                }
            }
            catch (Exception ex)
            {
                // Log the exception. In a production environment, use a proper logging mechanism.
                Console.WriteLine($"Error retrieving token: {ex.Message}");
            }

            // Set the Accept header for JSON responses
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        /// <summary>
        /// Returns an HttpClient for public, unauthenticated requests.
        /// Clears all headers to ensure no sensitive data is sent unintentionally.
        /// </summary>
        public HttpClient GetPublicHttpClient()
        {
            var client = _httpClientFactory.CreateClient("SystemApiClient");

            // Remove Authorization header and clear all headers
            client.DefaultRequestHeaders.Remove(AuthorizationHeaderKey);
            client.DefaultRequestHeaders.Clear();

            // Set the Accept header for JSON responses
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}