using System;
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
            LoadData();

            AppSettings.NumberOfStarts++;
            ShowReviewPane();

            DataContext = App.ViewModel;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (App.FontSizeChanged)
            {
                LoadData();
            }
        }

        // Load data for the ViewModel Items
        private void LoadData()
        {
            int numberOfStarts = AppSettings.NumberOfStarts;

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

                App.ViewModel.OnLoaded += () =>
                {
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

        private void ShowReviewPane()
        {
            var rate = new ReviewThisAppTask();
            rate.ShowAfterThreshold();
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

            OpenDetailsPage(selectedItem.Url);

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
                ShareHelper.ShareViaSocial(model);
            }
            else if (selected == "email...")
            {
                ShareHelper.ShareViaEmail(model);
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

        private void OpenDetailsPage(string url)
        {
            string encodedUri = HttpUtility.HtmlEncode(url);
            var uri = new Uri("/DetailsPage.xaml?uri=" + encodedUri, UriKind.Relative);
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
            if (App.ViewModel.Items == null) return;
            
            var firstItem = App.ViewModel.Items.FirstOrDefault();
            if (firstItem == null) return;

            this.AllListBox.ScrollTo(firstItem);
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

        private void NewListBox_Link(object sender, LinkUnlinkEventArgs e)
        {
            if (App.ViewModel.IsLoading) return;

            var listBox = sender as LongListSelector;
            if (listBox == null) return;
            
            var items = listBox.ItemsSource as ObservableCollection<ItemViewModel>;
            if (items == null) return;

            var currentItem = e.ContentPresenter.Content as ItemViewModel;
            if (currentItem == null) return;

            if (currentItem.Equals(App.ViewModel.Items.Last()))
            {
                App.ViewModel.LoadData(App.ViewModel.LastItem);
            }
        }
    }
}