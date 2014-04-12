using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace StockWatch.Advandced
{
    [Activity(Label = "RxUI StockWatch+", Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.Orientation)]
    public class SplashActivity : Activity
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            StartActivity(typeof(RootView));
        }

    }
}