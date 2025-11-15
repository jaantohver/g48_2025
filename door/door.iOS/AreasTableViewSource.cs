using System;
using System.Collections.Generic;

using UIKit;
using Foundation;

namespace door.iOS
{
    public class AreasTableViewSource : UITableViewSource
    {

        public event EventHandler<string> _RowSelected;

        public override nint RowsInSection(UITableView tableview, nint section) => 0;

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath) => 60;

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            AreaTableViewCell cell = new AreaTableViewCell(tableView.SeparatorInset.Left);

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            
        }
    }
}
