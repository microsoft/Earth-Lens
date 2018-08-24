using System.Collections.Generic;
using System.Globalization;
using EarthLens.iOS.Sidebar;
using EarthLens.iOS.Results;
using EarthLens.Models;
using CoreGraphics;
using SkiaSharp.Views.iOS;
using UIKit;

namespace EarthLens.iOS.ColourPicker
{
    public partial class ColourPickerViewController : UIViewController
    {
        public UIButton ClickedButton { get; set; }
        public Dictionary<Observation, UIButton> Observations { get; set; }
        public UITextView Category { get; set; }
        public FilterViewController ParentTableView { get; set; }
        public ResultsViewController RootResultsViewController { get; set; }

        private UIButton _lastTappedColorButton;
        private CGColor _originalColour;

        public ColourPickerViewController() : base("ColourPickerViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetColourPickerAttributes();

            // keep the original colour of colour button in case user want to revert to original colour
            _originalColour = ClickedButton.Layer.BackgroundColor;

            ClassName.Text = Category.Text;
        }

        private void SetColourPickerAttributes()
        {
            View.Frame = ColorPickerPopup.Frame;
            ColorPickerPopup.Layer.ShadowColor = Constants.ColorPickerPopupShadowColour;
            ColorPickerPopup.Layer.ShadowRadius = Constants.ColorPickerShadowRadius;
            ColorPickerPopup.Layer.ShadowOpacity = Constants.ColorPickerShadowOpacity;
            ColorPickerPopup.Layer.ShadowOffset =
                new CGSize(Constants.ColorPickerShadowOffset, Constants.ColorPickerShadowOffset);
            ColorPickerPopup.Layer.MasksToBounds = false;
            _originalColour = ClickedButton.Layer.BackgroundColor;
            EnableColorChoiceEncodingVoiceOver();
        }

        partial void ColorCancel_TouchUpInside(UIButton sender)
        {
            // remove all the previewed colour of bounding box and dismiss colour picker popup
            if (_lastTappedColorButton != null)
            {
                _lastTappedColorButton.Layer.ShadowColor = Constants.ColorPickerPopupUnselectedColour;
                _lastTappedColorButton.Layer.ShadowOpacity = Constants.ClearShadowOpacity;
                foreach (var binding in Observations)
                {
                    if (binding.Key.Category.Label != Category.Text) continue;
                    binding.Value.Hidden = true;
                    binding.Key.Category.Color = _originalColour.ToSKColor();
                    binding.Value.Layer.BorderColor = _originalColour;
                    binding.Value.Hidden = false;
                }
            }
            DismissModalViewController(false);
        }

        partial void ColourButtonClicker_TouchUpInside(UIButton sender)
        {
            var buttonColor = sender.Layer.BackgroundColor;
            if (_lastTappedColorButton != null)
            {
                _lastTappedColorButton.Layer.ShadowColor = Constants.ColorPickerPopupUnselectedColour;
                sender.Layer.ShadowOpacity = Constants.ClearShadowOpacity;
            }

            // set button hightlight effect
            sender.Layer.ShadowColor = buttonColor;
            sender.Layer.ShadowRadius = Constants.ColorButtonShadowRadius;
            sender.Layer.ShadowOpacity = Constants.ColorButtonShadowOpacity;
            sender.Layer.ShadowOffset =
                new CGSize(Constants.ColorPickerShadowOffset, Constants.ColorPickerShadowOffset);
            sender.Layer.MasksToBounds = false;

            // Reset bounding box colours
            ResetBoundingBoxColour(sender);
            _lastTappedColorButton = sender;
        }

        partial void ColourConfirm_TouchUpInside(UIButton sender)
        {
            // set bounding box color same as the previewed colour choosed by user and close colour picker popup
            if (_lastTappedColorButton != null)
            {
                var oldColour = _originalColour;
                var newColour = _lastTappedColorButton.BackgroundColor;

                ClickedButton.BackgroundColor = newColour;
                _originalColour = newColour.CGColor;
                ResetBoundingBoxColour(_lastTappedColorButton);
                RootResultsViewController.UpdateTitleForUnsaved();

                // Notify the sidebar table view that the colour has been changed
                if (!oldColour.Equals(newColour.CGColor))
                {
                    ParentTableView.NotifyBoundingBoxColourDidChange(oldColour,
                                                                     newColour.CGColor, Category.Text);
                    ParentTableView.ParentResultsViewController.NotifyColorChanged(Category.Text,
                        newColour.ToSKColor());
                }

                FilterViewController.ConfigureColourButtonVoiceoverAccessibilityAttributes(ClickedButton, newColour.CGColor);
            }

            // Dismiss view controller
            DismissModalViewController(false);
        }

        /// <summary>
        /// Reset the bounding box colour of a sprecific category to customized colour
        /// </summary>
        /// <param name="lastTappedColorButton"> The button with the customized colour chosen by user </param>
        private void ResetBoundingBoxColour(UIButton lastTappedColorButton)
        {
            foreach (var binding in Observations)
            {
                // Re-draw the colour of bounding boxes
                if (binding.Key.Category.Label != Category.Text) continue;
                if (!binding.Value.Hidden)
                {
                    binding.Value.Hidden = true;

                    // Reset the color attribute of the specific observation
                    binding.Key.Category.Color = lastTappedColorButton.BackgroundColor.ToSKColor();
                    binding.Value.Layer.BorderColor = lastTappedColorButton.BackgroundColor.CGColor;
                    binding.Value.Hidden = false;
                }
                else
                {
                    // Reset the color attribute of the specific observation
                    binding.Key.Category.Color = lastTappedColorButton.BackgroundColor.ToSKColor();
                    binding.Value.Layer.BorderColor = lastTappedColorButton.BackgroundColor.CGColor;
                }
            }
        }

        #region Accessibility Configuration
        /// <summary>
        /// Encoding all colour choice buttons that are not "cancel" or "confirm" to a voice over as colour #1, colour #2 ...
        /// </summary>
        private void EnableColorChoiceEncodingVoiceOver()
        {
            foreach(var subview in View.Subviews[0].Subviews)
            {
                if (subview is UIButton && subview.Tag != ColorConfirm.Tag && subview.Tag != ColorCancel.Tag)
                {
                    var colorChoiceButton = (UIButton)(subview);
                    var colorNumber = FilterViewController.GetColorVoiceOverEncoding(colorChoiceButton.BackgroundColor.CGColor);

                    colorChoiceButton.AccessibilityLabel = string.Format(CultureInfo.CurrentCulture, AccessibilityConstants.ColorPickerColorButtonAccessibilityLabel, colorNumber);
                    colorChoiceButton.AccessibilityHint = AccessibilityConstants.ColorPickerColorButtonAccessibilityHint;
                    colorChoiceButton.AccessibilityTraits = UIAccessibilityTrait.SummaryElement;
                }
            }
        }
        #endregion
    }
}

