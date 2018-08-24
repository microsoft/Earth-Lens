// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace EarthLens.iOS.ImageUpload
{
    [Register ("ImageUploadViewController")]
    partial class ImageUploadViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton GalleryButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton PhotoUploadButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (GalleryButton != null) {
                GalleryButton.Dispose ();
                GalleryButton = null;
            }

            if (PhotoUploadButton != null) {
                PhotoUploadButton.Dispose ();
                PhotoUploadButton = null;
            }
        }
    }
}