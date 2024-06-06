using System;
using System.Collections.Generic;

using UIKit;
using Foundation;

namespace ttki.iOS
{
    public class AreasTableViewSource : UITableViewSource
    {
        List<WorkArea> areas = new List<WorkArea>();

        public event EventHandler<string> _RowSelected;

        public override nint RowsInSection(UITableView tableview, nint section) => areas.Count;

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath) => 60;

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            AreaTableViewCell cell = new AreaTableViewCell(tableView.SeparatorInset.Left);
            cell.Update(areas[indexPath.Row]);

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            _RowSelected?.Invoke(this, areas[indexPath.Row].Uuid);
        }

        public void Update(List<WorkArea> areas, UITableView tableView)
        {
            this.areas = areas;

            tableView.ReloadData();
        }
    }
}
