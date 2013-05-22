using System;
using System.Net;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Tasks;
using TodayILearned.Resources;
using Utilities;

namespace TodayILearned
{
    public partial class DetailsPage
    {
        private volatile bool navigating;

        public DetailsPage()
        {
            InitializeComponent();

            webBrowser1.Navigated += webBrowser1_Navigated;
            webBrowser1.LoadCompleted += webBrowser1_LoadCompleted;

            this.DataContext = App.ViewModel.Item;
            if (this.DataContext == null)
            {
                if (App.ViewModel.OnLoaded == null)
                {
                    App.ViewModel.OnLoaded += () =>
                    {
                        webBrowser1.Source = new Uri(App.ViewModel.Item.Url, UriKind.Absolute);
                    };
                }
                if (App.ViewModel.OnError == null)
                {
                    App.ViewModel.OnError += App.HandleError;
                }
            }
        }

        #region Navigation

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (navigating)
            {
                GlobalLoading.Instance.IsLoading = false;
                //GlobalLoading.Instance.LoadingText = null;
                navigating = false;
            }
        }

        #endregion

        #region Web browser

        void webBrowser1_Navigated(object sender, NavigationEventArgs e)
        {
            if (!navigating)
            {
                GlobalLoading.Instance.IsLoading = true;
                //GlobalLoading.Instance.LoadingText = Strings.Loading;
            }
            navigating = true;
        }

        private void webBrowser1_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (navigating)
            {
                GlobalLoading.Instance.IsLoading = false;
                //GlobalLoading.Instance.LoadingText = null;
            }
            navigating = false;
        }

        #endregion

        #region Handlers

        private void ApplicationBarIconButton_Click_Prev(object sender, EventArgs e)
        {
            if (App.ViewModel == null) return;
            if (App.ViewModel.Items == null) return;
            if (App.ViewModel.Item == null) return;

            int index = App.ViewModel.Items.IndexOf(App.ViewModel.Item) - 1;
            if (index >= 0 && index < App.ViewModel.Items.Count)
            {
                App.ViewModel.Item = App.ViewModel.Items[index];
                this.DataContext = App.ViewModel.Item;
                string decodedUri = HttpUtility.HtmlDecode(App.ViewModel.Item.Url);
                webBrowser1.Source = new Uri(decodedUri, UriKind.Absolute);
            }
        }

        private void ApplicationBarIconButton_Click_Next(object sender, EventArgs e)
        {
            if (App.ViewModel == null) return;
            if (App.ViewModel.Items == null) return;
            if (App.ViewModel.Item == null) return;

            int index = App.ViewModel.Items.IndexOf(App.ViewModel.Item) + 1;
            if (index >= 0 && index < App.ViewModel.Items.Count)
            {
                App.ViewModel.Item = App.ViewModel.Items[index];
                this.DataContext = App.ViewModel.Item;
                string decodedUri = HttpUtility.HtmlDecode(App.ViewModel.Item.Url);
                webBrowser1.Source = new Uri(decodedUri, UriKind.Absolute);
            }
        }

        private void ApplicationBarIconButton_Click_Share(object sender, EventArgs e)
        {
            if (App.ViewModel == null) return;
            if (App.ViewModel.Items == null) return;
            if (App.ViewModel.Item == null) return;

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

        private void ApplicationBarIconButton_Click_Favorite(object sender, EventArgs e)
        {
            if (App.ViewModel == null) return;
            
            var model = App.ViewModel.Item;
            if (model == null) return;

            GlobalLoading.Instance.SetTimedText("Added to favorites...");
            App.ViewModel.AddFavorite(model);
            App.ViewModel.SaveFavorites();
        }

        private void ApplicationBarMenuItem_OnClick_OpenInIE(object sender, EventArgs e)
        {
            if (App.ViewModel == null) return;
            if (App.ViewModel.Items == null) return;
            if (App.ViewModel.Item == null) return;

            try
            {
                var webBrowserTask = new WebBrowserTask { Uri = new Uri(App.ViewModel.Item.Url, UriKind.Absolute) };
                webBrowserTask.Show();
            }
            catch
            {
            }
        }

        #endregion
    }
}