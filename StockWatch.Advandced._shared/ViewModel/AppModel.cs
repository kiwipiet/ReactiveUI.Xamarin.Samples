using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ReactiveUI;
using Splat;

namespace StockWatch.Advandced
{
    /// <summary>
    /// The AppModel holds all App / Global shared logic.
    /// </summary>
    public class AppModel : IScreen, IEnableLogger
    {
        public IPlatform Platform { get; private set; }
        public DataManager DataManager { get; private set; }
        public IRootView RootView { get; set; }
        public RoutingState Router { get; private set; }

        // RegEx
        private static Regex pathPattern = new Regex(@"/(?<tab>[^/]*)(/(?<page>[^?]*)(\?(?<parameter>.*))?)?");

        private readonly Dictionary<string, IDisposable> _subscriptions = null;

        /// <summary>
        /// Gets the tab view model.
        /// </summary>
        public Dictionary<string, Tuple<IRoutableViewModel, IRoutingParams>> ViewModelPool { get; private set; }


        // ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="AppModel" /> class.
        /// </summary>
        /// <param name="platform">The platform.</param>
        public AppModel(IPlatform platform)
        {
            Platform = platform;
            DataManager = new DataManager(platform);
            Router = new RoutingState();
            ViewModelPool = new Dictionary<string, Tuple<IRoutableViewModel, IRoutingParams>>();
            _subscriptions = new Dictionary<string, IDisposable>();

            //Init(); => Call after Activity is created because RxApp.MainThreadScheduler must be set
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Init()
        {
            if (!_subscriptions.ContainsKey("NavigateSubscription"))
            {
                _subscriptions.Add("NavigateSubscription", Router.Navigate.ObserveOn(RxApp.MainThreadScheduler).Subscribe(OnNavigate));
                _subscriptions.Add("NavigateBackSubscription", Router.NavigateBackViewModel.ObserveOn(RxApp.MainThreadScheduler).Subscribe(OnNavigate));
            }
        }


        /// <summary>
        /// Handels the navigate subscription and if nessesary transforms the viewmodel depending on the device / orientation
        /// </summary>
        /// <param name="routableView">The routable view.</param>
        public void OnNavigate(object routableView)
        {
            var viewModelWithParams = routableView.AsRoutableViewModel<IRoutableViewModel>();
            if (viewModelWithParams != null)
            {
                var viewModel = viewModelWithParams.Item1;
                this.Log().Debug("OnNavigate({0}) - {1}", viewModel, Platform.GetOrientationEnum());

                // Check if Tablet and ViewMode (Portrait or Landscapemode)
                if (Platform.DeviceType == DeviceType.Tablet && Platform.GetOrientationEnum() == DeviceOrientation.Landscape)
                {
                    if (typeof(WatchListItemDetailViewModel) == viewModel.GetType())
                    {
                        // View Detail in Landscapemode (List / Detail)
                        var listModel = ViewModelPool["WatchList"].Item1 as WatchListViewModel; // we know that the model is there
                        var detailModel = viewModel as WatchListItemDetailViewModel;
                        if (ViewModelPool.ContainsKey(detailModel.UrlPathSegment))
                        {
                            detailModel = ViewModelPool[detailModel.UrlPathSegment].Item1 as WatchListItemDetailViewModel;
                        }
                        else
                        {
                            ViewModelPool.Add(detailModel.UrlPathSegment, viewModelWithParams);
                        }

                        viewModel = new WatchListAndDetailViewModel(listModel, detailModel);
                    }
                    if (typeof(WatchListViewModel) == viewModel.GetType())
                    {
                        // View Detail in Landscapemode (List / Detail)
                        var listModel = ViewModelPool["WatchList"].Item1 as WatchListViewModel; // we know that the model is there
                        WatchListItemDetailViewModel detailModel = null;
                        // Check SelectedItem
                        if (listModel.SelectedItem != null)
                        {
                            detailModel = GetDetailViewModel(listModel.SelectedItem);
                        }
                        else
                        {
                            detailModel = GetDetailViewModel(listModel.WatchList.FirstOrDefault());
                        }

                        viewModel = new WatchListAndDetailViewModel(listModel, detailModel);
                    }
                }

                // Set Tab active
                if (!String.IsNullOrEmpty(viewModel.UrlPathSegment))
                {
                    if (pathPattern.IsMatch(viewModel.UrlPathSegment))
                    {
                        var match = pathPattern.Match(viewModel.UrlPathSegment);
                        var tabType = match.Groups["tab"].Value.ToLower();
                        if (String.Compare(tabType, "WatchList", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            RootView.ViewModel.SelectTab(TabType.WatchList, true);
                        }
                        else if (String.Compare(tabType, "Search", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            RootView.ViewModel.SelectTab(TabType.Search, true);
                        }
                        else if (String.Compare(tabType, "Profile", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            RootView.ViewModel.SelectTab(TabType.Settings, true);
                        }
                    }
                }

                RootView.OnNavigate(Tuple.Create<IRoutableViewModel, IRoutingParams>(viewModel, viewModelWithParams.Item2));

            }
        }

        /// <summary>
        /// Gets the detail view model.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public WatchListItemDetailViewModel GetDetailViewModel(WatchListItemViewModel item)
        {
            if (item == null) return null;
            WatchListItemDetailViewModel detailModel = null;
            var detailViewModelPath = string.Format("/WatchList/Detail?symbol={0}", item.Symbol);
            if (ViewModelPool.ContainsKey(detailViewModelPath))
            {
                this.Log().Debug("GetDetailViewModel({0}) => from pool", item.Symbol);
                detailModel = ViewModelPool[detailViewModelPath].Item1 as WatchListItemDetailViewModel;
            }
            else
            {
                // Create new 
                this.Log().Debug("GetDetailViewModel({0}) => create new", item.Symbol);
                detailModel = new WatchListItemDetailViewModel(item);
                ViewModelPool.Add(detailViewModelPath, Tuple.Create<IRoutableViewModel, IRoutingParams>(detailModel, null));
            }
            return detailModel;
        }
    
    }
}
