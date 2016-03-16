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
                        Version = "1.8.9",
                        Description =
                            "- Bug fixes\n" +
                            "- Privacy policy"
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

        public List<NewItem> PrivacyPolicyItems
        {
            get
            {
                return new List<NewItem>()
                {
                    new NewItem
                    {
                        Version = "",
                        Description = "We try to keep it simple, drop us a line if anything is unclear."
                    },
                    new NewItem
                    {
                        Version = "Personal Information",
                        Description = "We do not collect, transmit or store any personal information.\nWe physically can't.\nWe have no servers to store it."
                    },
                    new NewItem
                    {
                        Version = "Information Sharing",
                        Description = "Since we do not collect any personal information, we have no way of sharing or disclosing it."
                    },
                    new NewItem
                    {
                        Version = "Third Party",
                        Description = "We rely on a third party service to collect anonymous crash information. The information includes phone model, OS version, application version and error detail. It helps us to make the app better."
                    },
                    new NewItem
                    {
                        Version = "Changes to Policy",
                        Description = "We may revise the policy from time to time, but you can always access the latest version within the app."
                    },
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
            versionText.Text = "1.8.9";
        }
    }
}