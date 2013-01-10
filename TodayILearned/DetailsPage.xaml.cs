using System;
using System.Net;
using System.Windows;
using System.Windows.Navigation;
using TodayILearned.Resources;
using Utilities;

namespace TodayILearned
{
    public partial class DetailsPage
    {
        private volatile bool navigating;
        private Uri sourceUrl;

        public DetailsPage()
        {
            InitializeComponent();

            webBrowser1.Navigated += webBrowser1_Navigated;
            webBrowser1.LoadCompleted += webBrowser1_LoadCompleted;

            this.DataContext = App.ViewModel.Item;

            // ads
            AdBox.ErrorOccurred += AdBox_ErrorOccurred;
            AdBox.AdRefreshed += AdBox_AdRefreshed;
        }

        #region Navigation

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (App.ViewModel == null) return;
            if (App.ViewModel.Item == null) return;

            string decodedUri = HttpUtility.HtmlDecode(App.ViewModel.Item.Url);
            webBrowser1.Source = new Uri(decodedUri, UriKind.Absolute);

            //else if (isNewPage && State.ContainsKey("SourceUrl"))
            //{
            //    sourceUrl = new Uri(State["SourceUrl"].ToString());
            //    webBrowser1.Source = sourceUrl;
            //}
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (navigating)
            {
                GlobalLoading.Instance.IsLoading = false;
                GlobalLoading.Instance.LoadingText = null;
                navigating = false;
            }

            if (e.NavigationMode != NavigationMode.Back)
            {
                //State["SourceUrl"] = sourceUrl;
            }
        }

        #endregion

        #region Web browser

        void webBrowser1_Navigated(object sender, NavigationEventArgs e)
        {
            if (!navigating)
            {
                GlobalLoading.Instance.IsLoading = true;
                GlobalLoading.Instance.LoadingText = Strings.Loading;
            }
            navigating = true;
        }

        private void webBrowser1_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (navigating)
            {
                GlobalLoading.Instance.IsLoading = false;
                GlobalLoading.Instance.LoadingText = null;
            }
            navigating = false;
        }

        #endregion

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
    }
}