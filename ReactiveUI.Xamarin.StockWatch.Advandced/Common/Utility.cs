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
    internal static class Utility
    {
        
        #region internal static void ReleaseSubscriptions(Dictionary<string, IDisposable> subscriptions)

        /// <summary>
        /// Releases the subscriptions.
        /// </summary>
        /// <param name="subscriptions">The subscriptions.</param>
        internal static void ReleaseSubscriptions(Dictionary<string, IDisposable> subscriptions)
        {
            if (subscriptions != null)
            {
                foreach (var d in subscriptions.Values)
                {
                    d.Dispose();
                }
                subscriptions.Clear();
            }
        } 

        #endregion

        #region internal static string[] GetPathSegments(string path)

        /// <summary>
        /// Gets the path segments.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        internal static string[] GetPathSegments(string path)
        {
            if (!String.IsNullOrEmpty(path))
            {
                if (path.StartsWith("/")) path = path.TrimStart('/');
                return path.Split('/');
            }
            return null;
        } 

        #endregion

        // Extension Methods

        // *** Navigation ***

        #region internal static void Navigate(this IRoutingState router, IRoutableViewModel viewModel, IRoutingParams routingParams)

        /// <summary>
        /// Navigates the specified router.
        /// </summary>
        /// <param name="router">The router.</param>
        /// <param name="viewModel">The view model.</param>
        /// <param name="routingParams">The routing parameters.</param>
        internal static void Navigate(this IRoutingState router, IRoutableViewModel viewModel, IRoutingParams routingParams)
        {
            if (router != null)
            {
                router.Navigate.Execute(Tuple.Create(viewModel, routingParams));
            }
        } 

        #endregion

        #region internal static void Navigate(this IRoutingState router, IRoutableViewModel viewModel, bool notInNavigationStack)

        /// <summary>
        /// Navigates the specified router.
        /// </summary>
        /// <param name="router">The router.</param>
        /// <param name="viewModel">The view model.</param>
        /// <param name="notInNavigationStack">if set to <c>true</c> [not in navigation stack].</param>
        internal static void Navigate(this IRoutingState router, IRoutableViewModel viewModel, bool notInNavigationStack)
        {
            router.Navigate(viewModel,
                new RoutingParams
                {
                    NotInNavigationStack = notInNavigationStack
                });
        } 

        #endregion

        #region internal static void Navigate(this IRoutingState router, IRoutableViewModel viewModel, bool notInNavigationStack, bool reuseExistingView)

        /// <summary>
        /// Navigates the specified router.
        /// </summary>
        /// <param name="router">The router.</param>
        /// <param name="viewModel">The view model.</param>
        /// <param name="notInNavigationStack">if set to <c>true</c> [not in navigation stack].</param>
        /// <param name="reuseExistingView">if set to <c>true</c> [reuse existing view].</param>
        internal static void Navigate(this IRoutingState router, IRoutableViewModel viewModel, bool notInNavigationStack, bool reuseExistingView)
        {
            router.Navigate(viewModel,
                new CustomRoutingParams
                {
                    NotInNavigationStack = notInNavigationStack,
                    ReuseExistingView = reuseExistingView
                });
        } 

        #endregion

    }
}