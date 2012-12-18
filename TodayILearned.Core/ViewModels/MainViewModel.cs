using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TodayILearned.Core
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public Action OnLoaded;

        public MainViewModel()
        {
            this.Items = new ObservableCollection<ItemViewModel>();
            this.Favorites = new ObservableCollection<ItemViewModel>();
        }

        public ObservableCollection<ItemViewModel> Items { get; private set; }
        public ObservableCollection<ItemViewModel> Favorites { get; private set; }

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

        public void LoadData()
        {
            var uri = new Uri("http://www.reddit.com/r/todayilearned.json");
            var client = new WebClient();
            client.DownloadStringAsync(uri);
            client.DownloadStringCompleted += client_DownloadStringCompleted;
        }

        public void LoadFavorites()
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            using (var stream = new IsolatedStorageFileStream("favorites.json", FileMode.OpenOrCreate, FileAccess.Read, store))
            using (var reader = new StreamReader(stream))
            using (var jsonReader = new JsonTextReader(reader))
            {
                if (reader.EndOfStream) return;

                var result = JArray.Load(jsonReader);
                foreach (var item in Serializer.GetItems(result))
                {
                    this.Favorites.Add(item);
                }
            }
        }

        public void SaveFavorites()
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            using (var stream = new IsolatedStorageFileStream("favorites.json", FileMode.Create, FileAccess.Write, store))
            using (var writer = new StreamWriter(stream))
            {
                string serialized = JsonConvert.SerializeObject(this.Favorites);
                writer.Write(serialized);
            }
        }

        public void AddFavorite(ItemViewModel item)
        {
            this.Favorites.Add(item);
            NotifyPropertyChanged("Favorites");
        }

        public void RemoveFavorite(ItemViewModel item)
        {
            this.Favorites.Remove(item);
            NotifyPropertyChanged("Favorites");
        }

        private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                var result = JObject.Parse(e.Result);
                foreach (var item in Serializer.GetItems(result))
                {
                    this.Items.Add(item);
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