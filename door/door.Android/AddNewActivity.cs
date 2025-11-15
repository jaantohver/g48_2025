using System;

using Android.OS;
using Android.App;
using Android.Widget;
using AndroidX.AppCompat.App;

using Toolbar = AndroidX.AppCompat.Widget.Toolbar;
using Android.Views;

namespace door.Droid
{
	[Activity]
	public class AddNewActivity : AppCompatActivity
    {

        ListView lv;

        protected override void OnCreate (Bundle savedInstanceState)
		{
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_add);
            
            Toolbar myToolbar = FindViewById<Toolbar>(Resource.Id.my_toolbar);
            myToolbar.SetBackgroundColor(Android.Graphics.Color.ParseColor("#3d3d3d"));
            myToolbar.SetNavigationIcon(Resource.Drawable.ic_logo_wide);
            myToolbar.Title = "";
            SetSupportActionBar(myToolbar);

            lv = FindViewById<ListView>(Resource.Id.list);
        }

        protected override void OnResume()
        {
            base.OnResume();

            BLEManager.LockStatusChanged += OnLockStatusChanged;
        }

        protected override void OnPause()
        {
            base.OnPause();

            BLEManager.LockStatusChanged -= OnLockStatusChanged;
        }

        void OnLockStatusChanged(object sender, LockStatusChangedEventArgs e)
        {
            Console.WriteLine("tuut 2");
        }
    }
}
