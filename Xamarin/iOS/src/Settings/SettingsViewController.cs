using UIKit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EarthLens.iOS.Gallery;
using EarthLens.Models;
using Foundation;

namespace EarthLens.iOS.Settings
{
    public partial class SettingsViewController : UIViewController
    {
        private UITableView _menu;
        private UITextView _aboutView;
        private UITextView _repositoryView;
        private UITextView _privacyView;
        private UIStackView _preferencesView;
        private UIStepper _confidenceToggle;
        private UILabel _confidenceLabel;
        private UISwitch _galleryToggle;
        private List<UIView> _settingViews;
        private UITextView _gallerySubheading;
        private UITextView _confidenceSubheading;
        private UITextView _confidenceNote;
        private UITableViewCell _galleryCell;
        private UITableViewCell _confidenceCell;

        public SettingsViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            // Perform any additional setup after loading the view, typically from a nib.
            CreateMenu();
        }

        public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);
            UpdateSettingsView();
        }

        private void UpdateSettingsView()
        {
            _menu.Frame = new CoreGraphics.CGRect(0, Constants.NavBarHeight, View.Frame.Width / 3, View.Frame.Height);
            _aboutView.Frame = new CoreGraphics.CGRect(View.Frame.Width / 3 + Constants.TableLeadingMargin, 
                Constants.ViewTopMargin, (View.Frame.Width / 3 * 2) - Constants.TableTrailingMargin, View.Frame.Height);
            _repositoryView.Frame = new CoreGraphics.CGRect(View.Frame.Width / 3 + Constants.TableLeadingMargin, 
                Constants.ViewTopMargin, (View.Frame.Width / 3 * 2) - Constants.TableTrailingMargin, View.Frame.Height);
            _preferencesView.Frame = new CoreGraphics.CGRect(View.Frame.Width / 3, Constants.NavBarHeight, 
                View.Frame.Width / 3 * 2, View.Frame.Height);
            _privacyView.Frame = new CoreGraphics.CGRect(View.Frame.Width / 3 + Constants.TableLeadingMargin,
                Constants.ViewTopMargin, (View.Frame.Width / 3 * 2) - Constants.TableTrailingMargin, View.Frame.Height);
        }

        private void CreateMenu()
        {
            var settings = new[] { Constants.AboutLabel, Constants.RepositoryLabel, Constants.PreferencesLabel, Constants.PrivacyLabel };
            _menu = new UITableView(
                new CoreGraphics.CGRect(0, Constants.NavBarHeight, View.Frame.Width / 3, View.Frame.Height),
                UITableViewStyle.Grouped) {Source = new SettingsTableViewSource(this, settings)};
            _menu.Layer.BorderColor = Constants.TableBorderColour;
            _menu.Layer.BorderWidth = Constants.TableBorderSize; 
            View.AddSubview(_menu);
            View.BackgroundColor = UIColor.GroupTableViewBackgroundColor;
            _settingViews = new List<UIView>();
            CreateAboutView();
            CreateRepositoryView();
            CreatePreferencesView();
            CreatePrivacyView();
        }

        public void ShowView (int viewIndex)
        {
            foreach (var view in _settingViews)
            {
                view.Hidden = !view.Hidden;
                view.Hidden = !Equals(view, _settingViews[viewIndex]);
            }
        }

        private void CreateAboutView()
        {
            var attString = CreateLink(Constants.GarageLink, Constants.GarageLink, UIFont.PreferredFootnote);
            var aboutText = new NSMutableAttributedString(SharedConstants.AboutContent, font: UIFont.PreferredFootnote);
            var copyrightText = new NSMutableAttributedString(SharedConstants.CopyrightInfo, font: UIFont.PreferredFootnote);
            aboutText.Append(attString, copyrightText);
            _aboutView = CreateTextView(aboutText);
            _aboutView.AdjustsFontForContentSizeCategory = true;
            _aboutView.Hidden = false;
        }

        private void CreateRepositoryView()
        {
            _repositoryView = CreateTextView(CreateLink(Constants.GithubLink, Constants.GithubLinkLabel, UIFont.PreferredFootnote));
        }

        private void CreatePrivacyView()
        {
            _privacyView = CreateTextView(CreateLink(Constants.PrivacyLink, Constants.PrivacyLinkLabel, UIFont.PreferredFootnote));
        }

        private NSMutableAttributedString CreateLink(string link, string linklabel, UIFont fontStyle)
        {
            var htmlLink = string.Format(Constants.LinkFormat, link, linklabel);
            var attr = new NSAttributedStringDocumentAttributes()
            {
                DocumentType = NSDocumentType.HTML
            };
            var nsError = new NSError();
            var linkString = new NSAttributedString(htmlLink, attr, ref nsError);
            var attrHyperlink = new UIStringAttributes
            {
                UnderlineStyle = NSUnderlineStyle.None,
                Font = fontStyle
            };
            var attString = new NSMutableAttributedString(linkString);
            attString.AddAttributes(attrHyperlink, new NSRange(0, linkString.Length));

            return attString;
        }

        private UITextView CreateTextView(NSMutableAttributedString attString)
        {
            var view = new UITextView(new CoreGraphics.CGRect(
            View.Frame.Width / 3 + Constants.TableLeadingMargin, Constants.ViewTopMargin,
            View.Frame.Width / 3 * 2 - Constants.TableTrailingMargin, View.Frame.Height))
            {
                Editable = false,
                Font = UIFont.PreferredFootnote,
                TextColor = Constants.TextColour,
                TextContainerInset = new UIEdgeInsets(Constants.TextViewTopInset, Constants.TextViewInset, Constants.TextViewInset, Constants.TextViewInset),
                AttributedText = attString,
                Hidden = true,
                AdjustsFontForContentSizeCategory = true,
            };
            View.AddSubview(view);
            _settingViews.Add(view);
            return view;
        }

        private UITableViewCell CreateGalleryCell()
        {
            _galleryToggle = new UISwitch
            {
                On = NSUserDefaults.StandardUserDefaults.BoolForKey(Constants.GallerySettingsSwitchState)
            };
            _galleryToggle.ValueChanged += GalleryToggle_ValueChanged;
            _galleryToggle.TranslatesAutoresizingMaskIntoConstraints = false;

            var galleryCell = new UITableViewCell();
            galleryCell.TextLabel.Text = Constants.GalleryCellLabel;
            galleryCell.TextLabel.AdjustsFontForContentSizeCategory = true;
            galleryCell.BackgroundColor = UIColor.White;
            galleryCell.TranslatesAutoresizingMaskIntoConstraints = false;
            galleryCell.AddSubview(_galleryToggle);

            NSLayoutConstraint.Create(_galleryToggle, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal,
                _galleryToggle.Superview, NSLayoutAttribute.Trailing, 1f,
                Constants.GalleryToggleTrailingMargin).Active = true;
            NSLayoutConstraint.Create(_galleryToggle, NSLayoutAttribute.Top, NSLayoutRelation.Equal,
                _galleryToggle.Superview, NSLayoutAttribute.Top, 1f,
                Constants.ToggleTopMargin).Active = true;
            NSLayoutConstraint.Create(_galleryToggle, NSLayoutAttribute.Height, NSLayoutRelation.Equal,
                null, NSLayoutAttribute.NoAttribute, 1f, Constants.CellHeight).Active = true;
            NSLayoutConstraint.Create(_galleryToggle, NSLayoutAttribute.Width, NSLayoutRelation.Equal,
                null, NSLayoutAttribute.NoAttribute, 1f, Constants.ToggleWidth).Active = true;
            return galleryCell;
        }

        private UITableViewCell CreateConfidenceCell()
        {
            _confidenceToggle = new UIStepper
            {
                MinimumValue = Constants.ConfidenceMinValue,
                MaximumValue = Constants.ConfidenceMaxValue,
                StepValue = Constants.StepValue,
                Value = NSUserDefaults.StandardUserDefaults.DoubleForKey(Constants.ConfidenceThresholdUserDefaultName) *
                        100,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            _confidenceToggle.ValueChanged += ConfidenceToggle_ValueChanged;

            _confidenceLabel = new UILabel
            {
                Text = string.Format(CultureInfo.CurrentCulture, Constants.ConfidenceLabelFormat, 
                    _confidenceToggle.Value),
                Font = UIFont.SystemFontOfSize(Constants.TableFontSize),
                TextColor = Constants.SettingsSubheadingTextColour,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            var confidenceCell = new UITableViewCell();
            confidenceCell.TextLabel.Text = Constants.ConfidenceCellLabel;
            confidenceCell.TextLabel.AdjustsFontForContentSizeCategory = true;
            confidenceCell.BackgroundColor = UIColor.White;
            confidenceCell.TranslatesAutoresizingMaskIntoConstraints = false;
            confidenceCell.AddSubview(_confidenceToggle);
            confidenceCell.AddSubview(_confidenceLabel);

            NSLayoutConstraint.Create(_confidenceToggle, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal,
                _confidenceToggle.Superview, NSLayoutAttribute.Trailing, 1f,
                Constants.ToggleTrailingMargin).Active = true;
            NSLayoutConstraint.Create(_confidenceToggle, NSLayoutAttribute.Top, NSLayoutRelation.Equal,
                _confidenceToggle.Superview, NSLayoutAttribute.Top, 1f,
                Constants.ToggleTopMargin).Active = true;
            NSLayoutConstraint.Create(_confidenceToggle, NSLayoutAttribute.Height, NSLayoutRelation.Equal,
                null, NSLayoutAttribute.NoAttribute, 1f, Constants.CellHeight).Active = true;
            NSLayoutConstraint.Create(_confidenceToggle, NSLayoutAttribute.Width, NSLayoutRelation.Equal,
                null, NSLayoutAttribute.NoAttribute, 1f, Constants.ToggleWidth).Active = true;

            NSLayoutConstraint.Create(_confidenceLabel, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal,
                _confidenceLabel.Superview, NSLayoutAttribute.Trailing, 1f,
                Constants.ConfidenceLabelTrailingMargin).Active = true;
            NSLayoutConstraint.Create(_confidenceLabel, NSLayoutAttribute.Top, NSLayoutRelation.Equal,
                _confidenceLabel.Superview, NSLayoutAttribute.Top, 1f, 0.0f).Active = true;
            NSLayoutConstraint.Create(_confidenceLabel, NSLayoutAttribute.Height, NSLayoutRelation.Equal,
                null, NSLayoutAttribute.NoAttribute, 1f, Constants.CellHeight).Active = true;
            NSLayoutConstraint.Create(_confidenceLabel, NSLayoutAttribute.Width, NSLayoutRelation.Equal,
                null, NSLayoutAttribute.NoAttribute, 1f,
                Constants.ConfidenceLabelWidth).Active = true;
            return confidenceCell;
        }

        private void CreatePreferencesView()
        {
            _preferencesView = new UIStackView(new CoreGraphics.CGRect(View.Frame.Width / 3, Constants.NavBarHeight, 
                View.Frame.Width / 3 * 2, View.Frame.Height));

            _gallerySubheading = new UITextView
            {
                Text = Constants.GallerySubheading,
                TextColor = Constants.SettingsSubheadingTextColour,
                Editable = false,
                BackgroundColor = Constants.TableBackgroundColour,
                TranslatesAutoresizingMaskIntoConstraints = false,
                AdjustsFontForContentSizeCategory = true,
                UserInteractionEnabled = false,
                ScrollEnabled = false,
                Font = UIFont.GetPreferredFontForTextStyle(UIFontTextStyle.Subheadline)
            };

            _confidenceSubheading = new UITextView
            {
                Text = Constants.ConfidenceSubheading,
                TextColor = Constants.SettingsSubheadingTextColour,
                Editable = false,
                BackgroundColor = Constants.TableBackgroundColour,
                TranslatesAutoresizingMaskIntoConstraints = false,
                AdjustsFontForContentSizeCategory = true,
                UserInteractionEnabled = false,
                ScrollEnabled = false,
                Font = UIFont.GetPreferredFontForTextStyle(UIFontTextStyle.Subheadline)
            };

            _confidenceNote = new UITextView
            {
                Text = Constants.ConfidenceNote,
                TextColor = Constants.SettingsSubheadingTextColour,
                Editable = false,
                BackgroundColor =  Constants.TableBackgroundColour,
                TranslatesAutoresizingMaskIntoConstraints = false,
                AdjustsFontForContentSizeCategory = true,
                UserInteractionEnabled = false,
                ScrollEnabled = false,
                Font = UIFont.GetPreferredFontForTextStyle(UIFontTextStyle.Footnote)
            };

            _galleryCell = CreateGalleryCell();
            _confidenceCell = CreateConfidenceCell();

            _preferencesView.AddSubview(_gallerySubheading);
            _preferencesView.AddSubview(_galleryCell);
            _preferencesView.AddSubview(_confidenceSubheading);
            _preferencesView.AddSubview(_confidenceCell);
            _preferencesView.AddSubview(_confidenceNote);

            ConstrainItems();

            _preferencesView.Hidden = true;
            View.AddSubview(_preferencesView);
            _settingViews.Add(_preferencesView);
        }

        private void ConstrainItems()
        {
            NSLayoutConstraint.Create(_gallerySubheading, NSLayoutAttribute.Leading, NSLayoutRelation.Equal,
                _gallerySubheading.Superview, NSLayoutAttribute.Leading, 1f,
                Constants.LeadingMargin).Active = true;
            NSLayoutConstraint.Create(_gallerySubheading, NSLayoutAttribute.Top, NSLayoutRelation.Equal,
                _gallerySubheading.Superview, NSLayoutAttribute.Top, 1f,
                Constants.GallerySubheadingTopMargin).Active = true;
            NSLayoutConstraint.Create(_gallerySubheading, NSLayoutAttribute.Height, NSLayoutRelation.Equal,
                null, NSLayoutAttribute.NoAttribute, 1f, Constants.SubheadingHeight).Active = true;
            NSLayoutConstraint.Create(_gallerySubheading, NSLayoutAttribute.Width, NSLayoutRelation.Equal,
                null, NSLayoutAttribute.NoAttribute, 1f, Constants.SubheadingWidth).Active = true;

            NSLayoutConstraint.Create(_galleryCell, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal,
                _galleryCell.Superview, NSLayoutAttribute.Trailing, 1f,
                Constants.TrailingMargin).Active = true;
            NSLayoutConstraint.Create(_galleryCell, NSLayoutAttribute.Leading, NSLayoutRelation.Equal,
                _galleryCell.Superview, NSLayoutAttribute.Leading, 1f,
                Constants.LeadingMargin).Active = true;
            NSLayoutConstraint.Create(_galleryCell, NSLayoutAttribute.Top, NSLayoutRelation.Equal,
                _galleryCell.Superview, NSLayoutAttribute.Top, 1f,
                Constants.GalleryCellTopMargin).Active = true;
            NSLayoutConstraint.Create(_galleryCell, NSLayoutAttribute.Height, NSLayoutRelation.Equal,
                null, NSLayoutAttribute.NoAttribute, 1f, Constants.CellHeight).Active = true;

            NSLayoutConstraint.Create(_confidenceSubheading, NSLayoutAttribute.Leading, NSLayoutRelation.Equal,
                _confidenceSubheading.Superview, NSLayoutAttribute.Leading, 1f,
                Constants.LeadingMargin).Active = true;
            NSLayoutConstraint.Create(_confidenceSubheading, NSLayoutAttribute.Top, NSLayoutRelation.Equal,
                _confidenceSubheading.Superview, NSLayoutAttribute.Top, 1f,
                Constants.ConfidenceSubheadingTopMargin).Active = true;
            NSLayoutConstraint.Create(_confidenceSubheading, NSLayoutAttribute.Height, NSLayoutRelation.Equal,
                null, NSLayoutAttribute.NoAttribute, 1f, Constants.SubheadingHeight).Active = true;
            NSLayoutConstraint.Create(_confidenceSubheading, NSLayoutAttribute.Width, NSLayoutRelation.Equal,
                null, NSLayoutAttribute.NoAttribute, 1f, Constants.SubheadingWidth).Active = true;

            NSLayoutConstraint.Create(_confidenceCell, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal,
                _confidenceCell.Superview, NSLayoutAttribute.Trailing, 1f,
                Constants.TrailingMargin).Active = true;
            NSLayoutConstraint.Create(_confidenceCell, NSLayoutAttribute.Leading, NSLayoutRelation.Equal,
                _confidenceCell.Superview, NSLayoutAttribute.Leading, 1f,
                Constants.LeadingMargin).Active = true;
            NSLayoutConstraint.Create(_confidenceCell, NSLayoutAttribute.Top, NSLayoutRelation.Equal,
                _confidenceCell.Superview, NSLayoutAttribute.Top, 1f,
                Constants.ConfidenceCellTopMargin).Active = true;
            NSLayoutConstraint.Create(_confidenceCell, NSLayoutAttribute.Height, NSLayoutRelation.Equal,
                null, NSLayoutAttribute.NoAttribute, 1f, Constants.CellHeight).Active = true;

            NSLayoutConstraint.Create(_confidenceNote, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal,
                _confidenceNote.Superview, NSLayoutAttribute.Trailing, 1f, Constants.TrailingMargin).Active = true;
            NSLayoutConstraint.Create(_confidenceNote, NSLayoutAttribute.Leading, NSLayoutRelation.Equal,
                _confidenceNote.Superview, NSLayoutAttribute.Leading, 1f, Constants.LeadingMargin).Active = true;
            NSLayoutConstraint.Create(_confidenceNote, NSLayoutAttribute.Top, NSLayoutRelation.Equal,
                _confidenceNote.Superview, NSLayoutAttribute.Top, 1f, Constants.ConfidenceNoteTopMargin).Active = true;

        }

        private void GalleryToggle_ValueChanged(object sender, EventArgs e)
        {
            NSUserDefaults.StandardUserDefaults.SetBool(_galleryToggle.On, Constants.GallerySettingsSwitchState);
            if (_galleryToggle.On)
            {
                GalleryDataSource.ImageCollection =
                    new SortedList<DateTime, GalleryImageSection>(
                        GalleryDataSource.ImageCollection.ToDictionary(x => x.Key, x => x.Value));
            }
            else
            {
                GalleryDataSource.ImageCollection =
                    new SortedList<DateTime, GalleryImageSection>(
                        GalleryDataSource.ImageCollection.ToDictionary(x => x.Key, x => x.Value),
                        new ReverseDateTimeComparer());
            }
        }

        private void ConfidenceToggle_ValueChanged(object sender, EventArgs e)
        {
            _confidenceLabel.Text = string.Format(CultureInfo.CurrentCulture, Constants.ConfidenceLabelFormat, 
                _confidenceToggle.Value);

            NSUserDefaults.StandardUserDefaults.SetDouble(_confidenceToggle.Value / 100,
                Constants.ConfidenceThresholdUserDefaultName);
        }
    }
}
