using Caliburn.Micro;
using RMDesktopUI.EventModels;
using RMWPFUserInterface.Library.Api;
using RMWPFUserInterface.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>, IHandle<CheckOutEvent>
    {
        private readonly ILoggedInUserModel _loggedInUser;
        private readonly IEventAggregator _events;
        private readonly IAPIHelper _apiHelper;

        public bool IsLoggedIn
        {
            get { return !string.IsNullOrEmpty(_loggedInUser.Token); }
        }

        public bool IsNotLoggedIn
        {
            get { return string.IsNullOrEmpty(_loggedInUser.Token); }
        }

        public ShellViewModel(IEventAggregator events, ILoggedInUserModel loggedInUser, IAPIHelper apiHelper)
        {
            _events = events;
            _loggedInUser = loggedInUser;
            _apiHelper = apiHelper;

            _events.SubscribeOnUIThread(this);

            ActivateItemAsync(IoC.Get<LoginViewModel>());
        }

        public async void ExitApplication()
        {
            await TryCloseAsync();
        }

        public async void LogOut()
        {
            _loggedInUser.Clear();
            _apiHelper.ClearHeaders();
            await ActivateItemAsync(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);
            NotifyOfPropertyChange(() => IsNotLoggedIn);
        }

        public async void LogIn()
        {
            await ActivateItemAsync(IoC.Get<LoginViewModel>());
        }

        public async void UserManagement()
        {
            await ActivateItemAsync(IoC.Get<UserDisplayViewModel>());
        }

        public async void Sales()
        {
            await ActivateItemAsync(IoC.Get<SalesViewModel>());
        }

        public async Task HandleAsync(LogOnEvent logOnEvent, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(IoC.Get<SalesViewModel>(), cancellationToken);
            NotifyOfPropertyChange(() => IsLoggedIn);
            NotifyOfPropertyChange(() => IsNotLoggedIn);
        }

        public async Task HandleAsync(CheckOutEvent checkOutEvent, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(IoC.Get<SalesViewModel>(), cancellationToken);
        }
    }
}
