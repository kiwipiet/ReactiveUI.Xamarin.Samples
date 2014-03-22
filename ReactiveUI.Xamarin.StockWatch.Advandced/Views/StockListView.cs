using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ReactiveUI.Android;
using Splat;

namespace ReactiveUI.Sample.Routing
{
    public interface IStockListView
    {
        
    }

    public class StockListView : ReactiveSupportFragment<StockListViewModel>, IStockListView
    {

        public Button OpenMarket { get; private set; }
        public Button CloseMarket { get; private set; }
        public Button Reset { get; private set; }

        public TextView TxtOpenCanExecute { get; private set; }

        public ListView WatchList { get; private set; }

        private static View _view;
        private static IScreen _hostScreen;

        private IDisposable _activatedSubscription = null;
        private IDisposable _deactivatedSubscription = null;

        public StockListView()
        {
            
        }

        public StockListView(IScreen hostScreen)
        {
            _hostScreen = hostScreen;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (_view != null)
            {
                ViewGroup parent = (ViewGroup)_view.Parent;
                if (parent != null)
                    parent.RemoveView(_view);
            }
            try
            {
                _view = inflater.Inflate(Resource.Layout.StockList, container, false);

                //ViewModel = new StockListViewModel(_hostScreen);

                this.WireUpControls(_view);

                var adapter = new ReactiveListAdapter<WatchListItemViewModel>(
                ViewModel.Stocks,
                (viewModel, parent) => new WatchListItemView(viewModel, this.Activity, parent));

                WatchList.Adapter = adapter;

                this.OneWayBind(ViewModel, vm => vm.MarketState, c => c.TxtOpenCanExecute.Text);

                this.BindCommand(ViewModel, vm => vm.OpenMarketCommand, c => c.OpenMarket);
                this.BindCommand(ViewModel, vm => vm.CloseMarketCommand, c => c.CloseMarket);
                this.BindCommand(ViewModel, vm => vm.ResetCommand, c => c.Reset);

                if (_activatedSubscription == null && _deactivatedSubscription == null)
                {
                    _activatedSubscription = Activated.Subscribe(r => this.Log().Debug("StockListView => Activated"));
                    _deactivatedSubscription = Deactivated.Subscribe(r => this.Log().Debug("StockListView => Deactivated"));
                }
            }
            catch (InflateException e)
            {
                /* view is already there, just return view as it is */
            }
            return _view;
        }

    }
}