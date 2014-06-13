﻿using Microsoft.Phone.Tasks;
using System.Windows;
using TodayILearned.Resources;

namespace TodayILearned.Utilities
{
    public sealed class ReviewThisAppTask
    {
        private const int numberOfStartsThreshold = 5;
        private const int numberOfStartsThreshold2 = 50;

        public void ShowAfterThreshold(int starts)
        {
            if ((starts == numberOfStartsThreshold || starts == numberOfStartsThreshold2) &&
                GetMessageBoxResult() == MessageBoxResult.OK)
            {
                try
                {
                    var task = new MarketplaceReviewTask();
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
                Strings.MessageBoxRateThisAppSummary,
                Strings.MessageBoxRateThisAppTitle,
                MessageBoxButton.OKCancel);
        }
    }
}
