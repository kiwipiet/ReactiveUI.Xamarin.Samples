using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

    public class SearchView : ReactiveFragment<SearchViewModel>, ISearchView, AdapterView.IOnItemClickListener
    {

        public EditText Symbol { get; private set; }
        public Button Search { get; private set; }
        public ListView ResultList { get; private set; }

        private static View _view;

        private IDisposable _deactivatedSubscription = null;

        public SearchView()
        {
            RetainInstance = true;
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
                _view = inflater.Inflate(Resource.Layout.f_search, container, false);

                this.WireUpControls(_view, "search");

                var adapter = new ReactiveListAdapter<StockViewModel>(
                ViewModel.Quotes,
                (viewModel, parent) => new SearchResultItemView(viewModel, this.Activity, parent));

                ResultList.Adapter = adapter;
                ResultList.OnItemClickListener = this;

                this.BindCommand(ViewModel, vm => vm.SearchCommand, c => c.Search);
                this.Bind(ViewModel, vm => vm.Symbol, c => c.Symbol.Text);

            }
            catch (InflateException e)
            {
                /* view is already there, just return view as it is */
            }

            return _view;
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            var quote = ViewModel.Quotes[position];
            Toast.MakeText(this.Activity, quote.Symbol, ToastLength.Short).Show();
        }
    }
}