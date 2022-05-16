using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Portal.Authentication
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;
        private readonly IConfiguration _config;
        private readonly AuthenticationState _anonymous;

        private string TokenKeyName => _config["LocalStorage:KeyNames:AuthToken"];

        public AuthStateProvider(HttpClient httpClient,
                                 ILocalStorageService localStorageService,
                                 IConfiguration config)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
            _config = config;
            _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorageService.GetItemAsStringAsync(TokenKeyName);

            if (string.IsNullOrWhiteSpace(token))
                return _anonymous;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var claimList = JwtParser.ParseClaimsFromJWT(token);
            var claimsIdentity = new ClaimsIdentity(claimList, "jwtAuthType");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            return new AuthenticationState(claimsPrincipal);
        }

        public void NotifyUserLogIn(string token)
        {
            // parse token information into claims
            var claimList = JwtParser.ParseClaimsFromJWT(token);
            var claimsIdentity = new ClaimsIdentity(claimList, "jwtAuthType");
            var authenticatedUser = new ClaimsPrincipal(claimsIdentity);

            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogOut()
        {
            var authState = Task.FromResult(_anonymous);
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
