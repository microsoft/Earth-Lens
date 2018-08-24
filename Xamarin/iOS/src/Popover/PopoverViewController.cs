using System;
using System.Globalization;
using EarthLens.iOS.Sidebar;
using EarthLens.Models;
using CoreGraphics;
using Foundation;
using UIKit;

namespace EarthLens.iOS.Popover
{
    public partial class PopoverViewController : UIViewController
    {
        public Observation Observation { get; set; }
        public FilterViewController ParentFilterViewController { get; set; }

        private UIButton _editButton;
        private EditModalController _editModal;
        private UITextView _label;

        public PopoverViewController() : base("PopoverViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var newLabelString = GeneratePopoverText(Observation.Category.Label, Observation.Confidence);
            ConfigurePopoverViewLayout(newLabelString);
        }

        private void ConfigureEditButton()
        {
            _editButton = new UIButton();
            View.AddSubview(_editButton);

            var buttonBackground = UIImage.FromFile(Constants.PopoverEditIconFilePath);
            _editButton.SetBackgroundImage(buttonBackground, UIControlState.Normal);

            _editButton.Frame = new CGRect(
                PreferredContentSize.Width - (Constants.PopoverIconMargin + Constants.PopoverEditSquareIconSize),
                (View.Frame.Height - Constants.PopoverEditSquareIconSize) / 2, Constants.PopoverEditSquareIconSize,
                Constants.PopoverEditSquareIconSize);
            _editButton.TouchUpInside += (sender, e) =>
            {
                PresentEditModal();
                View.SizeToFit();
            };

            View.BringSubviewToFront(_editButton);

            ConfigureEditButtonAccessibilityAttributes(_editButton);
        }

        private void PresentEditModal()
        {
            _editModal = new EditModalController(this) { ModalPresentationStyle = UIModalPresentationStyle.Custom};

            PresentViewController(_editModal, false, null);
        }

        /// <summary>
        /// Pre-processes the user input with handling of silly input.
        /// </summary>
        /// <returns>A value pair of label string and confidence that is going to be used by other view controller.</returns>
        /// <param name="confidenceString">Raw confidence that the user input.</param>
        private static double PreProcessConfidenceInput(string confidenceString)
        {
            // check if confidence is only number
            double confidence = 100;

            if (string.IsNullOrWhiteSpace(confidenceString))
            {
                confidence = 100;
            }
            else if (decimal.TryParse(confidenceString, out var c))
            {
                confidence = (double)c;
                if (confidence > 100 || confidence < 0)
                {
                    confidence = 100;
                }
            }

            return confidence;
        }

        /// <summary>
        /// Generates the popover text with handling of silly user input.
        /// </summary>
        /// <returns>The popover text with different attributes.</returns>
        /// <param name="userLabel">Label that the user input</param>
        /// <param name="confidence">pre-processed confidence that the user input </param>
        private static NSMutableAttributedString GeneratePopoverText(string userLabel, double confidence)
        {
            var txt = string.Format(CultureInfo.CurrentCulture, SharedConstants.LabelConfidenceFormat, userLabel,
                Math.Round(confidence, 4) * 100);
            var atts = new UIStringAttributes();
            var attributedString = new NSMutableAttributedString(txt, atts);
            userLabel = string.Format(CultureInfo.CurrentCulture, SharedConstants.LabelOnlyFormat, userLabel);
            var labelLength = userLabel.Length;

            attributedString.BeginEditing();
            attributedString.AddAttribute(UIStringAttributeKey.Font,
                UIFont.PreferredHeadline, new NSRange(0, labelLength));
            attributedString.AddAttribute(UIStringAttributeKey.Font, UIFont.PreferredBody,
                new NSRange(labelLength + 1, txt.Length - labelLength - 1));
            attributedString.EndEditing();

            return attributedString;
        }

        /// <summary>
        /// A function to handle the update of popover when user change the label 
        /// Should only be called by the EditModalController
        /// </summary>
        /// <returns>The popover text with different attributes.</returns>
        /// <param name="inputValue">the raw Label that the user input</param>
        /// <param name="inputConfidence">the raw confidence string that the user input </param>
        public void NotifyPopoverDidChange(string inputValue, string inputConfidence)
        {
            // checking for silly input
            var confidence = PreProcessConfidenceInput(inputConfidence);

            // assign the new confidence
            confidence = confidence / 100;
            Observation.Confidence = confidence;

            var parentResultView = ParentFilterViewController.ParentResultsViewController;

            // update the current pop-up label
            parentResultView.DismissViewController(false, null);
            DismissViewController(false, null);   // this is not properly dismissing
            parentResultView.NotifyLabelOfObservationDidChange(Observation, inputValue);

        }


        private void ConfigurePopoverViewLayout(NSAttributedString newLabelString)
        {
            if (_label == null)
            {
                _label = new UITextView { AttributedText = newLabelString };
                _label.SizeToFit();
                _label.Editable = false;
                _label.TextAlignment = UITextAlignment.Left;
                _label.TextColor = Constants.PopoverTextColor;
                _label.BackgroundColor = Constants.PopoverBackgroundColor;

                // Create a UI view to show in the popover
                var popView = new UIScrollView(_label.Frame) { UserInteractionEnabled = true }; // change to false?? 
                popView.AddSubview(_label);
                popView.ScrollEnabled = true;
                popView.SizeToFit();
                View = popView;

                //change number to change the width and length of popover
                PreferredContentSize =
                    new CGSize(popView.Frame.Width + Constants.PopoverEditSquareIconSize + Constants.PopoverIconMargin * 2,
                        popView.Frame.Height);

                //add in the button 
                ConfigureEditButton();
            }
            else
            {
                _label.AttributedText = newLabelString;
                _label.SizeToFit();
                View.SizeToFit();
                PreferredContentSize =
                    new CGSize(_label.Frame.Width + Constants.PopoverEditSquareIconSize + Constants.PopoverIconMargin * 2,
                               _label.Frame.Height);

                _editButton.Frame = new CGRect(
                                        PreferredContentSize.Width - (Constants.PopoverIconMargin + Constants.PopoverEditSquareIconSize),
                                        (_label.Frame.Height - Constants.PopoverEditSquareIconSize) / 2, Constants.PopoverEditSquareIconSize,
                                        Constants.PopoverEditSquareIconSize);   
            }
        }

        #region Accessibility Configuration
        private void ConfigureEditButtonAccessibilityAttributes(UIButton editButton)
        {
            editButton.AccessibilityTraits = UIAccessibilityTrait.Button;
            editButton.AccessibilityLabel = AccessibilityConstants.PopOverEditButtonAccessibilityLabel;
            editButton.AccessibilityHint = AccessibilityConstants.PopOverEditButtonAccessibilityHint;
        }
        #endregion
    }
}

