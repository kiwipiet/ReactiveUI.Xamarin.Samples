using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveUI;

namespace StockWatch.Advandced
{
    public class WatchListItemViewModel : ReactiveObject
    {

        private decimal? priceSales;
        private decimal? priceBook;
        private decimal? changeInPercent;
        private decimal? dailyLow;
        private decimal? dailyHigh;
        private DateTime lastUpdate;

        //private ObservableAsPropertyHelper<decimal> change;
        //private ObservableAsPropertyHelper<double> percentChange; 

        public WatchListItemViewModel(WatchListItem item)
        {
            this.WatchListItem = item;

            Symbol = item.Symbol;
            Name = item.Name;
            PriceSales = item.PriceSales;
            PriceBook = item.PriceBook;
            ChangeInPercent = item.ChangeInPercent;
            DailyLow = item.DailyLow;
            DailyHigh = item.DailyHigh;
            LastUpdate = item.LastUpdate;

            //this.WhenAnyValue(v => v.PriceSales, v => v.DayOpen, (p, o) => p - o)
            //    .ToProperty(this, v => v.Change, out change);

            //this.WhenAnyValue(v => v.Change, v => v.Price, (c, p) => p != 0 ? (double)Math.Round(c / p, 4) : 0)
            //    .ToProperty(this, v => v.PercentChange, out percentChange);
        }


        public WatchListItem WatchListItem { get; private set; }

        public string Symbol { get; private set; }

        public string Name { get; private set; }

        public decimal? PriceSales
        {
            get { return priceSales; }
            set { this.RaiseAndSetIfChanged(ref priceSales, value); }
        }

        public decimal? PriceBook
        {
            get { return priceBook; }
            set { this.RaiseAndSetIfChanged(ref priceBook, value); }
        }

        public decimal? ChangeInPercent
        {
            get { return changeInPercent; }
            set { this.RaiseAndSetIfChanged(ref changeInPercent, value); }
        }

        public decimal? DailyLow
        {
            get { return dailyLow; }
            set { this.RaiseAndSetIfChanged(ref dailyLow, value); }
        }

        public decimal? DailyHigh
        {
            get { return dailyHigh; }
            set { this.RaiseAndSetIfChanged(ref dailyHigh, value); }
        }

        public DateTime LastUpdate
        {
            get { return lastUpdate; }
            set { this.RaiseAndSetIfChanged(ref lastUpdate, value); }
        }
    }
}