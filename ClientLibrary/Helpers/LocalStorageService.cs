using Blazored.LocalStorage;
using BaseLibrary.DTOs;

namespace ClientLibrary.Helpers
{
    public class LocalStorageService(ILocalStorageService localStorageService)
    {
        private const string StorageKey = "authentication-token";

        public async Task<UserSession?> GetToken()
        {
            var storedSession = await localStorageService.GetItemAsync<UserSession>(StorageKey);
            return storedSession;
        }

        public async Task SetToken(UserSession session)
        {
            await localStorageService.SetItemAsync(StorageKey, session);
        }

        public async Task RemoveToken()
        {
            await localStorageService.RemoveItemAsync(StorageKey);
        }
    }
}