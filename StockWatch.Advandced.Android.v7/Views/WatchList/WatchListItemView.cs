using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Views;
using Android.Widget;
using ReactiveUI;
using ReactiveUI.Android;

namespace StockWatch.Advandced
{
    public class WatchListItemView : ReactiveViewHost<WatchListItemViewModel>
    {
        public WatchListItemView(WatchListItemViewModel viewModel, Context ctx, ViewGroup parent) :
            base(ctx, Resource.Layout.v_watchlist_item, parent, prefix: "watchListItem")
        {
            ViewModel = viewModel;
            this.OneWayBind(ViewModel, vm => vm.Symbol, v => v.Symbol.Text);
            this.OneWayBind(ViewModel, vm => vm.Name, v => v.Name.Text);
            this.OneWayBind(ViewModel, vm => vm.PriceSales, v => v.PriceSales.Text, v => string.Format("{0:0.00}", v));
            //this.OneWayBind(ViewModel, vm => vm.PriceBook, v => v.PriceBook.Text, v => string.Format("{0:0.00}", v));
            this.OneWayBind(ViewModel, vm => vm.LastUpdate, v => v.LastUpdate.Text, v => v.ToLongTimeString());
            this.OneWayBind(ViewModel, vm => vm.ChangeInPercent, v => v.ChangeInPercent.Text, v => string.Format("{0:P2}", v));
            //this.OneWayBind(ViewModel, vm => vm.DailyHigh, v => v.DailyHigh.Text, v => string.Format("{0:0.00}", v));
            //this.OneWayBind(ViewModel, vm => vm.DailyLow, v => v.DailyLow.Text, v => string.Format("{0:0.00}", v));
        }

        public TextView Symbol { get; private set; }
        public TextView Name { get; private set; }

        public TextView PriceSales { get; private set; }
        //public TextView PriceBook { get; private set; }
        
        public TextView ChangeInPercent { get; private set; }
        //public TextView DailyHigh { get; private set; }
        //public TextView DailyLow { get; private set; }

        public TextView LastUpdate { get; private set; }

    }
}