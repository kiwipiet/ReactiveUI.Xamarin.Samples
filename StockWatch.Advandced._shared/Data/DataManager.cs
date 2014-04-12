using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockWatch.Advandced
{
    /// <summary>
    /// Decoupling the data access methods (e.g. for unit testing). 
    /// </summary>
    public class DataManager
    {
        static DataManager _current;
        public static DataManager Current
        {
            get { return _current; }
        }

        private readonly IPlatform _platform;

        public AppService AppService { get; private set; }

        private AppDb _appDb;

        /// <summary>
        /// Gets the conference database.
        /// </summary>
        /// <value>
        /// The conference database.
        /// </value>
        public AppDb AppDb
        {
            get
            {
                return _appDb ??
                (
                    _appDb = new AppDb(() =>
                        new SQLite.Net.SQLiteConnectionWithLock(
                            _platform.SqlitePlatform,
                            new SQLite.Net.SQLiteConnectionString(_platform.DatabasePath, true)))
                );
            }
        } 


        // ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="DataManager"/> class.
        /// </summary>
        /// <param name="platform">The platform.</param>
        public DataManager(IPlatform platform)
        {
            _platform = platform;
            AppService = new AppService();

            _current = this;
        }

        // methods

        /// <summary>
        /// Searches the symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns></returns>
        public async Task<StockViewModel> SearchSymbol(string symbol)
        {
            return await AppService.SearchSymbol(symbol).ConfigureAwait(false);
        }

        /// <summary>
        /// Fetches the symbols.
        /// </summary>
        /// <param name="symbols">The symbols.</param>
        /// <returns></returns>
        public async Task<List<StockViewModel>> FetchSymbols(List<string> symbols)
        {
            return await AppService.FetchSymbols(symbols).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the watch list.
        /// </summary>
        /// <returns></returns>
        public async Task<List<WatchListItem>> GetWatchList()
        {
            return await AppDb.Table<WatchListItem>().ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Adds the stock.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public async Task<int> AddStock(WatchListItem item)
        {
            return await AppDb.SaveWatchListItem(item).ConfigureAwait(false);
        }
    }
}
