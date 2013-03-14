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

        public DetailsPage()
        {
            InitializeComponent();

            webBrowser1.Navigated += webBrowser1_Navigated;
            webBrowser1.LoadCompleted += webBrowser1_LoadCompleted;

            this.DataContext = App.ViewModel.Item;
            if (this.DataContext == null)
            {
                App.ViewModel.OnLoaded += () =>
                {
                    webBrowser1.Source = new Uri(App.ViewModel.Item.Url, UriKind.Absolute);
                };
                App.ViewModel.OnError += exception =>
                {
                    // TODO: do something on error
                };
            }

            // ads
            AdBox.ErrorOccurred += AdBox_ErrorOccurred;
            AdBox.AdRefreshed += AdBox_AdRefreshed;
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

            ShareHelper.ShareViaSocial(App.ViewModel.Item);
        }

        private void ApplicationBarIconButton_Click_Favorite(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ApplicationBarMenuItem_OnClick_Email(object sender, EventArgs e)
        {
            if (App.ViewModel == null) return;
            if (App.ViewModel.Items == null) return;
            if (App.ViewModel.Item == null) return;

            ShareHelper.ShareViaEmail(App.ViewModel.Item);
        }

        #endregion
    }
}