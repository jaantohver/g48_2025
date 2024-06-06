using System;
using System.Collections.Generic;

using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Content;
using AndroidX.Fragment.App;
using System.Globalization;
using static Android.Provider.UserDictionary;

namespace ttki.Droid
{
	public class AreasFragment : Fragment
    {
        bool startWorkOnSelect;

        ListView listView;
        TextView noAreasLabel;

        List<WorkArea> areas;

		public AreasFragment(bool startWorkOnSelect)
		{
            this.startWorkOnSelect = startWorkOnSelect;
		}

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_areas, container, false);

            noAreasLabel = view.FindViewById<TextView>(Resource.Id.no_workareas);

            listView = view.FindViewById<ListView>(Resource.Id.areasList);
            listView.Adapter = new AreasListAdapter(Activity);
            listView.ItemClick += OnItemClick;

            Update();

            return view;
        }

        async void Update()
        {
            ISharedPreferences prefs = Activity.GetSharedPreferences("prefs", FileCreationMode.Private);

            string uid = prefs.GetString("uid", "");

            if (string.IsNullOrEmpty(uid))
            {
                noAreasLabel.Text = "Töötaja on isikustamata";
            }
            else
            {
                areas = await Networking.GetWorkAreas(uid);

                if (areas.Count == 0)
                {
                    noAreasLabel.Text = "Töötajaga ei ole seotud ühtegi töömaad";
                }

                (listView.Adapter as AreasListAdapter).Update(areas);
            }
        }

        async void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (startWorkOnSelect)
            {
                ISharedPreferences prefs = Activity.GetSharedPreferences("prefs", FileCreationMode.Private);
                string uid = prefs.GetString("uid", "");

                string id = areas[e.Position].Uuid;

                if (await Networking.StartWork(uid, id))
                {
                    prefs.Edit().PutLong("start", DateTime.UtcNow.ToSeconds()).Commit();
                    Activity.SupportFragmentManager.BeginTransaction().Replace(Resource.Id.flFragment, new HomeFragment()).Commit();
                }
                else
                {
                    //TODO
                }
            }
        }
    }
}
