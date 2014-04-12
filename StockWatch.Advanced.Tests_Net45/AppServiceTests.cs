using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockWatch.Advandced;
using Xunit;

namespace StockWatch.Advanced.Tests_Net45
{
    public class AppServiceTests
    {

        // The tests are used as an integration test and will fail if the remote service (Yahoo) is not available

        [Fact]
        public void FetchSingleSymbol()
        {

            var fixure = AppService.Fetch("MSFT");
            fixure.Wait();

            var quote = fixure.Result;

            Assert.NotNull(quote);

            Assert.Equal("MSFT", quote.Symbol);

            
        }

        [Fact]
        public void FetchMultipleSymbols()
        {

            var fixure = AppService.Fetch(new List<string>{"MSFT","AAPL"});
            fixure.Wait();

            var quote = fixure.Result;

            Assert.NotNull(quote);

            Assert.True(quote.Count>=2);

            Assert.NotNull(quote.SingleOrDefault(q => q.Symbol == "MSFT"));
            Assert.NotNull(quote.SingleOrDefault(q => q.Symbol == "AAPL"));

        }

    }
}
