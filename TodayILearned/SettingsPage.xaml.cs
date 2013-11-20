using System.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using TodayILearned.Utilities;
using Utilities;

namespace TodayILearned
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public int DisplayFontSize
        {
            get { return AppSettings.DisplayFontSize; }
            set { AppSettings.DisplayFontSize = value; }
        }

        public bool BrowserSelection
        {
            get { return AppSettings.BrowserSelection; }
            set { AppSettings.BrowserSelection = value; }
        }

        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            DataContext = this;
        }

        private void LiveTileToggle_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                GlobalLoading.Instance.IsLoading = true;
                BackgroundAgent agent = new BackgroundAgent();
                agent.Toggle();

                LiveTileToggle.IsChecked = AppSettings.LiveTileEnabled;
            }
            finally
            {
                Thread.CurrentThread.Join(250);
                GlobalLoading.Instance.IsLoading = false;
            }
        }

        private void BuyAddFreeVersionButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                var task = new MarketplaceDetailTask { ContentIdentifier = "9558e8d2-08b9-4464-9a40-5b27e25a3ced" };
                task.Show();
            }
            catch
            {
            }
        }

        private void FontSizeListPicker_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1 && e.RemovedItems.Count == 1)
            {
                App.FontSizeChanged = true;
            }
        }
    }
}
