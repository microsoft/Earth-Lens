using System;
using System.Globalization;
using Foundation;
using SkiaSharp.Views.iOS;
using UIKit;

namespace EarthLens.iOS.Gallery
{
    internal class GalleryCollectionViewSource : UICollectionViewSource
    {        
        private readonly GalleryViewController _galleryViewController;
         
        public GalleryCollectionViewSource(GalleryViewController galleryViewController)
        {
            _galleryViewController = galleryViewController;
        }

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return GalleryDataSource.ImageCollection.Count;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            return GalleryDataSource.ImageCollection.Values[(int) section].Count;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            if (collectionView == null || indexPath == null)
            {
                throw new ArgumentNullException();
            }

            var galleryImageCell =
                (GalleryImageCell) collectionView.DequeueReusableCell(GalleryImageCell.Key, indexPath);
            galleryImageCell.UpdateRow(
                GalleryDataSource.ImageCollection.Values[indexPath.Section].Values[(int) indexPath.Item].Image
                    .ToUIImage(),
                GalleryDataSource.ImageCollection.Values[indexPath.Section].Values[(int) indexPath.Item].CreationTime ??
                DateTime.MaxValue, indexPath);

            var imageName = GalleryDataSource.ImageCollection.Values[indexPath.Section].Values[(int)indexPath.Item].Name;

            ConfigureImageCellGeneralAccessibilityAttributes(galleryImageCell.GalleryImageView, imageName);

            return galleryImageCell;
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            if (collectionView == null)
            {
                throw new ArgumentNullException();
            }

            var galleryImageCell = (GalleryImageCell) collectionView.CellForItem(indexPath);
            galleryImageCell.CalculateCheckmarkFrameSize();

            if (!GalleryDataSource.SelectedImageIndexPaths.Contains(indexPath))
            {
                GalleryDataSource.SelectedImageIndexPaths.Add(indexPath);
                _galleryViewController.UpdateSelectedImageCount(++GalleryDataSource.NumberOfSelectedImages);
                galleryImageCell.GalleryImageCheckMark.Hidden = false;
                ConfigureImageCellUnCheckedAccessibilityAttributes(galleryImageCell.GalleryImageView);
            }
            else 
            {
                GalleryDataSource.SelectedImageIndexPaths.Remove(indexPath);
                _galleryViewController.UpdateSelectedImageCount(--GalleryDataSource.NumberOfSelectedImages);
                galleryImageCell.GalleryImageCheckMark.Hidden = true;
                ConfigureImageCellCheckedAccessibilityAttributes(galleryImageCell.GalleryImageView);
            }

            // Hide Toolbar if no images are selected
            _galleryViewController.ToggleToolBarVisibility(GalleryDataSource.SelectedImageIndexPaths.Count > 0);
        }

        public override UICollectionReusableView GetViewForSupplementaryElement(UICollectionView collectionView,
            NSString elementKind, NSIndexPath indexPath)
        {
            if (collectionView == null || indexPath == null)
            {
                throw new ArgumentNullException();
            }

            var headerView = (Header) collectionView.DequeueReusableSupplementaryView(elementKind,
                Constants.GalleryHeaderKey, indexPath);
            headerView.Text = GalleryDataSource.ImageCollection.Values[indexPath.Section].CreationDate
                .ToString(Constants.HeaderDatestampFormat, CultureInfo.CurrentCulture);
            
            ConfigureImageDateAccessibilityAttributes(headerView, headerView.Text);

            return headerView;
        }

        #region Configure Accessibility Attributes
        private void ConfigureImageDateAccessibilityAttributes(UIView headerView, string createdDate)
        {
            headerView.IsAccessibilityElement = true;
            headerView.AccessibilityTraits = UIAccessibilityTrait.StaticText;
            headerView.AccessibilityLabel = string.Format(AccessibilityConstants.ImageDateAccessibilityLabel, createdDate);
        }

        private void ConfigureImageCellGeneralAccessibilityAttributes(UIView imageView, string imageTitle)
        {
            imageView.IsAccessibilityElement = true;
            imageView.AccessibilityTraits = UIAccessibilityTrait.Button;
            imageView.AccessibilityElementsHidden = false;
            imageView.AccessibilityLabel = imageTitle;
            imageView.UserInteractionEnabled = true;
            ConfigureImageCellCheckedAccessibilityAttributes(imageView);
        }

        private static void ConfigureImageCellCheckedAccessibilityAttributes(UIView imageView)
        {
            imageView.AccessibilityHint = AccessibilityConstants.ImageSelectionAccessibilityCheckHint;
        }

        private static void ConfigureImageCellUnCheckedAccessibilityAttributes(UIView imageView)
        {
            imageView.AccessibilityHint = AccessibilityConstants.ImageSelectionAccessibilityUnCheckHint;
        }
        #endregion
    }
}