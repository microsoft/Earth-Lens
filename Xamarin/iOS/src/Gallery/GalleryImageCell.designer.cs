// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace EarthLens.iOS.Gallery
{
    [Register ("GalleryImageCell")]
    partial class GalleryImageCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView GalleryImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView GalleryImageCheck { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel GalleryImageTimeStamp { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (GalleryImage != null) {
                GalleryImage.Dispose ();
                GalleryImage = null;
            }

            if (GalleryImageCheck != null) {
                GalleryImageCheck.Dispose ();
                GalleryImageCheck = null;
            }

            if (GalleryImageTimeStamp != null) {
                GalleryImageTimeStamp.Dispose ();
                GalleryImageTimeStamp = null;
            }
        }
    }
}