using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace EarthLens.iOS.Gallery
{
    public partial class GalleryImageCell : UICollectionViewCell
    {
        public static readonly string Key = new NSString(Constants.GalleryImageCellKey);
        public UIImageView GalleryImageCheckMark { get; private set; }
        public UIImageView GalleryImageView { get; set;}
        private UILabel _timestamp;

        public GalleryImageCell (IntPtr handle) : base (handle)
        {
        }

        public void UpdateRow(UIImage img, DateTime creationTime, NSIndexPath indexPath)
        {
            SetGalleryImage(img);
            SetGalleryTimestamp(creationTime);
            if (!GalleryDataSource.SelectedImageIndexPaths.Contains(indexPath))
            {
                SetGalleryCheckmark();                
            }
        }
        
        public void SetGalleryCheckmark() 
        {
            GalleryImageCheckMark = GalleryImageCheck;
            GalleryImageCheckMark.Hidden = true;
        }

        public void CalculateCheckmarkFrameSize()
        {
            GalleryImageCheck.Frame = new CGRect(
                Frame.Width - Constants.GalleryCheckMarkXOffset - Constants.GalleryCheckMarkXPadding,
                Frame.Height - Constants.GalleryCheckMarkYOffset - Constants.GalleryCheckMarkYPadding,
                Constants.GalleryCheckMarkWidth,
                Constants.GalleryCheckMarkHeight);
        }

        private void SetGalleryImage(UIImage img)
        {
            GalleryImageView = GalleryImage;
            GalleryImage.Image = img;
            GalleryImage.Frame = new CGRect(
                0, 0, Frame.Width, Frame.Height - Constants.GalleryImageYOffset
            );
        }
        
        private void SetGalleryTimestamp(DateTime creationTime)
        {
            _timestamp = GalleryImageTimeStamp;
            _timestamp.Frame = new CGRect(
                0,
                Frame.Height - Constants.GalleryImageYOffset,
                Frame.Width,
                Constants.GalleryImageTimestampHeight
            );
            _timestamp.Text = creationTime.ToShortTimeString();
            _timestamp.TextAlignment = UITextAlignment.Right;

            ConfigureTimeStampAccessibilityAttributes();
        }

        #region Configure Accessibility Attributes
        private void ConfigureTimeStampAccessibilityAttributes()
        {
            _timestamp.IsAccessibilityElement = true;
            _timestamp.AccessibilityTraits = UIAccessibilityTrait.StaticText;
        }
        #endregion
    }
}