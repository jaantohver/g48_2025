using System.Collections.Generic;

using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content;
using AndroidX.Fragment.App;

namespace ttki.Droid
{
    public class WorksFragment : Fragment
    {
        ListView listView;
        TextView noWorksLabel;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_works, container, false);

            noWorksLabel = view.FindViewById<TextView>(Resource.Id.no_works);

            listView = view.FindViewById<ListView>(Resource.Id.worksList);
            listView.Adapter = new WorksListAdapter(Activity);

            Update();

            return view;
        }

        async void Update()
        {
            ISharedPreferences prefs = Activity.GetSharedPreferences("prefs", FileCreationMode.Private);

            string uid = prefs.GetString("uid", "");

            if (string.IsNullOrEmpty(uid))
            {
                noWorksLabel.Text = "Töötaja on isikustamata";
            }
            else
            {
                List<Work> works = await Networking.GetWorks(uid);
                List<WorkArea> areas = await Networking.GetWorkAreas(uid);

                if (works.Count == 0)
                {
                    noWorksLabel.Text = "Töötajal ei ole ühtegi tehtud tööd";
                }

                (listView.Adapter as WorksListAdapter).Update(works, areas);
            }
        }
    }
}
