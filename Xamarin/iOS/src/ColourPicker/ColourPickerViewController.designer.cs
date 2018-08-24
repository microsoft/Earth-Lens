// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace EarthLens.iOS.ColourPicker
{
    [Register ("ColourPickerViewController")]
    partial class ColourPickerViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView ClassName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ColorCancel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ColorConfirm { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ColorPickerPopup { get; set; }

        [Action ("ColorCancel_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ColorCancel_TouchUpInside (UIKit.UIButton sender);

        [Action ("ColourButtonClicker_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ColourButtonClicker_TouchUpInside (UIKit.UIButton sender);

        [Action ("ColourConfirm_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ColourConfirm_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ClassName != null) {
                ClassName.Dispose ();
                ClassName = null;
            }

            if (ColorCancel != null) {
                ColorCancel.Dispose ();
                ColorCancel = null;
            }

            if (ColorConfirm != null) {
                ColorConfirm.Dispose ();
                ColorConfirm = null;
            }

            if (ColorPickerPopup != null) {
                ColorPickerPopup.Dispose ();
                ColorPickerPopup = null;
            }
        }
    }
}