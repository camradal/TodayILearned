using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using TodayILearned.Utilities;
using Utilities;

namespace TodayILearned
{
    public partial class SearchPage : PhoneApplicationPage
    {
        public SearchPage()
        {
            InitializeComponent();
            this.DataContext = App.ViewModel;
            this.Loaded += (sender, args) =>
            {
                if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
                {
                    SearchTextBox.Focus();
                }
            };
        }

        // duplicate code from MainPage.xaml
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
            App.ViewModel.NavigationCollection = App.ViewModel.SearchItems;

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
                OpenDetailsPage(selectedItem.Url);
            }

            // reset selected index to null (no selection)
            listBox.SelectedItem = null;
        }

        // duplicate code from MainPage.xaml
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

        private void SearchTextBox_TextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (e.Text == "\b") return;
            this.Focus();
        }

        private void SearchTextBox_OnLostFocus(object sender, RoutedEventArgs e)
        {
            Search();
        }

        private void SearchTextBox_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Focus();
                Search();
            }
        }

        private void Search()
        {
            var searchTerm = SearchTextBox.Text;
            if (string.IsNullOrEmpty(searchTerm)) return;

            App.ViewModel.Search(searchTerm);
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
    }
}