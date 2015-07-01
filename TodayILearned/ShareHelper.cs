using System;
using System.Windows;
using Microsoft.Phone.Tasks;

namespace TodayILearned
{
    static internal class ShareHelper
    {
        internal static void ShareViaEmail(ItemViewModel model)
        {
            try
            {
                var task = new EmailComposeTask
                {
                    Subject = model.Title,
                    Body = model.Title + "\n\n" + model.Url + "\n\n" + "Shared via Trivia Buff\nhttp://www.triviabuffapp.com"
                };
                task.Show();
            }
            catch (Exception)
            {
                // fast-clicking can result in exception, so we just handle it
            }
        }

        internal static void ShareViaSocial(ItemViewModel model)
        {
            try
            {
                var task = new ShareLinkTask()
                {
                    Title = model.Title,
                    Message = model.Title,
                    LinkUri = new Uri(model.Url, UriKind.Absolute)
                };
                task.Show();
            }
            catch (Exception)
            {
                // fast-clicking can result in exception, so we just handle it
            }
        }

        internal static void ShareViaSms(ItemViewModel model)
        {
            try
            {
                var task = new SmsComposeTask()
                {
                    Body = model.Title + "\n" + model.Url
                };
                task.Show();
            }
            catch (Exception)
            {
                // fast-clicking can result in exception, so we just handle it
            }
        }

        internal static void ShareViaClipBoard(ItemViewModel model)
        {
            string text = model.Title + "\n" + model.Url;
            if (MessageBox.Show(text, "Copy to Clipboard?", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                Clipboard.SetText(text);
            }
        }
    }
}
