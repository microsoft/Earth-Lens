using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EarthLens.iOS.ImageUpload;
using EarthLens.iOS.Services;
using EarthLens.iOS.SingleImage;
using EarthLens.iOS.TimeLine;
using EarthLens.Models;
using EarthLens.Services;
using Foundation;
using SkiaSharp;
using UIKit;
using GMImagePicker;

namespace EarthLens.iOS.AnalyzingPage
{
    public partial class AnalyzingPageViewController : UIViewController
    {
        public IList<ImageEntry> ImageEntries { get; set; }
        public GMImagePickerController CurrentImagePicker { private get; set; }

        private CancellationToken _ct;
        private CancellationTokenSource _ts;
        private Task _task;
        private int _totalNumberOfChips;
        private int _numberOfProcessedChips;

        public AnalyzingPageViewController(IntPtr handle) : base(handle)
        {
        }

        /// <summary>
        /// Sets the cancellation button properties.
        /// </summary>
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetCancellationButtonProperties();
        }

        /// <summary>
        /// Runs the analysis on the uploaded image as a background <see cref="Task"/>.
        /// </summary>
        /// <param name="animated">If YES, the view was added to the window using an animation.</param>
        public override void ViewDidAppear(bool animated)
        {
            ConfigureAnalyzeStartedVoiceOverAccessibilityAttributes();
            base.ViewDidAppear(animated);
            if (!CheckImagesTooLarge())
            {
                StartAnalysis();
                return;
            }

            var actionSheet = UIAlertController.Create(SharedConstants.ImagesTooLargeTitle,
                SharedConstants.ImagesTooLargeMessage, UIAlertControllerStyle.Alert);

            actionSheet.AddAction(UIAlertAction.Create(SharedConstants.ImagesTooLargeBackAction,
                UIAlertActionStyle.Cancel,
                alert =>
                {
                    DisposeImages();
                    BeginInvokeOnMainThread(() =>
                    {
                        if (CurrentImagePicker != null)
                        {
                            NavigationController.PresentViewController(CurrentImagePicker, false, null);
                        }
                        var imageUploadStoryBoard =
                            UIStoryboard.FromName(Constants.ImageUploadStoryboardName, null);
                        var imageUploadViewController =
                            imageUploadStoryBoard.InstantiateInitialViewController() as ImageUploadViewController;
                        NavigationController.PushViewController(imageUploadViewController, true);
                    });
                }));

            PresentViewController(actionSheet, true, null);
        }

        /// <summary>
        /// Starts the analysis for the uploaded images.
        /// </summary>
        private void StartAnalysis()
        {
            if (ImageEntries == null || ImageEntries.Count == 0)
            {
                LaunchImageUploadPage();
            }

            _totalNumberOfChips = ImageEntries.Sum(imageEntry => GetNumberOfChips(imageEntry.Image));
            _numberOfProcessedChips = 0;

            _task = Task.Factory.StartNew(() =>
            {
                try
                {
                    foreach (var imageEntry in ImageEntries)
                    {
                        var categoryManagers = ImageAnalysisService.AnalyzeImage(imageEntry.Image,
                            ChipAnalysisService.AnalyzeChip, _ct,
                            () =>
                            {
                                Interlocked.Increment(ref _numberOfProcessedChips);
                                BeginInvokeOnMainThread(() =>
                                    {
                                        ProgressView.SetProgress((float)_numberOfProcessedChips / _totalNumberOfChips,
                                            true);
                                    });
                            })
                        .ToList();

                        var observations = PostProcessingService.SelectObservations(
                                categoryManagers,
                                SharedConstants.DefaultIOUThreshold,
                                NSUserDefaults.StandardUserDefaults.DoubleForKey(
                                    Constants.ConfidenceThresholdUserDefaultName), _ct)
                            .ToList();
                        imageEntry.Observations = observations;
                    }

                    if (_ts.IsCancellationRequested) return;
                    Task.Delay(Constants.DelayBeforeResults / _numberOfProcessedChips, _ct).Wait(_ct);
                    BeginInvokeOnMainThread(LaunchResultsView);
                }
                catch (OperationCanceledException)
                {
                    // ignored
                }
            }, _ct);
        }

