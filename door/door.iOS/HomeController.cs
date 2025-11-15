using System;
using System.Linq;
using System.Timers;

using UIKit;
using CoreNFC;
using Foundation;
using ObjCRuntime;

using GlobalToast;
using Microsoft.AppCenter.Crashes;

namespace door.iOS
{
    public class HomeController : UIViewController, INFCNdefReaderSessionDelegate
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

        HomeView contentView;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View = contentView = new HomeView();
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            Update();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            contentView.Button.TouchUpInside += OnButtonPress;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            contentView.Button.TouchUpInside -= OnButtonPress;
        }

        void Update()
        {
            string uid = NSUserDefaults.StandardUserDefaults.StringForKey("uid");
            string start = NSUserDefaults.StandardUserDefaults.StringForKey("start");

            if (!string.IsNullOrEmpty(uid))
            {
                contentView.SetWorkerId(uid);
            }

            if (string.IsNullOrEmpty(uid))
            {
                state = State.Unauthorised;
            }
            else if (!string.IsNullOrEmpty(start))
            {
                this.start = DateTime.Parse(start);

                state = State.Running;
            }
            else
            {
                state = State.Idle;
            }

            switch (state)
            {
                case State.Unauthorised:
                    contentView.SetIdentify();
                    break;
                case State.Running:
                    TimeSpan ts = DateTime.Now - this.start;

                    contentView.SetStop();
                    contentView.SetTimerString(TimeUtil.GetHourMinuteSecondString(ts));

                    timer = new Timer(1000);
                    timer.AutoReset = true;
                    timer.Elapsed += delegate
                    {
                        ts = DateTime.Now - this.start;

                        contentView.SetTimerString(TimeUtil.GetHourMinuteSecondString(ts));
                    };
                    timer.Start();

                    break;
                case State.Idle:
                    contentView.SetStart();
                    break;
            }
        }

        void OnButtonPress(object sender, EventArgs e)
        {
            LogController.Log("OnButtonPress");
            try
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
            catch (Exception ex)
            {
                LogController.Log(ex);
                Crashes.TrackError(ex);
            }
        }

        void Authorize()
        {
            try
            {
                LogController.Log("Authorize");

                UIAlertController alert = new UIAlertController();
                alert.Title = "Kuidas soovid isikustada?";
                alert.ModalPresentationStyle = UIModalPresentationStyle.None;

                UIPopoverPresentationController presentationPopover = alert.PopoverPresentationController;
                if (presentationPopover != null)
                {
                    presentationPopover.SourceView = contentView.Button;
                    presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Up;
                }

                UIAlertAction qr = UIAlertAction.Create("QR kood", UIAlertActionStyle.Default, ScanQr);
                UIAlertAction nfc = UIAlertAction.Create("RFID kiip", UIAlertActionStyle.Default, ScanNfc);
                UIAlertAction test = UIAlertAction.Create("Test", UIAlertActionStyle.Default, Test);
                UIAlertAction cancel = UIAlertAction.Create("Sulge", UIAlertActionStyle.Cancel, null);

                alert.AddAction(qr);
                //alert.AddAction(nfc);
                //alert.AddAction(test);
                alert.AddAction(cancel);

                PresentViewController(alert, true, null);
            } catch (Exception e)
            {
                LogController.Log(e);
                Crashes.TrackError(e);
            }
        }

        void Start()
        {
            LogController.Log("Start");

            AreasController controller = new AreasController(true);
            controller.AreaSelected += async (sender, e) =>
            {
            };

            UINavigationController navController = new UINavigationController(controller);

            PresentViewController(navController, true, null);
        }

        async void Stop()
        {
        }

        void ScanQr(UIAlertAction a)
        {
            LogController.Log("ScanQr");

            QRScanController qrController = new QRScanController();
            qrController.QrFound += (sender, e) =>
            {
                NSUserDefaults.StandardUserDefaults.SetString(e, "uid");

                Update();

                Toast.ShowToast("Id " + e, "Isikustamine õnnestus");
            };

            PresentViewController(qrController, true, null);
        }

        void ScanNfc(UIAlertAction a)
        {
            LogController.Log("ScanNfc");

            if (Runtime.Arch == Arch.SIMULATOR)
            {
                UIAlertController alert = UIAlertController.Create("Sorry :(", "This doesn't work in a simulator", UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create("Gotcha", UIAlertActionStyle.Default, null));

                PresentViewController(alert, true, null);
                return;
            }
            else
            {
                BeginNfcScan();
            }
        }

        void Test(UIAlertAction a)
        {
            LogController.Log("Test");

            NSUserDefaults.StandardUserDefaults.SetString("1189042686857873", "uid");

            Update();

            Toast.ShowToast("Id 1189042686857873", "Isikustamine õnnestus");
        }

        void BeginNfcScan()
        {
            NFCNdefReaderSession session = new NFCNdefReaderSession(this, null, true);
            session?.BeginSession();
        }

        public void DidInvalidate(NFCNdefReaderSession session, NSError error)
        {
            LogController.Log(error);
        }

        public void DidDetect(NFCNdefReaderSession session, NFCNdefMessage[] messages)
        {
            foreach (NFCNdefMessage m in messages)
            {
                string id = m.Records.FirstOrDefault()?.Identifier.ToString();

                InvokeOnMainThread(delegate
                {
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        NSUserDefaults.StandardUserDefaults.SetString(id, "uid");

                        Update();

                        Toast.ShowToast("Id " + id, "Isikustamine õnnestus");
                    }
                });
            }
        }
    }
}
