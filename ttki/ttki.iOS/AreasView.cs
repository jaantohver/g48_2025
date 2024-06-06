using System;
using System.Collections.Generic;

using UIKit;
using CoreGraphics;

namespace ttki.iOS
{
	public class AreasView : UIView
	{
		readonly UILabel noAreasLabel;
		readonly UITableView tableView;

		public event EventHandler<string> RowSelected;

		public AreasView()
		{
			BackgroundColor = UIColor.SystemBackground;

			noAreasLabel = new UILabel();
            noAreasLabel.Font = UIFont.SystemFontOfSize(15);
            noAreasLabel.TextColor = UIColor.LightGray;
            noAreasLabel.TextAlignment = UITextAlignment.Center;
			noAreasLabel.Hidden = true;

			tableView = new UITableView();
			tableView.Source = new AreasTableViewSource();
			(tableView.Source as AreasTableViewSource)._RowSelected += (sender, e) => RowSelected?.Invoke(this, e);

			AddSubviews(tableView, noAreasLabel);
		}

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

			noAreasLabel.Frame = new CGRect(0, 0, Frame.Width, 200);

			tableView.Frame = Bounds;
        }

		public void Update(List<WorkArea> areas)
		{
			if (areas.Count == 0)
			{
				noAreasLabel.Text = "Töötajaga ei ole seotud ühtegi töömaad";
				noAreasLabel.Hidden = false;
            }
			else
			{
                noAreasLabel.Hidden = true;
                (tableView.Source as AreasTableViewSource).Update(areas, tableView);
			}
		}

		public void SetNoWorker()
		{
			noAreasLabel.Text = "Töötaja on isikustamata";
			noAreasLabel.Hidden = false;
		}
    }
}
