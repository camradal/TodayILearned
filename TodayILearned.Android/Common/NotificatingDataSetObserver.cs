using System;
using Android.Database;

namespace EndlessScrollPort
{
    /// <summary>
    /// DataSet observer invoking callbacks on data change
    /// </summary>
    public class NotificatingDataSetObserver : DataSetObserver
    {
        private readonly Action _onChanged;
        private readonly Action _onIvalidated;

        public NotificatingDataSetObserver(Action onChanged, Action onIvalidated)
        {
            _onChanged = onChanged;
            _onIvalidated = onIvalidated;
        }

        public override void OnChanged()
        {
            _onChanged();
        }

        public override void OnInvalidated()
        {
            _onIvalidated();
        }
    }
}