using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveUI;
using Splat;

namespace StockWatch.Advandced
{

    /// <summary>
    /// The root of the App that hosts the other Views
    /// </summary>
    public class RootViewModel : ReactiveObject
    {

        public RootViewModel()
        {
            
        }

        private TabType _activeTab;
        public TabType ActiveTab
        {
            get { return _activeTab; }
            set { this.RaiseAndSetIfChanged(ref _activeTab, value); }
        }


        /// <summary>
        /// Selects the tab.
        /// </summary>
        /// <param name="activeTab">The active tab.</param>
        /// <param name="highligthOnly">if set to <c>true</c> [highligth only].</param>
        /// <exception cref="System.Exception">Unknown TabType</exception>
        public void SelectTab(TabType activeTab, bool highligthOnly = false)
        {
            ActiveTab = activeTab;

            if (highligthOnly)
            {
                this.Log().Debug("OnTabSelected (highligthOnly) => {0}", activeTab);
                return;
            }

            this.Log().Debug("OnTabSelected => {0}", activeTab);

            Tuple<IRoutableViewModel, IRoutingParams> tabViewModel = null;
            var app = Locator.CurrentMutable.GetService<IApp>();
            var viewModelPool = app.AppModel.ViewModelPool;

            //TODO: OnTabSelected-logic in RootViewModel
            if (!viewModelPool.ContainsKey(activeTab.ToString()))
            {
                this.Log().Debug("OnTabSelected => New TabViewModel {0}", activeTab);
                if (activeTab == TabType.WatchList)
                {
                    tabViewModel = Tuple.Create(new WatchListViewModel(app.AppModel) as IRoutableViewModel,
                            new CustomRoutingParams { ReuseExistingView = true } as IRoutingParams);
                    viewModelPool.Add(activeTab.ToString(), tabViewModel);
                }
                else if (activeTab == TabType.Search)
                {
                    tabViewModel = Tuple.Create(new SearchViewModel(app.AppModel) as IRoutableViewModel,
                            new CustomRoutingParams { ReuseExistingView = true } as IRoutingParams);
                    viewModelPool.Add(activeTab.ToString(), tabViewModel);
                }
                else if (activeTab == TabType.Profile)
                {
                    tabViewModel = Tuple.Create(new ProfileViewModel(app.AppModel) as IRoutableViewModel,
                            new CustomRoutingParams { ReuseExistingView = true } as IRoutingParams);
                    viewModelPool.Add(activeTab.ToString(), tabViewModel);
                }
                else
                {
                    throw new Exception("Unknown TabType");
                }
            }
            else
            {
                this.Log().Debug("OnTabSelected => Reuse TabViewModel {0}", activeTab);
                tabViewModel = viewModelPool[activeTab.ToString()];
            }

            app.Navigate(tabViewModel.Item1, tabViewModel.Item2);
        }

    }
}