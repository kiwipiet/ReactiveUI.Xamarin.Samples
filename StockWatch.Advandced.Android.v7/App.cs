using System;
using Android.App;
using Android.Runtime;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI.Android;
using ReactiveUI.Mobile;
using ReactiveUI.Android;
using Splat;

namespace StockWatch.Advandced
{
    [Application()]
    public class App : ReactiveUI.Android.App, IApp
    {
        static App _Current;
        public static App Current {
            get { return (_Current = _Current ?? new App()); }
        }

        private AppDb _appDb;

        public IAppDb AppDb
        {
            get
            {
                if (_appDb == null) _appDb = new AppDb();
                return _appDb;
            }
        }

        public AppService AppService { get; private set; }

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
            _Current = this;
        }

        #endregion

        #region protected App()

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        protected App() : base()
        {
            // => OnCreate
            _Current = this;
        } 

        #endregion

        // Methods

        #region public override void OnCreate()

        /// <summary>
        /// Called when [create].
        /// </summary>
        public override void OnCreate()
        {
            // Create App objects


            base.OnCreate();

            this.Log().Debug("Routing.Android.App.OnCreate()");

            
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
            resolver.Register(() => new IRNPCObservableForProperty(), typeof(ICreatesObservableForProperty));
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

            resolver.Register(() => new StockListView(), typeof(IViewFor<StockListViewModel>));
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



    }

}

