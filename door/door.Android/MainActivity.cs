using System;
using System.Linq;
using System.Collections.Generic;

using Android;
using Android.OS;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Content.PM;
using AndroidX.AppCompat.App;

using Toolbar = AndroidX.AppCompat.Widget.Toolbar;
using Android.Graphics;
using AndroidX.Core.Content.Resources;

namespace door.Droid
{
    [Activity(Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : AppCompatActivity
    {
        readonly string[] permissionList = {
            Manifest.Permission.Bluetooth,
            Manifest.Permission.BluetoothScan,
            Manifest.Permission.BluetoothAdmin,
            Manifest.Permission.BluetoothConnect,
            Manifest.Permission.Internet,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.AccessNetworkState,
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };

        readonly List<Lock> locks = new List<Lock>();

        ListView lv;
        TextView lastSeen, status;
        ImageView lockImage, backgroundImage;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            Toolbar myToolbar = FindViewById<Toolbar>(Resource.Id.my_toolbar);
            myToolbar.SetBackgroundColor(Android.Graphics.Color.ParseColor("#3d3d3d"));
            myToolbar.SetNavigationIcon(Resource.Drawable.ic_logo_wide);
            myToolbar.Title = "";
            SetSupportActionBar(myToolbar);

            lockImage = FindViewById<ImageView>(Resource.Id.lock_icon);

            backgroundImage = FindViewById<ImageView>(Resource.Id.lock_bg);

            lv = FindViewById<ListView>(Resource.Id.list);
            lv.Adapter = new ListAdapter(this);

            //Typeface typeface = ResourcesCompat.GetFont(this, Resources.GetFont(Resource.Id.ppp);

            FindViewById<TextView>(Resource.Id.name).Typeface = Typeface.CreateFromAsset(Assets, "PPPangramSansRoundedMedium.otf");

            FindViewById<TextView>(Resource.Id.last_seen).Typeface = Typeface.CreateFromAsset(Assets, "PPPangramSansRoundedMedium.otf");

            lastSeen = FindViewById<TextView>(Resource.Id.last_seen);

            status = FindViewById<TextView>(Resource.Id.status);

            Lock l1 = new Lock();
            l1.Name = "Lukk 1";
            l1.LastSeenTimestamp = 1763151909;
            l1.IsLocked = false;

            Lock l2 = new Lock();
            l2.Name = "Lukk 2";
            l2.LastSeenTimestamp = 1763151909;
            l2.IsLocked = true;
            
            Lock l3 = new Lock();
            l3.Name = "Lukk 3";
            l3.LastSeenTimestamp = 1763151909;
            l3.IsLocked = true;

            //locks.Add(l1);
            //locks.Add(l2);
            //locks.Add(l3);

            (lv.Adapter as ListAdapter).Update(locks);
        }

        protected override void OnStart()
        {
            base.OnStart();

            if ((int)Build.VERSION.SdkInt >= 23)
                RequestPermissions();

            BLEManager.GetManager(this).ExecuteManagerCommand(new ManagerCommand(null, ManagerMode.SCAN));
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

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu, menu);

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            //Hacky
            if (item.ItemId == 2131230790)
            {
                StartActivity(typeof(AddNewActivity));
            }

            return base.OnOptionsItemSelected(item);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (grantResults.Any(x => x == Permission.Denied))
                Toast.MakeText(this, "Some permissions have been denied, app may not work properly.", ToastLength.Long);

            for (int i = 0; i < permissions.Length; i++)
            {
                Console.WriteLine(permissions[i] + ' ' + grantResults[i]);
            }
        }

        void RequestPermissions()
        {
            RequestPermissions(permissionList, 98765);
        }

        int counter = 0;

        void OnLockStatusChanged(object sender, LockStatusChangedEventArgs e)
        {
            counter++;
            Console.WriteLine("[MainActivity] OnLockStatusChanged " + counter);

            if(e.IsOpen)
            {
                lockImage.SetImageResource(Resource.Drawable.ic_lock_open);
                backgroundImage.SetImageResource(Resource.Drawable.ic_button_bg_red);
                status.Text = "Unlocked";
            } else
            {
                lockImage.SetImageResource(Resource.Drawable.ic_lock);
                backgroundImage.SetImageResource(Resource.Drawable.ic_button_bg_blue);
                status.Text = "Locked";
            }

            if (locks.Any(l => l.Id == e.DeviceId))
            {
                locks.Find(l => l.Id == e.DeviceId).IsLocked = !e.IsOpen;

                (lv.Adapter as ListAdapter).Update(locks);
            }
            else
            {
                Lock l = new Lock();
                l.Name = "Front door";
                l.Id = e.DeviceId;
                l.IsLocked = !e.IsOpen;
                l.LastSeenTimestamp = 123;

                locks.Add(l);

                (lv.Adapter as ListAdapter).Update(locks);
            }


        }
    }
}
