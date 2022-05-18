using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Portal.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Portal.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorageService;
        private readonly IConfiguration _config;

        private string TokenKeyName => _config["authToken"];

        public AuthenticationService(HttpClient httpClient,
                                    AuthenticationStateProvider authStateProvider,
                                    ILocalStorageService localStorageService,
                                    IConfiguration config)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
            _localStorageService = localStorageService;
            _config = config;

            _httpClient.BaseAddress = new Uri(_config["api"]);
        }

        public async Task<AuthenticatedUserModel?> Login(AuthenticationUserModel credentials)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("password", credentials.Password),
                new KeyValuePair<string, string>("username", credentials.Email)
            });

            var authResult = await _httpClient.PostAsync(_config["tokenEndpoint"], data);
            var authContent = await authResult.Content.ReadAsStringAsync();

            if (!authResult.IsSuccessStatusCode)
                return null;

            var result = JsonSerializer.Deserialize<AuthenticatedUserModel>(
                authContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result == null)
                return null;

            await _localStorageService.SetItemAsStringAsync(TokenKeyName, result.Access_Token);

            ((AuthStateProvider)_authStateProvider).NotifyUserLogIn(result.Access_Token);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Access_Token);

            return result;
        }

        public async Task Logout()
        {
            await _localStorageService.RemoveItemAsync(TokenKeyName);
            ((AuthStateProvider)_authStateProvider).NotifyUserLogOut();
            _httpClient.DefaultRequestHeaders.Clear();
        }
    }
}
