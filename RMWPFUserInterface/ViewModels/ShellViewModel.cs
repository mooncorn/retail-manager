using Caliburn.Micro;
using RMWPFUserInterface.EventModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RMWPFUserInterface.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        private SalesViewModel _salesViewModel;
        private IEventAggregator _events;

        public ShellViewModel(SalesViewModel salesVM, IEventAggregator events)
        {
            _salesViewModel = salesVM;
            _events = events;

            _events.SubscribeOnUIThread(this);

            ActivateItemAsync(IoC.Get<LoginViewModel>());
        }

        public async Task HandleAsync(LogOnEvent logOnEvent, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(_salesViewModel);
        }
    }
}
