using Caliburn.Micro;
using RMWPFUserInterface.EventModels;
using RMWPFUserInterface.Library.Api;
using RMWPFUserInterface.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMWPFUserInterface.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _username;
        private string _password;
        private IAPIHelper _apiHelper;
        private string _errorMessage;
        private IEventAggregator _events;

        public LoginViewModel(IAPIHelper apiHelper, IEventAggregator events)
        {
            _apiHelper = apiHelper;
            _events = events;
        }

        public string Username
        {
            get { return _username; }
            set 
            {
                _username = value;
                NotifyOfPropertyChange(() => Username);
                NotifyOfPropertyChange(() => CanLogin);
            }
        }

        public string Password
        {
            get { return _password; }
            set 
            { 
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogin);
            }
        }

        public bool IsErrorVisible { get { return !String.IsNullOrEmpty(ErrorMessage); } }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set 
            { 
                _errorMessage = value;
                NotifyOfPropertyChange(() => ErrorMessage);
                NotifyOfPropertyChange(() => IsErrorVisible);
            }
        }

        public bool CanLogin { get { return !String.IsNullOrEmpty(Username) && !String.IsNullOrEmpty(Password); } }

        public async Task Login()
        {
            try
            {
                ErrorMessage = String.Empty;

                AuthenticatedUser result = await _apiHelper.Authenticate(Username, Password);
                await _apiHelper.GetLoggedInUserDetails(result.Access_Token);

                await _events.PublishOnUIThreadAsync(new LogOnEvent());

            } catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}
