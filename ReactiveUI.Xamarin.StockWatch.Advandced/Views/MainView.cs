using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ReactiveUI.Android;
using Splat;
using SupportActionBar = Android.Support.V7.App.ActionBar;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using SupportFragment = Android.Support.V4.App.Fragment;
using Tab = Android.Support.V7.App.ActionBar.Tab;


namespace ReactiveUI.Sample.Routing
{
    internal enum TabType
    {
        StockList = 0,
        Search = 1,
        Profile = 2
    }

    public interface IMainView : IScreen, IViewLocator, IEnableLogger
    {
        
    }

    [Activity(Label = "RxUI StockWatch+", MainLauncher = true, Icon = "@drawable/icon", ConfigurationChanges = ConfigChanges.Orientation)]
    public class MainView : ReactiveActionBarActivity<MainViewModel>, IMainView, SupportActionBar.ITabListener
    {
        
        // Fields

        private StateHolderFragment _stateHolderFragment;
        private const string _stateHolderFragmentTag = "Main_stateHolderFragment";
        private const string _routingStateKey = "RoutingState";

        private Dictionary<string, IDisposable> _subscriptions = null;
        private Dictionary<string, int> _tabIndicies = null;

        private string _currentFragmentTag = null;
        private bool _suppressTabSelected = false;

        public static int THEME = Resource.Style.Theme_AppCompat_Light;

        // Properties

        #region public RoutingState Router {get;}

        /// <summary>
        /// The Router associated with this Screen.
        /// </summary>
        public RoutingState Router
        {
            get
            {
                if (_stateHolderFragment != null){
                    var routingState = _stateHolderFragment.ViewState.Get<RoutingState>(_routingStateKey);
                    if (routingState == null){
                        routingState = new RoutingState();
                        _stateHolderFragment.ViewState.Add(_routingStateKey, routingState);
                    }
                    return routingState;
                }
                return null;
            }
        }

        #endregion

        #region internal Dictionary<string, Tuple<IRoutableViewModel, IRoutingParams>> TabViewModel

        /// <summary>
        /// Gets the tab view model.
        /// </summary>
        /// <value>
        /// The tab view model.
        /// </value>
        internal Dictionary<string, Tuple<IRoutableViewModel, IRoutingParams>> TabViewModel
        {
            get
            {
                if (_stateHolderFragment != null)
                {
                    var tabViewModel = _stateHolderFragment.ViewState.Get<Dictionary<string, Tuple<IRoutableViewModel, IRoutingParams>>>("TabViewModel");
                    if (tabViewModel == null)
                    {
                        tabViewModel = new Dictionary<string, Tuple<IRoutableViewModel, IRoutingParams>>();
                        _stateHolderFragment.ViewState.Add("TabViewModel", tabViewModel);
                    }
                    return tabViewModel;
                }
                return null;
            }
        } 

        #endregion

        

        // Methods

        #region protected override void OnCreate(Bundle savedInstanceState)

        /// <summary>
        /// OnCreate
        /// </summary>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetTheme(THEME);

            base.OnCreate(savedInstanceState);
            

            // register as ViewLocator
            Locator.CurrentMutable.RegisterConstant(this, typeof(IViewLocator));

            this.Log().Debug("Main => OnCreate");

            SupportActionBar.SetDisplayShowHomeEnabled(false);
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            int selectedTabIndex = 0;

            if (savedInstanceState != null)
            {
                selectedTabIndex = savedInstanceState.GetInt("tab", 0);
                _currentFragmentTag = savedInstanceState.GetString("currentFragmentTag");
                this.Log().Debug("Main => OnCreate => bundle != null => currentFragmentTag: '{0}'", _currentFragmentTag);
                this.Log().Debug("Main => OnCreate => bundle != null => selectedTabIndex: {0}", selectedTabIndex);
            }

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.MainView);

            EnsureStateFragmentExists();

            SupportActionBar.NavigationMode = SupportActionBar.NavigationModeTabs;

            AddTab("StockList", "", icon: Resource.Drawable.ic_list);
            AddTab("Search", "", icon: Resource.Drawable.ic_magnify);
            AddTab("Profile", "", icon: Resource.Drawable.ic_profile);

