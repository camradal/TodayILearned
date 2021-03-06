﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using TodayILearned.Core;
using TodayILearned.Utilities;
using Utilities;

namespace TodayILearned
{
    public partial class MainPage : PhoneApplicationPage
    {
        private readonly BackgroundAgent backgroundAgent = new BackgroundAgent();

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            int numberOfStarts = AppSettings.NumberOfStarts;

            LoadData(numberOfStarts);
            ShowReviewPane(numberOfStarts);
            ShowBuyThisAppPane(numberOfStarts);

            DataContext = App.ViewModel;
            AppSettings.NumberOfStarts = numberOfStarts + 1;
            
            // ads
            AdBox.ErrorOccurred += AdBox_ErrorOccurred;
            AdBox.AdRefreshed += AdBox_AdRefreshed;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            bool locked = AppSettings.OrientationLock;
            SetOrientation(locked);

            if (App.FontSizeChanged)
            {
                LoadData(AppSettings.NumberOfStarts);
            }
        }

        // Load data for the ViewModel Items
        private void LoadData(int numberOfStarts)
        {
            if (numberOfStarts == 0)
            {
                GlobalLoading.Instance.LoadingText = "Please wait while we set up...";
            }

            if (!App.ViewModel.IsLoaded || App.FontSizeChanged)
            {
                if (App.FontSizeChanged)
                {
                    App.ViewModel.Items.Clear();
                }

                App.ViewModel.OnInitialized += () =>
                {
                    AdPanel.Opacity = 1;
                    
                    if (!App.IsMemoryLimited && App.FirstLoad)
                    {
                        SetUpLiveTile(numberOfStarts);
                    }

                    if (App.IsMemoryLimited)
                    {
                        ((ApplicationBarIconButton)ApplicationBar.Buttons[0]).IsEnabled = false;
                    }
                };
                if (App.ViewModel.OnError == null)
                {
                    App.ViewModel.OnError += App.HandleError;
                }
                App.ViewModel.LoadData();
                App.ViewModel.LoadFavorites();
            }
        }

        private void SetUpLiveTile(int numberOfStarts)
        {
            bool agentStarted = backgroundAgent.StartIfEnabled();
            if (agentStarted && (numberOfStarts == 0))
            {
                InitialTileSetup();
            }
            if (!agentStarted)
            {
                AppSettings.LiveTileDisabled = true;
                backgroundAgent.ResetTileToDefault();
            }
        }

        private void InitialTileSetup()
        {
            if (App.ViewModel != null && App.ViewModel.Item != null)
            {
                LiveTile.UpdateLiveTile("Trivia Buff", App.ViewModel.Item.Title);
            }
        }

        private void ShowReviewPane(int numberOfStarts)
        {
            var rate = new ReviewThisAppTask();
            rate.ShowAfterThreshold(numberOfStarts);
        }

        private void ShowBuyThisAppPane(int numberOfStarts)
        {
            var buy = new BuyThisAppTask();
            buy.ShowAfterThreshold(numberOfStarts);
        }

        private void SetOrientation(bool locked)
        {
            this.SupportedOrientations = locked ? SupportedPageOrientation.Portrait : SupportedPageOrientation.PortraitOrLandscape;
            string text = locked ? "unlock orientation" : "lock orientation";
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).Text = text;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = (LongListSelector)sender;

            // if selected index is null (no selection) do nothing
            var selectedItem = listBox.SelectedItem as ItemViewModel;
            if (selectedItem == null)
                return;

            // navigate to the new page
            var root = Application.Current.RootVisual as FrameworkElement;
            if (root == null)
                return;

            root.DataContext = selectedItem;
            App.ViewModel.Item = selectedItem;

            var item = (PivotItem)this.MainPivot.SelectedItem;
            if ((string)item.Header == "new")
                App.ViewModel.NavigationCollection = App.ViewModel.Items;
            else
                App.ViewModel.NavigationCollection = App.ViewModel.Favorites;

            if (AppSettings.BrowserSelection)
            {
                try
                {
                    var webBrowserTask = new WebBrowserTask { Uri = new Uri(App.ViewModel.Item.Url, UriKind.Absolute) };
                    webBrowserTask.Show();
                }
                catch
                {
                }
            }
            else
            {
                OpenDetailsPage();                
            }

