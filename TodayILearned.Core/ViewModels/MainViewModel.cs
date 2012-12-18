using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using Newtonsoft.Json.Linq;

namespace TodayILearned.Core
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public Action OnLoaded;

        public MainViewModel()
        {
            this.Items = new ObservableCollection<ItemViewModel>();
        }

        public ObservableCollection<ItemViewModel> Items { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            var uri = new Uri("http://www.reddit.com/r/todayilearned.json");
            var client = new WebClient();
            client.DownloadStringAsync(uri);
            client.DownloadStringCompleted += client_DownloadStringCompleted;
        }

        void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                var result = JObject.Parse(e.Result);
                var entries = result["data"]["children"];
                foreach (var entry in entries)
                {
                    string title = ProcessString(entry["data"]["title"].Value<string>());
                    var itemViewModel = new ItemViewModel
                    {
                        Title = title,
                        Url = entry["data"]["url"].Value<string>(),
                        Domain = entry["data"]["domain"].Value<string>(),
                        Thumbnail = entry["data"]["thumbnail"].Value<string>()
                    };
                    this.Items.Add(itemViewModel);
                }
                this.IsDataLoaded = true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (OnLoaded != null)
                {
                    OnLoaded();
                }
            }
        }

        private string ProcessString(string value)
        {
            const string tilThat = "TIL that ";
            const string til = "TIL ";
            if (value.StartsWith(tilThat)) value = value.Substring(tilThat.Length);
            if (value.StartsWith(til)) value = value.Substring(til.Length);
            if (value.Length > 0) value = char.ToUpper(value[0]) + value.Substring(1);
            return value;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}