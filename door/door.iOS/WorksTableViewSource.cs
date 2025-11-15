using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using Foundation;

namespace door.iOS
{
	public class WorksTableViewSource : UITableViewSource
	{
        public override nint RowsInSection(UITableView tableview, nint section) => 0;

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath) => 60;

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            WorkTableViewCell cell = new WorkTableViewCell(tableView.SeparatorInset.Left);

            return cell;
        }
    }
}