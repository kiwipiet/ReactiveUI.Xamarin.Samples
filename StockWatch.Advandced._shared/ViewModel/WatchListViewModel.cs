using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using Splat;

namespace StockWatch.Advandced
{
    public class WatchListViewModel : ReactiveObject, IRoutableViewModel
    {

        // IRoutableViewModel
        public string UrlPathSegment
        {
            get { return "/WatchList"; }
        }

        public IScreen HostScreen { get; protected set; }

        
        private readonly ReactiveList<WatchListItemViewModel> _watchList = new ReactiveList<WatchListItemViewModel>();
        public IReadOnlyReactiveList<WatchListItemViewModel> WatchList
        {
            get { return _watchList; }
        }

        private WatchListItemViewModel _selectedItem;
        public WatchListItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set { this.RaiseAndSetIfChanged(ref _selectedItem, value); }
        }

        public Subject<WatchListItemViewModel> ItemClick { get; private set; }

        
        

        // ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="WatchListViewModel"/> class.
        /// </summary>
        /// <param name="hostScreen">The host screen.</param>
        public WatchListViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();

            DataManager.Current.AppDb
                .WatchListItemChange
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(OnWatchListItemChange);

            ItemClick = new Subject<WatchListItemViewModel>();
            ItemClick
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(OnItemClick);

            LoadDefaultStocks();
            
        }


        /// <summary>
        /// Called when [watch list item change].
        /// </summary>
        /// <param name="item">The item.</param>
        private void OnWatchListItemChange(Tuple<DbChangeTypeEnum, WatchListItem> item)
        {
            switch (item.Item1)
            {
                case DbChangeTypeEnum.Create:
                    _watchList.Add(new WatchListItemViewModel(item.Item2));
                    break;
                case DbChangeTypeEnum.Update:

                    break;
                case DbChangeTypeEnum.Delete:
                    
                    break;
            }
        }

        /// <summary>
        /// Called when [item click].
        /// </summary>
        /// <param name="item">The item.</param>
        private void OnItemClick(WatchListItemViewModel item)
        {
            // Navigate to WatchListItemDetail
            var app = Locator.CurrentMutable.GetService<IApp>();
            SelectedItem = item;
            app.Navigate(new WatchListItemDetailViewModel(item));
        }

        /// <summary>
        /// Loads the default stocks.
        /// </summary>
        private async void LoadDefaultStocks()
        {
            await DataManager.Current.AppDb.SetupDatabaseAsync();
            var stockList = await DataManager.Current.GetWatchList();

            using (_watchList.SuppressChangeNotifications())
            {
                _watchList.Clear();
                foreach (var item in stockList)
                {
                    _watchList.Add(new WatchListItemViewModel(item));
                }
            }
        }
    }

}