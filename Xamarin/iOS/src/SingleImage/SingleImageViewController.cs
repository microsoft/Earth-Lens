using System;
using EarthLens.iOS.Results;
using UIKit;
using System.Collections.Generic;

namespace EarthLens.iOS.SingleImage
{
    public partial class SingleImageViewController : ResultsViewController
    {
        /// <summary>
        /// Initializes a ViewController instance.
        /// </summary>
        public SingleImageViewController(IntPtr handle) : base(handle)
        {
        }

        /// <summary>
        /// Fetches the test image from URL, and prepares coordinates of the test bounding boxes.
        /// </summary>
        public override void ViewDidLoad()
        {
            SetUpViewController();
            SetStoryboardParameters();
            DisplayToolBar(ObjectBtn, ShareBtn, SaveBtn, SettingBtn);
            GenerateSideBar(SidebarClose);
            CreateFilterContent(Sidebar, ToggleView, SearchView);
            SetSideBarCategoryStates();
            View.BringSubviewToFront(SidebarView);
            UIApplication.Notifications.ObserveContentSizeCategoryChanged
            ((sender,args) => AccessibilityToolbarBtnHandler(
                ObjectBtn, ShareBtn, SaveBtn, SettingBtn, null
            ));
        }

        private void SetStoryboardParameters()
        {
            ThumbnailViewHeight = Constants.SingleImageThumbnailViewHeight;
            ToolBarView = ToolBar;
            SelectedClassTextView = SelectedClass;
            ClassCountTextView = ClassCount;
            SideBarView = SidebarView;
            ThresholdForHeight = Constants.ThresholdforHeight;
            Constraints = new Dictionary<string, NSLayoutConstraint>
            {
                {Constants.CloseButtonHeightConstraintKey, CloseBtnHeight},
                {Constants.SelectedClassHeightConstraintKey, SelectedClassHeight},
                {Constants.TextviewTopPaddingConstraintKey, TextviewTopPadding},
                {Constants.DefaultHeightConstraintKey, DefaultHeight},
                {Constants.ClassCountHeightConstraintKey, ClassCountHeight},
                {Constants.SidebarWidthConstraintKey, SidebarWidth},
                {Constants.SidebarYOffsetConstraintKey, sidebarYOffset},
                {Constants.ToggleHeightConstraintKey, ToggleHeight}
            };
        }

        partial void ObjectBtn_TouchUpInside(UIButton sender)
        {
            Constraints[Constants.DefaultHeightConstraintKey].Constant = View.Frame.Height - ThresholdForHeight * 2 - Constants.TopLayoutGuideHeight;

            ViewDetailsButton_TouchedUpInside();
        }

        partial void SidebarClose_TouchUpInside(UIButton sender)
        {
            base.SidebarClose_TouchUpInside(sender);
        }
 
        partial void ShareBtn_TouchUpInside(UIButton sender)
        {
            PresentSharePopover(ShareBtn);
        }

        protected override void SetSideBarCategoryStates()
        {
            foreach (var category in Categories)
            {
                SidebarController.CheckboxState.Add(category, true);
            }
        }
    }
}
