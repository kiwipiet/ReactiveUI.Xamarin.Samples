using System;
using System.IO;
using Android.App;
using Android.Content.Res;
using Android.Runtime;
using Android.Util;
using Android.Views;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI.Android;
using ReactiveUI.Mobile;
using ReactiveUI.Android;
using Splat;
using SQLite.Net.Interop;

namespace StockWatch.Advandced
{
    [Application()]
    public class App : ReactiveUI.Android.App, IApp
    {

        const string SqliteFilename = "StockWatch_Advanced.db3";

        static App _current;
        public static App Current {
            get { return _current; }
        }

        public AppModel AppModel { get; private set; }

        public string DatabasePath
        {
            get
            {
                return Path.Combine(
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
                    SqliteFilename);
            }
        }

        public ISQLitePlatform SqlitePlatform
        {
            get { return new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid(); }
        }

        public DeviceType DeviceType { get; private set; }


        // ctor

        #region public App(IntPtr handle, JniHandleOwnership transfer) : base (handle, transfer)

        /// <summary>
        /// Initializes a new instance of the <see cref="ReactiveUI.Xamarin.Android.App"/> class.
        /// </summary>
        /// <param name="handle">The handle.</param>
        /// <param name="transfer">The transfer.</param>
        public App(IntPtr handle, JniHandleOwnership transfer)
            : base(handle, transfer)
        {
            // => OnCreate
            _current = this;
        }

        #endregion

        #region protected App()

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        protected App() : base()
        {
            // => OnCreate
            _current = this;
        } 

        #endregion

        // Methods

        #region public override void OnCreate()

        /// <summary>
        /// Called when [create].
        /// </summary>
        public override void OnCreate()
        {
            base.OnCreate();

            // Create App objects
            AppModel = new AppModel(this);

            // Detect devicetype
            bool xlarge = (Resources.Configuration.ScreenLayout & ScreenLayout.SizeMask) >= ScreenLayout.SizeLarge;

            // If XLarge, checks if the Generalized Density is at least MDPI (160dpi)
            if (xlarge)
            {
                // MDPI=160, DEFAULT=160, DENSITY_HIGH=240, DENSITY_MEDIUM=160, DENSITY_TV=213, DENSITY_XHIGH=320
                if (Resources.DisplayMetrics.DensityDpi != DisplayMetricsDensity.Low)
                    DeviceType = DeviceType.Tablet; ;
            }

            this.Log().Debug("Routing.Android.App.OnCreate() - DeviceType: {0}", DeviceType);

            
        } 

        #endregion

        #region public override void RegisterServices()

        /// <summary>
        /// Registers the services.
        /// </summary>
        public override void RegisterServices()
        {
            var resolver = Locator.CurrentMutable;

            resolver.RegisterConstant(this, typeof(IApp));
            resolver.RegisterConstant(new AndroidLogger(), typeof(ILogger));

            // Register ReactiveUI
            this.Log().Debug("App.InitializeReactiveUI()");

            // Register ReactiveUI stuff
            //(new Registrations()).Register((f, t) => resolver.Register(f, t));

            // *** need to unregister stuff (DefaultViewLocator) => do the registrations here inline
            resolver.Register(() => new INPCObservableForProperty(), typeof(ICreatesObservableForProperty));
            resolver.Register(() => new IROObservableForProperty(), typeof(ICreatesObservableForProperty));
            resolver.Register(() => new POCOObservableForProperty(), typeof(ICreatesObservableForProperty));
            resolver.Register(() => new NullDefaultPropertyBindingProvider(), typeof(IDefaultPropertyBindingProvider));
            resolver.Register(() => new EqualityTypeConverter(), typeof(IBindingTypeConverter));
            resolver.Register(() => new StringConverter(), typeof(IBindingTypeConverter));
            //resolver.Register(() => new DefaultViewLocator(), typeof(IViewLocator)); // => use MainView as ViewLocator
            resolver.Register(() => new DummySuspensionHost(), typeof(ISuspensionHost));
            resolver.Register(() => new CanActivateViewFetcher(), typeof(IActivationForViewFetcher));

            // use CustomViewLocator for reusing of Views and customizing View-Creation with IRoutingParams
            //resolver.Register(() => new CustomViewLocator(), typeof(IViewLocator));

            //(new Mobile.Registrations()).Register((f, t) => resolver.Register(f, t));

            // Register Android stuff 
            (new ReactiveUI.Android.Registrations()).Register((f, t) => resolver.Register(f, t));
        } 

        #endregion

        #region public override void RegisterViewTypes()

        /// <summary>
        /// Registers the view types.
        /// </summary>
        public override void RegisterViewTypes()
        {
            var resolver = Locator.CurrentMutable;

            resolver.Register(() => new WatchListView(), typeof(IViewFor<WatchListViewModel>));
            resolver.Register(() => new WatchListItemDetailView(), typeof(IViewFor<WatchListItemDetailViewModel>));
            resolver.Register(() => new WatchListAndDetailView(), typeof(IViewFor<WatchListAndDetailViewModel>));

            resolver.Register(() => new SearchView(), typeof(IViewFor<SearchViewModel>));
            resolver.Register(() => new ProfileView(), typeof(IViewFor<ProfileViewModel>));
        } 

        #endregion



        // App Methods

        #region public string GetFilePath(string filename, SpecialFolder location = SpecialFolder.Personal)

        /// <summary>
        /// Gets the folder path.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="location">The location.</param>
        /// <returns></returns>
        public string GetFilePath(string filename, Environment.SpecialFolder location = Environment.SpecialFolder.Personal)
        {
            string path = filename;
            switch (location)
            {
                case Environment.SpecialFolder.Personal:
                    path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), filename);
                    break;
            }
            return path;
        }

        #endregion

        #region public void Navigate(IRoutableViewModel viewModel, IRoutingParams routingParams)

        /// <summary>
        /// Navigates the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="routingParams">The routing parameters.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Navigate(IRoutableViewModel viewModel, IRoutingParams routingParams)
        {
            AppModel.Router.Navigate(viewModel, routingParams);
        } 

        #endregion

        #region public void Navigate(IRoutableViewModel viewModel, bool notInNavigationStack = false)

        /// <summary>
        /// Navigates the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="notInNavigationStack">if set to <c>true</c> [not in navigation stack].</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Navigate(IRoutableViewModel viewModel, bool notInNavigationStack = false)
        {
            var viewModelWithParams = viewModel as IRoutableViewModelWithParams;
            if (viewModelWithParams != null)
            {
                if (viewModelWithParams.RoutingParams != null)
                {
                    viewModelWithParams.RoutingParams.NotInNavigationStack = notInNavigationStack;
                }
                else
                {
                    viewModelWithParams.RoutingParams = new RoutingParams { NotInNavigationStack = notInNavigationStack };
                }
                
                AppModel.Router.Navigate(viewModelWithParams.RoutableViewModel, viewModelWithParams.RoutingParams);
            }
            else
            {
                AppModel.Router.Navigate(viewModel, notInNavigationStack);
            }
            
        } 

        #endregion

    }

}

