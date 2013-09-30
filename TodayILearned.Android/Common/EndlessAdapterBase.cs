using System;
using System.Threading;
using System.Threading.Tasks;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Util.Concurrent.Atomic;

namespace EndlessScrollPort
{
    public abstract class EndlessAdapterBase : AdapterWrapper
    {
        private View _pendingItemView; // Item displaying pending message

        private readonly AtomicBoolean _keepOnAppending;
        private readonly Context _context;
        private readonly int _pendingItemResourceId;

        protected EndlessAdapterBase(IListAdapter wrapped)
            : base(wrapped)
        {
            _keepOnAppending = new AtomicBoolean(true);
            _pendingItemResourceId = -1;
        }

        protected EndlessAdapterBase(IListAdapter wrapped, bool keepOnAppending)
            : this(wrapped)
        {
            this.SetKeepOnAppending(keepOnAppending);
        }

        protected EndlessAdapterBase(Context context, IListAdapter wrapped, int pendingItemResourceId)
            : this(wrapped)
        {
            this._context = context;
            this._pendingItemResourceId = pendingItemResourceId;
        }

        protected EndlessAdapterBase(Context context, IListAdapter wrapped, int pendingItemResourceId, bool keepOnAppending)
            : this(wrapped)
        {
            this._context = context;
            this._pendingItemResourceId = pendingItemResourceId;
            this.SetKeepOnAppending(keepOnAppending);
        }

        /// <summary>
        /// Override to perfom load action in background
        /// Must return true if there's more item to add
        /// </summary>
        abstract protected Task<bool> LoadData();

        /// <summary>
        /// Override to append newly loaded data
        /// </summary>
        abstract protected void AppendCachedData();

        public void StopAppending()
        {
            SetKeepOnAppending(false);
        }

        public void RestartAppending()
        {
            SetKeepOnAppending(true);
        }

        /// <summary>
        /// Use to manually notify the adapter that it's dataset 
        /// has changed. Will remove the pendingView and update the display.
        /// </summary>
        public void OnDataReady()
        {
            _pendingItemView = null;
            NotifyDataSetChanged();
        }

        public override int Count
        {
            get
            {
                int result = _keepOnAppending.Get() ? base.Count + 1 : base.Count; // one more for "pending"
                return result;
            }
        }

        /// <summary>
        /// Masks ViewType so the AdapterView replaces the "Pending" row when new data is loaded.
        /// </summary>
        public override int GetItemViewType(int position)
        {
            int result = (position == WrappedAdapter.Count)
                             ? Adapter.IgnoreItemViewType
                             : base.GetItemViewType(position);

            return result;
        }

        /// Masks ViewType so the AdapterView replaces the
        /// "Pending" row when new data is loaded.
        /// getItemViewType(int)
        public override int ViewTypeCount
        {
            get
            {
                return base.ViewTypeCount + 1;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            Java.Lang.Object result = position >= base.Count ? null : base.GetItem(position);
            return result;
        }

        public override bool AreAllItemsEnabled()
        {
            return false;
        }

        public override bool IsEnabled(int position)
        {
            bool result = position < base.Count && base.IsEnabled(position);
            return result;
        }

        /// <summary>
        /// Get a View that displays the data at the specified  position in the data set. In this case, if we are at
        /// the end of the list and we are still in append mode, we ask for a pending view and return it, plus kick off the
        /// background task to append more data to the wrapped adapter.
        /// </summary>
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View result;

            if (position == base.Count && _keepOnAppending.Get())
            {
                _pendingItemView = result = GetPendingItemView(parent);

                Task.Factory.StartNew(() => ExecuteItemLoadAsync(), CancellationToken.None, TaskCreationOptions.None,
                                      TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
                result = base.GetView(position, convertView, parent);

            return result;
        }

        private void SetKeepOnAppending(bool newValue)
        {
            bool same = (newValue == _keepOnAppending.Get());
            _keepOnAppending.Set(newValue);

            if (!same)
                NotifyDataSetChanged();
        }

        /// <summary>
        /// Inflates pending view using the pendingResource ID passed into the constructor
        /// returns inflated pending view, or null if the context passed into the pending view constructor was null.
        /// </summary>
        protected virtual View GetPendingItemView(ViewGroup parent)
        {
            if (_context == null)
                throw new InvalidOperationException("You must either override getPendingView() or supply a pending View resource via the constructor");

            // Inflate pending item view if needed
            if (_pendingItemView == null)
            {
                LayoutInflater inflater = (LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService);
                _pendingItemView = inflater.Inflate(_pendingItemResourceId, parent, false);
            }

            return _pendingItemView;
        }

        private async Task ExecuteItemLoadAsync()
        {
            try
            {
                bool hasMoreItems = await LoadData();

                SetKeepOnAppending(hasMoreItems);
                AppendCachedData();
                OnDataReady();
            }
            catch (Exception e)
            {
                Log.Error("EndlessAdapter", "Exception while executing items loading", e);
                SetKeepOnAppending(false);
            }
        }
    }
}