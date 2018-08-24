using System;
using System.Collections.Generic;
using GMImagePicker;
using Photos;
using EarthLens.iOS.AnalyzingPage;
using EarthLens.iOS.Gallery;
using EarthLens.Models;
using CoreGraphics;
using SkiaSharp.Views.iOS;
using UIKit;

namespace EarthLens.iOS.ImageUpload
{
    public class ImageUploadTemplate : UIViewController
    {

        protected readonly UIViewController PresentParentViewController;
        protected UINavigationController ParentNavigationViewController { get; private set; }
        protected IList<ImageEntry> ImageEntries { get; private set; }

        private UIImagePickerController _imagePicker;
        private UIStoryboard _analyzingPageStoryBoard;
        private GMImagePickerController _photolibraryimagepicker;

        protected ImageUploadTemplate(UIViewController presentParentViewController)
        {
            PresentParentViewController = presentParentViewController;
            ParentNavigationViewController = NavigationController ?? presentParentViewController.NavigationController;
        }
        
        protected ImageUploadTemplate(IntPtr handle) : base(handle)
        {
            ParentNavigationViewController = NavigationController;
        }
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (NavigationController == null && ParentViewController != null)
            {
                ParentNavigationViewController = ParentViewController.NavigationController;

            } 
            else if (ParentNavigationViewController == null && NavigationController != null)
            {
                ParentNavigationViewController = NavigationController;
            }

            _analyzingPageStoryBoard = UIStoryboard.FromName(Constants.AnalyzingPageStoryboardName, null);
        }

        protected virtual void PhotoUploadButton_TouchUpInside(object sender, EventArgs e)
        {
            CreateImagePickerController();
        }

        protected virtual void GalleryButton_TouchUpInside(object sender, EventArgs e)
        {
            var galleryStoryBoard = UIStoryboard.FromName(Constants.GalleryStoryboardName, null);
            var galleryViewController = galleryStoryBoard.InstantiateInitialViewController() as GalleryViewController;

            ParentNavigationViewController.PushViewController(galleryViewController, true);
        }

        private void CreateImagePickerController()
        {
            _photolibraryimagepicker = new GMImagePickerController
            {
                Title = string.Empty,
                NavigationBarTextColor = Constants.ImagePickerNavigationBarTextColor,
                CustomSmartCollections = null,
                ColsInPortrait = Constants.NumberofCols,
                ColsInLandscape = Constants.NumberofCols,
                MinimumInteritemSpacing = Constants.NumberofInteritemSpacings
            };

            _photolibraryimagepicker.FinishedPickingAssets += Handle_PickedImages;
            ParentNavigationViewController.TopViewController.PresentViewController(_photolibraryimagepicker, false, null);
        }

        private void Handle_PickedImages(object sender, MultiAssetEventArgs e)
        {
            ImageEntries = new List<ImageEntry>();
            var phAssetArray = e.Assets;
            var manager = PHImageManager.DefaultManager;
            using (var options = new PHImageRequestOptions
            {
                Synchronous = true,
                DeliveryMode = PHImageRequestOptionsDeliveryMode.HighQualityFormat,
                ResizeMode = PHImageRequestOptionsResizeMode.Exact
            })
            {
                foreach (var asset in phAssetArray)
                {
                    if (asset.MediaType == PHAssetMediaType.Video)
                    {
                        HandleVideoSelected();
                        return;
                    }
                    manager.RequestImageForAsset(asset, new CGSize(asset.PixelWidth, asset.PixelHeight),
                        PHImageContentMode.Default, options, (result, info) =>
                        {
                            var resources = PHAssetResource.GetAssetResources(asset);
                            var name = resources != null && resources.Length > 0
                                ? resources[0].OriginalFilename
                                : string.Empty;
                            ImageEntries.Add(new ImageEntry(result.ToSKImage(), name, (DateTime) asset.CreationDate,
                                null));
                        });
                }

                LaunchAnalysisScreen(ImageEntries);
            }
        }

        private void HandleVideoSelected()
        {
            DismissViewController(true, null);
            var actionSheet = UIAlertController.Create(SharedConstants.VideoSelectedAlertTitle, 
                SharedConstants.VideoSelectedAlertMessage, UIAlertControllerStyle.Alert);

            actionSheet.AddAction(UIAlertAction.Create(SharedConstants.VideoSelectedAlertOkAction,
                                                       UIAlertActionStyle.Cancel,
                alert =>
                {
                    PresentViewController(_photolibraryimagepicker, false, null);
                }));

            PresentViewController(actionSheet, true, null);
        }

        protected void LaunchAnalysisScreen(IList<ImageEntry> imageEntries)
        {
            if (!(_analyzingPageStoryBoard.InstantiateInitialViewController()
                is AnalyzingPageViewController analyzingPageViewController))
            {
                return;
            }
            analyzingPageViewController.ImageEntries = imageEntries;
            if (_photolibraryimagepicker!=null)
            {
                analyzingPageViewController.CurrentImagePicker = _photolibraryimagepicker;
            }
            ParentNavigationViewController.PushViewController(analyzingPageViewController, true);
        }
    }
}