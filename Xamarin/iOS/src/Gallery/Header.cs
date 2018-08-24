using Foundation;
using UIKit;

namespace EarthLens.iOS.Gallery
{
    public sealed partial class Header : UICollectionReusableView
    {
        private readonly UILabel _label;

        public string Text
        {
            get => _label.Text;
            set 
            { 
                _label.Text = value;
                SetNeedsDisplay(); 
            }
        }

        [Export("initWithFrame:")]
        public Header(System.Drawing.RectangleF frame)
          : base(frame)
        {
            _label = new UILabel
            {
                Frame = new System.Drawing.RectangleF(
                    Constants.GalleryHeaderXPos,
                    Constants.GalleryHeaderYPos,
                    Constants.GalleryHeaderWidth,
                    Constants.GalleryHeaderHeight),
                Font = UIFont.GetPreferredFontForTextStyle(UIFontTextStyle.Headline),
                AdjustsFontForContentSizeCategory = true
            };
            AddSubview(_label);
        }
    }
}