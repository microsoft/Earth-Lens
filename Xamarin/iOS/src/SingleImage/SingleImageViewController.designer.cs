// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace EarthLens.iOS.SingleImage
{
    [Register ("ViewController")]
    partial class SingleImageViewController
    {
        [Outlet]
        UIKit.UIButton Button { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView ClassCount { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint ClassCountHeight { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint CloseBtnHeight { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint DefaultHeight { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ObjectBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SaveBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISearchDisplayController searchDisplayController { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView SearchView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView SelectedClass { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint SelectedClassHeight { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SettingBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ShareBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStackView Sidebar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SidebarClose { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView SideBarToggle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView SidebarView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint SidebarWidth { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint sidebarYOffset { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint TextviewTopPadding { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint ToggleHeight { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ToggleView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIStackView ToolBar { get; set; }

        [Action ("ObjectBtn_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ObjectBtn_TouchUpInside (UIKit.UIButton sender);

        [Action ("ShareBtn_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ShareBtn_TouchUpInside (UIKit.UIButton sender);

        [Action ("SidebarClose_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void SidebarClose_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ClassCount != null) {
                ClassCount.Dispose ();
                ClassCount = null;
            }

            if (ClassCountHeight != null) {
                ClassCountHeight.Dispose ();
                ClassCountHeight = null;
            }

            if (CloseBtnHeight != null) {
                CloseBtnHeight.Dispose ();
                CloseBtnHeight = null;
            }

            if (DefaultHeight != null) {
                DefaultHeight.Dispose ();
                DefaultHeight = null;
            }

            if (ObjectBtn != null) {
                ObjectBtn.Dispose ();
                ObjectBtn = null;
            }

            if (SaveBtn != null) {
                SaveBtn.Dispose ();
                SaveBtn = null;
            }

            if (searchDisplayController != null) {
                searchDisplayController.Dispose ();
                searchDisplayController = null;
            }

            if (SearchView != null) {
                SearchView.Dispose ();
                SearchView = null;
            }

            if (SelectedClass != null) {
                SelectedClass.Dispose ();
                SelectedClass = null;
            }

            if (SelectedClassHeight != null) {
                SelectedClassHeight.Dispose ();
                SelectedClassHeight = null;
            }

            if (SettingBtn != null) {
                SettingBtn.Dispose ();
                SettingBtn = null;
            }

            if (ShareBtn != null) {
                ShareBtn.Dispose ();
                ShareBtn = null;
            }

            if (Sidebar != null) {
                Sidebar.Dispose ();
                Sidebar = null;
            }

            if (SidebarClose != null) {
                SidebarClose.Dispose ();
                SidebarClose = null;
            }

            if (SideBarToggle != null) {
                SideBarToggle.Dispose ();
                SideBarToggle = null;
            }

            if (SidebarView != null) {
                SidebarView.Dispose ();
                SidebarView = null;
            }

            if (SidebarWidth != null) {
                SidebarWidth.Dispose ();
                SidebarWidth = null;
            }

            if (sidebarYOffset != null) {
                sidebarYOffset.Dispose ();
                sidebarYOffset = null;
            }

            if (TextviewTopPadding != null) {
                TextviewTopPadding.Dispose ();
                TextviewTopPadding = null;
            }

            if (ToggleHeight != null) {
                ToggleHeight.Dispose ();
                ToggleHeight = null;
            }

            if (ToggleView != null) {
                ToggleView.Dispose ();
                ToggleView = null;
            }

            if (ToolBar != null) {
                ToolBar.Dispose ();
                ToolBar = null;
            }
        }
    }
}