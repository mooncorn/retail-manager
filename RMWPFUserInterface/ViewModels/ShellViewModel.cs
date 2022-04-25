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
        private SimpleContainer _container;

        public ShellViewModel(SalesViewModel salesVM, IEventAggregator events, SimpleContainer container)
        {
            _salesViewModel = salesVM;
            _events = events;
            _container = container;

            _events.SubscribeOnUIThread(this);

            ActivateItemAsync(_container.GetInstance<LoginViewModel>());
        }

        public async Task HandleAsync(LogOnEvent logOnEvent, CancellationToken cancellationToken)
        {
            await ActivateItemAsync(_salesViewModel);
        }
    }
}
