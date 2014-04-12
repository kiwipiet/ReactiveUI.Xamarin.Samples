using System;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using Splat;

namespace StockWatch.Advandced
{
    public class StockViewModel : ReactiveObject
    {

        public ICommand AddCommand { get; private set; }

        public StockViewModel()
        {
            //TODO: CanExecute => prüfen ob bereits vorhanden
            AddCommand = ReactiveCommand.CreateAsync(a => AddStock());
        }

        public string Symbol { get; set; }
        public decimal? DailyLow { get; set; }
        public decimal? DailyHigh { get; set; }
        public string Name { get; set; }
        public decimal? ChangeInPercent { get; set; }
        public decimal? PriceSales { get; set; }
        public decimal? PriceBook { get; set; }
        public DateTime LastUpdate { get; set; }


        private async Task<Unit> AddStock()
        {
            var newItem = new WatchListItem();
            newItem.Symbol =Symbol;
            newItem.Name = Name;
            newItem.PriceSales = PriceSales;
            newItem.PriceBook = PriceBook;
            newItem.ChangeInPercent = ChangeInPercent;
            newItem.DailyHigh = DailyHigh;
            newItem.DailyLow = DailyLow;
            newItem.LastUpdate = LastUpdate;

            // Add to DB
            await DataManager.Current.AddStock(newItem);   //note: await will return to the UI-Thread

            // Navigate to WatchListItemDetail
            var app = Locator.CurrentMutable.GetService<IApp>();
            app.Navigate(new WatchListItemDetailViewModel(new WatchListItemViewModel(newItem)));

            return Unit.Default;
        }

        public override string ToString()
        {
            return string.Format("{0} - '{1}'", Symbol, Name);
        }
    }
}