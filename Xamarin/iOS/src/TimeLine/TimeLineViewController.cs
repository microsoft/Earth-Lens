using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EarthLens.iOS.DataVisualization;
using EarthLens.iOS.Results;
using EarthLens.Models;
using CoreGraphics;
using SkiaSharp;
using SkiaSharp.Views.iOS;
using UIKit;

namespace EarthLens.iOS.TimeLine
{
    public partial class TimeLineViewController : ResultsViewController
    {
        public IList<ImageEntry> InputImageEntries { get; set; }

        private DataVisualizer _dataVisualizer;
        private UIScrollView _legendScrollView;
        private UIScrollView _timeLineScrollView;
        private UIView _legendView;
        private UIStackView _thumbnailStackView;
        private UIStackView _subHeadingStackView;
        private UIButton _dataVisualizerCloseButton;
        private Dictionary<string, SKColor> _updatedColors;
        private nint _lastVisitedTag;
        private bool _dataVisualizerOpened;

        /// <summary>
        /// Initializes a ViewController instance.
        /// </summary>
        public TimeLineViewController(IntPtr handle) : base(handle)
        {
        }

        /// <summary>
        /// Fetches the multiple images stored within the Image Array and displays the images with the data.
        /// </summary>
        public override void ViewDidLoad()
        {
            // Perform any additional setup after loading the view, typically from a nib.
            InputImageEntry = InputImageEntries.First();
            CreateThumbnails();
            CreateThumbnailView();
            SetStoryboardParameters();
            SetUpViewController();
            DisplayToolBar(ObjectBtn, ShareBtn, SaveBtn, SettingBtn, DataVisualizerBtn);
            GenerateSideBar(SidebarClose);
            CreateFilterContent(Sidebar, ToggleView, SearchView);
            SetSideBarCategoryStates();
            View.BringSubviewToFront(SidebarView);
            UIApplication.Notifications.ObserveContentSizeCategoryChanged
            ((sender,args) => AccessibilityToolbarBtnHandler(
                ObjectBtn, ShareBtn, SaveBtn, SettingBtn, DataVisualizerBtn
            ));
            UIApplication.Notifications.ObserveContentSizeCategoryChanged((sender,args) => AccessibilityFontHandler());
            _updatedColors = new Dictionary<string, SKColor>();
        }

        public override string NotifyLabelOfObservationDidChange(Observation observation, string newCategory)
        {
            var result = base.NotifyLabelOfObservationDidChange(observation, newCategory);

            if (!_dataVisualizerOpened) return result;
            CloseDataVisualizer();
            OpenDataVisualizer();

            return result;
        }

        protected override void SetSideBarCategoryStates()
        {
            var categories = InputImageEntries
                .SelectMany(imageEntry => imageEntry.Observations.Select(o => o.Category).Distinct())
                .Distinct();
            foreach (var category in categories)
            {
                SidebarController.CheckboxState.Add(category, true);
            }
        }
        
        private void AccessibilityFontHandler()
        {
            foreach (var subview in _subHeadingStackView.Subviews)
            {
                SetTimelineSubheaderInsets((UITextView) subview);
            }
        }

