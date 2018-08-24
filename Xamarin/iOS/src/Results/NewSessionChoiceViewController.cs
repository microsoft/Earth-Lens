using System;
using System.Collections.Generic;
using CoreGraphics;
using UIKit;
using EarthLens.iOS.ImageUpload;

namespace EarthLens.iOS.Results
{
    public partial class NewSessionChoiceViewController : ImageUploadTemplate
    {
        public NewSessionChoiceViewController(UIViewController parentController) : base(parentController)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            PreferredContentSize = new CGSize(Constants.DropDownMenuWidth, Constants.DropDownMenuHeight);
            View.UserInteractionEnabled = true;

            //set event handler
            UploadPhotoButton.TouchUpInside += PhotoUploadButton_TouchUpInside;
            ChooseFromGalleryButton.TouchUpInside += GalleryButton_TouchUpInside;

            //set accessibility handler
            var btns = new[] {UploadPhotoButton, ChooseFromGalleryButton};
            UIApplication.Notifications.ObserveContentSizeCategoryChanged((sender,args) => AccessibilityHandler(btns));            
            SetButtonsAccessibility(btns);
        }

        private static void SetButtonsAccessibility(IEnumerable<UIButton> btns)
        {
            foreach (var btn in btns)
            {
                btn.TitleLabel.AdjustsFontForContentSizeCategory = true;       
            }
        }

        private static void AccessibilityHandler(IEnumerable<UIButton> btns)
        {
            foreach (var btn in btns)
            {
                btn.SetNeedsLayout();
            }
        }

        protected override void PhotoUploadButton_TouchUpInside(object sender, EventArgs e)
        {
            ParentNavigationViewController.PopToViewController(PresentParentViewController,false);
            DismissViewController(false, null);
            base.PhotoUploadButton_TouchUpInside(sender, e);
        }

        protected override void GalleryButton_TouchUpInside(object sender, EventArgs e)
        {
            ParentNavigationViewController.PopToViewController(PresentParentViewController, false);
            DismissViewController(false, null);
            base.GalleryButton_TouchUpInside(sender, e);
        }
    }
}