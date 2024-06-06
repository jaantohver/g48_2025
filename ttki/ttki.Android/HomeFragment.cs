using System;
using System.Text;
using System.Timers;

using Android;
using Android.OS;
using Android.Nfc;
using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Runtime;
using Android.Graphics;
using Android.Content.PM;
using AndroidX.Core.Content;
using AndroidX.Fragment.App;
using AndroidX.AppCompat.App;
using Google.Android.Material.Button;

namespace ttki.Droid
{
	public class HomeFragment : Fragment, NfcAdapter.IReaderCallback
	{
        enum State
        {
            Unauthorised,
            Running,
            Idle
        }

        State state;
        Timer timer;
        DateTime start;

        View contentView;
        TextView workerIdLabel, timerLabel;
        MaterialButton button;

        AlertDialog dialog;

        public HomeFragment()
		{
		}

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            contentView = inflater.Inflate(Resource.Layout.fragment_home, container, false);

            workerIdLabel = contentView.FindViewById<TextView>(Resource.Id.worker_id);

            timerLabel = contentView.FindViewById<TextView>(Resource.Id.timer);

            button = contentView.FindViewById<MaterialButton>(Resource.Id.button);
            button.Click += OnButtonPress;

            Update();

            return contentView;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == 1)
            {
                for (int i = 0; i < permissions.Length; i++)
                {
                    string permission = permissions[i];
                    Permission result = grantResults[i];

                    if (permission == Manifest.Permission.Camera && result == Permission.Granted)
                    {
                        StartQrActivity();
                    }
                }
            }
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 2 && resultCode == -1)
            {
                ISharedPreferences prefs = Activity.GetSharedPreferences("prefs", FileCreationMode.Private);
                prefs.Edit().PutString("uid", data?.Extras.GetString("qr")).Commit();

                Update();

                Toast.MakeText(Activity, "Isikustamine õnnestus: Id " + data?.Extras.GetString("qr"), ToastLength.Short).Show();
            }
        }

        public void OnTagDiscovered(Tag tag)
        {
            string id = Encoding.Default.GetString(tag.GetId());

            if (!string.IsNullOrWhiteSpace(id))
            {
                ISharedPreferences prefs = Activity.GetSharedPreferences("prefs", FileCreationMode.Private);
                prefs.Edit().PutString("uid", id).Commit();

                Activity.RunOnUiThread(delegate {
                    Update();

                    Toast.MakeText(Activity, "Isikustamine õnnestus: Id " + id, ToastLength.Short).Show();

                    dialog.Dismiss();
                });
            }
        }

        void Update()
        {
            ISharedPreferences prefs = Activity.GetSharedPreferences("prefs", FileCreationMode.Private);

            string uid = prefs.GetString("uid", "");
            long start = prefs.GetLong("start", -1);

            if (!string.IsNullOrEmpty(uid))
            {
                workerIdLabel.Text = "Töötaja id: " + uid;
            }

            if (string.IsNullOrEmpty(uid))
            {
                state = State.Unauthorised;
            }
            else if (start > 0)
            {
                this.start = DateTimeUtils.SecondsToDateTime(start);

                state = State.Running;
            }
            else
            {
                state = State.Idle;
            }

            switch (state)
            {
                case State.Unauthorised:
                    SetIdentify();

                    break;
                case State.Running:
                    TimeSpan ts = DateTime.UtcNow - this.start;

                    SetStop();
                    timerLabel.Text = TimeUtil.GetHourMinuteSecondString(ts);

                    timer = new Timer(1000);
                    timer.AutoReset = true;
                    timer.Elapsed += delegate
                    {
                        ts = DateTime.UtcNow - this.start;

                        Activity.RunOnUiThread(delegate
                        {
                            timerLabel.Text = TimeUtil.GetHourMinuteSecondString(ts);
                        });
                    };
                    timer.Start();

                    break;
                case State.Idle:
                    SetStart();

                    break;
            }
        }

        void OnButtonPress(object sender, EventArgs e)
        {
            switch (state)
            {
                case State.Unauthorised:
                    Authorize();
                    break;
                case State.Running:
                    Stop();
                    break;
                case State.Idle:
                    Start();
                    break;
            }
        }

        void Authorize()
        {
            NfcAdapter adapter = NfcAdapter.GetDefaultAdapter(Activity);
            //adapter.EnableReaderMode(Activity, this, NfcReaderFlags.NfcA, null);

            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
            builder.SetTitle("Isikusta QR koodiga");
            builder.SetItems(new string[]{ "QR kood", /*"Test",*/ "Sulge" }, (sender, e) => {
                switch(e.Which)
                {
                    case 0:
                        ScanQr();
                        break;
                    case 1:
                        //Test();
                        break;
                    default:
                        break;
                }
            });
            dialog = builder.Show();
            //dialog.DismissEvent += delegate
            //{
            //    Timer t = new Timer(3000);
            //    t.Elapsed += (sender, e) =>
            //    {
            //        adapter.DisableReaderMode(Activity);
            //        adapter.Dispose();

            //        (sender as Timer).Stop();
            //        (sender as Timer).Dispose();
            //    };
            //};
        }

        void Start()
        {
            Activity.SupportFragmentManager.BeginTransaction().Replace(Resource.Id.flFragment, new AreasFragment(true)).AddToBackStack("fragment_home").Commit();
        }

        async void Stop()
        {
            ISharedPreferences prefs = Activity.GetSharedPreferences("prefs", FileCreationMode.Private);

            string uid = prefs.GetString("uid", "");

            if (await Networking.StopWork(uid))
            {
                timer.Stop();
                timer.Dispose();

                start = DateTime.MinValue;
                prefs.Edit().PutLong("start", -1).Commit();
                state = State.Idle;

                SetStart();

                timerLabel.Text = "00:00:00";
            }
            else
            {
                //TODO
            }
        }

        void ScanQr()
        {
            Console.WriteLine("ScanQr");

            if (ContextCompat.CheckSelfPermission(Activity, Android.Manifest.Permission.Camera) != Permission.Granted)
            {
                RequestCameraPermission();
            }
            else
            {
                StartQrActivity();
            }
        }

        void Test()
        {
            Console.WriteLine("Test");

            ISharedPreferences prefs = Activity.GetSharedPreferences("prefs", FileCreationMode.Private);
            prefs.Edit().PutString("uid", "1189042686857873").Commit();

            Update();

            Toast.MakeText (Activity, "Isikustamine õnnestus: Id 1189042686857873", ToastLength.Short).Show();
        }

        void RequestCameraPermission()
        {
            RequestPermissions(new string[] { Android.Manifest.Permission.Camera }, 1);
        }

        void StartQrActivity()
        {
            StartActivityForResult(new Intent(Activity, typeof(QrScannerActivity)), 2);
        }

        void SetIdentify()
        {
            workerIdLabel.Visibility = ViewStates.Invisible;

            button.SetBackgroundColor(Color.Rgb(181, 226, 253));
            button.Text = "Isikusta";

            timerLabel.Visibility = ViewStates.Invisible;
        }

        void SetStart()
        {
            workerIdLabel.Visibility = ViewStates.Visible;

            button.SetBackgroundColor(Color.Rgb(67, 160, 71));
            button.Text = "Alusta";

            timerLabel.Visibility = ViewStates.Visible;
        }

        void SetStop()
        {
            workerIdLabel.Visibility = ViewStates.Visible;

            button.SetBackgroundColor(Color.Rgb(160, 71, 70));
            button.Text = "Lõpeta";

            timerLabel.Visibility = ViewStates.Visible;
        }
    }
}