        /// <summary>
        /// Sets storyboard parameters that will be accessed within the results view controller.
        /// </summary>
        private void SetStoryboardParameters()
        {
            ThumbnailViewHeight = Constants.TimeLineScrollViewHeight;
            ToolBarView = ToolBar;
            SelectedClassTextView = SelectedClass;
            ClassCountTextView = ClassCount;
            SideBarView = SidebarView;
            TimeLineView = _timeLineScrollView;
            ThresholdForHeight = Constants.TimelineThresholdForHeight;
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

        /// <summary>
        /// Creates thumbnails and subheadings for the timeline scrollview.
        /// </summary>
        private void CreateThumbnails()
        {
            var xPoint = Constants.XPoint;
            const int yPoint = Constants.YPoint;
            _thumbnailStackView = new UIStackView();
            _subHeadingStackView = new UIStackView();

            foreach (var pair in InputImageEntries.Select((value, index) => new { index, value }))
            {
                var thumbnailView =
                    new UIImageView(new CGRect(xPoint, yPoint, Constants.ThumbnailWidth, Constants.ThumbnailHeight))
                    {
                        Image = pair.value.Image.ToUIImage(),
                        ContentMode = UIViewContentMode.ScaleToFill,
                        UserInteractionEnabled = true,
                        Tag = pair.index
                    };
                if (pair.value.CreationTime != null)
                {
                    var subheadingView = new UITextView(new CGRect(xPoint, Constants.Origin, Constants.ThumbnailWidth,
                        Constants.SubheadingViewHeight))
                    {
                        Text = pair.value.CreationTime.Value.ToString(Constants.DateTimeFormat,
                            CultureInfo.CurrentCulture),
                        TextColor = Constants.SubheadingTextColour,
                        TextAlignment = Constants.SubheadingTextAlignment,
                        Font = UIFont.GetPreferredFontForTextStyle(UIFontTextStyle.Caption1),
                        AdjustsFontForContentSizeCategory = true,
                        BackgroundColor = Constants.TimeLineScrollViewColour,
                        Editable = false
                    };
                    SetTimelineSubheaderInsets(subheadingView);
                    _subHeadingStackView.Add(subheadingView);
                }

                _thumbnailStackView.Add(thumbnailView);
                xPoint += Constants.XIncrement;

                ConfigureInputImageThumbnailAccessibilityAttributes(thumbnailView,pair.value.CreationTime);
            }
            _thumbnailStackView.UserInteractionEnabled = true;

        }

        /// <summary>
        /// Creates a timeline scrollview using the thumbnail and subheading stackviews.
        /// </summary>
        private void CreateThumbnailView()
        {
            _timeLineScrollView = new UIScrollView(new CGRect(Constants.Origin,
                View.Frame.Height - Constants.TimeLineScrollViewHeight - Constants.ResultsToolBarHeight,
                View.Frame.Width, Constants.TimeLineScrollViewHeight))
            {
                ContentSize = new CGSize(InputImageEntries.Count * Constants.TotalThumbnailWidth,
                    Constants.TimeLineScrollViewHeight),
                BackgroundColor = Constants.TimeLineScrollViewColour,
                UserInteractionEnabled = true
            };
            View.AddSubview(_timeLineScrollView);
            _timeLineScrollView.AddSubview(_thumbnailStackView);
            _timeLineScrollView.AddSubview(_subHeadingStackView);

            _timeLineScrollView.AddGestureRecognizer(new UITapGestureRecognizer(tap =>
            {
                foreach (var imageView in _timeLineScrollView.Subviews[0].Subviews)
                {
                    var location = tap.LocationInView(imageView);
                    var hitImageView = imageView.HitTest(location, null);
                    if (hitImageView == null) continue;
                    InputImageEntry = InputImageEntries[(int)imageView.Tag];
                    GenerateTitle();
                    ClearBindings();
                    ClearPopover();
                    HighlightThumbnail(imageView.Tag);
                    CalculateScaledDimensions(InputImageEntries[(int)imageView.Tag].Image);
                    UpdateImage();
                    UpdateScrollViewDimensions();
                    UpdateCategoryColors(InputImageEntry);
                    DisplayBoundingBoxes();
                    UpdateSidebarContent();
                }
            }));
            HighlightThumbnail(0);
        }

        /// <summary>
        /// Unhighlights the previously slected thumbnail and highlights the selected thumbnail and subheading. 
        /// </summary>
        private void HighlightThumbnail(nint tag)
        {
            _timeLineScrollView.Subviews[Constants.ThumbnailStackViewIndex].Subviews[_lastVisitedTag].Layer.BorderWidth
                               = Constants.UnselectedBorderWidth;
            _timeLineScrollView.Subviews[Constants.SubheadingStackViewIndex].Subviews[_lastVisitedTag].Layer.BorderWidth
                               = Constants.UnselectedBorderWidth;
            _timeLineScrollView.Subviews[Constants.ThumbnailStackViewIndex].Subviews[tag].Layer.BorderColor
                               = UIColor.FromRGB(Constants.HighlightRedValue, Constants.HighlightGreenValue, 
                                 Constants.HighlighBlueValue).CGColor;
            _timeLineScrollView.Subviews[Constants.ThumbnailStackViewIndex].Subviews[tag].Layer.BorderWidth
                               = Constants.SelectedBorderWidth;
            _timeLineScrollView.Subviews[Constants.SubheadingStackViewIndex].Subviews[tag].Layer.BorderColor
                               = UIColor.FromRGB(Constants.HighlightRedValue, Constants.HighlightGreenValue, 
                                 Constants.HighlighBlueValue).CGColor;
            _timeLineScrollView.Subviews[Constants.SubheadingStackViewIndex].Subviews[tag].Layer.BorderWidth
                               = Constants.SelectedBorderWidth;
            _lastVisitedTag = tag;
        }

        /// <summary>
        /// Updates sidebar contents based on image selected 
        /// </summary>
        private void UpdateSidebarContent()
        {
            SidebarController.FilteredItems = Categories;
            SidebarController.DisplayItems = Categories;
            SidebarController.Bindings = Bindings;
            SidebarController.SelectedClassCount = GetFilteredCount();
            SidebarController.GenerateColoursInUse();
            ConfigureSidebarTitle();
            SidebarController.ConfigureMegaSection();
            SidebarController.TableView.ReloadData();
        }

        private int GetFilteredCount()
        {
            var filteredCount = Categories.Count;

            foreach (var category in Categories)
            {
                if (!SidebarController.CheckboxState[category])
                {
                    filteredCount--;
                }
            }

            return filteredCount;
        }

        /// <summary>
        /// Updates colors for each <see cref="Category"/> used in the current image.
        /// </summary>
        private void UpdateCategoryColors(ImageEntry imageEntry)
        {
            foreach (var observation in imageEntry.Observations)
            {
                if (_updatedColors.ContainsKey(observation.Category.Label))
                {
                    observation.Category.Color = _updatedColors[observation.Category.Label];
                }
            }
        }
        
        /// <summary>
        /// Set Timeline subheading margins based on font size in app
        /// </summary>
        private static void SetTimelineSubheaderInsets(UITextView textView)
        {
            textView.TextContainerInset = textView.Font.PointSize <= Constants.SubheadingFontSizeCutoff
                ? new UIEdgeInsets(Constants.SubheadingTopSmallFontInset,0,0,0) 
                : new UIEdgeInsets(Constants.SubheadingTopLargeFontInset,0,0,0);
        }

        /// <summary>
        /// Clears bindings from the previous image selected.
        /// </summary>
        private void ClearBindings()
        {
            foreach (var binding in Bindings)
            {
                binding.Value.RemoveFromSuperview();
            }
            Bindings.Clear();
        }

        /// <inheritdoc />
        /// <summary>
        /// Handles rotation operations.
        /// </summary>
        protected override void HandleRotation()
        {
            base.HandleRotation();
            if (_dataVisualizerOpened)
            {
                _timeLineScrollView.Frame = new CGRect(Constants.Origin,
                    View.Frame.Height - Constants.ResultsToolBarHeight - ThumbnailViewHeight + Constants.LegendHeight,
                    View.Frame.Width, ThumbnailViewHeight);

                _legendView.Frame = new CGRect(Constants.Origin,
                    View.Frame.Height - Constants.ResultsToolBarHeight - ThumbnailViewHeight,
                    View.Frame.Width, Constants.LegendHeight);

                _dataVisualizerCloseButton.Frame = new CGRect(
                    View.Frame.Width - _dataVisualizerCloseButton.Frame.Width - Constants.CloseButtonMargin,
                    (Constants.LegendHeight - _dataVisualizerCloseButton.Frame.Height) / 2,
                    _dataVisualizerCloseButton.Frame.Width,
                    _dataVisualizerCloseButton.Frame.Height);

                _legendScrollView.Frame = new CGRect(0, 0,
                    View.Frame.Width - _dataVisualizerCloseButton.Frame.Width - Constants.CloseButtonMargin * 2,
                    Constants.LegendHeight);

                _dataVisualizerCloseButton.Frame = new CGRect(
                    View.Frame.Width - _dataVisualizerCloseButton.Frame.Width - Constants.CloseButtonMargin,
                    (Constants.LegendHeight - _dataVisualizerCloseButton.Frame.Height) / 2,
                    _dataVisualizerCloseButton.Frame.Width,
                    _dataVisualizerCloseButton.Frame.Height);
            }
            else
            {
                _timeLineScrollView.Frame = new CGRect(Constants.Origin,
                    View.Frame.Height - Constants.TimeLineScrollViewHeight
                                      - Constants.ResultsToolBarHeight, View.Frame.Width,
                    Constants.TimeLineScrollViewHeight);
            }

            _timeLineScrollView.ContentSize = new CGSize(InputImageEntries.Count * Constants.TotalThumbnailWidth,
                Constants.TimeLineScrollViewHeight);
        }

        /// <inheritdoc />
        /// <summary>
        /// Updates the color of the specified <see cref="T:EarthLens.Models.Category" /> to the specified <see cref="T:SkiaSharp.SKColor" />.
        /// </summary>
        /// <param name="category">The specified <see cref="T:EarthLens.Models.Category" />.</param>
        /// <param name="color">The specified <see cref="T:SkiaSharp.SKColor" />.</param>
        public override void NotifyColorChanged(string category, SKColor color)
        {
            if (_updatedColors.ContainsKey(category))
            {
                _updatedColors[category] = color;
            }
            else
            {
                _updatedColors.Add(category, color);
            }
        }

        /// <summary>
        /// Calls a method to handle events once the object button is touched
        /// </summary>
        partial void ObjectBtn_TouchUpInside(UIButton sender)
        {
            if (_dataVisualizerOpened)
            {
                CloseDataVisualizer();
            }

            Constraints[Constants.DefaultHeightConstraintKey].Constant = View.Frame.Height - ThresholdForHeight * 2 - Constants.TopLayoutGuideHeight;

            ViewDetailsButton_TouchedUpInside();
        }

        /// <summary>
        /// Calls a method to handle events once the sidebar is closed
        /// </summary>
        partial void SidebarClose_TouchUpInside(UIButton sender)
        {
            base.SidebarClose_TouchUpInside(sender);
        }

        /// <summary>
        /// Calls a method to handle events once the share button is touched
        /// </summary>
        partial void ShareBtn_TouchUpInside(UIButton sender)
        {
            PresentSharePopover(ShareBtn);
        }

        /// <summary>
        /// Calls a method to handle events once the data visualizer button is touched
        /// </summary>
        partial void DataVisualizerBtn_TouchUpInside(UIButton sender)
        {
            SidebarCloseButtonTouched();
            if (_dataVisualizerOpened)
            {
                CloseDataVisualizer();
            }
            else
            {
                OpenDataVisualizer();
            }
        }

        /// <summary>
        /// Updates the image view once a new image is selected
        /// </summary>
        private void UpdateImage()
        {
            ImageView.Frame = new CGRect(Constants.Origin, Constants.Origin, ImgWidth, ImgHeight);
            ImageView.Image = InputImageEntry.Image.ToUIImage();
        }

        /// <summary>
        /// Updates the scrollview properties once a new image is selected.
        /// </summary>
        private void UpdateScrollViewDimensions()
        {
            ScrollView.Frame = new CGRect((View.Frame.Width - ImgWidth) / 2,
                (ViewHeight - ImgHeight) / 2 + NavBarHeight, ImgWidth, ImgHeight);
            ScrollView.ContentSize = ImageView.Image.Size;
            ScrollView.ZoomScale = Constants.MinimumZoomScale;
        }

        /// <summary>
        /// Saves all the images within the timeline session to the gallery.
        /// </summary>
        protected override void HandleSaveToGalleryYes()
        {
            var currentImageEntry = InputImageEntry;
            foreach (var imageEntry in InputImageEntries)
            {
                InputImageEntry = imageEntry;
                base.HandleSaveToGalleryYes();
            }
            InputImageEntry = currentImageEntry;
        }

        /// <summary>
        /// Opens data visualizer with the detected <see cref="Observation"/>s.
        /// </summary>
        private void OpenDataVisualizer()
        {
            if (_dataVisualizerOpened)
            {
                return;
            }

            _dataVisualizerOpened = true;

            var allDataPoints = new List<Dictionary<Category, int>>();
            var allCategories = new HashSet<Category>();

            foreach (var imageEntry in InputImageEntries)
            {
                UpdateCategoryColors(imageEntry);
                var dataPoints = new Dictionary<Category, int>();
                foreach (var observation in imageEntry.Observations)
                {
                    if (dataPoints.ContainsKey(observation.Category))
                    {
                        dataPoints[observation.Category]++;
                    }
                    else
                    {
                        dataPoints.Add(observation.Category, 1);
                    }

                    allCategories.Add(observation.Category);
                }
                allDataPoints.Add(dataPoints);
            }

            _dataVisualizer = new BarChartSeries(allDataPoints, allCategories);
            _dataVisualizer.Update(allCategories.Where(category => SidebarController.CheckboxState[category]),
                new Category[] { }, View);

            ThumbnailViewHeight = Constants.TimeLineScrollViewHeight + Constants.BarChartSeriesStackViewHeight +
                                  Constants.LegendHeight;

            _timeLineScrollView.Frame = new CGRect(Constants.Origin,
                View.Frame.Height - Constants.ResultsToolBarHeight - ThumbnailViewHeight + Constants.LegendHeight,
                View.Frame.Width, ThumbnailViewHeight);

            CalculateScaledDimensions(InputImageEntry.Image);
            ImageView.Frame = new CGRect(0, 0, ImgWidth, ImgHeight);

            UpdateScrollViewDimensions();

            UpdateBoundingBoxes();

            var barChartsTopConstraint = NSLayoutConstraint.Create(_dataVisualizer.Chart, NSLayoutAttribute.Top,
                NSLayoutRelation.Equal, _timeLineScrollView, NSLayoutAttribute.Top, 1f, 0f);
            var barChartsLeadingConstraint = NSLayoutConstraint.Create(_dataVisualizer.Chart, NSLayoutAttribute.Leading,
                NSLayoutRelation.Equal, _timeLineScrollView, NSLayoutAttribute.Leading, 1f, 0f);

            _timeLineScrollView.AddSubview(_dataVisualizer.Chart);
            _timeLineScrollView.AddConstraints(new[]
            {
                barChartsTopConstraint,
                barChartsLeadingConstraint
            });

            _thumbnailStackView.Frame = new CGRect(Constants.Origin,
                Constants.BarChartSeriesStackViewHeight,
                Constants.TotalThumbnailWidth * InputImageEntries.Count,
                Constants.ThumbnailHeight);

            _subHeadingStackView.Frame = new CGRect(Constants.Origin,
                Constants.BarChartSeriesStackViewHeight,
                Constants.TotalThumbnailWidth * InputImageEntries.Count,
                Constants.SubheadingViewHeight);

            _legendScrollView = new UIScrollView(new CGRect(0, 0, View.Frame.Width, Constants.LegendHeight))
            {
                ContentSize = new CGSize(_dataVisualizer.Legend.Frame.Width, Constants.LegendHeight),
                BackgroundColor = Constants.LegendViewColour,
                UserInteractionEnabled = true,
                ScrollEnabled = true
            };

            _legendView = new UIView(new CGRect(Constants.Origin,
                View.Frame.Height - Constants.ResultsToolBarHeight - ThumbnailViewHeight,
                View.Frame.Width, Constants.LegendHeight))
            {
                BackgroundColor = Constants.LegendViewColour,
                UserInteractionEnabled = true
            };

            View.AddSubview(_legendView);

            var legendLeadingConstraint = NSLayoutConstraint.Create(_dataVisualizer.Legend,
                NSLayoutAttribute.LeadingMargin, NSLayoutRelation.Equal, _legendScrollView,
                NSLayoutAttribute.LeadingMargin, 1f, 0f);
            var legendCenterYConstraint = NSLayoutConstraint.Create(_dataVisualizer.Legend, NSLayoutAttribute.CenterY,
                NSLayoutRelation.Equal, _legendScrollView, NSLayoutAttribute.CenterY, 1f, 0f);

            _legendScrollView.AddSubview(_dataVisualizer.Legend);
            _legendScrollView.AddConstraints(new[]
            {
                legendLeadingConstraint,
                legendCenterYConstraint
            });

            _dataVisualizerCloseButton = new UIButton(UIButtonType.System)
            {
                TintColor = Constants.CloseBtnColor
            };
            _dataVisualizerCloseButton.SetImage(new UIImage(Constants.CloseBtnImage), UIControlState.Normal);
            _dataVisualizerCloseButton.SizeToFit();

            _dataVisualizerCloseButton.Frame = new CGRect(
                View.Frame.Width - _dataVisualizerCloseButton.Frame.Width - Constants.CloseButtonMargin,
                (Constants.LegendHeight - _dataVisualizerCloseButton.Frame.Height) / 2,
                _dataVisualizerCloseButton.Frame.Width,
                _dataVisualizerCloseButton.Frame.Height);

            ConfigureDataVisualizeCloseButtonAccessibilityAttribute(_dataVisualizerCloseButton);

            _legendScrollView.Frame = new CGRect(0, 0,
                View.Frame.Width - _dataVisualizerCloseButton.Frame.Width - Constants.CloseButtonMargin * 2,
                Constants.LegendHeight);

            _legendView.AddSubview(_legendScrollView);
            _legendView.AddSubview(_dataVisualizerCloseButton);

            _dataVisualizerCloseButton.TouchUpInside += DataVisualizerCloseButton_TouchUpInside;

            _legendScrollView.BringSubviewToFront(_dataVisualizerCloseButton);
        }

        /// <summary>
        /// Handles touch-up-inside event for <see cref="_dataVisualizerCloseButton"/>.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The arguments of the event.</param>
        private void DataVisualizerCloseButton_TouchUpInside(object sender, EventArgs e)
        {
            CloseDataVisualizer();
        }

        /// <summary>
        /// Closes the data visualizer.
        /// </summary>
        private void CloseDataVisualizer()
        {
            if (!_dataVisualizerOpened)
            {
                return;
            }

            _dataVisualizerOpened = false;

            ThumbnailViewHeight = Constants.TimeLineScrollViewHeight;

            _timeLineScrollView.Frame = new CGRect(Constants.Origin,
                View.Frame.Height - Constants.TimeLineScrollViewHeight
                                  - Constants.ResultsToolBarHeight, View.Frame.Width,
                Constants.TimeLineScrollViewHeight);

            CalculateScaledDimensions(InputImageEntry.Image);
            ImageView.Frame = new CGRect(0, 0, ImgWidth, ImgHeight);

            UpdateScrollViewDimensions();
            UpdateBoundingBoxes();

            _thumbnailStackView.Frame = new CGRect(Constants.Origin,
                Constants.Origin,
                Constants.TotalThumbnailWidth * InputImageEntries.Count,
                Constants.ThumbnailHeight);

            _subHeadingStackView.Frame = new CGRect(Constants.Origin,
                Constants.Origin,
                Constants.TotalThumbnailWidth * InputImageEntries.Count,
                Constants.SubheadingViewHeight);

            _dataVisualizer.Chart.RemoveFromSuperview();
            _legendView.RemoveFromSuperview();

            _dataVisualizer.Dispose();
            _legendView.Dispose();
            _legendScrollView.Dispose();
            _dataVisualizerCloseButton.Dispose();

            _dataVisualizer = null;
            _legendView = null;
            _legendScrollView = null;
            _dataVisualizerCloseButton = null;
        }

        #region Accessibility Configuration
        private static void ConfigureInputImageThumbnailAccessibilityAttributes(UIImageView thumbnailView, DateTime? createdTime)
        {
            thumbnailView.IsAccessibilityElement = true;
            thumbnailView.AccessibilityElementsHidden = false;
            var time = createdTime ?? DateTime.Now;
            thumbnailView.AccessibilityLabel = time.ToString(CultureInfo.CurrentCulture);
            thumbnailView.AccessibilityHint = AccessibilityConstants.ImageThumbNameAccessibilityHint;
        }

        private static void ConfigureDataVisualizeCloseButtonAccessibilityAttribute(UIButton closeButton)
        {
            closeButton.AccessibilityTraits = UIAccessibilityTrait.Button;
            closeButton.AccessibilityLabel = AccessibilityConstants.DataVisualizerCloseButtonAccessibilityLabel;
            closeButton.AccessibilityHint = AccessibilityConstants.DataVisualizerCloseButtonAccessibilityHint;
        }
        #endregion
    }
}
