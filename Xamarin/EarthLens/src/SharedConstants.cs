using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

namespace EarthLens
{
    public static class SharedConstants
    {
        #region Environment Variables
        public const string EnvironmentResouceName = "Environment.txt";
        public const string AppCenterSecretKeyIOS = "APP_CENTER_SECRET_IOS";
        public const string ResourceStringFormat = "{0}.{1}";
        public const string AppName = "EarthLens";
        #endregion

        #region Share/Export Graphic Properties
        public static readonly SKColor BoundingBoxColorExport = SKColors.Red;
        public const float BoundingBoxWidthExport = 2f;
        #endregion

        #region Database Constants
        public const string DBName = "database.db";
        public const string ImageCollectionName = "ImageCollection";
        public const string ObservationCollectionName = "Observations";
        #endregion

        #region Model Pre-/Post-Processing Constants
        public const int DefaultChipWidth = 300;
        public const int DefaultChipHeight = 300;
        #endregion

        #region Model MegaClasses Labeling Constants
        public const string MegaCategoryMappingJsonFilePath =
            @"EarthLens.Resources.MegaCategory_Category_mapping.json";
        public const string CustomMegaCategoryLabel = "Custom";
        #endregion

        #region Image Base64 Encoding
        public const SKEncodedImageFormat EncodingFormat = SKEncodedImageFormat.Jpeg;
        public const int EncodingQuality = 100;
        #endregion
        
        #region UX Constants
        public const int WelcomeSplashDelay = 2000;
        public const string CancelButtonText = "Cancel";
        public const int NumberOfImageUploadComponents = 2;
        public static readonly SKColor DefaultBoundingBoxColor = new SKColor(255, 59, 48);
        #endregion

        #region CoreML Model Constants
        public const double DefaultIOUThreshold = 0.5;
        public const double DefaultConfidenceThreshold = 0.1;
        #endregion

        #region Share Extensions
        public const string ImageKey = "com.microsoft.EarthLens.SharedImage";
        public const string AppGroupID = "group.com.microsoft.EarthLens";
        #endregion

        #region JSON Constants
        public const string JsonId = "Id";
        public const string JsonConfidence = "Confidence";
        public const string JsonCategoryId = "CategoryId";
        public const string JsonCategoryLabel = "CategoryLabel";
        public const string JsonBoundBoxLeft = "BoundBoxLeft";
        public const string JsonBoundBoxRight = "BoundBoxRight";
        public const string JsonBoundBoxTop = "BoundBoxTop";
        public const string JsonBoundBoxBottom = "BoundBoxBottom";
        public const string JsonUploadDate = "UploadDate";
        public const string JsonBase64 = "Base64";
        public const string JsonName = "Name";
        public const string JsonCreationTime = "CreationTime";
        public const string JsonObservations = "Observations";
        #endregion

        #region Image Too Large Notification
        public const int ImagesTooLargeNumberOfChipsThreshold = 40;
        public const string ImagesTooLargeTitle = "Image(s) Too Large";
        public const string ImagesTooLargeMessage = "The image(s) are too large and cannot be processed.";
        public const string ImagesTooLargeBackAction = "Back";
        #endregion

        #region Gallery
        public const string SaveToGalleryTitle = "Save Session to Gallery?";
        public const string SaveToGalleryDescription =
            "Save this session to the internal gallery to store it for quick access later.";
        public const string SaveToGalleryYes = "Yes";
        public const string SaveToGalleryNo = "No";
        public const string SaveToGalleryCancel = "Cancel";
        public const string SelectedImageCountFormatPlural = "{0} Images Selected";
        public const string SelectedImageCountFormatSingular = "{0} Image Selected";
        public const string ConfirmGalleryDelete = "Delete";
        public const string DeleteFromGalleryDescription =
            "These photo(s) will only be deleted from your " + AppName + " Gallery";
        public const string ConfirmDeleteTitleFormatPlural = "Delete {0} Photos";
        public const string ConfirmDeleteTitleFormatSingular = "Delete {0} Photo";
        #endregion

        #region Analyzing Page
        public const string StoppingAnalysisLabelText = "Stopping analysis...";
        #endregion

        #region Sidebar
        public const string NoResultFormat = "No Results for \"{0}\"";
        public const string SidebarSubtitleFormatPlural = "Showing {0} of {1} classes";
        public const string SidebarSubtitleFormatSingular = "Showing {0} of {1} class";
        public const string ClassCountFormatPlural = "{0} Classes Found";
        public const string ClassCountFormatSingular = "{0} Class Found";
        #endregion

        #region Popover
        public const string LabelConfidenceFormat = " {0}  {1}%";
        public const string LabelOnlyFormat = " {0}";
        #endregion

        #region Results
        public const string NotificationTextFormat = "You have chosen the same colour for {0}";
        public const string ImageTitleFormat = "{0} - ";
        public const string PageTitleFormat = "{0}{1} - {2} (UTC)";
        public const string SavedLabel = " - Saved";
        #endregion

        #region Image Upload
        public const string CameraAccessDeniedAlertTitle = "Camera Permissions";
        public const string CameraAccessDeniedAlertMessage =
            "The app does not have permission to access your camera. Please enter settings.";
        public const string CameraAccessDeniedAlertCloseAction = "Close";
        public const string CameraAccessDeniedAlertOpenSettingsAction = "Open Settings";
        public const string VideoSelectedAlertTitle = "Video Selected";
        public const string VideoSelectedAlertMessage = "Please select an image instead.";
        public const string VideoSelectedAlertOkAction = "OK";
        public const string ShareExtensionErrorAlertTitle = "Error Launching from Share Extension";
        public const string ShareExtensionErrorAlertMessage = "Please try again.";
        public const string ShareExtensionErrorAlertOkAction = "OK";
        #endregion

        #region Color Picker
        public static readonly IList<SKColor> SystemDefaultColors = new List<SKColor>
        {
            new SKColor(255, 59, 48),
            new SKColor(255, 149, 0),
            new SKColor(255, 204, 0),
            new SKColor(76, 217, 100),
            new SKColor(90, 200, 250),
            new SKColor(0, 122, 255),
            new SKColor(88, 86, 214),
            new SKColor(255, 45, 85)
        }.AsReadOnly();
        public static readonly IList<SKColor> OtherRecommendedColors = new List<SKColor>
        {
            new SKColor(231, 159, 213),
            new SKColor(12, 255, 255),
            new SKColor(1, 69, 12),
            new SKColor(0, 0, 0),
            new SKColor(186, 0, 75),
            new SKColor(203, 251, 211),
            new SKColor(139, 87, 42),
            new SKColor(196, 223, 255),
            new SKColor(255, 204, 145),
            new SKColor(146, 1, 176),
            new SKColor(255, 239, 239),
            new SKColor(74, 74, 74),
            new SKColor(155, 155, 155),
            new SKColor(64, 1, 140)
        }.AsReadOnly();
        public static readonly IList<SKColor> AllAvailableColors =
            SystemDefaultColors.Concat(OtherRecommendedColors).ToList().AsReadOnly();
        #endregion

        #region Settings
        public const string AboutContent =
        "Earth Lens, a Microsoft Garage project, is an iOS application that helps people quickly identify and classify objects in aerial imagery through the power of machine learning.\n\n" +
        "The Microsoft Garage is an outlet for experimental projects for you to try. Learn more at ";
        public const string CopyrightInfo =
        ".\n\nVersion 0.1\n\n"+
        "© Microsoft 2018. All rights reserved.";
        #endregion Settings

        #region WelcomeScreen
        public const string LinkText = "Learn more at ";
        #endregion
    }
}
