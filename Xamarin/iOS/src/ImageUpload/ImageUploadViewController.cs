using System;
using EarthLens.Models;
using CoreGraphics;
using Foundation;
using SkiaSharp.Views.iOS;
using UIKit;

namespace EarthLens.iOS.ImageUpload
{
    public partial class ImageUploadViewController : ImageUploadTemplate
    {
        public ImageUploadViewController(IntPtr handle) : base(handle)
        {
        }

        /// <summary>
        /// Sets up attributes, touch events of UI elements, launching from share extension, and an accessibility handler
        /// </summary>
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetButtonTouchEvents();
            LaunchFromShareExtension();
            SetViewButtonsAttributes();
            UIApplication.Notifications.ObserveContentSizeCategoryChanged((sender,args) => AccessibilityHandler());            
        }

        /// <summary>
        /// Retrieves image(s) from share extension and launches the analysis page.
        /// </summary>
        private void LaunchFromShareExtension()
        {
            if (!AppDelegate.LaunchedFromShareExtension)
            { 
                return; 
            }
            AppDelegate.LaunchedFromShareExtension = false;
            var sharedDefaults = new NSUserDefaults(SharedConstants.AppGroupID, NSUserDefaultsType.SuiteName);
            var imageData = sharedDefaults.DataForKey(SharedConstants.ImageKey);
            sharedDefaults[SharedConstants.ImageKey] = new NSData();
            sharedDefaults.Dispose();
            try 
            {
                var image = UIImage.LoadFromData(imageData);
                var imageName = string.Empty;
                ImageEntries.Add(new ImageEntry(image.ToSKImage(), imageName, DateTime.UtcNow, null));
                LaunchAnalysisScreen(ImageEntries);
            }
            catch (ArgumentNullException)
            {
                var alert = UIAlertController.Create(SharedConstants.ShareExtensionErrorAlertTitle,
                    SharedConstants.ShareExtensionErrorAlertMessage,
                    UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create(SharedConstants.ShareExtensionErrorAlertOkAction,
                    UIAlertActionStyle.Default, null));
                PresentViewController(alert, true, null);
            }
        }

        private void SetViewButtonsAttributes()
        {
            SetPhotoUploadButtonAttributes();
            SetGalleryButtonAtrributes();
        }
        private void AccessibilityHandler()
        {
            GalleryButton.SetNeedsLayout();
            PhotoUploadButton.SetNeedsLayout();
        }
       
        /// <summary>
        /// Sets photo upload button attributes.
        /// </summary>
        private void SetPhotoUploadButtonAttributes()
        {
            //get current view dimensions
            PhotoUploadButton.SizeToFit();
            var screenHeight = View.Frame.Height - Constants.NavBarHeight;
            var screenWidth = View.Frame.Width;
            SetButtonProperties(PhotoUploadButton);
            //adjust the button position after every rotation
            PhotoUploadButton.Frame = new CGRect(
                0,
                Constants.NavBarHeight,
                screenWidth,
                screenHeight / SharedConstants.NumberOfImageUploadComponents);
            PhotoUploadButton.SetImage(PhotoUploadButton.ImageView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal), UIControlState.Normal);
            var btnScaler = PhotoUploadButton.ImageView.Frame.Height / 2 - Constants.UploadBtnVerticalOffset;
            PhotoUploadButton.ImageEdgeInsets = new UIEdgeInsets(
                -btnScaler,
                (PhotoUploadButton.Frame.Width - PhotoUploadButton.ImageView.Frame.Width + Constants.UploadBtnHorizontalOffset) / 2,
                btnScaler,
                (PhotoUploadButton.Frame.Width - PhotoUploadButton.ImageView.Frame.Width - Constants.UploadBtnHorizontalOffset) / 2);
            PhotoUploadButton.TitleEdgeInsets = new UIEdgeInsets(btnScaler, - PhotoUploadButton.ImageView.Frame.Width, -btnScaler, 0);
            PhotoUploadButton.ContentEdgeInsets = new UIEdgeInsets(
                btnScaler,
                0,
                btnScaler,
                0);
        }

        /// <summary>
        /// Sets gallery button attributes.
        /// </summary>
        private void SetGalleryButtonAtrributes()
        {
            //get current view dimensions
            GalleryButton.SizeToFit();
            var screenHeight = View.Frame.Height - Constants.NavBarHeight;
            var screenWidth = View.Frame.Width;
            SetButtonProperties(GalleryButton);
            //adjust the button position after every rotation
            GalleryButton.Frame = new CGRect(
                0,
                screenHeight / SharedConstants.NumberOfImageUploadComponents + Constants.NavBarHeight,
                screenWidth,
                screenHeight / SharedConstants.NumberOfImageUploadComponents);
            GalleryButton.SetImage(GalleryButton.ImageView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysOriginal),UIControlState.Normal);
            var btnScaler = GalleryButton.ImageView.Frame.Height / 2 - Constants.UploadBtnVerticalOffset;
            GalleryButton.ImageEdgeInsets = new UIEdgeInsets(
                -btnScaler,
                (GalleryButton.Frame.Width - GalleryButton.ImageView.Frame.Width + Constants.UploadBtnHorizontalOffset) / 2,
                btnScaler,
                (GalleryButton.Frame.Width - GalleryButton.ImageView.Frame.Width - Constants.UploadBtnHorizontalOffset) / 2);
            GalleryButton.TitleEdgeInsets = new UIEdgeInsets(btnScaler, - GalleryButton.ImageView.Frame.Width, -btnScaler, 0);
            GalleryButton.ContentEdgeInsets = new UIEdgeInsets(
                btnScaler,
                0,
                btnScaler,
                0);
        }

        /// <summary>
        /// Sets button properties for the specified <see cref="UIButton"/>.
        /// </summary>
        /// <param name="btn">The specified <see cref="UIButton"/>.</param>
        private static void SetButtonProperties(UIButton btn)
        {
            btn.Layer.BorderWidth = Constants.UploadButtonBorderWidth;
            btn.Layer.BorderColor = Constants.UploadButtonBorderColor;  
            btn.AdjustsImageSizeForAccessibilityContentSizeCategory = true;
            btn.TitleLabel.AdjustsFontForContentSizeCategory = true;
        }

        /// <summary>
        /// Sets touch events for the buttons.
        /// </summary>
        private void SetButtonTouchEvents()
        {
            PhotoUploadButton.TouchUpInside += PhotoUploadButton_TouchUpInside;
            GalleryButton.TouchUpInside += GalleryButton_TouchUpInside;
            NavigationItem.HidesBackButton = true;
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);
            SetViewButtonsAttributes();
        }
    }
}
