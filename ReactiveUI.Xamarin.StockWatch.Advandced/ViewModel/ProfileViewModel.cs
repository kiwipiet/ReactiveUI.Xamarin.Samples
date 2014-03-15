using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Splat;

namespace ReactiveUI.Sample.Routing
{
    public class ProfileViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment {
            get { return "/Profile"; }
        }

        public IScreen HostScreen { get; protected set; }


        public DateTime CreationDate { get; set; }

        public ICommand NavigateSearchCommand { get; private set; }
        public ICommand NavigateSearchNoBackStackCommand { get; private set; }



        public ProfileViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();

            CreationDate = DateTime.Now;

            // With backstack
            NavigateSearchCommand = ReactiveCommand.Create(
                a => HostScreen.Router.Navigate(new SearchViewModel(HostScreen), false, true));

            // Without backstack
            NavigateSearchNoBackStackCommand = ReactiveCommand.Create(
                a => HostScreen.Router.Navigate(new SearchViewModel(HostScreen), true, true));
        }

    }
}