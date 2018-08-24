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
    [Register ("GalleryViewController")]
    partial class GalleryViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UICollectionView GalleryCollectionView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView GalleryMainView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (GalleryCollectionView != null) {
                GalleryCollectionView.Dispose ();
                GalleryCollectionView = null;
            }

            if (GalleryMainView != null) {
                GalleryMainView.Dispose ();
                GalleryMainView = null;
            }
        }
    }
}