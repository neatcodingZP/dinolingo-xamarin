using System;
using System.Collections.Generic;
using System.Linq;


using Foundation;
using UIKit;

using System.Diagnostics;
using Plugin.GoogleAnalytics;
using CarouselView.FormsPlugin.iOS;
using Plugin.FirebasePushNotification;
using Plugin.FirebasePushNotification.Abstractions;

namespace DinoLingo.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.

    

    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //

        public int orientation = 0;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Debug.WriteLine("FinishedLaunching");


            global::Xamarin.Forms.Forms.Init();

            CarouselViewRenderer.Init();
            Forms9Patch.iOS.Settings.Initialize(this);


            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(); 
            var config = new FFImageLoading.Config.Configuration()
            {
                DiskCacheDuration = TimeSpan.FromDays(365)
            };
            global::FFImageLoading.ImageService.Instance.Initialize(config);

            // google analytics
            GoogleAnalytics.Current.Config.TrackingId = "UA-131015357-1";
            GoogleAnalytics.Current.Config.AppId = "DinoLingo";
            GoogleAnalytics.Current.Config.AppName = "DinoLingo";
            GoogleAnalytics.Current.Config.AppVersion = "1.0";
            GoogleAnalytics.Current.Config.ReportUncaughtExceptions = true;
            GoogleAnalytics.Current.Config.Debug = App.DEBUG;
            GoogleAnalytics.Current.InitTracker();

            LoadApplication(new App());

            UI_Sizes.SetAllSizes((int)UIScreen.MainScreen.Bounds.Width, (int)UIScreen.MainScreen.Bounds.Height, 1.0f);
            RateWidget.IsFreshIOS = UIDevice.CurrentDevice.CheckSystemVersion(10, 3);

            FirebasePushNotificationManager.Initialize(options, new NotificationUserCategory[]
        {
                new NotificationUserCategory("message",new List<NotificationUserAction> {
                    new NotificationUserAction("Reply","Reply",NotificationActionType.Foreground)
                }),
                new NotificationUserCategory("request",new List<NotificationUserAction> {
                    new NotificationUserAction("Accept","Accept"),
                    new NotificationUserAction("Reject","Reject",NotificationActionType.Destructive)
                })

        });

            /*
            var key = new NSString("AppleLanguages");
            string[] languageValues = { "en-US" };
            NSUserDefaults.StandardUserDefaults.SetValueForKey(NSArray.FromObjects(languageValues), key);
            NSUserDefaults.StandardUserDefaults.Synchronize();
            */

            return base.FinishedLaunching(app, options);
        }



        [Export("application:supportedInterfaceOrientationsForWindow:")]
        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations (UIApplication application, UIWindow forWindow)
        {
            var mainPage = Xamarin.Forms.Application.Current.MainPage;

            /*
            if (App.IsLandscape)
                return UIInterfaceOrientationMask.LandscapeRight;
            else return UIInterfaceOrientationMask.Portrait;
            */

            if (orientation == 0)
            {
                return UIInterfaceOrientationMask.Portrait;
            }

            else
            {
                return UIInterfaceOrientationMask.Landscape;
            }

        }
        

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            FirebasePushNotificationManager.DidRegisterRemoteNotifications(deviceToken);
            Debug.WriteLine("AppDelegate -> RegisteredForRemoteNotifications");
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            FirebasePushNotificationManager.RemoteNotificationRegistrationFailed(error);
            Debug.WriteLine("AppDelegate -> FailedToRegisterForRemoteNotifications");
        }
        // To receive notifications in foregroung on iOS 9 and below.
        // To receive notifications in background in any iOS version
        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            // If you are receiving a notification message while your app is in the background,
            // this callback will not be fired 'till the user taps on the notification launching the application.

            // If you disable method swizzling, you'll need to call this method. 
            // This lets FCM track message delivery and analytics, which is performed
            // automatically with method swizzling enabled.
            FirebasePushNotificationManager.DidReceiveMessage(userInfo);
            // Do your magic to handle the notification data
            System.Console.WriteLine(userInfo);
            Debug.WriteLine("AppDelegate -> DidReceiveRemoteNotification -> userInfo = " + userInfo);
            completionHandler(UIBackgroundFetchResult.NewData);
        }
    }
}
