using System.Collections.Generic;
using System.Windows;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using TodayILearned.Resources;

namespace TodayILearned
{
    public class NewItem
    {
        public string Version { get; set; }
        public string Description { get; set; }
    }

    public partial class AboutPage : PhoneApplicationPage
    {
        public List<NewItem> NewItems
        {
            get
            {
                return new List<NewItem>()
                {
                    new NewItem
                    {
                        Version = "",
                        Description = "We read all emails, please drop us a line with any suggestions."
                    },
                    new NewItem
                    {
                        Version = "1.1",
                        Description =
                            "- Option to show only the front of the live tile\n" +
                            "- Option to buy an ad-free version inside the app\n" +
                            "- Open articles in Internet Explorer\n" +
                            "- More facts on the live tile\n" +
                            "- Fix reported bugs"

                    },
                    new NewItem
                    {
                        Version = "1.0",
                        Description =
                            "- Initial release"
                    }
                };
            }
        }

        public AboutPage()
        {
            InitializeComponent();
            ReadVersionFromManifest();
            DataContext = this;
        }

        private void feedbackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EmailComposeTask task = new EmailComposeTask();
                task.Subject = Strings.FeedbackOn;
                task.Body = Strings.FeedbackTemplate;
                task.To = Strings.ContactEmail;
                task.Show();
            }
            catch
            {
                // prevent exceptions from double-click
            }
        }

        private void rateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var task = new MarketplaceReviewTask();
                task.Show();
            }
            catch
            {
                // prevent exceptions from double-click
            }
        }

        private void ReadVersionFromManifest()
        {
            versionText.Text = "1.1";
        }
    }
}