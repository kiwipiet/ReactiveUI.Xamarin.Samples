using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SQLite;
using SQLite.Net.Attributes;

namespace StockWatch.Advandced
{
    public class WatchListItem
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(10)]
        public string Symbol { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        
        public decimal? DailyLow { get; set; }
        public decimal? DailyHigh { get; set; }

        public decimal? ChangeInPercent { get; set; }
        public decimal? PriceSales { get; set; }
        public decimal? PriceBook { get; set; }
        public DateTime LastUpdate { get; set; }

    }
}