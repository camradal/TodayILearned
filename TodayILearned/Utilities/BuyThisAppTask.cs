using System.Windows;
using Microsoft.Phone.Tasks;
using TodayILearned.Resources;
using TodayILearned.Utilities;

namespace Utilities
{
    public sealed class BuyThisAppTask
    {
        private const int numberOfStartsThreshold = 9;

        public void ShowAfterThreshold()
        {
            int starts = AppSettings.NumberOfStarts;
            if ((starts == numberOfStartsThreshold) && GetMessageBoxResult() == MessageBoxResult.OK)
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
        }

        private MessageBoxResult GetMessageBoxResult()
        {
            return MessageBox.Show(
                Strings.MessageBoxBuyThisAppSummary,
                Strings.MessageBoxBuyThisAppTitle,
                MessageBoxButton.OKCancel);
        }
    }
}