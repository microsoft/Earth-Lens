using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EarthLens.iOS.ClassExtensions;
using EarthLens.iOS.Gallery;
using EarthLens.iOS.Sidebar;
using EarthLens.Models;
using EarthLens.Services;
using CoreGraphics;
using Foundation;
using SkiaSharp.Views.iOS;
using UIKit;
using EarthLens.iOS.Popover;
using SkiaSharp;
using EarthLens.iOS.Settings;

namespace EarthLens.iOS.Results
{
    public abstract partial class ResultsViewController : UIViewController
    {
        public ImageEntry InputImageEntry { get; set; }

        protected UIView ToolBarView { get; set; }
        protected UITextView SelectedClassTextView { get; set; }
        protected UITextView ClassCountTextView { get; set; }
        protected Dictionary<Observation, UIButton> Bindings { get; }
        protected double ThumbnailViewHeight { get; set; }
        protected FilterViewController SidebarController { get; private set; }
        protected UIScrollView ScrollView { get; private set; }
        protected UIImageView ImageView { get; private set; }
        protected double ImgWidth { get; private set; }
        protected double ImgHeight { get; private set; }
        protected double NavBarHeight { get; private set; }
        protected double ViewHeight { get; private set; }
        protected List<Category> Categories { get; set; }
        protected UIView SideBarView { get; set; }
        protected UIScrollView TimeLineView { get; set; }
        protected float ThresholdForHeight { get; set; }
        protected Dictionary<string, NSLayoutConstraint> Constraints { get; set; }

        private UIViewController _lastVisitedPopover;
        private NSObject _lastTappedButton;
        private UIViewController _dismissedPopover;
        private UIView _presentingNotification;
        private UIViewController _presentingNewSesionDropDownMenu;
        private double _fullWidth;
        private double _fullHeight;
        private double _toolBarHeight;
        private bool _sessionSaved;
        private UIPopoverController _sharePopoverController;
        private UIButton _shareButton;
        private bool _showSharePopover;
        private nfloat _sideBarRatio;
        private UIButton _lastClickedBoundingBox;


        protected ResultsViewController(IntPtr handle) : base(handle)
        {
            Bindings = new Dictionary<Observation, UIButton>();
        }

        public override void ViewWillAppear(bool b)
        {
            base.ViewWillAppear(b);
            _presentingNewSesionDropDownMenu?.DismissViewController(false, null);
            GenerateTitle();

            HandleRotation();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NavigationItem.Title = @"Back to Session";
        }

        protected void SetUpViewController()
        {
            // Id is null for images that aren't in the DB yet
            _sessionSaved = InputImageEntry.Id != null;
            OrderButtonBySize();
            CalculateScaledDimensions(InputImageEntry.Image);
            CreateImageView();
            CreateScrollView();
            DisplayBoundingBoxes();
            SetZoomProperties();
            GenerateTitle();
            GenerateNewSessionButton();
            GenerateViewGalleryButton();
        }

        public void UpdateTitleForUnsaved()
        {
            // No need to update if we already had an unsaved session
            if (!_sessionSaved) return;

            _sessionSaved = false;
            GenerateTitle();
        }

        protected void GenerateTitle()
        {
            var time = InputImageEntry.CreationTime ?? DateTime.Now;

            // We display the time as UTC for all users, regardless of where they're located
            time = time.ToUniversalTime();

            var formattedDate = time.ToShortDateString();
            var formattedTime = time.ToLongTimeString();

            var imageName = string.IsNullOrWhiteSpace(InputImageEntry.Name)
                ? string.Empty
                : string.Format(CultureInfo.CurrentCulture, SharedConstants.ImageTitleFormat, InputImageEntry.Name);

            var title = string.Format(CultureInfo.CurrentCulture, SharedConstants.PageTitleFormat, imageName,
                formattedDate, formattedTime);
            if (_sessionSaved)
            {
                title += SharedConstants.SavedLabel;
            }

            var titleLabel = new UILabel
            {
                Text = title,
                Font = UIFont.BoldSystemFontOfSize(Constants.ResultsNavTitleFontSize)
            };

            titleLabel.SizeToFit();

            NavigationItem.Title = title;
            NavigationItem.TitleView = titleLabel;

        }

        private void GenerateNewSessionButton()
        {
            var newSessionButton = new UIBarButtonItem { Title = Constants.NewSessionTitle };

            newSessionButton.Clicked += (o, e) =>
            {
                ClearPopover();

                var newSessionPopup =
                    new NewSessionChoiceViewController(this)
                    {
                        ModalPresentationStyle = UIModalPresentationStyle.Popover
                    };

                var presentationSourceController = newSessionPopup.PopoverPresentationController;
                if (presentationSourceController != null)
                {
                    presentationSourceController.BarButtonItem = newSessionButton;
                    presentationSourceController.ContainerViewWillLayoutSubviews();
                    presentationSourceController.PermittedArrowDirections = UIPopoverArrowDirection.Up;
                    presentationSourceController.BackgroundColor = UIColor.White;
                }
                _presentingNewSesionDropDownMenu = newSessionPopup;
                PresentModalViewController(newSessionPopup,false);
            };

            NavigationItem.RightBarButtonItem = newSessionButton;
            NavigationItem.HidesBackButton = true;
        }

        private void GenerateViewGalleryButton()
        {
            var viewGalleryButton = new UIBarButtonItem { Title = Constants.ViewGalleryTitle };

            var galleryStoryBoard = UIStoryboard.FromName(Constants.GalleryStoryboardName, null);
            var galleryViewController =
                galleryStoryBoard.InstantiateInitialViewController() as GalleryViewController;
            NavigationItem.LeftBarButtonItem = viewGalleryButton;
            viewGalleryButton.Clicked += (sender, e) =>
            {
                ClearPopover();
                NavigationItem.BackBarButtonItem = new UIBarButtonItem(Constants.GalleryBackButtonTitle,
                    UIBarButtonItemStyle.Plain, null);
                NavigationController.PushViewController(galleryViewController, true);
            };
        }

