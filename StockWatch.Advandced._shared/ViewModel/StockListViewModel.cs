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
    public class StockListViewModel : ReactiveObject, IRoutableViewModel
    {

        private readonly object marketStateLock = new object();
        private readonly double rangePercent = 0.002;
        private readonly ReactiveList<WatchListItemViewModel> stocks = new ReactiveList<WatchListItemViewModel>();


        private readonly TimeSpan updateInterval = TimeSpan.FromMilliseconds(2000);
        private readonly Random updateOrNotRandom = new Random();
        private readonly object updateStockPricesLock = new object();
        private MarketState marketState = MarketState.Closed;

        private Subject<Tuple<string, decimal>> _stockUpdates;
        

        private IDisposable timer;
        private volatile bool updatingStockPrices;

        public ICommand OpenMarketCommand { get; private set; }
        public ICommand CloseMarketCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }


        public IReadOnlyReactiveList<WatchListItemViewModel> Stocks
        {
            get { return stocks; }
        }

        public MarketState MarketState
        {
            get { return marketState; }
            private set { this.RaiseAndSetIfChanged(ref marketState, value); }
        }

        // IRoutableViewModel

        public string UrlPathSegment {
            get { return "/StockList"; }
        }

        public IScreen HostScreen { get; protected set; }

        // ctor

        public StockListViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();

            OpenMarketCommand = ReactiveCommand.CreateAsync(this.WhenAnyValue(vm => vm.MarketState, m => m == MarketState.Closed), a => OpenMarket(), RxApp.TaskpoolScheduler); //, RxApp.MainThreadScheduler

            CloseMarketCommand = ReactiveCommand.CreateAsync(this.WhenAnyValue(vm => vm.MarketState, m => m == MarketState.Open), a => CloseMarket(), RxApp.TaskpoolScheduler); // , RxApp.MainThreadScheduler

            ResetCommand = ReactiveCommand.CreateAsync(this.WhenAnyValue(vm => vm.MarketState, m => m == MarketState.Closed), a => Reset(), RxApp.TaskpoolScheduler); // , RxApp.MainThreadScheduler

            ((ReactiveCommand<Unit>)OpenMarketCommand).CanExecuteObservable.Subscribe(
                r => this.Log().Debug("'{0}' CanExecute: {1}", "OpenMarketCommand", r));

            ((ReactiveCommand<Unit>)CloseMarketCommand).CanExecuteObservable.Subscribe(
                r => this.Log().Debug("'{0}' CanExecute: {1}", "CloseMarketCommand", r));

            ((ReactiveCommand<Unit>)ResetCommand).CanExecuteObservable.Subscribe(
                r => this.Log().Debug("'{0}' CanExecute: {1}", "ResetCommand", r));


            _stockUpdates = new Subject<Tuple<string, decimal>>();
            _stockUpdates.ObserveOn(RxApp.MainThreadScheduler).Subscribe(UpdateStockItemUi);

            LoadDefaultStocks();
            
        }



        private void UpdateStockItemUi(Tuple<string, decimal> stockInfo)
        {
            if (stockInfo != null)
            {
                var stock = stocks.SingleOrDefault(s => s.Symbol == stockInfo.Item1);
                if (stock != null)
                {
                    stock.Price += stockInfo.Item2;
                }
            }
        }

        private bool TryUpdateStockPrice(WatchListItemViewModel stock)
        {
            //this.Log().Debug("UpdateStockPrice '{0}'", stock.Symbol);

            // Randomly choose whether to udpate this stock or not
            var r = updateOrNotRandom.NextDouble();
            if (r > 0.1)
            {
                return false;
            }

            Task.Delay(400).Wait();

            // Update the stock price by a random factor of the range percent
            var random = new Random((int)Math.Floor(stock.Price));
            var percentChange = random.NextDouble()*rangePercent;
            var pos = random.NextDouble() > 0.51;
            var change = Math.Round(stock.Price * (decimal)percentChange, 2);
            change = pos ? change : -change;


            //stock.Price += change;
            //this.Log().Debug("UpdateStockPrice '{0}' => {1}", stock.Symbol, (stock.Price + change));

            _stockUpdates.OnNext(new Tuple<string, decimal>(stock.Symbol, (stock.Price + change)));

            return true;
        }

        private void UpdateStockPrices()
        {
            // This function must be re-entrant as it's running as a timer interval handler
            if (!updatingStockPrices)
            {
                lock (updateStockPricesLock)
                {
                    updatingStockPrices = true;

                    foreach (var stock in stocks)
                    {
                        TryUpdateStockPrice(stock);
                    }
                    updatingStockPrices = false;
                }
            }
        }

        public Task<Unit> OpenMarket()
        {
            return Task.Run(() =>
            {
                lock (marketStateLock)
                {
                    if (MarketState != MarketState.Open)
                    {
                        MarketState = MarketState.Open;
                    }
                }

                timer = Observable.Timer(updateInterval, updateInterval, RxApp.TaskpoolScheduler) // RxApp.MainThreadScheduler
                            .Subscribe(_ => UpdateStockPrices());

                Task.Delay(4000).Wait();
                this.Log().Debug("OpenMarket => ready");

                return Unit.Default;
            });
        }

        public Task<Unit> CloseMarket()
        {
            return Task.Run(() =>
            {
                lock (marketStateLock)
                {
                    if (MarketState == MarketState.Open)
                    {
                        MarketState = MarketState.Closed;
                    }
                }

                if (timer != null)
                {
                    timer.Dispose();
                }

                Task.Delay(4000).Wait();
                this.Log().Debug("CloseMarket => ready");

                return Unit.Default;
            });
        }

        public Task<Unit> Reset()
        {
            return Task.Run(() =>
            {
                
                lock (marketStateLock)
                {
                    if (MarketState != MarketState.Closed)
                    {
                        throw new InvalidOperationException("Market must be closed before it can be reset.");
                    }
                }

                LoadDefaultStocks();

                //Task.Delay(4000).Wait();
                this.Log().Debug("Reset => ready");

                return Unit.Default;
            });
        }

        private void LoadDefaultStocks()
        {
            using (stocks.SuppressChangeNotifications())
            {
                stocks.Clear();
                stocks.Add(new WatchListItemViewModel("MSFT") {Price = 36.91m});
                stocks.Add(new WatchListItemViewModel("AAPL") {Price = 545.09m});
                stocks.Add(new WatchListItemViewModel("GOOG") {Price = 1107.32m});
                stocks.Add(new WatchListItemViewModel("FB") {Price = 54.77m});
            }
        }
    }

    public enum MarketState
    {
        Closed,
        Open
    }
    
}