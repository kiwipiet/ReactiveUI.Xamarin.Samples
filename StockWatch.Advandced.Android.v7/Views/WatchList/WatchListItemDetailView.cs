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
    public class WatchListItemDetailView : ReactiveFragment<WatchListItemDetailViewModel>, IWatchListItemDetailView
    {
        
        public TextView Symbol { get; private set; }
        public TextView Name { get; private set; }

        private static View _view;

        public WatchListItemDetailView()
        {
            //RetainInstance = true; // => ATTENTION!! throws error "No view found for id" if Layout is different (switch between Portrait and Landscape Mode) !!!
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
                _view = inflater.Inflate(Resource.Layout.f_watchlistitem_detail, container, false);

                if (ViewModel != null)
                {
                    this.WireUpControls(_view, "watchlistitem_detail");

                    this.OneWayBind(ViewModel, vm => vm.Symbol, c => c.Symbol.Text);
                    this.OneWayBind(ViewModel, vm => vm.Name, v => v.Name.Text);
                }
                else
                {
                    this.Log().Debug("ViewModel == NULL");
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