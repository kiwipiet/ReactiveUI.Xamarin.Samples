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

namespace ReactiveUI.Sample.Routing
{
    public class CustomRoutingParams : RoutingParams
    {
        public bool ReuseExistingView { get; set; }

        public static CustomRoutingParams GetValueOrDefault(object param, bool reuseExistingView = true)
        {
            CustomRoutingParams result = null;
            var routeParams = param.AsRoutableViewModel<IRoutableViewModel>();
            if (routeParams != null)
            {
                result = routeParams.Item2 as CustomRoutingParams;
            }
            if (result == null)
            {
                result = new CustomRoutingParams { ReuseExistingView = reuseExistingView };
            }
            return result;
        }

    }
}