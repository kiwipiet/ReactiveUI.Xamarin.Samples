using ReactiveUI;
using SQLite.Net.Interop;

namespace StockWatch.Advandced
{
    public interface IApp : IPlatform
    {
        AppModel AppModel { get; }

        // Navigation
        void Navigate(IRoutableViewModel viewModel, bool notInNavigationStack = false);
        void Navigate(IRoutableViewModel viewModel, IRoutingParams routingParams);
        
    }
}