using System;
using System.Linq;
using System.Collections.Generic;

using UIKit;
using Foundation;

namespace door.iOS
{
	public class WorksTableViewSource : UITableViewSource
	{
        List<Work> works = new List<Work>();
        List<WorkArea> areas = new List<WorkArea>();

        public override nint RowsInSection(UITableView tableview, nint section) => works.Count;

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath) => 60;

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            Work w = works[indexPath.Row];
            WorkArea a = areas.FirstOrDefault(x => x.Uuid == w.WorkArea);

            WorkTableViewCell cell = new WorkTableViewCell(tableView.SeparatorInset.Left);
            cell.Update(a, w);

            return cell;
        }

        public void Update(List<Work> works, List<WorkArea> areas, UITableView tableView)
        {
            this.works = works;
            this.areas = areas;

            tableView.ReloadData();
        }
    }
}