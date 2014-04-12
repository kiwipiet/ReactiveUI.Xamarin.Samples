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

using ReactiveUI;
using ReactiveUI.Android;
using Splat;

namespace StockWatch.Advandced
{

    public class WatchListView : ReactiveFragment<WatchListViewModel>, IWatchListView, AdapterView.IOnItemClickListener
    {

        public ListView Listview { get; private set; }

        private static View _view;

        private IDisposable _activatedSubscription = null;
        private IDisposable _deactivatedSubscription = null;

        public WatchListView()
        {
            //RetainInstance = true; // => ATTENTION!! throws error "No view found for id" if Layout is different (switch between Portrait and Landscape Mode)!!!
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
                this.Log().Debug("OnCreateView");
                _view = inflater.Inflate(Resource.Layout.f_watchlist, container, false);

                this.WireUpControls(_view, "watchList");

                var adapter = new ReactiveListAdapter<WatchListItemViewModel>(
                    ViewModel.WatchList,
                    (viewModel, parent) => new WatchListItemView(viewModel, this.Activity, parent));

                Listview.Adapter = adapter;
                Listview.OnItemClickListener = this;

                if (_activatedSubscription == null && _deactivatedSubscription == null)
                {
                    _activatedSubscription = Activated.Subscribe(r => this.Log().Debug("Activated"));
                    _deactivatedSubscription = Deactivated.Subscribe(r => this.Log().Debug("Deactivated"));
                }
            }
            catch (InflateException e)
            {
                /* view is already there, just return view as it is */
            }
            return _view;
        }


        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            var item = ViewModel.WatchList[position];
            ViewModel.ItemClick.OnNext(item);
        }
    }
}