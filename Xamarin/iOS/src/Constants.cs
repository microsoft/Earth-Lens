using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace EarthLens.iOS
{
    /// <summary>
    /// This class holds constants that are used throughout the application
    /// </summary>
    public static class Constants
    {
        public const int ImageChannelSize = 3;
        public const string ModelResourceExt = "mlmodelc";   //extension of a compiled ML Model 
        public const string CloseBtnImage = "img/t-close.png";
        public const int SequenceLength = 1;
        public const int BatchSize = 1;
        public const int TableRowHeight = 70;
        public const int TableRowWidth = 320;
        public const int TableRowMargin = 20;
        public const int ColourButtonSize = 25;
        public const int CellTextXOffset = 25;
        public const int CellCountTextXOffset = 35;
        public const string ResourceName = @"";
        public const int NumClasses = 23;
        public const int NumAnchorBoxes = 1917;
        public const int ImageEdgeToolbar = 15;
        public const int TableCellFontSize = 18;
        public const int IconMargin = 20;
        public const int UploadButtonBorderWidth = 1;
        public const int UploadBtnVerticalOffset = 10;
        public const int UploadBtnHorizontalOffset = 30;
        public const int PopoverFontSize = 15;
        public const int MegaClassPadding = 5;
        public const int ColourDuplicateNoteFontSize = 14;
        public const int ConflicClassStartPosition = 36;
        public const int CheckboxBorderWidth = 1;
        public const int SearchbarPadding = 10;
        public const int SearchViewHeight = 140;
        public const string IDRegexString = @"(?<=id:)[^\s\\]+";
        public const string LabelRegexString = @"(?<=name:')[^']+";
        public const string SlidingAnimation = "slideAnimation";
        public const string FadeOutAnimation = "fadeflag";
        public const string PdfFileExtension = ".pdf";
        public const double ModelYScale = 10.0; // Scale ratio found in model .config files
        public const double ModelXScale = 10.0;
        public const double ModelHeightScale = 5.0;
        public const double ModelWidthScale = 5.0;
        public const string DefaultInputFeatureName = @"Preprocessor__sub__0";
        public const string ModelOutputBoxFeatureName = @"concat__0";
        public const string ModelOutputClassFeatureName = @"concat_1__0";
        public const string ConflictClassJoinString = " and ";
        public const float ColorPickerShadowOpacity = 1.0f;
        public const float ColorPickerShadowOffset = 1.0f;
        public const float ColorPickerShadowRadius = 10;
        public const float ColorButtonShadowRadius = 4.0f;
        public const float ColorButtonShadowOpacity = 1.0f;
        public const float ClearShadowOpacity = 0;
        public const float BoundingBoxCornerRadius = 2.0f;
        public const float BoundingBoxBorderWidth = 4.0f;
        public const float ThresholdforHeight = 100.0f;
        public const float TopLayoutGuideHeight = 64.0f;
        public const float ClassLabelWidth = 150.0f;
        public const int SidebarClassLabelMaxLines = 2;
        public const int SidebarClassLabelHeightPadding = 10;
        public const float Multiplier = 1.0f;
        public const float SidebarBorderWidth = 1.0f;
        public const float ToggleYOffset = 1.0f;
        public const float ColourDuplicateNoteCornerRadius = 4.0f;
        public const float ColourDuplicateNoteOpacity = 0.8f;
        public const float ColourDuplicateNoteWidth = 300.0f;
        public const float ColourDuplicateNoteHeight = 80.0f;
        public const float ColourDuplicateNoteMargin = 15.0f;
        public const float ColourDuplicateNoteTextHeight = 50.0f;
        public const float ColourDuplicateNotePositionRatio = 0.7f;
        public const float SlidingAnimationDuration = 0.5f;
        public const float FadeOutAnimationDuration = 2.0f;
        public const float FadeOutAnimationDelay = 5.5f;
        public const float FadeOutAnimationTransparency = 0;
        public const float SectionHeaderClear = 0;
        public static readonly CGColor CheckboxSelectedBorderColor = UIColor.Clear.CGColor;
        public static readonly CGColor CheckboxUnselectedBorderColor = UIColor.LightGray.CGColor;
        public static readonly CGColor ColorPickerPopupShadowColour = UIColor.Gray.CGColor;
        public static readonly CGColor ColorPickerPopupUnselectedColour = UIColor.Clear.CGColor;
        public static readonly UIColor SearchbarBackgroundColor = UIColor.White;
        public static readonly UIColor TableTextColor = UIColor.Gray;
        public static readonly UIColor TableTextBackgroundColor = UIColor.Clear;
        public static readonly UIColor PopoverTextColor = UIColor.Black;
        public static readonly UIColor CloseBtnColor = UIColor.LightGray;
        public static readonly UIColor SidebarColor = UIColor.GroupTableViewBackgroundColor;
        public static readonly CGColor UploadButtonBorderColor = UIColor.Gray.CGColor;
        public static readonly UIColor PopoverBackgroundColor = UIColor.Clear;
        public static readonly UIColor ColourDuplicateNoteColor = UIColor.White;
        public static readonly string ShareExtensionCheck =
            NSBundle.MainBundle.BundleIdentifier + ".LaunchedFromShareExtension";
        public static readonly UIColor ImagePickerNavigationBarTextColor = UIColor.Clear;
        public const float StartButtonBottomMargin = 150.0f;

        #region Results ViewController UIConstants
        public const double ResultsToolBarHeight = 50.0;
        public const double ResultsNavBarHeight = 64.0;
        public const int ResultsNavTitleFontSize = 20;
        public const float BottomMargin = 10.0f;
        //public static readonly UIColor ResultsToolBarButtonColor = UIColor.table;
        #endregion

        public const double SingleImageThumbnailViewHeight = 0.0;
        public static readonly UIColor ScrollViewBackgroundColor = UIColor.Black;
        public const string CloseButtonHeightConstraintKey = "CloseBtnHeight";
        public const string SelectedClassHeightConstraintKey = "SelectedClassHeight";
        public const string TextviewTopPaddingConstraintKey = "TextviewTopPadding";
        public const string DefaultHeightConstraintKey = "DefaultHeight";
        public const string ClassCountHeightConstraintKey = "ClassCountHeight";
        public const string SidebarWidthConstraintKey = "SidebarWidth";
        public const string SidebarYOffsetConstraintKey = "SidebarYOffset";
        public const string ToggleHeightConstraintKey = "ToggleHeight";

        #region Analyzing Page
        public const int DelayBeforeResults = 1000;
        #endregion

        #region PDFGeneration
        public static readonly CGRect PDFPageSize = new CGRect(0, 0, 612, 792);
        public static readonly string[] CategoryTableHeader = { "Class Name","Count" };
        public static readonly string[] ObservationTableHeader = { "Number", "Class Name", "Count" };
        public const string CategoriesText = "Categories";
        public const string CategoriesJoinText = ", ";
        public static readonly CGColor TextLabelBackgroundColour = UIColor.White.CGColor;
        public static readonly CGColor TextLabelTextColour = UIColor.Black.CGColor;
        public static readonly CGColor PDFTableStrokeColour = UIColor.Black.CGColor;
        public const int PDFImageTextLabelFontSize = 8;
        public const int PDFPagePadding = 20;
        public const int PDFPageImageTitleFontSize = 16;
        public const int PDFPageTextPadding = 5;
        public const int PDFPageTextFontSize = 12;
        public const int PDFPageTableTextPadding = 3;
        public const int PDFPageTableRowHeight = 20;
        public const int PDFPageTableColumnWidth = 150;
        public const int PDFObservationTableNumberOfColumn = 3;
        public const int PDFCategoryTableNumberOfColumn = 2;
        public const float PDFPageTableLineWidth = 1.0f;
        public const string ObservationConfidenceFormat = "{0}%";
        public const string NumberLabelFormat = "{0}";
        #endregion

        #region Model-Related
        public const string AnchorsResourceName = @"EarthLens.iOS.Resources.anchors.csv";
        public const string ClassLabelMapResourceName = @"EarthLens.iOS.Resources.classes_label_map.txt";
        #endregion
        
        #region NoResultPage
        public const int NoResultPromptFontSize = 18;
        public static readonly UIColor NoResultPageBackgroundColour = UIColor.White;
        public const int NoResultStringLeftOffset = 2;
        #endregion
        
        #region UI Strings
        public const string NewSessionTitle = "New Session";
        #endregion

        #region Gallery
        public const string ImageGalleryTitle = "Image Gallery";
        public const string ViewGalleryTitle = "View Gallery";
        public const string GalleryBackButtonTitle = "Back to Session";
        public const string GalleryCancelButtonTitle = "Cancel";
        public const int GalleryHeaderXPos = 20;
        public const int GalleryHeaderYPos = 0;
        public const int GalleryHeaderWidth = 500;
        public const int GalleryHeaderHeight = 40;
        public const float GalleryLayoutMinIterItemSpacing = 50f;
        public const float GalleryLayoutMinLinSpacing = 10f;
        public const int GalleryTileWidth = 200;
        public const int GalleryTileHeight = 200;
        public const int GallerySectionSize = 20;
        public const int GalleryCheckMarkXOffset = 30;
        public const int GalleryCheckMarkYOffset = 50;
        public const int GalleryCheckMarkHeight = 30;
        public const int GalleryCheckMarkWidth = 30;
        public const int GalleryCheckMarkXPadding = 4;
        public const int GalleryCheckMarkYPadding = 4;
        public const string GalleryImageCellKey = "GalleryImageCell";
        public const string GalleryHeaderKey = "HeaderView";
        public const string GalleryCheckmarkImage = "img/t-gallery-check.png";
        public const int GallerySectionVerticalSpacing = 5;
        public const string GalleryNextButtonTitle = "Next";
        public const int GalleryToolbarHeight = 40;
        public const int GalleryImageYOffset = 20;
        public const int GalleryImageTimestampHeight = 20;
        public const string HeaderDatestampFormat = "MMMM dd, yyyy";
        public const string GallerySettingsSwitchState = "GallerySettingsSwitchState";
        public const string ConfidenceThresholdUserDefaultName = "ConfidenceThreshold";
        #endregion

        #region TimeLine
        public const int XPoint = 0;
        public const int YPoint = 50;
        public const int XIncrement = 117;
        public const string UIMediaType = "public.image";
        public const int NumberofCols = 4;
        public const int NumberofInteritemSpacings = 2;
        public const int ThumbnailWidth = 115;
        public const int ThumbnailHeight = 125;
        public const int TimeLineScrollViewHeight = 175;
        public const int Origin = 0;
        public const int SubheadingViewHeight = 50;
        public const int SubheadingFontSizeCutoff = 13;
        public const int SubheadingTopLargeFontInset = 4;
        public const int SubheadingTopSmallFontInset = 20;
        public const string DateTimeFormat = "MM/dd/yyyy H:mm";
        public const float MinimumZoomScale = 1f;
        public static readonly UIColor TimeLineScrollViewColour = UIColor.GroupTableViewBackgroundColor;
        public static readonly UIColor LegendViewColour = UIColor.GroupTableViewBackgroundColor;
        public static readonly UIColor SubheadingTextColour = UIColor.Black;
        public const UITextAlignment SubheadingTextAlignment = UITextAlignment.Center;
        public const int TotalThumbnailWidth = 117;
        public const float UnselectedBorderWidth = 0;
        public const float SelectedBorderWidth = 2;
        public const int ThumbnailStackViewIndex = 0;
        public const int SubheadingStackViewIndex = 1;
        public const int CloseButtonMargin = 10;
        public const float TimelineThresholdForHeight = 170.0f; 
        public const int HighlightRedValue = 14;
        public const int HighlightGreenValue = 122;
        public const int HighlighBlueValue = 254;
        #endregion

        #region Sidebar
        public const string FilterCellViewControllerName = "FilterCellViewController";
        public const string FilterCellIdentifier = "CellID";
        #endregion

        #region relabing model UI setting 
        public const int PopoverEditSquareIconSize = 22;
        public const int PopoverIconMargin = 10;
        public const int RelabelMaxCharacters = 25;
        public const string PopoverEditIconFilePath = "img/PenTool_22.png";
        public const string EditModalControllerName = "EditModalController";
        public const float EditModalShadowOffset = 1.0f;
        public static readonly CGColor EditModalShadowColour = UIColor.Gray.CGColor;
        public const float EditModalShadowOpacity = 1.0f;
        public const float EditModalShadowRadius = 10;

        #endregion

        #region Data Visualization
        public const UILayoutConstraintAxis LegendStackViewAxis = UILayoutConstraintAxis.Horizontal;
        public const UIStackViewAlignment LegendStackViewAlignment = UIStackViewAlignment.Center;
        public const UIStackViewDistribution LegendStackViewDistribution = UIStackViewDistribution.EqualSpacing;
        public static readonly nfloat LegendStackViewSpacing = 25;
        public const UILayoutConstraintAxis LabelStackViewAxis = UILayoutConstraintAxis.Horizontal;
        public const UIStackViewAlignment LabelStackViewAlignment = UIStackViewAlignment.Center;
        public const UIStackViewDistribution LabelStackViewDistribution = UIStackViewDistribution.EqualCentering;
        public static readonly nfloat LabelStackViewSpacing = 10;
        public const int LegendHeight = 40;
        public const int LabelCircleRadius = 10;
        public const int LabelFontSize = 15;
        public static readonly UIColor LabelTextColor = UIColor.Black;
        public static readonly UIColor CountLabelTextColor = UIColor.White;
        public static readonly UIColor CountLabelBackgroundColor = UIColor.Black;
        public const int CountLabelTextFontSize = 12;
        public const int CountLabelXOffest = 2;
        public const float CountLabelOpacity = 0.65f;
        public const float CountLabelCornerRatio = 0.2f;
        #endregion

        #region Bar Chart
        public const UILayoutConstraintAxis BarChartStackViewAxis = UILayoutConstraintAxis.Horizontal;
        public const UIStackViewAlignment BarChartStackViewAlignment = UIStackViewAlignment.Bottom;
        public const UIStackViewDistribution BarChartStackViewDistribution = UIStackViewDistribution.EqualCentering;
        public static readonly nfloat BarChartStackViewSpacing = 3;
        public static readonly nfloat BarChartMaxBarHeight = 150;
        public static readonly nfloat BarChartBarWidthTotal = 60;
        public static readonly nfloat BarChartBarCornerRadius = 5;
        public static readonly nfloat BarChartWidth = ThumbnailWidth;
        public static readonly nfloat BarChartHeight = BarChartMaxBarHeight;
        #endregion

        #region Bar Chart Series
        public const UILayoutConstraintAxis BarChartSeriesStackViewAxis = UILayoutConstraintAxis.Horizontal;
        public const UIStackViewAlignment BarChartSeriesStackViewAlignment = UIStackViewAlignment.Center;
        public const UIStackViewDistribution BarChartSeriesStackViewDistribution =
            UIStackViewDistribution.EqualCentering;
        public static readonly nfloat BarChartSeriesStackViewSpacing = 2;
        public static readonly nfloat BarChartSeriesStackViewHeight = BarChartHeight;
        public static readonly nfloat MaxDataPointFactor = (nfloat) 0.9;
        #endregion

        #region New Session Dropdown Menu
        public const int DropDownMenuWidth = 270;
        public const int DropDownMenuHeight = 118;
        #endregion

        #region Settings
        public const string AboutLabel = "About";
        public const string RepositoryLabel = "View Repository";
        public const string PreferencesLabel = "Preferences";
        public const string PrivacyLabel = "Privacy & Cookies";
        public const float NavBarHeight = 64;
        public const float TableLeadingMargin = 20;
        public const float TableTrailingMargin = 40;
        public const float ViewTopMargin = NavBarHeight + 36;
        public const int TableFontSize = 14;
        public static readonly UIColor TextColour = UIColor.Black;
        public const string GalleryCellLabel = "View Chronologically";
        public const string ConfidenceCellLabel = "Set Confidence Threshold";
        public const double ConfidenceMinValue = 1;
        public const double ConfidenceMaxValue = 100;
        public const double StepValue = 1;
        public const string GallerySubheading = "GALLERY PREFERENCES";
        public const string ConfidenceSubheading = "SESSION PREFERENCES";
        public const string ConfidenceNote =
            "The new confidence threshold will be effective next time an image is processed.";
        public static readonly UIColor SettingsSubheadingTextColour = UIColor.LightGray; 
        public static readonly CGColor TableBorderColour = UIColor.LightGray.CGColor;
        public const float TableBorderSize = 1;
        public static readonly UIColor TableBackgroundColour = UIColor.GroupTableViewBackgroundColor;
        public const string SettingCellKey = "SettingCell";
        public const int SettingsNumberOfRows = 4;
        public const int SettingsNumberOfSections = 1;
        public const float LeadingMargin = 20;
        public const float GallerySubheadingTopMargin = 40;
        public const float SubheadingHeight = 30;
        public const float SubheadingWidth = 400;
        public const float TrailingMargin = -20;
        public const float GalleryCellTopMargin = GallerySubheadingTopMargin + 30;
        public const float CellHeight = 50;
        public const float ConfidenceSubheadingTopMargin = GalleryCellTopMargin + 60;
        public const float ConfidenceCellTopMargin = ConfidenceSubheadingTopMargin + 30;
        public const float ConfidenceNoteTopMargin = ConfidenceCellTopMargin + 50;
        public const float ToggleWidth = 50;
        public const float ToggleTopMargin = 10;
        public const float ToggleTrailingMargin = -50;
        public const float ConfidenceLabelWidth = 40;
        public const float ConfidenceLabelTrailingMargin = -100;
        public const float GalleryToggleTrailingMargin = -10;
        public const string ConfidenceLabelFormat = "{0}%";
        public const float TextViewTopInset = 15;
        public const float TextViewInset = 10;
        public const int LinkFontSize = 20;
        public const string GithubLink = "https://github.com/Microsoft/Earth-Lens";
        public const string GithubLinkLabel = "Github Repository";
        public const string LinkFormat = "<a href='{0}'>{1}</a>";
        public const string PrivacyLink = "https://go.microsoft.com/fwlink/?LinkId=521839";
        public const string PrivacyLinkLabel = "Privacy & Cookies";
        public const string GarageLink = "http://garage.microsoft.com";
        #endregion

        #region Storyboards
        public const string AnalyzingPageStoryboardName = "AnalyzingPage";
        public const string GalleryStoryboardName = "Gallery";
        public const string ImageUploadStoryboardName = "ImageUpload";
        public const string SingleImageStoryboardName = "SingleImage";
        public const string TimeLineStoryboardName = "TimeLine";
        public const string WelcomeSplashStoryboardName = "WelcomeSplash";
        public const string SettingsStoryboardName = "Settings";
        #endregion
    }
}
