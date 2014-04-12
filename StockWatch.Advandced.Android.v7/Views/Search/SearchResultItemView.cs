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

namespace StockWatch.Advandced
{
    public class SearchResultItemView : ReactiveViewHost<StockViewModel>
    {

        public SearchResultItemView(StockViewModel viewModel, Context ctx, ViewGroup parent) :
            base(ctx, Resource.Layout.v_search_resultitem, parent, prefix: "searchResultItem")
        {
            ViewModel = viewModel;
            this.OneWayBind(ViewModel, vm => vm.Symbol, v => v.Symbol.Text);
            this.OneWayBind(ViewModel, vm => vm.Name, v => v.Name.Text);
            this.OneWayBind(ViewModel, vm => vm.PriceSales, v => v.PriceSales.Text, v => string.Format("{0:0.00}", v));
            //this.OneWayBind(ViewModel, vm => vm.LastUpdate, v => v.LastUpdate.Text, v => v.ToLongTimeString());
            this.OneWayBind(ViewModel, vm => vm.ChangeInPercent, v => v.ChangeInPercent.Text, v => string.Format("{0:P2}", v));

            this.BindCommand(ViewModel, vm => vm.AddCommand, c => c.Add);
        }

        public TextView Symbol { get; private set; }
        public TextView Name { get; private set; }
        public TextView PriceSales { get; private set; }
        public TextView ChangeInPercent { get; private set; }

        public Button Add { get; private set; }

    }
}