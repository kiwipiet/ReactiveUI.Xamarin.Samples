using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using SQLite.Net.Interop;

namespace StockWatch.Advandced
{
    public enum DeviceType : byte
    {
        Phone,
        Tablet
    }
    
    public interface IPlatform : IPlatformOperations
    {
        string DatabasePath { get; }
        ISQLitePlatform SqlitePlatform { get; }

        DeviceType DeviceType { get; }

    }
}
