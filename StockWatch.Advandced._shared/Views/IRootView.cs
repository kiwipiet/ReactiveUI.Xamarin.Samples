using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace StockWatch.Advandced
{

    public enum TabType
    {
        None = 0,
        WatchList = 1,
        Search = 2,
        Settings = 3
    }

    /// <summary>
    /// The root of the App that hosts the other Views
    /// </summary>
    public interface IRootView : IViewFor<RootViewModel>, IViewLocator
    {
        TabType ActiveTab { get; }

        void OnNavigate(Tuple<IRoutableViewModel, IRoutingParams> viewModelWithParams);

    }

}
