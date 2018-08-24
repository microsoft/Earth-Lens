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

namespace EarthLens.iOS.Results
{
    [Register ("NewSessionChoiceViewController")]
    partial class NewSessionChoiceViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ChooseFromGalleryButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView MenuView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton UploadPhotoButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ChooseFromGalleryButton != null) {
                ChooseFromGalleryButton.Dispose ();
                ChooseFromGalleryButton = null;
            }

            if (MenuView != null) {
                MenuView.Dispose ();
                MenuView = null;
            }

            if (UploadPhotoButton != null) {
                UploadPhotoButton.Dispose ();
                UploadPhotoButton = null;
            }
        }
    }
}