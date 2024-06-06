using System.Collections.Generic;

using Android.Views;
using Android.Widget;
using Android.Content;

namespace ttki.Droid
{
	public class AreasListAdapter : BaseAdapter<WorkArea>
	{
        Context context;

        List<WorkArea> areas = new List<WorkArea>();

        public AreasListAdapter(Context context)
        {
            this.context = context;
        }

        public override int Count => areas.Count;

        public override WorkArea this[int position] => areas[position];

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;

            if (view == null)
            {
                view = LayoutInflater.From(context).Inflate(Resource.Layout.area_listview_cell, null, false);
            }

            TextView tv = view.FindViewById<TextView>(Resource.Id.text);
            tv.Text = areas[position].Name;

            return view;
        }

        public void Update(List<WorkArea> areas)
        {
            this.areas = areas;

            NotifyDataSetChanged();
        }
    }
}
