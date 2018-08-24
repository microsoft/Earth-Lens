// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace EarthLens.iOS.Popover
{
    [Register ("EditModalController")]
    partial class EditModalController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CancelButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField ConfidenceTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView EditTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView LabelStaticText { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField LabelTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ModalView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton OkButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CancelButton != null) {
                CancelButton.Dispose ();
                CancelButton = null;
            }

            if (ConfidenceTextField != null) {
                ConfidenceTextField.Dispose ();
                ConfidenceTextField = null;
            }

            if (EditTitle != null) {
                EditTitle.Dispose ();
                EditTitle = null;
            }

            if (LabelStaticText != null) {
                LabelStaticText.Dispose ();
                LabelStaticText = null;
            }

            if (LabelTextField != null) {
                LabelTextField.Dispose ();
                LabelTextField = null;
            }

            if (ModalView != null) {
                ModalView.Dispose ();
                ModalView = null;
            }

            if (OkButton != null) {
                OkButton.Dispose ();
                OkButton = null;
            }
        }
    }
}