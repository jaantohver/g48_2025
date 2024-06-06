using System;

using UIKit;
using CoreGraphics;

namespace ttki.iOS
{
	public class WorkTableViewCell : UITableViewCell
	{
		nfloat padding;

		UILabel workAreaLabel, timeLabel;

		public WorkTableViewCell(nfloat padding)
		{
			this.padding = padding;

            SelectionStyle = UITableViewCellSelectionStyle.None;

			workAreaLabel = new UILabel();

            timeLabel = new UILabel();
			timeLabel.Font = UIFont.SystemFontOfSize(UIFont.SmallSystemFontSize, UIFontWeight.Thin);
			timeLabel.TextColor = UIColor.Gray;

			AddSubviews(workAreaLabel, timeLabel);
		}

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

			workAreaLabel.Frame = new CGRect(
				padding,
				0,
				Frame.Width - padding,
                Frame.Height / 3 * 2
            );

			timeLabel.Frame = new CGRect(
				padding,
				workAreaLabel.Frame.Bottom,
				Frame.Width - padding,
				Frame.Height / 3
			);
        }

		public void Update(WorkArea area, Work work)
		{
			workAreaLabel.Text = area.Name;

			timeLabel.Text = work.Start + " - " + work.Stop;
		}
    }
}
