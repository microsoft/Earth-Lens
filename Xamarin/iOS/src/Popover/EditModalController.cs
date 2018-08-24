using System;
using CoreGraphics;
using UIKit;

namespace EarthLens.iOS.Popover
{
    public partial class EditModalController : UIViewController
    {
        private readonly PopoverViewController _parentPopOverController;

        public EditModalController(PopoverViewController parent) : base(Constants.EditModalControllerName, null)
        {
            _parentPopOverController = parent ?? throw new ArgumentNullException();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ModalView.Layer.ShadowColor = Constants.EditModalShadowColour;
            ModalView.Layer.ShadowRadius = Constants.EditModalShadowRadius;
            ModalView.Layer.ShadowOpacity = Constants.EditModalShadowOpacity;
            ModalView.Layer.ShadowOffset = new CGSize(Constants.EditModalShadowOffset, Constants.EditModalShadowOffset);
            ModalView.ClipsToBounds = true;
            ModalView.Layer.MasksToBounds = false;

            // set the maximum number of characters for label input 
            LabelTextField.EditingChanged += (sender, e) => 
            {
                var text = LabelTextField.Text;
                if (text.Length <= Constants.RelabelMaxCharacters) return;
                text = text.Remove(text.Length - 1);
                LabelTextField.Text = text;
            };

            CancelButton.TouchUpInside += (sender, e) =>
            {
                DismissViewController(false, null);
            };

            OkButton.TouchUpInside += (sender, e) =>
            {
                var userInputLabel = LabelTextField.Text;
                var userInputConfidence = ConfidenceTextField.Text;

                _parentPopOverController.NotifyPopoverDidChange(userInputLabel, userInputConfidence);

                DismissViewController(false, null);
            };
        }

        #region Accessibility Configurations
        private void ConfigureEditButtonsAccessibilityAttributes()
        {
            LabelStaticText.AccessibilityLabel = AccessibilityConstants.LabelStaticTextAccessibilityLabel;
        }
        #endregion
    }
}
