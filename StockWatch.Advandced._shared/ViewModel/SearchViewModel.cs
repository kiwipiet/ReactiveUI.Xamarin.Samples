using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using Splat;

namespace StockWatch.Advandced
{
    public class SearchViewModel : ReactiveObject, IRoutableViewModel
    {

        private readonly ReactiveList<StockViewModel> quotes = new ReactiveList<StockViewModel>();

        public ICommand SearchCommand { get; private set; }
        

        public IReadOnlyReactiveList<StockViewModel> Quotes
        {
            get { return quotes; }
        }

        private string symbol = string.Empty;
        public string Symbol
        {
            get { return symbol; }
            set { this.RaiseAndSetIfChanged(ref symbol, value); }
        }


        public string UrlPathSegment {
            get { return "/Search"; }
        }

        public IScreen HostScreen { get; protected set; }
        public DateTime CreationDate { get; set; }

        // ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchViewModel"/> class.
        /// </summary>
        /// <param name="hostScreen">The host screen.</param>
        public SearchViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();

            CreationDate = DateTime.Now;

            SearchCommand = ReactiveCommand.CreateAsync(a => SearchQuote());
        }

        /// <summary>
        /// Searches the quote.
        /// </summary>
        /// <returns></returns>
        public async Task<Unit> SearchQuote()
        {
            this.Log().Debug("SearchQuote");

            quotes.Clear();

            if (String.IsNullOrEmpty(Symbol)) return Unit.Default;

            var result = await DataManager.Current.SearchSymbol(Symbol);   //note: await will return to the UI-Thread

            this.Log().Debug("SearchQuote => ready ({0})", result);

            quotes.Add(result);

            return Unit.Default;

        }

    }
}