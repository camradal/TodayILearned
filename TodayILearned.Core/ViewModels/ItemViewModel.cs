using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TodayILearned
{
    public class ItemViewModel : INotifyPropertyChanged
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
    }
}