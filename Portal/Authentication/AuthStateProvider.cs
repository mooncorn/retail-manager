using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using RMWPFUserInterface.Library.Api;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Portal.Authentication
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly IConfiguration _config;
        private readonly IAPIHelper _apiHelper;
        private readonly AuthenticationState _anonymous;

        private string TokenKeyName => _config["authToken"];

        public AuthStateProvider(ILocalStorageService localStorageService,
                                 IConfiguration config,
                                 IAPIHelper apiHelper)
        {
            _localStorageService = localStorageService;
            _config = config;
            _apiHelper = apiHelper;
            _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        /// <summary>
        /// Initializes user's authentication state on app startup. 
        /// If a JWT Token exists in the local storage, a request will be sent to the server
        /// to confirm the validity of the token. 
        /// </summary>
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string? token = await _localStorageService.GetItemAsStringAsync(TokenKeyName);

            if (string.IsNullOrWhiteSpace(token))
                return _anonymous;

            try
            {
                await _apiHelper.GetLoggedInUserDetails(token);

                var claimList = JwtParser.ParseClaimsFromJWT(token);
                var claimsIdentity = new ClaimsIdentity(claimList, "jwtAuthType");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                return new AuthenticationState(claimsPrincipal);
            }
            catch (Exception ex)
            {
                // TODO: Log this exception
                return _anonymous;
            }
        }

        /// <summary>
        /// Log in the user into the application.
        /// </summary>
        /// <param name="token">UTF-8 encoded JWT Token</param>
        public async void NotifyUserLogInAsync(string token)
        {
            await _localStorageService.SetItemAsStringAsync(TokenKeyName, token);
            AuthenticationState? authState = await GetAuthenticationStateAsync();

            if (authState != null)
            {
                NotifyAuthenticationStateChanged(Task.FromResult(authState));
            }
            else
            {
                await NotifyUserLogOutAsync();
            }
        }

        /// <summary>
        /// Log out the user from the application. This method removes JWT Token from local storage
        /// and by clears headers from future requests.
        /// </summary>
        public async Task NotifyUserLogOutAsync()
        {
            // delete expired token
            await _localStorageService.RemoveItemAsync(TokenKeyName);

            _apiHelper.ClearHeaders();

            NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
        }
    }
}
