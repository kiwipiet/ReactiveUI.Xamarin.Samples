using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using SQLite.Net.Attributes;

namespace StockWatch.Advandced
{
    public class Settings
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public bool EnableAsyncCommands { get; set; }

        public bool SlowDownServiceResponse { get; set; }

        [MaxLength(3)]
        public string Currency { get; set; }

    }
}
