using System;
using UIKit;
using Foundation;
using EarthLens.Services;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using EarthLens.iOS.WelcomeSplash;
using EarthLens.iOS.ImageUpload;

namespace EarthLens.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        private UIWindow _window;

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Start AppCenter Analytics and Crashes services
            EnvironmentService.SetEnvironmentVariables();

            // For more information on how to get a Visual Studio App Center API key refer to:
            // https://docs.microsoft.com/en-us/appcenter/sdk/getting-started/xamarin
            var apiKey = Environment.GetEnvironmentVariable(SharedConstants.AppCenterSecretKeyIOS);
            AppCenter.Start(apiKey, typeof(Analytics), typeof(Crashes));

            // Launch application
            InitializeApp();
            return true;
        }

        public static bool LaunchedFromShareExtension
        {
            get => NSUserDefaults.StandardUserDefaults.BoolForKey(Constants.ShareExtensionCheck);
            set => NSUserDefaults.StandardUserDefaults.SetBool(value, Constants.ShareExtensionCheck);
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject
            annotation)
        {
            if (url == null) 
            {
                return false;
            }
            LaunchedFromShareExtension = true;
            _window = new UIWindow(UIScreen.MainScreen.Bounds);
            var storyboard = UIStoryboard.FromName(Constants.ImageUploadStoryboardName, null);
            var imageUploadViewController = storyboard.InstantiateInitialViewController() as ImageUploadViewController;
            var navigationController = new UINavigationController(imageUploadViewController);
            _window.RootViewController = navigationController;
            _window.MakeKeyAndVisible();
            return true;
        }


        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
        }

        
        /// <summary>
        /// Initializes the application
        /// </summary>
        private void InitializeApp()
        {
            DatabaseService.CreateTables();

            NSUserDefaults.StandardUserDefaults.SetDouble(SharedConstants.DefaultConfidenceThreshold,
                Constants.ConfidenceThresholdUserDefaultName);

            _window = new UIWindow(UIScreen.MainScreen.Bounds);

            var welcomeSplash = UIStoryboard.FromName(Constants.WelcomeSplashStoryboardName, null);
            var welcomeSplashViewController =
                welcomeSplash.InstantiateInitialViewController() as WelcomeSplashViewController;

            var welcomeSplashNavigationController = new UINavigationController(welcomeSplashViewController);

            if (_window == null) return;
            _window.RootViewController = welcomeSplashNavigationController;
            _window.MakeKeyAndVisible();
        }
    }
}

