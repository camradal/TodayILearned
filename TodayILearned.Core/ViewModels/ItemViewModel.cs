using System;
using System.ComponentModel;

namespace TodayILearned
{
    public class ItemViewModel : INotifyPropertyChanged, IEquatable<ItemViewModel>
    {
        private string _title;
        private string _description;
        private string _url;
        private string _domain;
        private string _thumbnail;

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (value != _description)
                {
                    _description = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }

        public string Url
        {
            get
            {
                return _url;
            }
            set
            {
                if (value != _url)
                {
                    _url = value;
                    NotifyPropertyChanged("Url");
                }
            }
        }
        
        public string Domain
        {
            get
            {
                return _domain;
            }
            set
            {
                if (value != _domain)
                {
                    _domain = value;
                    NotifyPropertyChanged("Domain");
                }
            }
        }

        public string Thumbnail
        {
            get
            {
                return _thumbnail;
            }
            set
            {
                if (value != _thumbnail)
                {
                    _thumbnail = value;
                    NotifyPropertyChanged("Thumbnail");
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

        #region IEquitable implementation
        
        public bool Equals(ItemViewModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_title, other._title);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ItemViewModel)obj);
        }

        public override int GetHashCode()
        {
            return _title.GetHashCode();
        }

        #endregion
    }
}