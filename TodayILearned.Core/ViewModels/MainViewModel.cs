﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpGIS;

namespace TodayILearned.Core
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private volatile bool isInitialized;
        private volatile bool isLoading;
        private volatile bool isSearching;

        private ItemViewModel item;

        public Action BeginLoading;
        public Action OnLoaded;
        public Action OnInitialized;
        public Action<Exception> OnError;

        public MainViewModel()
        {
            this.Items = new ObservableCollection<ItemViewModel>();
            this.Favorites = new ObservableCollection<ItemViewModel>();
            this.SearchItems = new ObservableCollection<ItemViewModel>();
        }

        public bool IsLoading
        {
            get { return isLoading; }
            private set { isLoading = value; }
        }

        public bool IsSearching
        {
            get { return isSearching; }
            private set { isSearching = value; }
        }

        public bool IsLoaded { get; private set; }
        public string LastItem { get; private set; }

        public ObservableCollection<ItemViewModel> Items { get; private set; }
        public ObservableCollection<ItemViewModel> Favorites { get; private set; }
        public ObservableCollection<ItemViewModel> SearchItems { get; private set; } 

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
            if (IsLoading)
                return;

            IsLoading = true;
            if (BeginLoading != null)
            {
                BeginLoading();
            }

            string uriString = "http://www.reddit.com/r/todayilearned.json";
            if (!string.IsNullOrEmpty(lastItem))
            {
                uriString += "?after=" + lastItem;
            }
            var uri = new Uri(uriString);
            var client = new GZipWebClient();
            client.DownloadStringCompleted += client_DownloadStringCompleted;
            client.DownloadStringAsync(uri);
        }

        private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                var result = JObject.Parse(e.Result);
                foreach (ItemViewModel model in Serializer.GetItems(result))
                {
                    this.Items.Add(model);
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
                if (OnLoaded != null)
                {
                    OnLoaded();
                }

                if (!isInitialized && OnInitialized != null)
                {
                    isInitialized = true;
                    OnInitialized();
                }
            }
        }

        public void Search(string term)
        {
            Search(term, "");
        }

        public void Search(string term, string lastItem)
        {
            if (IsSearching)
                return;

            IsSearching = true;
            if (BeginLoading != null)
            {
                BeginLoading();
            }

            string uriString = "http://www.reddit.com/r/todayilearned/search.json?sort=relevance&restrict_sr=on&t=all&q=" + term;
            if (!string.IsNullOrEmpty(lastItem))
            {
                uriString += "&after=" + lastItem;
            }
            else
            {
                this.SearchItems.Clear();                
            }

            var uri = new Uri(uriString);
            var client = new GZipWebClient();
            client.DownloadStringCompleted += client_DownloadSearchStringCompleted;
            client.DownloadStringAsync(uri);
        }

        private void client_DownloadSearchStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                var result = JObject.Parse(e.Result);
                foreach (ItemViewModel model in Serializer.GetItems(result))
                {
                    this.SearchItems.Add(model);
                }
            }
            catch (Exception ex)
            {
                if (OnError != null) OnError(ex);
            }
            finally
            {
                IsSearching = false;
                if (OnLoaded != null) OnLoaded();
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
                foreach (var favorite in Serializer.GetItems(result))
                {
                    this.Favorites.Add(favorite);
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

        public void AddFavorite(ItemViewModel model)
        {
            // do not insert dupes
            if (Favorites.Any(favorite => favorite.Title == model.Title))
            {
                return;
            }

            this.Favorites.Add(model);
            NotifyPropertyChanged("Favorites");
        }

        public void RemoveFavorite(ItemViewModel model)
        {
            this.Favorites.Remove(model);
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