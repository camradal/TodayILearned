using System;
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
                    Body = model.Title + "\n\n" + model.Url
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
    }
}
