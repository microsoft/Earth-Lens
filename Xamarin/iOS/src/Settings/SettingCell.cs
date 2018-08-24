using System;
using Foundation;
using UIKit;

namespace EarthLens.iOS.Settings
{
    public partial class SettingCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString(Constants.SettingCellKey);

        public SettingCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}
