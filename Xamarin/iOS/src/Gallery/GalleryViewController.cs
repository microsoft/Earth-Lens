using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EarthLens.iOS.SingleImage;
using EarthLens.iOS.TimeLine;
using EarthLens.Models;
using EarthLens.Services;
using CoreGraphics;
using Foundation;
using UIKit;

namespace EarthLens.iOS.Gallery
{
	public partial class GalleryViewController : UIViewController
	{
		private UIToolbar _toolbar;

		public GalleryViewController (IntPtr handle) : base (handle)
		{
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            GalleryDataSource.LoadImages();
            GalleryCollectionView.RegisterClassForSupplementaryView(typeof(Header),
                UICollectionElementKindSection.Header, Constants.GalleryHeaderKey);
            GalleryCollectionView.Source = new GalleryCollectionViewSource(this);

            var layout = new UICollectionViewFlowLayout
            {
                SectionInset = new UIEdgeInsets(
                    Constants.GallerySectionVerticalSpacing, 
                    Constants.GallerySectionSize,
                    Constants.GallerySectionSize,
                    Constants.GallerySectionSize),
                ItemSize = new CGSize(Constants.GalleryTileWidth, Constants.GalleryTileHeight),
                ScrollDirection = UICollectionViewScrollDirection.Vertical,
                MinimumLineSpacing = Constants.GalleryLayoutMinLinSpacing,
                MinimumInteritemSpacing = Constants.GalleryLayoutMinIterItemSpacing,
                HeaderReferenceSize = new CGSize(GalleryCollectionView.Frame.Size.Width, Constants.GalleryHeaderHeight)
            };
            GalleryCollectionView.SetCollectionViewLayout(layout, true);

            GenerateNavBarButtons();
            GenerateToolBar();
	        ResetGalleryTitle();
        }

		public override void ViewWillAppear(bool animated)
		{
			GalleryDataSource.LoadImages();
			GalleryCollectionView.ReloadData();
		}
		
		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			ClearSelectedImages();
			ResetGalleryTitle();
			ToggleToolBarVisibility(false);
		}
		
		public void ToggleToolBarVisibility(bool state)
		{
			_toolbar.Hidden = !state;
			NavigationItem.RightBarButtonItem.Enabled = state;
		}
		
		public void UpdateSelectedImageCount(int count)
		{
			NavigationItem.Title = count == 0
				? Constants.ImageGalleryTitle
				: string.Format(CultureInfo.CurrentCulture,
					count > 1
						? SharedConstants.SelectedImageCountFormatPlural
						: SharedConstants.SelectedImageCountFormatSingular, count);
		}
		
        private void GenerateNavBarButtons()
        {
			var selectButton = new UIBarButtonItem {Title = Constants.GalleryNextButtonTitle};
			NavigationItem.RightBarButtonItem = selectButton;
	        NavigationItem.RightBarButtonItem.Enabled = false;
	        NavigationItem.RightBarButtonItem.Clicked += SendToResultView;
        }

		private void ResetGalleryTitle()
		{
			NavigationItem.Title = Constants.ImageGalleryTitle;
		}

		private void UncheckSelectedImages(object sender, EventArgs e)
		{
			foreach (var indexPath in GalleryDataSource.SelectedImageIndexPaths)
			{
				var galleryImageCell = (GalleryImageCell) GalleryCollectionView.DequeueReusableCell(GalleryImageCell.Key, indexPath);
				galleryImageCell.SetGalleryCheckmark();
			}
			GalleryCollectionView.ReloadData();
			ClearSelectedImages();
			ResetGalleryTitle();
			ToggleToolBarVisibility(false);
		}

		private static void ClearSelectedImages()
		{
			GalleryDataSource.SelectedImageIndexPaths.Clear();
			GalleryDataSource.NumberOfSelectedImages = 0; 
		}

		private void GenerateToolBar()
	    {
	        _toolbar = new UIToolbar();
	        _toolbar.SizeToFit ();
		    _toolbar.Frame = new CGRect(0, View.Frame.Height - NavigationController.NavigationBar.Frame.Height,
			    View.Frame.Width, Constants.GalleryToolbarHeight);
		    _toolbar.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleWidth;
		    _toolbar.Hidden = true;  
		    View.AddSubview(_toolbar);
		    
		    CreateToolbarItems();
	    }

		private void CreateToolbarItems()
		{
			var spacerBtn = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
			var cancelBtn =
				new UIBarButtonItem(Constants.GalleryCancelButtonTitle, UIBarButtonItemStyle.Bordered, null);
			var deleteImageBtn = new UIBarButtonItem(UIBarButtonSystemItem.Trash);
			
			//set toolbar click events

			cancelBtn.Clicked += UncheckSelectedImages;

			deleteImageBtn.Clicked += DisplayConfirmImageDeleteAlert;
			
			var buttons = new [] {spacerBtn, cancelBtn, spacerBtn, deleteImageBtn };
			_toolbar.SetItems(buttons, false);
		}

		private void SendToResultView(object sender, EventArgs e)
		{
			if (GalleryDataSource.SelectedImageIndexPaths.Count <= 1)
			{
				LaunchSingleImageView();
			}
			else
			{
				LaunchTimelineView();
			}
		}

