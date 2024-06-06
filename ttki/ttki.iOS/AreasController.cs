using System;
using System.Collections.Generic;

using UIKit;
using Foundation;

namespace ttki.iOS
{
	public class AreasController : UIViewController
	{
        bool startWorkOnSelect;

        UIBarButtonItem closeButton;

        AreasView contentView;

        public event EventHandler<string> AreaSelected;

        public AreasController(bool startWorkOnSelect)
        {
            this.startWorkOnSelect = startWorkOnSelect;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Töömaad";

            View = contentView = new AreasView();

            closeButton = new UIBarButtonItem(UIBarButtonSystemItem.Close);
            closeButton.Title = "Sulge";
            closeButton.Clicked += delegate {
                DismissViewController(true, null);
            };

            if (PresentingViewController != null)
            {
                NavigationItem.RightBarButtonItem = closeButton;
            }
        }

        public override async void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;

            string uid = NSUserDefaults.StandardUserDefaults.StringForKey("uid");

            if (string.IsNullOrEmpty(uid))
            {
                contentView.SetNoWorker();
            }
            else
            {
                List<WorkArea> areas = await Networking.GetWorkAreas(uid);

                contentView.Update(areas);
            }

            UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            contentView.RowSelected += OnRowSelected;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            contentView.RowSelected -= OnRowSelected;
        }

        void OnRowSelected(object sender, string e)
        {
            AreaSelected?.Invoke(this, e);

            if (startWorkOnSelect)
            {
                DismissViewController(true, null);
            }
        }
    }
}
