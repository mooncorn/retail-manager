using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Portal.Models;
using RMWPFUserInterface.Library.Api;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Portal.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILocalStorageService _localStorageService;
        private readonly IConfiguration _config;
        private readonly IAPIHelper _apiHelper;

        private string TokenKeyName => _config["authToken"];

        public AuthenticationService(AuthenticationStateProvider authStateProvider,
                                    ILocalStorageService localStorageService,
                                    IConfiguration config,
                                    IAPIHelper apiHelper)
        {
            _authStateProvider = authStateProvider;
            _localStorageService = localStorageService;
            _config = config;
            _apiHelper = apiHelper;
        }

        public async Task<AuthenticatedUserModel?> Login(AuthenticationUserModel credentials)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("password", credentials.Password),
                new KeyValuePair<string, string>("username", credentials.Email)
            });

            var authResult = await _apiHelper.ApiClient.PostAsync(_config["tokenEndpoint"], data);
            var authContent = await authResult.Content.ReadAsStringAsync();

            if (!authResult.IsSuccessStatusCode)
                return null;

            var result = JsonSerializer.Deserialize<AuthenticatedUserModel>(
                authContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result == null)
                return null;

            await _localStorageService.SetItemAsStringAsync(TokenKeyName, result.Access_Token);

            ((AuthStateProvider)_authStateProvider).NotifyUserLogInAsync(result.Access_Token);

            _apiHelper.ApiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Access_Token);

            return result;
        }

        public async Task Logout()
        {
            await ((AuthStateProvider)_authStateProvider).NotifyUserLogOutAsync();
        }
    }
}
