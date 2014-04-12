using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace StockWatch.Advandced
{
    public class WatchListAndDetailViewModel : ReactiveObject, IRoutableViewModel
    {
        // IRoutableViewModel
        public string UrlPathSegment
        {
            get { return "/WatchList/Detail"; }
        }

        public IScreen HostScreen { get; protected set; }

        public WatchListViewModel ListViewModel { get; private set; }
        public WatchListItemDetailViewModel DetailViewModel { get; private set; }

        private IDisposable _ItemClickSubscription = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="WatchListAndDetailViewModel"/> class.
        /// </summary>
        /// <param name="listViewModel">The list view model.</param>
        /// <param name="detailViewModel">The detail view model.</param>
        public WatchListAndDetailViewModel(WatchListViewModel listViewModel, WatchListItemDetailViewModel detailViewModel)
        {
            ListViewModel = listViewModel;
            DetailViewModel = detailViewModel;

            // Set Details OnItemClick
            _ItemClickSubscription = 
                ListViewModel
                .ItemClick
                .Subscribe(i =>
                {
                    if (DetailViewModel.Symbol != i.Symbol)
                    {
                        DetailViewModel.SetWatchListItem(i);
                    }
                });
        }



    }
}
