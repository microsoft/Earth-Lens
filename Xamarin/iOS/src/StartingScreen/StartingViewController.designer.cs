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

namespace EarthLens.iOS.StartingScreen
{
    [Register ("StartingViewController")]
    partial class StartingViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView PrivacyLink { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton StartButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView StartDisclaimer { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView StartSubtitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView StartTitle { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (PrivacyLink != null) {
                PrivacyLink.Dispose ();
                PrivacyLink = null;
            }

            if (StartButton != null) {
                StartButton.Dispose ();
                StartButton = null;
            }

            if (StartDisclaimer != null) {
                StartDisclaimer.Dispose ();
                StartDisclaimer = null;
            }

            if (StartSubtitle != null) {
                StartSubtitle.Dispose ();
                StartSubtitle = null;
            }

            if (StartTitle != null) {
                StartTitle.Dispose ();
                StartTitle = null;
            }
        }
    }
}