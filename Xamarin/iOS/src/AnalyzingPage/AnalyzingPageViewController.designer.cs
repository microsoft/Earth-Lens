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

namespace EarthLens.iOS.AnalyzingPage
{
    [Register ("AnalysisViewController")]
    partial class AnalyzingPageViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel AnalyzingLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIProgressView ProgressView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AnalyzingLabel != null) {
                AnalyzingLabel.Dispose ();
                AnalyzingLabel = null;
            }

            if (ProgressView != null) {
                ProgressView.Dispose ();
                ProgressView = null;
            }
        }
    }
}