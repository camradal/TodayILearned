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
                        Version = "1.8.8",
                        Description =
                            "- Bug fixes"
                    },
                    new NewItem
                    {
                        Version = "1.8.1",
                        Description =
                            "- Full support for Windows Phone resolutions\n" +
                            "- Transparent live tile\n" +
                            "- Sharing improvements"
                    },
                    new NewItem
                    {
                        Version = "1.5.1",
                        Description =
                            "- Bug fixes\n" +
                            "- Performance improvements"
                    },
                    new NewItem
                    {
                        Version = "1.5",
                        Description =
                            "- Offline mode\n" +
                            "- Favorites sort (newest/oldest)\n" +
                            "- Report inaccurate facts from the app\n" +
                            "- Improve grammar parsing\n" +
                            "- Improve items refresh\n" +
                            "- Visual style update"
                    },
                    new NewItem
                    {
                        Version = "1.4",
                        Description =
                            "- Share via clipboard\n" +
                            "- Important fix for settings getting reset\n" +
                            "- Bug fixes"
                    },
                    new NewItem
                    {
                        Version = "1.3",
                        Description =
                            "- Search functionality\n" +
                            "- Orientation lock option\n" +
                            "- Fix sharing using context menu\n" +
                            "- Fix duplicate facts displayed on the main screen\n" +
                            "- Fix navigation arrows on details screen"
                    },
                    new NewItem
                    {
                        Version = "1.2",
                        Description =
                            "- Sharing improvements: text, e-mail and social networks\n" +
                            "- Option to always open articles in Internet Explorer\n" +
                            "- Fix for duplicates in the favorites\n" +
                            "- Improvement to load times (especially on slower networks)"
                    },
                    new NewItem
                    {
                        Version = "1.1",
                        Description =
                            "- Option to show only the front of the live tile\n" +
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
            versionText.Text = "1.8.8";
        }
    }
}