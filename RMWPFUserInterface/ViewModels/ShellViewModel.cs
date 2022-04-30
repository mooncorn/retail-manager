using Caliburn.Micro;
using RMWPFUserInterface.EventModels;
using RMWPFUserInterface.Library.Api;
using RMWPFUserInterface.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RMWPFUserInterface.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>, IHandle<CheckOutEvent>
    {
        private SalesViewModel _salesViewModel;
        private ILoggedInUserModel _loggedInUser;
        private IEventAggregator _events;
        private IAPIHelper _apiHelper;

        public bool IsLoggedIn
        {
            get { return !string.IsNullOrEmpty(_loggedInUser.Token); }
        }

        public ShellViewModel(SalesViewModel salesVM, IEventAggregator events, ILoggedInUserModel loggedInUser, IAPIHelper apiHelper)
        {
            _salesViewModel = salesVM;
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
        }

        public async Task HandleAsync(LogOnEvent logOnEvent, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(_salesViewModel);
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        public async Task HandleAsync(CheckOutEvent checkOutEvent, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(IoC.Get<SalesViewModel>());
        }
    }
}
