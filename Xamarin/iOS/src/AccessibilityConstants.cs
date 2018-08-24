namespace EarthLens.iOS
{
    public static class AccessibilityConstants
    {
        #region ResultsViewController
        public const string SidebarCloseButtonAccessibilityLabel = "Close";
        public const string ImageTitleAccessibilityLabel = "Image name {0}";
        public const string SidebarCloseButtonAccessibilityHint = "Dismiss the view details sidebar";
        #endregion

        #region FilterViewController
        public const string SidebarAccessibilityLabel = "Sidebar";
        public const string SidebarCellAccessibilityLabel = "Detected class";
        public const string SidebarColorButtonAccessibilityLabel = "Legend, colour number {0}";
        public const string SidebarColorButtonAccessibilityHint = "Click to change color";
        public const string SidebarCheckBoxAccessibilityLabel = "Check Box";
        public const string SidebarCheckBoxAccessibilityUncheckHint = "Uncheck to hide the class' bounding boxes";
        public const string SidebarCheckBoxAccessibilityCheckHint = "Check to show the class' bounding boxes";
        #endregion

        #region ResultsViewController Bounding Boxes
        public const string BoundingBoxAccessibilityLabel = "Bounding box {0} color number {1}";
        public const string BoundingBoxAccessibilityHint = "Click to view more";
        #endregion

        #region ColorPicker
        public const string ColorPickerColorButtonAccessibilityLabel = "Color number";
        public const string ColorPickerColorButtonAccessibilityHint = "Click to select the colour";
        #endregion

        #region same colour notification view
        public const float SlidingAnimationDurationWithVoiceOver = 12f;
        #endregion

        #region popover
        public const string PopOverEditButtonAccessibilityLabel = "Edit";
        public const string PopOverEditButtonAccessibilityHint = "Click to Edit the popover detail label";
        #endregion

        #region EditModalController 
        public const string LabelStaticTextAccessibilityLabel = "Class name label";
        #endregion

        #region Timeline View
        public const string ImageThumbNameAccessibilityHint = "Click to view the details of the image";
        public const string TimelineLegendColorButtonAccessibilityLabel = "Legend, colour number {0}";
        #endregion

        #region Data Visualizer
        public const string DataVisualizerCloseButtonAccessibilityLabel = "Close";
        public const string DataVisualizerCloseButtonAccessibilityHint = "Dismiss the data visualizer";
        public const string CountPopoverAccessibilityLabel = "Class count: {0}";
        public const string BarAccessibilityLabel = "Color number {0}, {1}, count: {2}";
        public const string BarAccessibilityHint = "Click to view class count";
        #endregion

        #region Gallery
        public const string ImageDateAccessibilityLabel = "Created date: {0}";
        public const string ImageSelectionAccessibilityCheckHint = "Check to select the image";
        public const string ImageSelectionAccessibilityUnCheckHint = "Uncheck to deselect the image";
        #endregion

        #region
        public const string AnalyzeStartedNotificationAccessibilityString = "Processing your upload";
        #endregion
    }
}