            // reset selected index to null (no selection)
            listBox.SelectedItem = null;
        }
        
        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null) return;

            var selected = menuItem.Header as string;
            if (selected == null) return;

            var model = menuItem.DataContext as ItemViewModel;
            if (model == null) return;

            if (selected == "share...")
            {
                App.ViewModel.Item = model;

                var uri = new Uri("/SharePage.xaml", UriKind.Relative);
                Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        NavigationService.Navigate(uri);
                    }
                    catch (Exception)
                    {
                        // prevent double-click errors
                    }
                });
            }
            else if (selected == "add to favorites")
            {
                GlobalLoading.Instance.SetTimedText("Added to favorites...");
                App.ViewModel.AddFavorite(model);
                App.ViewModel.SaveFavorites();
            }
            else if (selected == "remove from favorites")
            {
                GlobalLoading.Instance.SetTimedText("Removed from favorites...");
                App.ViewModel.RemoveFavorite(model);
                App.ViewModel.SaveFavorites();
            }
        }

        private void contextMenu_Unloaded(object sender, RoutedEventArgs e)
        {
            var menu = sender as ContextMenu;
            if (menu != null)
            {
                menu.ClearValue(FrameworkElement.DataContextProperty);
            }
        }

        private void OpenDetailsPage()
        {
            var uri = new Uri("/DetailsPage.xaml", UriKind.Relative);
            Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    NavigationService.Navigate(uri);
                }
                catch (Exception)
                {
                    // prevent double-click errors
                }
            });
        }

        #region Menu

        private void ApplicationBarIconPinToStartButton_OnClick(object sender, EventArgs e)
        {
            bool agentStarted = backgroundAgent.StartIfEnabled();
            if (agentStarted)
            {
                AppSettings.LiveTileEnabled = true;

                ShellTile tile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("DefaultTitle=new"));
                if (tile != null)
                {
                    tile.Delete();
                }

                if (App.ViewModel != null && App.ViewModel.Item != null)
                {
                    LiveTileCreator.CreateLiveTile("Trivia Buff", App.ViewModel.Item.Title);
                }
            }
        }

        private void ApplicationBarIconRefreshButton_OnClick(object sender, EventArgs e)
        {
            App.ViewModel.ReloadData();
        }

        private void ApplicationBarIconSearchButton_OnClick(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/SearchPage.xaml", UriKind.Relative)));
        }

        private void ApplicationBarOrientationMenuItem_OnClick(object sender, EventArgs e)
        {
            bool locked = !AppSettings.OrientationLock;
            AppSettings.OrientationLock = locked;
            SetOrientation(locked);
        }

        private void ApplicationBarRateMenuItem_OnClick(object sender, EventArgs e)
        {
            try
            {
                var task = new MarketplaceReviewTask();
                task.Show();
            }
            catch
            {
                // prevent exceptions from double-click
            }
        }

        private void MoreAppsMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var task = new MarketplaceSearchTask();
                task.ContentType = MarketplaceContentType.Applications;
                task.SearchTerms = "Dapper Panda";
                task.Show();
            }
            catch
            {
                // prevent exceptions from double-click
            }
        }

        private void ApplicationBarSettingsMenuItem_OnClick(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/SettingsPage.xaml", UriKind.Relative)));
        }

        private void ApplicationBarAboutMenuItem_OnClick(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative)));
        }

        #endregion

        private void AllListBox_OnItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (App.ViewModel.IsLoading) return;

            var listBox = sender as LongListSelector;
            if (listBox == null) return;

            var items = listBox.ItemsSource as ObservableCollection<ItemViewModel>;
            if (items == null) return;

            var currentItem = e.Container.Content as ItemViewModel;
            if (currentItem == null) return;

            if (currentItem.Equals(App.ViewModel.Items.Last()))
            {
                App.ViewModel.LoadData(App.ViewModel.LastItem);
            }
        }

        #region Ads

        void AdBox_AdRefreshed(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                AdDuplexAdControl.Visibility = Visibility.Collapsed;
                AdBox.Visibility = Visibility.Visible;
            });
        }

        void AdBox_ErrorOccurred(object sender, Microsoft.Advertising.AdErrorEventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                AdBox.Visibility = Visibility.Collapsed;
                AdDuplexAdControl.Visibility = Visibility.Visible;
            });
        }

        #endregion

        #region Reverse Sort

        private void ApplicationBarReverseSortButton_OnClick(object sender, EventArgs e)
        {
            bool previousValue = AppSettings.ReverseSort;
            AppSettings.ReverseSort = !previousValue;

            App.ViewModel.LoadFavorites();
        }

        private void MainPivot_OnLoadingPivotItem(object sender, PivotItemEventArgs e)
        {
            string pivotName = e.Item.Header.ToString();
            if (pivotName == "favorites")
            {
                var button = new ApplicationBarIconButton
                {
                    IconUri = new Uri("/icons/appbar.sort.png", UriKind.Relative),
                    Text = "reverse sort"
                };
                button.Click += ApplicationBarReverseSortButton_OnClick;
                this.ApplicationBar.Buttons.Add(button);
            }
            else
            {
                if (this.ApplicationBar.Buttons.Count > 3)
                {
                    this.ApplicationBar.Buttons.RemoveAt(3);
                }
            }
        }

        #endregion
    }
}