        protected virtual void HandleSaveToGalleryYes()
        {
            InputImageEntry.Id = DatabaseService.InsertOrUpdate(InputImageEntry).Id;
            _sessionSaved = true;
            GenerateTitle();
        }

        private static void HandleSaveToGalleryNo()
        {
        }

        private void DisplaySaveToGalleryAlert(object sender, EventArgs e)
        {
            var alertController = UIAlertController.Create(
                SharedConstants.SaveToGalleryTitle,
                SharedConstants.SaveToGalleryDescription,
                UIAlertControllerStyle.Alert);

            alertController.AddAction(
                UIAlertAction.Create(
                    SharedConstants.SaveToGalleryYes,
                    UIAlertActionStyle.Default,
                    action => HandleSaveToGalleryYes()));

            alertController.AddAction(
                UIAlertAction.Create(
                    SharedConstants.SaveToGalleryNo,
                    UIAlertActionStyle.Default,
                    action => HandleSaveToGalleryNo()));

            alertController.AddAction(
                UIAlertAction.Create(
                    SharedConstants.SaveToGalleryCancel,
                    UIAlertActionStyle.Cancel,
                    action => HandleSaveToGalleryNo()));

            var presentationAlert = alertController.PopoverPresentationController;

            if (presentationAlert != null)
            {
                presentationAlert.SourceView = View;
                presentationAlert.PermittedArrowDirections = UIPopoverArrowDirection.Up;
            }

            PresentViewController(alertController, true, null);
        }

        private void LaunchSettingsPage(object sender, EventArgs e)
        {
            ClearPopover();
            var settingsStoryBoard = UIStoryboard.FromName(Constants.SettingsStoryboardName, null);
            var settingsViewController =
                settingsStoryBoard.InstantiateInitialViewController() as SettingsViewController;
            NavigationController.PushViewController(settingsViewController, true);
        }

        protected void CalculateScaledDimensions(SKImage image)
        {
            if (image == null)
            {
                throw new ArgumentNullException();
            }

            NavBarHeight = Constants.ResultsNavBarHeight;
            _toolBarHeight = Constants.ResultsToolBarHeight;
            ViewHeight = View.Frame.Height - NavBarHeight - _toolBarHeight - ThumbnailViewHeight;

            var imgRatio = image.Width / (double) image.Height;
            var screenRatio = View.Frame.Width / ViewHeight;
            if (screenRatio > imgRatio)
            {
                ImgWidth = image.Width * (ViewHeight / image.Height);
                ImgHeight = ViewHeight;
            }
            else
            {
                ImgWidth = View.Frame.Width;
                ImgHeight = image.Height * (View.Frame.Width / image.Width);
            }
        }

        private void CreateImageView()
        {
            ImageView = new UIImageView(new CGRect(0, 0, ImgWidth, ImgHeight))
            {
                Image = InputImageEntry.Image.ToUIImage(),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };
        }

        private void CreateScrollView()
        {
            View.BackgroundColor = Constants.ScrollViewBackgroundColor;
            ScrollView = new UIScrollView(new CGRect((View.Frame.Width - ImgWidth) / 2,
                ((ViewHeight - ImgHeight) / 2 + NavBarHeight), ImgWidth, ImgHeight));
            View.AddSubview(ScrollView);
            ScrollView.ContentSize = ImageView.Image.Size;
            ScrollView.BouncesZoom = false;
            ScrollView.AddSubview(ImageView);
            ImageView.ClipsToBounds = true;
            NSLayoutConstraint.Create(ImageView, NSLayoutAttribute.Top, NSLayoutRelation.Equal,
                                      ScrollView, NSLayoutAttribute.Top, 1f, 0.0f).Active = true;
            NSLayoutConstraint.Create(ImageView, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal,
                                      ScrollView, NSLayoutAttribute.Bottom, 1f, 0.0f).Active = true;
        }

        private void SetZoomProperties()
        {
            ScrollView.MaximumZoomScale = 3f;
            ScrollView.MinimumZoomScale = 1f;
            ScrollView.ViewForZoomingInScrollView += sv => ImageView;
            ScrollView.DidZoom += ScrollView_DidZoom;
            ScrollView.Bounces = false;

            // Define Dragging motion handler
            ScrollView.DraggingStarted += (sender, e) => DismissPopoverViewController(_lastVisitedPopover);
            ScrollView.DraggingEnded += (sender, e) =>
            {
                KillScroll();
                if (!ScrollView.Dragging)
                {
                    PresentPopoverViewController();
                }
            };

            // Define Deceleration motion handler after user lifts finger from screen
            ScrollView.DecelerationEnded += (sender, e) => PresentPopoverViewController();

            // Define zooming event handler
            ScrollView.ZoomingStarted += (sender, e) => DismissPopoverViewController(_lastVisitedPopover);
            ScrollView.ZoomingEnded += (sender, e) => PresentPopoverViewController();
        }

        private void ScrollView_DidZoom(object sender, EventArgs e)
        {
            CalculateFullScreenDimensions();
            if (ScrollView.ZoomScale == 1)
            {
                ImageView.Frame = new CGRect(0, 0, ImgWidth, ImgHeight);
            }
            UpdateBoundingBoxes();
        }