            if (_subscriptions == null) _subscriptions = new Dictionary<string, IDisposable>();

            if (!_subscriptions.ContainsKey("NavigateSubscription"))
            {
                _subscriptions.Add("NavigateSubscription", Router.Navigate.Subscribe(OnNavigate));
                _subscriptions.Add("NavigateBackSubscription", Router.NavigateBackViewModel.Subscribe(OnNavigate));
            }
            

            // set ViewModel
            ViewModel = new MainViewModel();

            SupportActionBar.SetSelectedNavigationItem(selectedTabIndex);

            
        } 

        #endregion

        #region private void EnsureStateFragmentExists()

        /// <summary>
        /// Ensures the state fragment exists.
        /// </summary>
        private void EnsureStateFragmentExists()
        {
            _stateHolderFragment = SupportFragmentManager.FindFragmentByTag(_stateHolderFragmentTag) as StateHolderFragment;
            if (_stateHolderFragment == null)
            {
                _stateHolderFragment = new StateHolderFragment();
                var tx = SupportFragmentManager.BeginTransaction();
                tx.Add(_stateHolderFragment, _stateHolderFragmentTag);
                tx.Commit();
            }
        } 

        #endregion

        #region private void AddTab(string tag, string text, int? icon = null)

        /// <summary>
        /// Adds the tab.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="text">The text.</param>
        /// <param name="icon">The icon.</param>
        private void AddTab(string tag, string text, int? icon = null)
        {
            var tab = SupportActionBar.NewTab();
            tab.SetText(text);
            if (icon.HasValue)
            {
                tab.SetIcon(icon.GetValueOrDefault());
            }
            
            tab.SetTabListener(this);
            tab.SetTag(tag);
            SupportActionBar.AddTab(tab, false);

            _tabIndicies = _tabIndicies ?? new Dictionary<string, int>();
            _tabIndicies.Add(tag, _tabIndicies.Count);
        }

        #endregion

        // Activity lifecycle

        #region public override bool OnCreateOptionsMenu(IMenu menu)

        /// <summary>
        /// Called when [create options menu].
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <returns></returns>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {

            //menu.Add(1, 1, 0, "Hinzufügen")
            //    .SetIcon(Resource.Drawable.ic_add)
            //    .SetShowAsAction(MenuItem.ShowAsActionIfRoom);

            //menu.Add(1, 2, 0, "Bearbeiten")
            //    .SetIcon(Resource.Drawable.ic_edit)
            //    .SetShowAsAction(MenuItem.ShowAsActionIfRoom | MenuItem.ShowAsActionWithText);

            return true;
        }

        #endregion

        #region public override bool OnOptionsItemSelected(IMenuItem item)

        /// <summary>
        /// Called when [options item selected].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            //switch (item.ItemId)
            //{
            //    case 1:
            //        new Dialog_AddObject().Show(SupportFragmentManager, "Current_Dialog_AddObject");
            //        return true;
            //    case 2:
            //        new AlertDialog.Builder(this).SetMessage("Bearbeiten").Show();
            //        return true;
            //}
            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region protected override void OnSaveInstanceState(Bundle outState)

        /// <summary>
        /// Called when [save instance state].
        /// </summary>
        /// <param name="outState">State of the out.</param>
        protected override void OnSaveInstanceState(Bundle outState)
        {
            this.Log().Debug("OnSaveInstanceState => {0}", Convert.ToString(outState));

            // 
            Utility.ReleaseSubscriptions(_subscriptions);

            outState.PutInt("tab", SupportActionBar.SelectedNavigationIndex);
            outState.PutString("currentFragmentTag", _currentFragmentTag);

            base.OnSaveInstanceState(outState);
            

            this.Log().Debug("OnSaveInstanceState => currentFragmentTag '{0}'", _currentFragmentTag);
        }

        #endregion

        #region public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)

        /// <summary>
        /// Called when [configuration changed].
        /// </summary>
        /// <param name="newConfig">The new configuration.</param>
        public override void OnConfigurationChanged(global::Android.Content.Res.Configuration newConfig)
        {
            this.Log().Debug("OnConfigurationChanged => {0}", Convert.ToString(newConfig));
            base.OnConfigurationChanged(newConfig);

            //if (newConfig.Orientation ==
            //        Android.Content.Res.Orientation.Portrait)
            //{
            //    _tv.LayoutParameters = _layoutParamsPortrait;
            //    _tv.Text = "Changed to portrait";
            //}
            //else if (newConfig.Orientation ==
            //      Android.Content.Res.Orientation.Landscape)
            //{
            //    _tv.LayoutParameters = _layoutParamsLandscape;
            //    _tv.Text = "Changed to landscape";
            //}
        }

        #endregion

        #region protected override void OnResume()

        /// <summary>
        /// Called when [resume].
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();

            if (!_subscriptions.ContainsKey("NavigateSubscription"))
            {
                _subscriptions.Add("NavigateSubscription", Router.Navigate.Subscribe(OnNavigate));
                _subscriptions.Add("NavigateBackSubscription", Router.NavigateBackViewModel.Subscribe(OnNavigate));
            }
        }

        #endregion

        #region protected override void OnPause()

        /// <summary>
        /// Called when [pause].
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();

            //var tab = SupportActionBar.SelectedTab;
            //Stack<String> backStack = _backStacks[(TabType)Convert.ToInt32(tab.Tag)];
            //if (backStack.Count > 0)
            //{
            //    // Detach topmost fragment otherwise it will not be correctly displayed
            //    // after orientation change
            //    String tag = backStack.Peek();
            //    FragmentTransaction ft = SupportFragmentManager.BeginTransaction();
            //    var fragment = SupportFragmentManager.FindFragmentByTag(tag);
            //    ft.Detach(fragment);
            //    ft.Commit();
            //}
        }

        #endregion

        #region public override void OnBackPressed()

        /// <summary>
        /// Called when [back pressed].
        /// </summary>
        public override void OnBackPressed()
        {
            this.Log().Debug("Main => OnBackPressed");
            Router.NavigateBack.Execute(null);
        }

        #endregion

        // Routing

        #region private void OnNavigate(object routableView)

        /// <summary>
        /// Called when [navigate].
        /// </summary>
        /// <param name="routableView">The routable view.</param>
        private void OnNavigate(object routableView)
        {
            this.Log().Debug("OnNavigate => {0} (StackCount: {1}; CurrentFragmentTag: {2})", Convert.ToString(routableView), Router.NavigationStack.Count, _currentFragmentTag);

            var viewModelWithParams = routableView.AsRoutableViewModel<IRoutableViewModel>();
            if (viewModelWithParams != null)
            {

                var urlPathSegment = viewModelWithParams.Item1.UrlPathSegment;
                var pathSegments = Utility.GetPathSegments(urlPathSegment);

                // Reusable View?
                var customRouteParams = CustomRoutingParams.GetValueOrDefault(viewModelWithParams.Item2);

                var fragment = SupportFragmentManager.FindFragmentByTag(urlPathSegment);
                IViewFor view = null;
                if (fragment == null || !customRouteParams.ReuseExistingView)
                {
                    var viewType = typeof(IViewFor<>);
                    this.Log().Debug("OnNavigate => Fragment not found for '{0}'. Create new", viewModelWithParams.Item1.GetType().Name);
                    view = AttemptToResolveView(viewType.MakeGenericType(viewModelWithParams.Item1.GetType()), customRouteParams.Contract);
                }
                else
                {
                    this.Log().Debug("OnNavigate => Fragment found for '{0}'!!", viewModelWithParams.Item1.GetType().Name);
                    view = fragment as IViewFor;
                }

                //this.Log().Debug(string.Format("OnNavigate => view: {0}", Convert.ToString(view)));


                
                this.Log().Debug("OnNavigate => {0}", urlPathSegment);
                if (view != null)
                {
                    var ft = SupportFragmentManager.BeginTransaction();

                    // Set ViewModel
                    if (view.ViewModel != viewModelWithParams.Item1)
                    {
                        view.ViewModel = viewModelWithParams.Item1;
                    }
                    else
                    {
                        this.Log().Debug("OnNavigate => ViewModel unchanged");
                    }

                    //var current = ((ViewGroup)((ViewGroup)FindViewById(global::Android.Resource.Id.Content)).GetChildAt(0));
                    //if (current != null)
                    //{
                    //    this.Log().Debug("OnNavigate => current Content: {0} ({1})", current, current.ChildCount);
                    //}

                    // Detach current fragment if different
                    if (!String.IsNullOrEmpty(_currentFragmentTag) && _currentFragmentTag != urlPathSegment)
                    {
                        var currentfragment = SupportFragmentManager.FindFragmentByTag(_currentFragmentTag);
                        if (currentfragment != null && !currentfragment.IsDetached)
                        {
                            this.Log().Debug("OnNavigate => Detach: {0}", _currentFragmentTag);
                            ft.Detach(currentfragment);
                        }
                    }

                    // Attach
                    var newFragment = view as SupportFragment;
                    if (newFragment != null)
                    {
                        if (!newFragment.IsAdded)
                        {
                            this.Log().Debug("OnNavigate => Add: {0}", urlPathSegment);
                            ft.Add(global::Android.Resource.Id.Content, newFragment, urlPathSegment);
                        }
                        if (newFragment.IsDetached)
                        {
                            this.Log().Debug("OnNavigate => Attach: {0}", urlPathSegment);
                            ft.Attach(newFragment);
                        }
                    }
                    ft.Commit();

                    // Change Tab?
                    if (!String.IsNullOrEmpty(_currentFragmentTag) && _currentFragmentTag != urlPathSegment)
                    {
                        var currentpathSegments = Utility.GetPathSegments(_currentFragmentTag);
                        if (pathSegments != null && currentpathSegments != null && pathSegments[0] != currentpathSegments[0])
                        {
                            // FindTabIndex
                            if (_tabIndicies.ContainsKey(pathSegments[0]))
                            {
                                var newTabIndex = _tabIndicies[pathSegments[0]];
                                if (SupportActionBar.SelectedNavigationIndex != newTabIndex)
                                {
                                    _suppressTabSelected = true;
                                    SupportActionBar.SetSelectedNavigationItem(newTabIndex);
                                }
                            }
                        }
                    }

                    // Set currentFragmentTag
                    _currentFragmentTag = urlPathSegment;

                }
            }
        } 

        #endregion



        // IViewLocator

        #region public IViewFor ResolveView<T>(T viewModel, string contract = null) where T : class

        /// <summary>
        /// Resolves the view.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="viewModel">The view model.</param>
        /// <param name="contract">The contract.</param>
        /// <returns></returns>
        public IViewFor ResolveView<T>(T viewModel, string contract = null) where T : class
        {
            if (viewModel == null) return null;

            this.Log().Debug("ResolveView => '{0}' (contract: '{1}')", viewModel, contract);

            // Check if viewModel is IRoutableViewModelWithParams
            var viewModelWithParams = viewModel.AsRoutableViewModel<T>();
            var viewModelClass = viewModelWithParams.Item1;
            var routableViewModel = viewModelWithParams.Item1 as IRoutableViewModel;

            // Reusable View?
            var customRouteParams = viewModelWithParams.Item2 as CustomRoutingParams;
            bool reuseExistingView = false;
            if (customRouteParams != null)
            {
                reuseExistingView = customRouteParams.ReuseExistingView;
            }

            var viewType = typeof(IViewFor<>);
            IViewFor view = null;
            if (!reuseExistingView)
            {
                this.Log().Debug("ViewLocator => create new View '{0}'", viewModelClass.GetType().Name);
                view = AttemptToResolveView(viewType.MakeGenericType(viewModelClass.GetType()), contract);
            }
            else
            {
                //TODO: find existing fragment or create new
                this.Log().Debug("ViewLocator => reuse View '{0}'", viewModelClass.GetType().Name);

                string tag = viewModelClass.GetType().AssemblyQualifiedName;
                if (routableViewModel != null)
                {
                    tag = routableViewModel.UrlPathSegment;
                }
                var fragment = SupportFragmentManager.FindFragmentByTag(tag);
                if (fragment == null)
                {
                    this.Log().Debug("ViewLocator => Fragment not found for '{0}'. Create new", viewModelClass.GetType().Name);
                    view = AttemptToResolveView(viewType.MakeGenericType(viewModelClass.GetType()), contract);
                }
                else
                {
                    this.Log().Debug("ViewLocator => Fragment found for '{0}'!!", viewModelClass.GetType().Name);
                    view = fragment as IViewFor;
                }
            }

            if (view != null)
            {
                view.ViewModel = viewModelClass;
            }

            return view;
        } 

        #endregion

        #region IViewFor attemptToResolveView(Type type, string contract)

        /// <summary>
        /// Attempts to resolve view.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="contract">The contract.</param>
        /// <returns></returns>
        IViewFor AttemptToResolveView(Type type, string contract)
        {
            if (type == null) return null;

            this.Log().Debug("ViewLocator => attemptToResolveView type '{0}', contract '{1}'", type.Name, contract);

            var ret = default(IViewFor);

            try
            {
                ret = (IViewFor)Locator.Current.GetService(type, contract);
            }
            catch (Exception ex)
            {
                this.Log().ErrorException("Failed to instantiate view: " + type.FullName, ex);
                throw;
            }

            return ret;
        } 

        #endregion


        // ITabListener

        #region public void OnTabSelected(Tab tab, FragmentTransaction ft)

        /// <summary>
        /// Called when [tab selected].
        /// </summary>
        /// <param name="tab">The tab.</param>
        /// <param name="ft">The ft.</param>
        /// <exception cref="Java.Lang.IllegalArgumentException">Unknown TabType</exception>
        public void OnTabSelected(Tab tab, FragmentTransaction ft)
        {
            if (_suppressTabSelected)
            {
                this.Log().Debug("Main => OnTabSelected (suppressed) => {0}", tab.Tag);
                _suppressTabSelected = false;
                return;
            }

            this.Log().Debug("Main => OnTabSelected => {0}", tab.Tag);

            Tuple<IRoutableViewModel, IRoutingParams> tabViewModel = null;
            var tabType = Convert.ToString(tab.Tag);
            if (!String.IsNullOrEmpty(tabType) && !TabViewModel.ContainsKey(tabType))
            {
                this.Log().Debug("Main => OnTabSelected => New TabViewModel {0}", tab.Tag);
                if (tabType == "StockList")
                {
                    tabViewModel = Tuple.Create(new StockListViewModel(this) as IRoutableViewModel,
                            new CustomRoutingParams { ReuseExistingView = true } as IRoutingParams);
                    TabViewModel.Add(tabType, tabViewModel);
                }
                else if (tabType == "Search")
                {
                    tabViewModel = Tuple.Create(new SearchViewModel(this) as IRoutableViewModel,
                            new CustomRoutingParams { ReuseExistingView = true } as IRoutingParams);
                    TabViewModel.Add(tabType, tabViewModel);
                }
                else if (tabType == "Profile")
                {
                    tabViewModel = Tuple.Create(new ProfileViewModel(this) as IRoutableViewModel,
                            new CustomRoutingParams { ReuseExistingView = true } as IRoutingParams);
                    TabViewModel.Add(tabType, tabViewModel);
                }
                else
                {
                    throw new Exception("Unknown TabType");
                }
            }
            else
            {
                this.Log().Debug("Main => OnTabSelected => Reuse TabViewModel {0}", tabType);
                tabViewModel = TabViewModel[tabType];
            }

            this.Router.ExcecuteNavigateWithParams(tabViewModel);
            
        }

        #endregion

        #region public void OnTabReselected(Tab tab, FragmentTransaction ft)

        /// <summary>
        /// Called when [tab reselected].
        /// </summary>
        /// <param name="tab">The tab.</param>
        /// <param name="ft">The ft.</param>
        public void OnTabReselected(Tab tab, FragmentTransaction ft)
        {

            this.Log().Debug("Main => OnTabReselected => {0}", tab.Tag);

            //// Select proper stack
            //Stack<String> backStack = _backStacks[(TabType)Convert.ToInt32(tab.Tag)];

            //if (backStack.Count > 1)
            //    ft.SetCustomAnimations(Android.Resource.Animation.SlideOutRight, Android.Resource.Animation.SlideInLeft);
            //// Clean the stack leaving only initial fragment
            //while (backStack.Count > 1)
            //{
            //    // Pop topmost fragment
            //    String tag = backStack.Pop();
            //    var fragment = SupportFragmentManager.FindFragmentByTag(tag);
            //    // Remove it
            //    ft.Remove(fragment);
            //}
            //ShowFragment(backStack, ft);
        }

        #endregion

        #region public void OnTabUnselected(Tab tab, FragmentTransaction ft)

        /// <summary>
        /// Called when [tab unselected].
        /// </summary>
        /// <param name="tab">The tab.</param>
        /// <param name="ft">The ft.</param>
        public void OnTabUnselected(Tab tab, FragmentTransaction ft)
        {

            this.Log().Debug("Main => OnTabUnselected => {0}", tab.Tag);

            //// Select proper stack
            //Stack<String> backStack = _backStacks[(TabType)Convert.ToInt32(tab.Tag)];
            //// Get topmost fragment
            //String tag = backStack.Peek();
            //var fragment = SupportFragmentManager.FindFragmentByTag(tag);
            //// Detach it
            //ft.Detach(fragment);

            //if (TabUnselected != null)
            //{
            //    TabUnselected(fragment, null);
            //}
            //if (MainViewChanged != null)
            //{
            //    MainViewChanged(this, new MainViewChangedEventArgs());
            //}
        }

        #endregion

        // Fragment handling

        #region internal void AddFragment(SupportFragment fragment)

        /// <summary>
        /// Adds the fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        internal void AddFragment(SupportFragment fragment)
        {

            this.Log().Debug("Main => AddFragment => {0}", fragment.GetType());

            // Select proper stack
            var tab = SupportActionBar.SelectedTab;
            //Stack<String> backStack = _backStacks[(TabType)Convert.ToInt32(tab.Tag)];

            //FragmentTransaction ft = SupportFragmentManager.BeginTransaction();
            //// Animate transfer to new fragment
            ////ft.SetCustomAnimations(Android.Resource.Animation.SlideOutRight, Android.Resource.Animation.SlideOutRight);
            //// Get topmost fragment
            //String tag = backStack.Peek();
            //var top = SupportFragmentManager.FindFragmentByTag(tag);
            //ft.Detach(top);
            //// Add new fragment
            //AddFragment(fragment, backStack, ft);
            //ft.Commit();
        }

        #endregion

        #region internal void AddFragment(SupportFragment fragment, Stack<String> backStack, FragmentTransaction ft)

        /// <summary>
        /// Adds the fragment.
        /// </summary>
        /// <param name="fragment">The fragment.</param>
        /// <param name="backStack">The back stack.</param>
        /// <param name="ft">The ft.</param>
        internal void AddFragment(SupportFragment fragment, Stack<String> backStack, FragmentTransaction ft)
        {

            this.Log().Debug("Main => AddFragment with backStack => {0}", fragment.GetType());

            // Add fragment to back stack with unique tag
            String tag = Guid.NewGuid().ToString();
            ft.Add(global::Android.Resource.Id.Content, fragment, tag);
            backStack.Push(tag);

            this.Log().Debug("Main => AddFragment with backStack => {0} READY!!", fragment.GetType());
        }

        #endregion

        #region internal void ShowFragment(Stack<String> backStack, FragmentTransaction ft)

        /// <summary>
        /// Shows the fragment.
        /// </summary>
        /// <param name="backStack">The back stack.</param>
        /// <param name="ft">The ft.</param>
        internal void ShowFragment(Stack<String> backStack, FragmentTransaction ft)
        {

            this.Log().Debug("Main => ShowFragment...");

            // Peek topmost fragment from the stack
            String tag = backStack.Peek();
            var fragment = SupportFragmentManager.FindFragmentByTag(tag);

            this.Log().Debug("Main => ShowFragment => {0}", fragment.GetType());

            // and attach it
            ft.Attach(fragment);
        }

        #endregion


    }
}