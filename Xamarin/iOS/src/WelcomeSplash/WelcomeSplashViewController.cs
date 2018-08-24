using EarthLens.iOS.StartingScreen;
using System;
using System.Threading.Tasks;
using UIKit;

namespace EarthLens.iOS.WelcomeSplash
{
    public partial class WelcomeSplashViewController : UIViewController
    {
        public WelcomeSplashViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            NavigationController.NavigationBarHidden = true;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            NavigationController.NavigationBarHidden = false;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            Task.Delay(SharedConstants.WelcomeSplashDelay).Wait();
            LaunchStartingScreen();
        }

        private void LaunchStartingScreen()
        {
            var startingScreenStoryBoard = UIStoryboard.FromName("StartingScreen", null);
            var startingViewController = startingScreenStoryBoard.InstantiateInitialViewController() as StartingViewController;
            NavigationController.PushViewController(startingViewController, true);
        }
    }
}