        private void CalculateFullScreenDimensions()
        {
            _fullWidth = ScrollView.ZoomScale * (ImgWidth);
            _fullHeight = ScrollView.ZoomScale * (ImgHeight);
            ViewHeight = View.Frame.Height - NavBarHeight - _toolBarHeight - ThumbnailViewHeight;

            if (_fullWidth > View.Frame.Width && _fullHeight > ViewHeight)
            {
                ScrollView.Frame = new CGRect((View.Frame.Width - _fullWidth) / 2,
                    (((ViewHeight - _fullHeight) / 2) + NavBarHeight), View.Frame.Width, ViewHeight);
            }
            else if (_fullWidth > View.Frame.Width && _fullHeight <= ViewHeight)
            {
                ScrollView.Frame = new CGRect((View.Frame.Width - _fullWidth) / 2,
                    (((ViewHeight - _fullHeight) / 2) + NavBarHeight), View.Frame.Width, _fullHeight);
            }
            else if (_fullWidth <= View.Frame.Width && _fullHeight > ViewHeight)
            {
                ScrollView.Frame = new CGRect((View.Frame.Width - _fullWidth) / 2,
                    (((ViewHeight - _fullHeight) / 2) + NavBarHeight), _fullWidth, ViewHeight);
            }
            else
            {
                ScrollView.Frame = new CGRect((View.Frame.Width - _fullWidth) / 2,
                    (((ViewHeight - _fullHeight) / 2) + NavBarHeight), _fullWidth, _fullHeight);
            }
            ScrollView.Center = new CGPoint(View.Frame.Width / 2, ((ViewHeight / 2) + NavBarHeight));
        }

        protected static void AccessibilityToolbarBtnHandler(UIButton objectButton, UIButton shareButton, UIButton saveButton, UIButton
            settingsButton, UIButton dataVisualizerButton)
        {
            objectButton.SetNeedsLayout();
            shareButton.SetNeedsLayout();
            saveButton.SetNeedsLayout();
            settingsButton.SetNeedsLayout();
            dataVisualizerButton?.SetNeedsLayout();
        }
        
