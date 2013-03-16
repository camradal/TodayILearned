using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Utilities;

namespace TodayILearned.Core
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ItemViewModel item;

        public Action OnLoaded;
        public Action<Exception> OnError;

        public MainViewModel()
        {
            this.Items = new ObservableCollection<ItemViewModel>();
            this.Favorites = new ObservableCollection<ItemViewModel>();
        }

        public bool IsLoading { get; private set; }
        public bool IsLoaded { get; private set; }
        public string LastItem { get; private set; }

        public ObservableCollection<ItemViewModel> Items { get; private set; }
        public ObservableCollection<ItemViewModel> Favorites { get; private set; }

        public ItemViewModel Item
        {
            get { return item; }
            set
            {
                item = value;
                NotifyPropertyChanged("Item");
            }
        }

        public void LoadData()
        {
            LoadData("");
        }

        public void LoadData(string lastItem)
        {
            IsLoading = true;
            GlobalLoading.Instance.IsLoading = true;

            string uriString = "http://www.reddit.com/r/todayilearned.json";
            if (!string.IsNullOrEmpty(lastItem))
            {
                uriString += "?after=" + lastItem;
            }
            var uri = new Uri(uriString);
            var client = new WebClient();
            client.DownloadStringAsync(uri);
            client.DownloadStringCompleted += client_DownloadStringCompleted;
        }

        private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                var result = JObject.Parse(e.Result);
                foreach (ItemViewModel item in Serializer.GetItems(result))
                {
                    this.Items.Add(item);
                }
                Item = this.Items.FirstOrDefault();
                LastItem = result["data"]["after"].ToString();
                this.IsLoaded = true;
            }
            catch (Exception ex)
            {
                if (OnError != null)
                {
                    OnError(ex);
                }
            }
            finally
            {
                IsLoading = false;
                GlobalLoading.Instance.IsLoading = false;
                GlobalLoading.Instance.LoadingText = null;
                if (OnLoaded != null)
                {
                    OnLoaded();
                }
            }
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