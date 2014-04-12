using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
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
using AppFragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using AppFragmentManager = Android.Support.V4.App.FragmentManager;
using AppFragment = Android.Support.V4.App.Fragment;

namespace StockWatch.Advandced
{
    public class WatchListAndDetailView : ReactiveFragment<WatchListAndDetailViewModel>, IWatchListAndDetailView
    {

        private static View _view;

        private WatchListView _list;
        private WatchListItemDetailView _details;
        

        public WatchListAndDetailView()
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
                _view = inflater.Inflate(Resource.Layout.f_watchlist_and_detail, container, false);

                this.WireUpControls(_view, "watchlist_and_detail");

                var fm = this.FragmentManager;
                var ft = fm.BeginTransaction();

                _list = fm.FindFragmentByTag("watchlist_and_detail_list") as WatchListView;
                _details = fm.FindFragmentByTag("watchlist_and_detail_detail") as WatchListItemDetailView;

                // List
                if (_list == null)
                {
                    _list = new WatchListView();
                    _list.ViewModel = ViewModel.ListViewModel;

                    this.Log().Debug("OnCreateView => Add: List");
                    ft.Add(Resource.Id.watchList_and_detail_listcontainer, _list, "watchlist_and_detail_list");
                }
                else
                {
                    _list.ViewModel = ViewModel.ListViewModel;
                    this.Log().Debug("OnCreateView => Attach: List");
                    ft.Attach(_list);
                }

                // Detail
                if (_details == null)
                {
                    _details = new WatchListItemDetailView();
                    _details.ViewModel = ViewModel.DetailViewModel;

                    this.Log().Debug("OnCreateView => Add: Detail");
                    ft.Add(Resource.Id.watchList_and_detail_detailscontainer, _details, "watchlist_and_detail_detail");
                }
                else
                {
                    _details.ViewModel = ViewModel.DetailViewModel;
                    this.Log().Debug("OnCreateView => Attach: Detail");
                    ft.Attach(_details);
                }
                ft.Commit();
            }
            catch (InflateException e)
            {
                /* view is already there, just return view as it is */
            }

            return _view;
        }

        /// <summary>
        /// Releases / Detaches the Fragments from the View.
        /// </summary>
        public void ReleaseComposition()
        {
            // Detach fragments
            var fm = this.FragmentManager;
            var ft = fm.BeginTransaction();
            ft.Detach(_list);
            ft.Detach(_details);
            ft.Commit();
        }

    }
}