        /// <summary>
        /// Sets attributes in toolbar.
        /// </summary>
        protected void DisplayToolBar(UIButton objectButton, UIButton shareButton, UIButton saveButton, UIButton
            settingsButton)
        {
            if (objectButton == null || shareButton == null || saveButton == null || settingsButton == null)
            {
                throw new ArgumentNullException();
            }
            // set toolbar buttons margins
            objectButton.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, Constants.ImageEdgeToolbar);
            shareButton.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, Constants.ImageEdgeToolbar);
            saveButton.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, Constants.ImageEdgeToolbar);
            settingsButton.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, Constants.ImageEdgeToolbar);

            // set toolbar buttons to be accessible
            objectButton.TitleLabel.AdjustsFontForContentSizeCategory = true;
            shareButton.TitleLabel.AdjustsFontForContentSizeCategory = true;
            saveButton.TitleLabel.AdjustsFontForContentSizeCategory = true;
            settingsButton.TitleLabel.AdjustsFontForContentSizeCategory = true;

            //set toolbar buttons event handler
            saveButton.TouchUpInside += DisplaySaveToGalleryAlert;
            settingsButton.TouchUpInside += LaunchSettingsPage;
            ConfigureToolBarAccessibilityAttributes();
            View.BringSubviewToFront(ToolBarView);
        }
        
        /// <summary>
        /// Sets attributes in toolbar for timelineview.
        /// </summary>
        protected void DisplayToolBar(UIButton objectButton, UIButton shareButton, UIButton saveButton, UIButton settingsButton, UIButton dataVisualizerButton)
        {
            if (dataVisualizerButton == null)
            {
                throw new ArgumentNullException();
            }
            DisplayToolBar(objectButton, shareButton, saveButton, settingsButton);
            dataVisualizerButton.ImageEdgeInsets = new UIEdgeInsets(0, 0, 0, Constants.ImageEdgeToolbar);
            dataVisualizerButton.TitleLabel.AdjustsFontForContentSizeCategory = true;
        }

        /// <summary>
        /// Presents the UIActivity View Controller to export the NSMutable Data as a PDF. 
        /// </summary>
        protected void PresentSharePopover(UIButton shareButton)
        {
            ClearPopover();
            _shareButton = shareButton;
            var activityItems = new NSObject[] { GeneratePDF() };
            var activityController = new UIActivityViewController(activityItems, null);
            _sharePopoverController = new UIPopoverController(activityController);
            _sharePopoverController.PresentFromRect(shareButton.Frame, ToolBarView, UIPopoverArrowDirection.Down, true);
            _sharePopoverController.DidDismiss += _sharePopoverController_DidDismiss;
            _showSharePopover = true;
        }

        /// <summary>
        /// Set showpopover bool to false when dismissed.
        /// </summary>
        private void _sharePopoverController_DidDismiss(object sender, EventArgs e)
        {
            _showSharePopover = false;
        }

        /// <summary>
        /// Generates data required to export a PDF. 
        /// </summary>
        private NSMutableData GeneratePDF()
        {
            // generate an image with bounding boxes
            var generatedImage = GraphicService.DrawObservations(InputImageEntry.Image, InputImageEntry.Observations.ToArray());
            var pdfGenerator = new Services.PDFGeneration(generatedImage, InputImageEntry);

            return pdfGenerator.GeneratePDF();
        }

        protected void ViewDetailsButton_TouchedUpInside()
        {
            DisableBackgroundBoundingBoxVoiceOver();
            UIAccessibility.PostNotification(UIAccessibilityPostNotification.ScreenChanged, ClassCountTextView);

            if (_lastTappedButton != null && SideBarView.Hidden)
            {
                var frameinView = ScrollView.ConvertRectToView(((UIButton)_lastTappedButton).Frame, View);
                if (frameinView.IntersectsWith(SideBarView.Frame))
                {
                    ClearPopover();
                }
            }
            SideBarView.Hidden = !SideBarView.Hidden;
        }

        protected void ClearPopover()
        {
            DismissPopoverViewController(_lastVisitedPopover);
            _lastVisitedPopover = null;
            _dismissedPopover = null;
            _lastTappedButton = null;
        }

        protected void SidebarCloseButtonTouched()
        {
            SideBarView.Hidden = true;

            SidebarController.DismissKeyboard();
        }

        /// <summary>
        /// Set up side bar atttributes.
        /// </summary>
        protected void GenerateSideBar(UIButton sidebarClose)
        {
            if (sidebarClose == null)
            {
                throw new ArgumentNullException();
            }

            SideBarView.BackgroundColor = Constants.SidebarColor;
            sidebarClose.SetImage(new UIImage(Constants.CloseBtnImage), UIControlState.Normal);
            sidebarClose.TintColor = Constants.CloseBtnColor;

            ConfigureSideBarCloseButtonAccessibilityAttributes(sidebarClose);
        }

        /// <summary>
        /// Set up side bar content atttributes.
        /// </summary>
        protected void CreateFilterContent(UIStackView sidebar, UIView toggleView, UIView searchView)
        {
            if (Constraints == null || sidebar == null || toggleView == null)
            {
                throw new ArgumentNullException();
            }

            //pass parameter to table view
            SidebarController = new FilterViewController
            {
                NumberSelected = SelectedClassTextView,
                Bindings = Bindings,
                SearchView = searchView,
                ParentResultsViewController = this,
                DisplayItems = Categories,
                SideBarView = SideBarView,
                Constraints = Constraints,
            };

            ConfigureSidebarTitle();

            Constraints[Constants.DefaultHeightConstraintKey].Constant = View.Frame.Height - ThresholdForHeight * 2 - Constants.TopLayoutGuideHeight;

            // set text contents in sidebar header
            sidebar.AddArrangedSubview(SidebarController.TableView);
            _sideBarRatio = SideBarView.Frame.Height / View.Frame.Height;

            // add gesture to side bar to enable sidebar expansion
            var slideOnSideBar = new UIPanGestureRecognizer();
            slideOnSideBar = new UIPanGestureRecognizer(swipedOnViewAction =>
            {
                // Set up threshold for min and max y position of sidebar
                var yLocationTouched = slideOnSideBar.LocationInView(View).Y;
                var yOffsetMin = SideBarView.Frame.Y + Constraints[Constants.CloseButtonHeightConstraintKey].Constant +
                                 Constraints[Constants.SelectedClassHeightConstraintKey].Constant +
                                 Constraints[Constants.TextviewTopPaddingConstraintKey].Constant;
                var yOffsetMax = View.Frame.Height - ThresholdForHeight - TopLayoutGuide.Length;
                if (yLocationTouched < yOffsetMin || yLocationTouched > yOffsetMax) return;
                // Calculate new view frame after expand/collapse 
                Constraints[Constants.DefaultHeightConstraintKey].Constant =
                    yLocationTouched - TopLayoutGuide.Length -
                    Constraints[Constants.ClassCountHeightConstraintKey].Constant -
                    Constraints[Constants.TextviewTopPaddingConstraintKey].Constant;
                SideBarView.LayoutIfNeeded();
                SidebarController.TableView.Frame =
                    new CGRect(0,
                        Constraints[Constants.CloseButtonHeightConstraintKey].Constant,
                        Constraints[Constants.SidebarWidthConstraintKey].Constant,
                        yLocationTouched - TopLayoutGuide.Length -
                        Constraints[Constants.SidebarYOffsetConstraintKey].Constant -
                        Constraints[Constants.CloseButtonHeightConstraintKey].Constant -
                        Constraints[Constants.ToggleHeightConstraintKey].Constant);
                toggleView.Frame =
                    new CGRect(Constants.SidebarBorderWidth,
                        yLocationTouched - Constraints[Constants.CloseButtonHeightConstraintKey].Constant + Constants.ToggleYOffset,
                        Constraints[Constants.SidebarWidthConstraintKey].Constant,
                        Constraints[Constants.ToggleHeightConstraintKey].Constant);
                toggleView.LayoutIfNeeded();

                if (_lastTappedButton != null)
                {
                    var frameinView = ScrollView.ConvertRectToView(((UIButton)_lastTappedButton).Frame, View);
                    if (frameinView.IntersectsWith(SideBarView.Frame))
                    {
                        ClearPopover();
                    }
                }
            });
            SideBarView.ClipsToBounds = true;
            toggleView.AddGestureRecognizer(slideOnSideBar);

            NSLayoutConstraint.Create(sidebar, NSLayoutAttribute.Height, NSLayoutRelation.Equal,
                null, NSLayoutAttribute.NoAttribute, Constants.Multiplier,
                View.Frame.Height - Constants.ThresholdforHeight).Active = true;
            NSLayoutConstraint.Create(toggleView, NSLayoutAttribute.Top, NSLayoutRelation.Equal,
                SidebarController.TableView, NSLayoutAttribute.Bottom, Constants.Multiplier,
                Constants.SidebarBorderWidth).Active = true;
            View.BringSubviewToFront(sidebar);
        }

        /// <summary>
        /// Configures the top title of sidebar
        /// </summary>
        protected void ConfigureSidebarTitle()
        {
            var distinctObservations = InputImageEntry.Observations.Select(o => o.Category).Distinct();
            var numberofClasses = distinctObservations.Count();
            var numberofSelectedClasses = 0;

            foreach(var category in distinctObservations)
            {
                var boundingBoxes = Bindings.ToList().FindAll(binding => binding.Key.Category.Equals(category)).Select(box => box.Value);
                if(boundingBoxes.All(binding => binding.Hidden == false))
                {
                    numberofSelectedClasses++;
                }
            }
            ClassCountTextView.Text = string.Format(CultureInfo.CurrentCulture,
                numberofClasses > 1 ? SharedConstants.ClassCountFormatPlural : SharedConstants.ClassCountFormatSingular,
                numberofClasses);
            ClassCountTextView.Font = UIFont.GetPreferredFontForTextStyle(UIFontTextStyle.Headline);
            ClassCountTextView.AdjustsFontForContentSizeCategory = true;
            SelectedClassTextView.Text = string.Format(CultureInfo.CurrentCulture,
                numberofSelectedClasses > 1
                    ? SharedConstants.SidebarSubtitleFormatPlural
                    : SharedConstants.SidebarSubtitleFormatSingular,
                    numberofSelectedClasses, numberofSelectedClasses);
            SelectedClassTextView.Font = UIFont.GetPreferredFontForTextStyle(UIFontTextStyle.Body);
            SelectedClassTextView.AdjustsFontForContentSizeCategory = true;
        }

        protected virtual void SidebarClose_TouchUpInside(UIButton sender)
        {
            SidebarCloseButtonTouched();
            EnableBackgroundBoundingBoxVoiceOver();
        }

        /// <summary>
        /// Generate popovers on bounding boxes
        /// </summary>
        private void DisplayPopover(Observation observation, UIButton button)
        {
            _lastClickedBoundingBox = button;

            // Create a popover view controller
            var popover = new PopoverViewController
            {
                Observation = observation,
                ModalPresentationStyle = UIModalPresentationStyle.Popover,
                ParentFilterViewController = SidebarController,
            };

            if (CheckForPopoverOverlap(button))
            {
                ClearPopover();
                return;
            }

            // Present the popover
            PresentViewController(popover, false, null);
            var presentationPopover = popover.PopoverPresentationController;
            ScrollView.AddGestureRecognizer(new UITapGestureRecognizer(tap =>
            {
                if (_lastVisitedPopover == null) return;
                _lastVisitedPopover.DismissViewController(false, null);
                _lastVisitedPopover = null;
            }));
            if (presentationPopover != null)
            {
                presentationPopover.PassthroughViews = TimeLineView != null ? new[] { TimeLineView,
                    NavigationController.NavigationBar, SideBarView, ScrollView, ToolBarView } : new[]
                    { NavigationController.NavigationBar, SideBarView, ScrollView, ToolBarView };
                presentationPopover.SourceView = button;
                presentationPopover.ContainerViewWillLayoutSubviews();
                presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Any;
                presentationPopover.SourceRect = button.Bounds;
            }

            // add current popover to the list
            _lastVisitedPopover = popover;
        }

        private bool CheckForPopoverOverlap(UIButton button)
        {
            if (button == null) return false;
            var frameinView = ScrollView.ConvertRectToView(button.Frame, View);
            if (TimeLineView != null)
            {
                if ((frameinView.IntersectsWith(SideBarView.Frame) && !SideBarView.Hidden) || 
                    frameinView.IntersectsWith(NavigationController.NavigationBar.Frame) || 
                    frameinView.IntersectsWith(ToolBarView.Frame) || frameinView.IntersectsWith(TimeLineView.Frame)) 
                {
                    return true;
                }
            }
            else
            {
                if ((frameinView.IntersectsWith(SideBarView.Frame) && !SideBarView.Hidden) || 
                    frameinView.IntersectsWith(NavigationController.NavigationBar.Frame) || 
                    frameinView.IntersectsWith(ToolBarView.Frame))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Dismisses share popover view controller if open.
        /// </summary>
        public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillRotate(toInterfaceOrientation, duration);
            if (_sharePopoverController == null || !_showSharePopover) return;
            _sharePopoverController.Dismiss(false);
            _showSharePopover = true;
        }

        /// <summary>
        /// Re-draws all bounding boxes on the screen given the new screen orientation.
        /// </summary>
        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);
            HandleRotation();
        }

        /// <summary>
        /// Handles rotation operations.
        /// </summary>
        protected virtual void HandleRotation()
        {
            CalculateScaledDimensions(InputImageEntry.Image);
            if (ScrollView.ZoomScale == 1)
            {
                ImageView.Frame = new CGRect(0, 0, ImgWidth, ImgHeight);
            }
            CalculateFullScreenDimensions();
            UpdateBoundingBoxes();
            if (_showSharePopover)
            {
                _sharePopoverController.PresentFromRect(_shareButton.Frame, ToolBarView, UIPopoverArrowDirection.Down,
                    true);
            }

            // Re-calculate the position of colour notification
            UpdateColourNotification();

            UpdateSideBarHeight();

            if (_lastVisitedPopover != null)
            {
                ClearPopover();
            }
        }

        /// <summary>
        /// Should be called when user rotates device, the height of the sidebar will be recalculated
        /// </summary>
        private void UpdateSideBarHeight()
        {
            _sideBarRatio = SideBarView.Frame.Height / View.Frame.Height;

            var bottomGap = View.Frame.Height - TopLayoutGuide.Length - 
                Constraints[Constants.SidebarYOffsetConstraintKey].Constant - (View.Frame.Height * _sideBarRatio) - 
                Constants.BottomMargin - Constants.ResultsToolBarHeight - ThumbnailViewHeight;

            if (bottomGap < ThresholdForHeight)
            {
                var diff = Constants.ThresholdforHeight - bottomGap;
                Constraints[Constants.DefaultHeightConstraintKey].Constant = (View.Frame.Height * _sideBarRatio) - (nfloat)diff;
            }
            else
            {
                Constraints[Constants.DefaultHeightConstraintKey].Constant = View.Frame.Height * _sideBarRatio;
            } 
        }

        /// <summary>
        /// Should be called when user rotates device, the position of the notification will be re-calculated
        /// </summary>
        private void UpdateColourNotification()
        {
            // If a notification is presenting while user is rotating device, re-calculate position
            if (_presentingNotification != null)
            {
                _presentingNotification.Frame = new CGRect(View.Frame.Width - Constants.ColourDuplicateNoteWidth,
                    View.Frame.Height * Constants.ColourDuplicateNotePositionRatio, Constants.ColourDuplicateNoteWidth,
                    Constants.ColourDuplicateNoteHeight);
            }
        }

        /// <summary>
        /// Should be called by <see cref="FilterViewController"/> when user choose same colour more than once
        /// </summary>
        /// <param name="colour">Colour.</param>
        public void NotifyColourDuplicated(CGColor colour)
        {
            var categories = Categories.FindAll(category => category.Color.Equals(colour.ToSKColor()))
                .Select(category => category.Label);
            var conflictClasses = string.Join(Constants.ConflictClassJoinString, categories.ToArray());
            CreateNotification(conflictClasses, colour);
        }

        /// <summary>
        /// Creates all the UI elements in notification view programmatically.
        /// </summary>
        /// <param name="conflictClasses">Confilct classes.</param>
        /// <param name="confilctColor">Confilct color.</param>
        private void CreateNotification(string conflictClasses, CGColor confilctColor)
        {
            // Create UI elements
            var containerView = new UIView();
            var notification = new UITextView();
            var colourBlock = new UIView();

            // Set attributes for notification container
            containerView.Frame = new CGRect(View.Frame.Width,
                View.Frame.Height * Constants.ColourDuplicateNotePositionRatio, Constants.ColourDuplicateNoteWidth,
                Constants.ColourDuplicateNoteHeight);
            containerView.BackgroundColor = Constants.ColourDuplicateNoteColor;
            containerView.Layer.Opacity = Constants.ColourDuplicateNoteOpacity;
            containerView.Layer.CornerRadius = Constants.ColourDuplicateNoteCornerRadius;
            containerView.AddSubview(notification);
            containerView.AddSubview(colourBlock);

            // Set attributes for the block view shows the duplicate chosen colour
            colourBlock.BackgroundColor = confilctColor.ToSKColor().ToUIColor();
            colourBlock.Frame = new CGRect(Constants.ColourDuplicateNoteMargin,
                (containerView.Frame.Height - Constants.ColourButtonSize) / 2,
                Constants.ColourButtonSize, Constants.ColourButtonSize);
            colourBlock.Layer.CornerRadius = Constants.ColourButtonSize / 2;

            // Set string attributes
            var notificationText = string.Format(CultureInfo.CurrentCulture, SharedConstants.NotificationTextFormat,
                conflictClasses);
            notification.AttributedText = GenerateAttributedString(notificationText);
            notification.SizeToFit();
            notification.Frame = new CGRect(Constants.ColourDuplicateNoteMargin + Constants.ColourButtonSize,
                (containerView.Frame.Height - Constants.ColourDuplicateNoteTextHeight) / 2,
                containerView.Frame.Width - Constants.ColourButtonSize - Constants.ColourDuplicateNoteMargin,
                Constants.ColourDuplicateNoteTextHeight);
            View.AddSubview(containerView);
            _presentingNotification = containerView;

            // Set the slide in animation
            SetupSlidingAnimation(containerView);

            // Set the fade out animation after 2 seconds
            SetupFadeAnimation(containerView);

            ConfigureSameColourNotificationAccessibilityAttributes(notificationText);
        }

        /// <summary>
        /// Generates the attributed string for text in notification view.
        /// </summary>
        /// <returns>The attributed string.</returns>
        /// <param name="text">Text.</param>
        private static NSAttributedString GenerateAttributedString(string text)
        {
            var atts = new UIStringAttributes();
            var attributedString = new NSMutableAttributedString(text, atts);
            attributedString.BeginEditing();

            // Set class names to bold font
            attributedString.AddAttribute(UIStringAttributeKey.Font,
                UIFont.BoldSystemFontOfSize(Constants.ColourDuplicateNoteFontSize),
                new NSRange(Constants.ConflicClassStartPosition, text.Length - Constants.ConflicClassStartPosition));

            // Text other than class names set to normal font
            attributedString.AddAttribute(UIStringAttributeKey.Font,
                UIFont.SystemFontOfSize(Constants.ColourDuplicateNoteFontSize),
                new NSRange(0, Constants.ConflicClassStartPosition - 1));
            attributedString.EndEditing();
            return attributedString;
        }

        /// <summary>
        /// Setups the fade out animation for notification view.
        /// </summary>
        /// <param name="containerView">Container view.</param>
        private static void SetupFadeAnimation(UIView containerView)
        {
            UIView.BeginAnimations(Constants.FadeOutAnimation);

            var delay = UIAccessibility.IsVoiceOverRunning ? AccessibilityConstants.SlidingAnimationDurationWithVoiceOver : 
                                                         Constants.FadeOutAnimationDelay;

            UIView.Animate(Constants.FadeOutAnimationDuration, delay,
                UIViewAnimationOptions.TransitionNone,
                () =>
                {
                    containerView.Alpha = Constants.FadeOutAnimationTransparency;
                }, null);
            UIView.CommitAnimations();
        }

        /// <summary>
        /// Setups the sliding in animation for notification view.
        /// </summary>
        /// <param name="containerView">Container view.</param>
        private void SetupSlidingAnimation(UIView containerView)
        {
            UIView.BeginAnimations(Constants.SlidingAnimation);
            UIView.SetAnimationDuration(Constants.SlidingAnimationDuration);
            UIView.SetAnimationDelegate(this);
            containerView.Frame = new CGRect(View.Frame.Width - Constants.ColourDuplicateNoteWidth,
                View.Frame.Height * Constants.ColourDuplicateNotePositionRatio, Constants.ColourDuplicateNoteWidth,
                Constants.ColourDuplicateNoteHeight);
            UIView.CommitAnimations();
        }

        /// <summary>
        /// Displays bounding boxes on the screen according to the specified bounding box coordinates.
        /// </summary>
        protected void DisplayBoundingBoxes()
        {
            Categories = InputImageEntry.Observations.Select(o => o.Category).Distinct().ToList();
            
            foreach (var obs in InputImageEntry.Observations)
            {
                var obsCoordinate = obs.BoundingBox.ToCGRect();
                var button = new UIButton(UIButtonType.System)
                {
                    Frame = FitObservation(obsCoordinate)
                };
                button.Layer.BorderWidth = Constants.BoundingBoxBorderWidth;
                button.Layer.CornerRadius = Constants.BoundingBoxCornerRadius;
                foreach (var catagory in Categories)
                {
                    if (obs.Category.Equals(catagory))
                    {
                        button.Layer.BorderColor = catagory.Color.ToUIColor().CGColor;
                    }
                }
                button.TouchUpInside += (sender, e) =>
                {
                    // if there is a popover opened 
                    if (_lastVisitedPopover != null && _lastTappedButton != null)
                    {
                        _lastVisitedPopover.DismissViewController(false, null);
                        _lastVisitedPopover = null;

                        // if the clicked button is not related to the opened popover, open the popover related
                        // to current clicked button
                        if (!Equals(_lastTappedButton, button))
                        {
                            DisplayPopover(obs, button);
                            _lastTappedButton = button;
                        }
                        else
                        {
                            _lastTappedButton = null;
                        }
                    }

                    // If there is no popover presenting, open related popover
                    else
                    {
                        DisplayPopover(obs, button);
                        _lastTappedButton = button;
                    }
                };
                ScrollView.AddSubview(button);
                Bindings.Add(obs, button);

                ConfigureBoundingBoxAccessibilityAttributes(obs, button);
            }
        }

        /// <summary>
        /// Updates all bounding boxes on the screen according to the screen orientation.
        /// </summary>
        protected void UpdateBoundingBoxes()
        {
            foreach (var binding in Bindings)
            {
                var button = binding.Value;
                var observation = binding.Key;

                // if bounding box already hidden by switch button, re-draw bounding box and keep hiding it
                if (button.Hidden)
                {
                    button.Frame = FitObservation(observation.BoundingBox.ToCGRect());
                    continue;
                }

                // if the bounding box is shown, first hide it and re-draw bounding box, and then show it again
                button.Hidden = true;
                button.Frame = FitObservation(observation.BoundingBox.ToCGRect());
                button.Hidden = false;
            }

            // update popover if one of the popover is currently displaying
            _lastVisitedPopover?.DismissModalViewController(false);
        }

        protected abstract void SetSideBarCategoryStates();

        /// <summary>
        /// Fits the normalized 0..1 bounding box coordinates in the frame of the image.
        /// </summary>
        /// <returns>The <see cref="CGRect"/> that fits in the frame of the image.</returns>
        /// <param name="observation">The <see cref="CGRect"/> representing a bounding box in normalized 0..1 coordinates.</param>
        private CGRect FitObservation(CGRect observation)
        {
            return observation.Normalize(new CGSize(InputImageEntry.Image.Width,
                    (nfloat)InputImageEntry.Image.Height))
                .Scale(GetImageSize())
                .Translate(ImageView.Frame.Location);
        }

        /// <summary>
        /// Returns the actual size of the image fitting in the <see cref="ImageView"/>.
        /// <see cref="ImageView"/> uses scaleAspectFit content mode.
        /// <seealso href="https://developer.apple.com/documentation/uikit/uiviewcontentmode/scaleaspectfit">scaleAspectFit</seealso>
        /// </summary>
        /// <returns>The actual size of the image.</returns>
        private CGSize GetImageSize()
        {
            var widthRatio = ImageView.Frame.Width / ImageView.Image.Size.Width;
            var heightRatio = ImageView.Frame.Height / ImageView.Image.Size.Height;
            var scale = Math.Min(widthRatio, heightRatio);
            return new CGSize(
                scale * ImageView.Image.Size.Width,
                scale * ImageView.Image.Size.Height);
        }

        /// <summary>
        /// Orders the observations by bounding box size
        /// </summary>
        private void OrderButtonBySize()
        {
            InputImageEntry.Observations =
                InputImageEntry.Observations.OrderByDescending(o => o.BoundingBox.Width * o.BoundingBox.Height).ToList();
        }

        /// <summary>
        /// Dismisses the popover view controller.
        /// </summary>
        /// <param name="popover">Popover.</param>
        private void DismissPopoverViewController(UIViewController popover)
        {
            if (popover == null) return;
            _dismissedPopover = popover;
            popover.DismissViewController(false, null);
        }

        /// <summary>
        /// Presents the popover view controller.
        /// </summary>
        protected void PresentPopoverViewController()
        {
            if (_dismissedPopover == null) return;
            var obs = Bindings.FirstOrDefault(binding => Equals(binding.Value, (UIButton)_lastTappedButton)).Key;
            DisplayPopover(obs, (UIButton)_lastTappedButton);
            _dismissedPopover = null;
        }

        /// <summary>
        /// Disable the scroll after user lifts finger from screen.
        /// </summary>
        private void KillScroll()
        {
            ScrollView.PagingEnabled = false;
            ScrollView.DecelerationRate = nfloat.MinValue;
        }

        /// <summary>
        /// Assigns a different Category label to the <see cref="Observation"/>, with the Category label can be of new
        /// <see cref="Category"/> that does not exist yet.
        /// Should be called only by <see cref="EditModalController"/>, when a label of an observation is changed.
        /// </summary>
        /// <param name="observation"> the observation whose category is to be changed</param>
        /// <param name="newCategory"> the new <see cref="Category"/> labelString to be assigned </param>
        public virtual string NotifyLabelOfObservationDidChange(Observation observation, string newCategory)
        {
            var originalCategory = observation.Category;

            if (observation == null || newCategory == null)
            {
                throw new ArgumentNullException();
            }

            UpdateTitleForUnsaved();

            // check if label is an empty string
            if (string.IsNullOrWhiteSpace(newCategory))
            {
                return observation.Category.Label;
            }

            // check if a Category of label of categoryString already exist
            Category existingCategory = null;
            foreach (var category in Categories)
            {
                if (!string.Equals(category.Label, newCategory.Trim(),
                    StringComparison.OrdinalIgnoreCase)) continue;
                existingCategory = category;
                break;
            }

            var classNumIncrementation = 0;
            if (existingCategory != null)
            {
                observation.ResetCategory(existingCategory);

                // Find the correct colour for the edited bounding box
                Bindings.FirstOrDefault(binding => binding.Key.Equals(observation)).Value.Layer.BorderColor =
                    existingCategory.Color.ToCGColor();

                if (SidebarController.CountNumObservationOfCategory(originalCategory) == 0)
                {
                    Categories.Remove(originalCategory);
                    FilterViewController.RemovePreviousColour(SidebarController.ColoursInUse, originalCategory.Color.ToCGColor());
                    classNumIncrementation--;
                }
            }
            else   // a custom label by user not used before
            {
                var customCategory = new Category(0, newCategory);   // arbitrary ID
                observation.ResetCategory(customCategory);

                Categories.Add(customCategory);
                ReconfigureBoundingBoxColour(customCategory);
                classNumIncrementation++;

                // check if original category is now empty, if so, delete
                if (SidebarController.CountNumObservationOfCategory(originalCategory) == 0)
                {
                    Categories.Remove(originalCategory);
                    FilterViewController.RemovePreviousColour(SidebarController.ColoursInUse, originalCategory.Color.ToCGColor());
                    classNumIncrementation--;
                }

                // set the checkbox state for new category
                if (!SidebarController.CheckboxState.ContainsKey(customCategory))
                {
                    SidebarController.CheckboxState.Add(customCategory, true);
                }
            }
            InputImageEntry.Observations = Bindings.Keys.ToList();

            SidebarController.ReconfigureSidebarContent(classNumIncrementation);

            ConfigureSidebarTitle();

            // configure the mega-classes in table view after edit label
            SidebarController.FilteredItems = Categories;
            SidebarController.ConfigureMegaSection();
            SidebarController.TableView.ReloadData();

            DisplayPopover(observation, _lastClickedBoundingBox);

            return observation.Category.Label;
        }

        /// <summary>
        /// Updates the color of the specified <see cref="Category"/> to the specified <see cref="SKColor"/>.
        /// </summary>
        /// <param name="category">The specified <see cref="Category"/>.</param>
        /// <param name="color">The specified <see cref="SKColor"/>.</param>
        public virtual void NotifyColorChanged(string category, SKColor color)
        {
        }

        /// <summary>
        /// Re-configures bounding box colour after relabeling.
        /// </summary>
        /// <param name="customCategory">Custom category.</param>
        private void ReconfigureBoundingBoxColour(Category customCategory)
        {
            var observation = Bindings.FirstOrDefault(binding => binding.Key.Category.Equals(customCategory)).Value;
            observation.Layer.BorderColor = customCategory.Color.ToCGColor();

            SidebarController.NotifyBoundingBoxColourDidChange(null, customCategory.Color.ToCGColor(), customCategory.Label);
        }

        #region Accessibility Functions 
        private void DisableBackgroundBoundingBoxVoiceOver()
        {
            foreach (var item in Bindings)
            {
                item.Value.IsAccessibilityElement = false;
            }
        }

        private void EnableBackgroundBoundingBoxVoiceOver()
        {
            foreach (var item in Bindings)
            {
                item.Value.IsAccessibilityElement = true;
            }
        }

        private void ConfigureTitleAccessibilityAttributes()
        {
            var titleView = NavigationItem.TitleView;
            titleView.IsAccessibilityElement = true;
            titleView.AccessibilityTraits = UIAccessibilityTrait.StaticText | 
                                            UIAccessibilityTrait.SummaryElement | UIAccessibilityTrait.Header;
            titleView.AccessibilityLabel = AccessibilityConstants.ImageTitleAccessibilityLabel;
            titleView.AccessibilityHint = NavigationItem.Title;
        }

        private static void ConfigureSideBarCloseButtonAccessibilityAttributes(UIButton sidebarClose)
        {
            sidebarClose.AccessibilityTraits = UIAccessibilityTrait.Button;
            sidebarClose.AccessibilityLabel = AccessibilityConstants.SidebarCloseButtonAccessibilityLabel;
            sidebarClose.AccessibilityHint = AccessibilityConstants.SidebarCloseButtonAccessibilityHint;
        }

        private void ConfigureBoundingBoxAccessibilityAttributes(Observation obs,UIButton boundingBox)
        {
            var colourNumber = FilterViewController.GetColorVoiceOverEncoding(boundingBox.Layer.BorderColor);
            boundingBox.AccessibilityTraits = UIAccessibilityTrait.Button;
            boundingBox.AccessibilityLabel = string.Format(CultureInfo.CurrentCulture, AccessibilityConstants.BoundingBoxAccessibilityLabel, obs.Category.Label, colourNumber);
            boundingBox.AccessibilityHint = AccessibilityConstants.BoundingBoxAccessibilityHint;
        }

        private void ConfigureSameColourNotificationAccessibilityAttributes(string text)
        {
            UIAccessibility.PostNotification(UIAccessibilityPostNotification.ScreenChanged, new NSString(text));
        }

        private void ConfigureToolBarAccessibilityAttributes()
        {
            ToolBarView.AccessibilityTraits = UIAccessibilityTrait.Header;
        }
        #endregion
    }
}
