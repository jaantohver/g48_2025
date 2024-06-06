using System.Linq;
using System.Collections.Generic;

using Android.Views;
using Android.Widget;
using Android.Content;

namespace ttki.Droid
{
    public class WorksListAdapter : BaseAdapter<Work>
    {
        Context context;

        List<Work> works = new List<Work>();
        List<WorkArea> areas = new List<WorkArea>();

        public WorksListAdapter(Context context)
        {
            this.context = context;
        }

        public override int Count => works.Count;

        public override Work this[int position] => works[position];

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;

            if (view == null)
            {
                view = LayoutInflater.From(context).Inflate(Resource.Layout.work_listview_cell, null, false);
            }

            Work work = works[position];
            WorkArea area = areas.FirstOrDefault(a => a.Uuid == work.WorkArea);

            TextView areaLabel = view.FindViewById<TextView>(Resource.Id.area_text);
            areaLabel.Text = area?.Name;

            TextView timeLabel = view.FindViewById<TextView>(Resource.Id.time_text);
            timeLabel.Text = work.Start + " - " + work.Stop;

            return view;
        }

        public void Update(List<Work> works, List<WorkArea> areas)
        {
            this.works = works;
            this.areas = areas;

            NotifyDataSetChanged();
        }
    }
}
