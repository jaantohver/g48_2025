using System;

using UIKit;
using CoreGraphics;

namespace door.iOS
{
	public class AreaTableViewCell : UITableViewCell
	{
		nfloat padding;

		readonly UILabel nameLabel;

		public AreaTableViewCell(nfloat padding)
		{
			this.padding = padding;

			SelectionStyle = UITableViewCellSelectionStyle.None;

			nameLabel = new UILabel();

			AddSubview(nameLabel);
		}

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

			nameLabel.Frame = new CGRect(
				padding,
				0,
				Frame.Width - padding,
				Frame.Height
			);
        }
    }
}
