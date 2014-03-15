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
using SupportFragment = Android.Support.V4.App.Fragment;

namespace ReactiveUI.Sample.Routing
{
    public class StateHolderFragment : SupportFragment, IEnableLogger
    {
        private ViewStateBag _viewState = new ViewStateBag();

        /// <summary>
        /// Gets the state of the view.
        /// </summary>
        /// <value>
        /// The state of the view.
        /// </value>
        public ViewStateBag ViewState
        {
            get { return _viewState; }
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            RetainInstance = true;

            this.Log().Debug(string.Format("{0} (StateHolderFragmentBase) => OnActivityCreated", this.GetType().Name));

        }
    }
}