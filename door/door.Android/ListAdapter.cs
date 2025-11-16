using System;
using System.Collections.Generic;

using Android.Views;
using Android.Widget;
using Android.Content;

namespace door.Droid
{
    public class ListAdapter : BaseAdapter<Lock>
    {
        public event EventHandler<LockStatusChangedEventArgs> LockStatusChanged;

        Context context;

        List<Lock> locks = new List<Lock>();

        public ListAdapter(Context context)
        {
            this.context = context;
        }

        public override int Count => locks.Count;

        public override Lock this[int position] => locks[position];

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;

            view ??= LayoutInflater.From(context).Inflate(Resource.Layout.lock_listview_cell, null, false);

            Lock l = locks[position];

            TextView nameLabel = view.FindViewById<TextView>(Resource.Id.name);
            nameLabel.Text = l?.Name;

            DateTime now = DateTime.Now;
            long nowTs = (now.Ticks - 621355968000000000) / 10000000;

            long lastSeen = nowTs - l.LastSeenTimestamp;

            TextView lastSeenLabel = view.FindViewById<TextView>(Resource.Id.last_seen);
            lastSeenLabel.Text = lastSeen.ToTimeString() + " ago";

            ImageView lockImage = view.FindViewById<ImageView>(Resource.Id.lock_icon);

            if (l.IsLocked)
            {
                lockImage.SetImageResource(Resource.Drawable.ic_lock);
            } else
            {
                lockImage.SetImageResource(Resource.Drawable.ic_lock_open);
            }

            return view;
        }

        public void Update(List<Lock> locks)
        {
            this.locks = locks;

            NotifyDataSetChanged();
        }
    }
}
