using System.Collections.Generic;

using UIKit;
using CoreGraphics;

namespace door.iOS
{
	public class WorksView : UIView
	{
		readonly UILabel noWorksLabel;
		readonly UITableView tableView;

		public WorksView()
		{
			BackgroundColor = UIColor.SystemBackground;

			noWorksLabel = new UILabel();
            noWorksLabel.Font = UIFont.SystemFontOfSize(15);
            noWorksLabel.TextColor = UIColor.LightGray;
			noWorksLabel.TextAlignment = UITextAlignment.Center;
			noWorksLabel.Hidden = true;

			tableView = new UITableView();
			tableView.Source = new WorksTableViewSource();

			AddSubviews(tableView, noWorksLabel);
		}

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            noWorksLabel.Frame = new CGRect(0, 0, Frame.Width, 200);

            tableView.Frame = Bounds;
        }

        public void SetNoWorker()
        {
            noWorksLabel.Text = "Töötaja on isikustamata";
            noWorksLabel.Hidden = false;
        }
    }
}
