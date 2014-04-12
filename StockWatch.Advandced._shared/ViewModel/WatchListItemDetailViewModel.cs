using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace StockWatch.Advandced
{
    public class WatchListItemDetailViewModel : ReactiveObject, IRoutableViewModel
    {
        private string _symbol;
        private string _name;
        private decimal? _priceSales;
        private decimal? _priceBook;
        private decimal? _changeInPercent;
        private decimal? _dailyLow;
        private decimal? _dailyHigh;
        private DateTime _lastUpdate;
        
        // IRoutableViewModel

        public string UrlPathSegment
        {
            get { return string.Format("/WatchList/Detail?symbol={0}", Symbol); }
        }

        public IScreen HostScreen { get; protected set; }


        public WatchListItemViewModel WatchListItemViewModel { get; private set; }

        public string Symbol
        {
            get { return _symbol; }
            set { this.RaiseAndSetIfChanged(ref _symbol, value); }
        }

        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }

        public decimal? PriceSales
        {
            get { return _priceSales; }
            set { this.RaiseAndSetIfChanged(ref _priceSales, value); }
        }

        public decimal? PriceBook
        {
            get { return _priceBook; }
            set { this.RaiseAndSetIfChanged(ref _priceBook, value); }
        }

        public decimal? ChangeInPercent
        {
            get { return _changeInPercent; }
            set { this.RaiseAndSetIfChanged(ref _changeInPercent, value); }
        }

        public decimal? DailyLow
        {
            get { return _dailyLow; }
            set { this.RaiseAndSetIfChanged(ref _dailyLow, value); }
        }

        public decimal? DailyHigh
        {
            get { return _dailyHigh; }
            set { this.RaiseAndSetIfChanged(ref _dailyHigh, value); }
        }

        public DateTime LastUpdate
        {
            get { return _lastUpdate; }
            set { this.RaiseAndSetIfChanged(ref _lastUpdate, value); }
        }


        // ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="WatchListItemDetailViewModel"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public WatchListItemDetailViewModel(WatchListItemViewModel item)
        {
            SetWatchListItem(item);
        }

        // Methods

        /// <summary>
        /// Sets the watch list iteml.
        /// </summary>
        /// <param name="item">The item.</param>
        public void SetWatchListItem(WatchListItemViewModel item)
        {
            this.WatchListItemViewModel = item;

            Symbol = item.Symbol;
            Name = item.Name;
            PriceSales = item.PriceSales;
            PriceBook = item.PriceBook;
            ChangeInPercent = item.ChangeInPercent;
            DailyLow = item.DailyLow;
            DailyHigh = item.DailyHigh;
            LastUpdate = item.LastUpdate;
        }
    }
}
