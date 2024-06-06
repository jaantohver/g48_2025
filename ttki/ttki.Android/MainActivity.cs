using Android.OS;
using Android.App;
using Android.Views;
using Android.Content.PM;
using AndroidX.AppCompat.App;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Analytics;

using Google.Android.Material.BottomNavigation;

namespace ttki.Droid
{
    [Activity(Label = "Terake Lite", Icon = "@mipmap/ic_launcher", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : AppCompatActivity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        HomeFragment homeFragment = new HomeFragment();
        AreasFragment areasFragment = new AreasFragment(false);
        WorksFragment worksFragment = new WorksFragment();

        BottomNavigationView bottomNavigationView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            AppCenter.Start("711e947b-3f41-47b4-9c9c-5a075483b673", typeof(Analytics), typeof(Crashes));

            SetContentView(Resource.Layout.tab_layout);

            bottomNavigationView = FindViewById<BottomNavigationView>(Resource.Id.bottomNavigationView);

            bottomNavigationView.SetOnNavigationItemSelectedListener(this);
            bottomNavigationView.SelectedItemId = Resource.Id.home;
        }

        public override void OnBackPressed()
        {
            if (SupportFragmentManager.BackStackEntryCount > 0)
            {
                SupportFragmentManager.PopBackStack();
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public bool OnNavigationItemSelected(IMenuItem p0)
        {
            switch(p0.ItemId)
            {
                case Resource.Id.home:
                    SupportFragmentManager.BeginTransaction().Replace(Resource.Id.flFragment, homeFragment).Commit();
                    return true;
                case Resource.Id.areas:
                    SupportFragmentManager.BeginTransaction().Replace(Resource.Id.flFragment, areasFragment).Commit();
                    return true;
                case Resource.Id.works:
                    SupportFragmentManager.BeginTransaction().Replace(Resource.Id.flFragment, worksFragment).Commit();
                    return true;
                default:
                    return false;
            }
        }
    }
}
