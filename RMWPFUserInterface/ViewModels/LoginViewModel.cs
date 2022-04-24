using Caliburn.Micro;
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

        public bool CanLogin
        {
            get
            {
                return !String.IsNullOrEmpty(Username) && !String.IsNullOrEmpty(Password);
            }
        }

        public void Login()
        {
            
        }

    }
}
