using System;
using System.Net;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using TodayILearned.Utilities;
using Utilities;

namespace TodayILearned
{
    public partial class DetailsPage
    {
        private volatile bool navigating;

        public DetailsPage()
        {
            InitializeComponent();

            webBrowser1.Navigating += webBrowser1_OnNavigating;
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

        private void SetOrientation(bool locked)
        {
            this.SupportedOrientations = locked ? SupportedPageOrientation.Portrait : SupportedPageOrientation.PortraitOrLandscape;
            string text = locked ? "unlock orientation" : "lock orientation";
            ((ApplicationBarMenuItem)ApplicationBar.MenuItems[0]).Text = text;
        }

        #region Navigation

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            bool locked = AppSettings.OrientationLock;
            SetOrientation(locked);
        }

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

        private void webBrowser1_OnNavigating(object sender, NavigatingEventArgs navigatingEventArgs)
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
            if (App.ViewModel.NavigationCollection == null) return;
            if (App.ViewModel.Item == null) return;

            int index = App.ViewModel.NavigationCollection.IndexOf(App.ViewModel.Item) - 1;
            if (index >= 0 && index < App.ViewModel.NavigationCollection.Count)
            {
                App.ViewModel.Item = App.ViewModel.NavigationCollection[index];
                this.DataContext = App.ViewModel.Item;
                string decodedUri = HttpUtility.HtmlDecode(App.ViewModel.Item.Url);
                webBrowser1.Source = new Uri(decodedUri, UriKind.Absolute);
            }
        }

        private void ApplicationBarIconButton_Click_Next(object sender, EventArgs e)
        {
            if (App.ViewModel == null) return;
            if (App.ViewModel.NavigationCollection == null) return;
            if (App.ViewModel.Item == null) return;

            int index = App.ViewModel.NavigationCollection.IndexOf(App.ViewModel.Item) + 1;
            if (index >= 0 && index < App.ViewModel.NavigationCollection.Count)
            {
                App.ViewModel.Item = App.ViewModel.NavigationCollection[index];
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

        private void ApplicationBarOrientationMenuItem_OnClick(object sender, EventArgs e)
        {
            bool locked = !AppSettings.OrientationLock;
            AppSettings.OrientationLock = locked;
            SetOrientation(locked);
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

        private void ApplicationBarMenuItem_OnClick_Report(object sender, EventArgs e)
        {
            if (App.ViewModel == null) return;
            if (App.ViewModel.Items == null) return;
            if (App.ViewModel.Item == null) return;

            try
            {
                var task = new EmailComposeTask
                {
                    To = "dapper.panda@gmail.com",
                    Subject = "Trivia Buff Inaccurate Fact",
                    Body = App.ViewModel.Item.Title + "\n\n" + App.ViewModel.Item.Url
                };
                task.Show();
            }
            catch (Exception)
            {
                // fast-clicking can result in exception, so we just handle it
            }
        }
    }
}