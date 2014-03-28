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
    public interface ISearchView
    {
        
    }

    public class SearchView : ReactiveSupportFragment<SearchViewModel>, ISearchView
    {

        public TextView TxtPath { get; private set; }
        public TextView TxtCreationDate { get; private set; }


        private static View _view;
        private static IScreen _hostScreen;

        public SearchView()
        {
            
        }

        public SearchView(IScreen hostScreen)
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
                _view = inflater.Inflate(Resource.Layout.Search, container, false);

                //ViewModel = new StockListViewModel(_hostScreen);

                this.WireUpControls(_view, "search");

                this.OneWayBind(ViewModel, vm => vm.UrlPathSegment, c => c.TxtPath.Text);
                this.OneWayBind(ViewModel, vm => vm.CreationDate, c => c.TxtCreationDate.Text);

            }
            catch (InflateException e)
            {
                /* view is already there, just return view as it is */
            }


            return _view;
        }

    }
}