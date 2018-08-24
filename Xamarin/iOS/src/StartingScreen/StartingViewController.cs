using System;
using Foundation;
using UIKit;

namespace EarthLens.iOS.StartingScreen
{
    public partial class StartingViewController : UIViewController
    {
        public StartingViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            UIApplication.Notifications.ObserveContentSizeCategoryChanged((sender,args) => AccessibilityHandler());
            StartButton.TouchUpInside += Start_button_TouchUpInside;
            StartButton.TitleLabel.AdjustsFontForContentSizeCategory = true;
            NavigationItem.HidesBackButton = true;
            SetConstraints();
            AddLinks();
        }

        private void Start_button_TouchUpInside(object sender, EventArgs eventArgs)
        {
            var storyboard = UIStoryboard.FromName("ImageUpload", null);
            var imageViewController = storyboard.InstantiateInitialViewController() as UIViewController;
            NavigationController.PushViewController(imageViewController, true);
        }

        private void SetConstraints()
        {
            NSLayoutConstraint.Create(StartButton, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal,
            View, NSLayoutAttribute.Bottom, 1f, -Constants.StartButtonBottomMargin).Active = true;
        }

        private void AddLinks()
        {
            var linkText = new NSMutableAttributedString(SharedConstants.LinkText, font: UIFont.PreferredBody);
            linkText.Append(CreateLink(Constants.GarageLink, Constants.GarageLink));
            StartDisclaimer.AttributedText = linkText;
            StartDisclaimer.TextAlignment = UITextAlignment.Center;
            PrivacyLink.AttributedText = CreateLink(Constants.PrivacyLink, Constants.PrivacyLinkLabel);
            PrivacyLink.TextAlignment = UITextAlignment.Center;
        }

        private NSMutableAttributedString CreateLink(string link, string linklabel)
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
                Font = UIFont.PreferredBody
            };
            var attString = new NSMutableAttributedString(linkString);
            attString.AddAttributes(attrHyperlink, new NSRange(0, linkString.Length));
            return attString;
        }
        
        private void AccessibilityHandler()
        {
            StartButton.SetNeedsLayout();
            StartSubtitle.SetNeedsLayout();
            PrivacyLink.SetNeedsLayout();
        }
    }
}
