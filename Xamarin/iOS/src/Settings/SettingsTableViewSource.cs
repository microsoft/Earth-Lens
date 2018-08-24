using System;
using Foundation;
using UIKit;

namespace EarthLens.iOS.Settings
{
    public partial class SettingsTableViewSource : UITableViewSource
    {

        private readonly string[] _settings;
        private readonly SettingsViewController _settingsViewController;

        public SettingsTableViewSource(SettingsViewController settingsViewController, string[] settings)
        {
            _settingsViewController = settingsViewController;
            _settings = settings;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(SettingCell.Key);
            if (cell != null)
            {
                cell.TextLabel.Text = _settings[indexPath.Row];
                return cell;
            }
            cell = new UITableViewCell(UITableViewCellStyle.Value1, SettingCell.Key)
            {
                Accessory = UITableViewCellAccessory.DisclosureIndicator
            };
            cell.TextLabel.Text = _settings[indexPath.Row];
            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return Constants.SettingsNumberOfRows; 
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return Constants.SettingsNumberOfSections;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            _settingsViewController.ShowView(indexPath.Row);
        }
    }
}

