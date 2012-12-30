using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Utilities;

namespace TodayILearned
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!App.ViewModel.IsLoaded)
            {
                GlobalLoading.Instance.IsLoading = true;
                App.ViewModel.OnLoaded += () => GlobalLoading.Instance.IsLoading = false;
                App.ViewModel.LoadData();
                App.ViewModel.LoadFavorites();
            }
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
            }
            else if (selected == "email...")
            {
            }
            else if (selected == "add to favorites")
            {
                App.ViewModel.AddFavorite(model);
                App.ViewModel.SaveFavorites();
            }
            else if (selected == "remove from favorites")
            {
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

        private void ApplicationBarAboutMenuItem_OnClick(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.Relative)));
        }

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