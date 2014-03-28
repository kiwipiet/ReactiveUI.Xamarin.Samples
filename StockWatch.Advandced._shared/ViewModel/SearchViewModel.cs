using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveUI;
using Splat;

namespace StockWatch.Advandced
{
    public class SearchViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment {
            get { return "/Search"; }
        }

        public IScreen HostScreen { get; protected set; }

        public DateTime CreationDate { get; set; }

        public SearchViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();

            CreationDate = DateTime.Now;
        }
    }
}