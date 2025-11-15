using System.Collections.Generic;

using UIKit;
using Foundation;

namespace door.iOS
{
	public class WorksController : UIViewController
	{
		WorksView contentView;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Tehtud tööd";

            View = contentView = new WorksView();
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
                List<Work> works = await Networking.GetWorks(uid);
                List<WorkArea> areas = await Networking.GetWorkAreas(uid);

                contentView.Update(works, areas);
            }

            UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
        }
    }
}
