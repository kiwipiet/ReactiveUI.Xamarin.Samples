using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StockWatch.Advandced
{
    public interface IAppService
    {
    }


    // Code is based on the the sample from http://www.jarloo.com/get-yahoo-finance-api-data-via-yql/

    public class AppService : IAppService
    {

        private const string BASE_URL = "http://query.yahooapis.com/v1/public/yql?q=" +
                                        @"select%20{0}%20from%20yahoo.finance.quotes%20where%20symbol%20in%20%28{1}%29" +
                                        @"&env=store://datatables.org/alltableswithkeys";

        private const string QUOTE_FIELDS = "Symbol,Name,PriceSales,PriceBook,ChangeinPercent,DaysLow,DaysHigh";

        /// <summary>
        /// Searches the symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns></returns>
        public async Task<StockViewModel> SearchSymbol(string symbol)
        {
            //TODO: Like search
            string symbolList = String.Format("%22{0}%22", symbol);
            string url = string.Format(BASE_URL, QUOTE_FIELDS, symbolList);

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync(url).ConfigureAwait(false);
                var result = await Parse(new List<string> { symbol }, response).ConfigureAwait(false);
                if (result.Count >= 1)
                {
                    return result.First();
                }
                return null;
            }
        }

        /// <summary>
        /// Fetches the symbols.
        /// </summary>
        /// <param name="symbols">The symbols.</param>
        /// <returns></returns>
        public async Task<List<StockViewModel>> FetchSymbols(List<string> symbols)
        {
            string symbolList = String.Join("%2C", symbols.Select(w => "%22" + w + "%22").ToArray());
            string url = string.Format(BASE_URL, QUOTE_FIELDS, symbolList);

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetStringAsync(url).ConfigureAwait(false);
                return await Parse(symbols, response).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Parses the specified symbols.
        /// </summary>
        /// <param name="symbols">The symbols.</param>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        private async static Task<List<StockViewModel>> Parse(IEnumerable<string> symbols, string response)
        {
            List<StockViewModel> quotes = new List<StockViewModel>();

            await Task.Run(() =>
            {
                XDocument doc = XDocument.Parse(response);
                XElement results = doc.Root.Element("results");
                foreach (var symbol in symbols)
                {
                    XElement q =
                        results.Elements("quote").First(w => w.Element("Symbol").Value.ToLower() == symbol.ToLower());

                    var quote = new StockViewModel();

                    quote.Symbol = q.Element("Symbol").Value;

                    quote.DailyLow = GetDecimal(q.Element("DaysLow").Value);
                    quote.DailyHigh = GetDecimal(q.Element("DaysHigh").Value);
                    quote.Name = q.Element("Name").Value;
                    quote.ChangeInPercent = GetDecimal(q.Element("ChangeinPercent").Value);
                    quote.PriceSales = GetDecimal(q.Element("PriceSales").Value);
                    quote.PriceBook = GetDecimal(q.Element("PriceBook").Value);


                    quote.LastUpdate = DateTime.Now;

                    quotes.Add(quote);
                }
            }).ConfigureAwait(false);
            return quotes;
        }

        private static decimal? GetDecimal(string input)
        {
            if (input == null) return null;

            input = input.Replace("%", "");

            decimal value;

            if (Decimal.TryParse(input, out value)) return value;
            return null;
        }

        private static DateTime? GetDateTime(string input)
        {
            if (input == null) return null;

            DateTime value;

            if (DateTime.TryParse(input, out value)) return value;
            return null;
        }


    }
}