		private void LaunchSingleImageView()
		{
			var resultsPageStoryBoard = UIStoryboard.FromName(Constants.SingleImageStoryboardName, null);

			if (!(resultsPageStoryBoard.InstantiateInitialViewController() is SingleImageViewController
				resultsViewController))
				return;

			var selectedImageIndexPath = GalleryDataSource.SelectedImageIndexPaths.First();
			var selectedImageId = GalleryDataSource.ImageCollection.Values[selectedImageIndexPath.Section]
				                      .Values[(int) selectedImageIndexPath.Item].Id ?? 0;
			var imageDao = DatabaseService.GetImage(selectedImageId);
			resultsViewController.InputImageEntry = new ImageEntry(
				ImageEncodingService.Base64ToSKImage(imageDao.Base64),
				imageDao.Name,
				imageDao.CreationTime,
				imageDao.Id,
				imageDao.Observations.Select(obs => obs.ToObservation()));

			NavigationController.PushViewController(resultsViewController, true);
		}

		private void LaunchTimelineView()
		{
			var timelineStoryBoard = UIStoryboard.FromName(Constants.TimeLineStoryboardName, null);
			if (!(timelineStoryBoard.InstantiateInitialViewController() is TimeLineViewController
				timelineViewController)) return;

			var imageEntries = (from imgIndex in GalleryDataSource.SelectedImageIndexPaths
					select GalleryDataSource.ImageCollection.Values[imgIndex.Section]
						       .Values[(int) imgIndex.Item]
						       .Id ?? 0
					into selectedImageId
					select DatabaseService.GetImage(selectedImageId)
					into imageDao
					select new ImageEntry(ImageEncodingService.Base64ToSKImage(imageDao.Base64), imageDao.Name,
						imageDao.CreationTime, imageDao.Id, imageDao.Observations.Select(obs => obs.ToObservation())))
				.ToList();

			timelineViewController.InputImageEntries = imageEntries;
			NavigationController.PushViewController(timelineViewController, true);
		}

		private void DisplayConfirmImageDeleteAlert(object sender, EventArgs e)
		{
			var alertController = UIAlertController.Create(
				CreateConfirmDeleteModalTitle(),
				SharedConstants.DeleteFromGalleryDescription,
				UIAlertControllerStyle.Alert); 
			
			alertController.AddAction(
				UIAlertAction.Create(
					SharedConstants.SaveToGalleryCancel,
					UIAlertActionStyle.Cancel,
					action => HandleCancelGalleryDelete()));
			
			alertController.AddAction(
				UIAlertAction.Create(
					SharedConstants.ConfirmGalleryDelete,
					UIAlertActionStyle.Destructive,
					action => HandleConfirmGalleryDelete()));
			
			var presentationAlert = alertController.PopoverPresentationController;

			if (presentationAlert != null)
			{
				presentationAlert.SourceView = View;
				presentationAlert.PermittedArrowDirections = UIPopoverArrowDirection.Up;
			}

			PresentViewController(alertController, true, null);
		}

		private static string CreateConfirmDeleteModalTitle()
		{
			return string.Format(CultureInfo.CurrentCulture,
				GalleryDataSource.NumberOfSelectedImages > 1
					? SharedConstants.ConfirmDeleteTitleFormatPlural
					: SharedConstants.ConfirmDeleteTitleFormatSingular,
				GalleryDataSource.NumberOfSelectedImages);
		}

		private void HandleConfirmGalleryDelete()
		{
			DeleteSelectedImages();
			ClearSelectedImages();
			ResetGalleryTitle();
			ToggleToolBarVisibility(false);
		}

		private static void HandleCancelGalleryDelete()
		{
		}

		private void DeleteSelectedImages()
		{
			var imageIdsToBeDeleted = DeleteImagesFromDataSource();
			DeleteSectionsFromDataSource();

			foreach (var imageId in imageIdsToBeDeleted)
			{
				DatabaseService.DeleteImage(imageId);
			}
		}

		private IEnumerable<int> DeleteImagesFromDataSource()
		{
			//store intermediary images and indices to be deleted, than delete
			
			var imageIndicesToBeDeleted = new List<NSIndexPath>();
			var imagesToBeDeleted = new List<(int section, int item)>();
			var imageIdsToBeDeleted = new HashSet<int>();

			foreach (var indexPath in GalleryDataSource.SelectedImageIndexPaths)
			{
				imagesToBeDeleted.Add((indexPath.Section, (int) indexPath.Item));
				imageIndicesToBeDeleted.Add(indexPath);
				imageIdsToBeDeleted.Add(GalleryDataSource.ImageCollection.Values[indexPath.Section]
					                        .Values[(int) indexPath.Item].Id ?? 0);
			}

			var sortedImagesToBeDeleted = imagesToBeDeleted.OrderByDescending(pair => pair.item);

			foreach (var image in sortedImagesToBeDeleted)
			{
				var row = GalleryDataSource.ImageCollection.Values[image.section];
				row.Remove(row.Keys[image.item]);
			}

			GalleryCollectionView.DeleteItems(imageIndicesToBeDeleted.ToArray());

			return imageIdsToBeDeleted;
		}
		
		private void DeleteSectionsFromDataSource()
		{
			// store intermediary sections and indices to be deleted, than delete
			using (var sectionIndicesToBeDeleted = new NSMutableIndexSet())
			{
				var sectionsToBeDeleted = new List<int>();

				foreach (var indexElementPair in GalleryDataSource.ImageCollection.Select((pair, index) =>
					new {index, pair}))
				{
					if (indexElementPair.pair.Value.Count != 0) continue;
					sectionsToBeDeleted.Add(indexElementPair.index);
					sectionIndicesToBeDeleted.Add((nuint) indexElementPair.index);
				}

				var sortedSectionsToBeDeleted = sectionsToBeDeleted.OrderByDescending(x => x);

				foreach (var section in sortedSectionsToBeDeleted)
				{
					GalleryDataSource.ImageCollection.Remove(GalleryDataSource.ImageCollection.Keys[section]);
				}

				GalleryCollectionView.DeleteSections(sectionIndicesToBeDeleted);
			}
		}
	}
}