        /// <summary>
        /// Launches the <see cref="SingleImageViewController"/> with the specified <see cref="Observation"/>s for each
        /// image.
        /// </summary>
        private void LaunchResultsView()
        {
            if (ImageEntries.Count == 1)
            {
                var singleImageStoryboard = UIStoryboard.FromName(Constants.SingleImageStoryboardName, null);
                if (!(singleImageStoryboard.InstantiateInitialViewController()
                    is SingleImageViewController singleImageViewController))
                {
                    return;
                }

                singleImageViewController.InputImageEntry = ImageEntries[0];
                NavigationController.PushViewController(singleImageViewController, true);
            }
            else
            {
                var timelineStoryBoard = UIStoryboard.FromName(Constants.TimeLineStoryboardName, null);
                if (!(timelineStoryBoard.InstantiateInitialViewController()
                    is TimeLineViewController timelineViewController))
                {
                    return;
                }
                timelineViewController.InputImageEntries = ImageEntries;
                NavigationController.PushViewController(timelineViewController, true);
            }
        }

        /// <summary>
        /// Sets the cancelltation button properties.
        /// </summary>
        private void SetCancellationButtonProperties()
        {
            _ts = new CancellationTokenSource();
            _ct = _ts.Token;
            var cancelButton = new UIBarButtonItem { Title = SharedConstants.CancelButtonText };
            cancelButton.Clicked += (o, e) =>
            {
                BeginInvokeOnMainThread(() => { AnalyzingLabel.Text = SharedConstants.StoppingAnalysisLabelText; });

                //Send cancellation request to background task
                _ts.Cancel();

                try
                {
                    Task.WaitAll(_task);
                }
                catch (AggregateException)
                {
                    // ignored
                }
                catch (OperationCanceledException)
                {
                    // ignored
                }
                finally
                {
                    _ts.Dispose();
                }

                DisposeImages();

                LaunchImageUploadPage();
            };

            NavigationItem.LeftBarButtonItem = cancelButton;
            NavigationItem.HidesBackButton = true;
        }

        /// <summary>
        /// Returns true if any of the input images in <see cref="ImageEntries"/> is larger than the thresholds for
        /// too-large images.
        /// </summary>
        /// <returns>True if any of the input images in <see cref="ImageEntries"/> is larger than thresholds.</returns>
        private bool CheckImagesTooLarge()
        {
            return ImageEntries.Any(imageEntry =>
                GetNumberOfChips(imageEntry.Image) > SharedConstants.ImagesTooLargeNumberOfChipsThreshold);
        }

        /// <summary>
        /// Disposes all images uploaded to the model.
        /// </summary>
        private void DisposeImages()
        {
            foreach (var imageEntry in ImageEntries)
            {
                imageEntry.Image.Dispose();
            }

            ImageEntries = null;
        }

        /// <summary>
        /// Launches the image upload page.
        /// </summary>
        private void LaunchImageUploadPage()
        {
            var imageUploadStoryBoard = UIStoryboard.FromName(Constants.ImageUploadStoryboardName, null);
            var imageUploadViewController =
                imageUploadStoryBoard.InstantiateInitialViewController() as ImageUploadViewController;
            NavigationController.PushViewController(imageUploadViewController, true);
        }

        /// <summary>
        /// Returns the number of chips contained in the specified <see cref="UIImage"/> using the default chip width
        /// and the default chip size.
        /// </summary>
        /// <param name="image">The specified <see cref="UIImage"/>.</param>
        /// <returns></returns>
        private static int GetNumberOfChips(SKImage image)
        {
            var width = image.Width;
            var height = image.Height;
            var numberWidth = Convert.ToInt32(Math.Ceiling((double)width / SharedConstants.DefaultChipWidth));
            var numberHeight = Convert.ToInt32(Math.Ceiling((double)height / SharedConstants.DefaultChipHeight));
            return numberWidth * numberHeight;
        }

        #region Configure Accessibility Attributes
        private void ConfigureAnalyzeStartedVoiceOverAccessibilityAttributes()
        {
            UIAccessibility.PostNotification(UIAccessibilityPostNotification.ScreenChanged, new NSString(AccessibilityConstants.AnalyzeStartedNotificationAccessibilityString));
        }
        #endregion
    }
}
