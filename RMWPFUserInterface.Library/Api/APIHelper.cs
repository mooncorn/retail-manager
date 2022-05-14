using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using RMWPFUserInterface.Library.Models;
using Microsoft.Extensions.Configuration;

namespace RMWPFUserInterface.Library.Api
{
    public class APIHelper : IAPIHelper
    {
        private HttpClient _apiClient;
        private ILoggedInUserModel _loggedInUser;
        private IConfiguration _config;

        public HttpClient ApiClient { get { return _apiClient; } }

        public APIHelper(ILoggedInUserModel loggedInUserModel, IConfiguration config)
        {
            _loggedInUser = loggedInUserModel;
            _config = config;
            InitializeClient();
        }

        private void InitializeClient()
        {
            string api = _config.GetValue<string>("Api");

            _apiClient = new HttpClient();
            _apiClient.BaseAddress = new Uri(api);
            _apiClient.DefaultRequestHeaders.Accept.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<AuthenticatedUserModel> Authenticate(string username, string password)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                //new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password)
            });

            using (HttpResponseMessage response = await _apiClient.PostAsync("/api/Token", data))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsAsync<AuthenticatedUserModel>();
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task GetLoggedInUserDetails(string token)
        {
            _apiClient.DefaultRequestHeaders.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            using (HttpResponseMessage response = await _apiClient.GetAsync("/api/User"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var userDetails = await response.Content.ReadAsAsync<LoggedInUserModel>();
                    
                    _loggedInUser.Token = token;
                    _loggedInUser.Id = userDetails.Id;
                    _loggedInUser.FirstName = userDetails.FirstName;
                    _loggedInUser.LastName = userDetails.LastName;
                    _loggedInUser.EmailAddress = userDetails.EmailAddress;
                    _loggedInUser.CreatedDate = userDetails.CreatedDate;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public void ClearHeaders()
        {
            _apiClient.DefaultRequestHeaders.Clear();
        }
    }
}
