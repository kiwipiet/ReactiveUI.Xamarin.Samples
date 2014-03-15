using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Splat;

namespace ReactiveUI.Sample.Routing
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