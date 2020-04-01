using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;


using System.Threading.Tasks;
using Android.Content;
using Plugin.GoogleAnalytics;
using Plugin.InAppBilling;
using Plugin.CurrentActivity;
using CarouselView.FormsPlugin.Android;
using Plugin.FirebasePushNotification;
using Xamarin.Forms;
using DinoLingo;

namespace DinoLingo.Droid
{
    [Activity(Label = "DinoLingo", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    //[Activity(Label = "DinoLingo", Icon = "@mipmap/icon", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {        

        protected override void OnCreate(Bundle bundle)
        {
            //TabLayoutResource = Resource.Layout.Tabbar;
            //ToolbarResource = Resource.Layout.Toolbar;

            if (App.IsLandscape)
            {
                RequestedOrientation = Android.Content.PM.ScreenOrientation.SensorLandscape;
            }
            else
            {
                RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
            }


            #region PORTRAIT TO LANDSCAPE
            //allowing the device to change the screen orientation based on the rotation 
            MessagingCenter.Subscribe<ContentPage>(this, "ForcePortrait", sender =>
            {
                Console.WriteLine("MessagingCenter -> ForcePortrait");
                RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
            });

            //during page close setting back to portrait
            MessagingCenter.Subscribe<ContentPage>(this, "ForceLandscape", sender =>
            {
                Console.WriteLine("MessagingCenter -> ForceLandscape");
                RequestedOrientation = Android.Content.PM.ScreenOrientation.SensorLandscape;
            });
            #endregion


            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                // Kill status bar underlay added by FormsAppCompatActivity
                // Must be done before calling FormsAppCompatActivity.OnCreate()
                var statusBarHeightInfo = typeof(Xamarin.Forms.Platform.Android.FormsAppCompatActivity).GetField("statusBarHeight", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (statusBarHeightInfo == null)
                {
                    statusBarHeightInfo = typeof(Xamarin.Forms.Platform.Android.FormsAppCompatActivity).GetField("_statusBarHeight", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                }
                statusBarHeightInfo?.SetValue(this, 0);
            }

            base.OnCreate(bundle);

            this.Window.AddFlags(WindowManagerFlags.Fullscreen | WindowManagerFlags.TurnScreenOn);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            CarouselViewRenderer.Init();
            Forms9Patch.Droid.Settings.Initialize(this);


            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true); 
            var config = new FFImageLoading.Config.Configuration()
            {
                DiskCacheDuration = TimeSpan.FromDays(365)
            };
            global::FFImageLoading.ImageService.Instance.Initialize(config);



            

            UI_Sizes.SetAllSizes((int)(Resources.DisplayMetrics.WidthPixels), (int)(Resources.DisplayMetrics.HeightPixels), Resources.DisplayMetrics.Density);

            // google analytics
            GoogleAnalytics.Current.Config.TrackingId = "UA-131015357-1";
            GoogleAnalytics.Current.Config.AppId = "DinoLingo";
            GoogleAnalytics.Current.Config.AppName = "DinoLingo";
            GoogleAnalytics.Current.Config.AppVersion = "1.0";
            GoogleAnalytics.Current.Config.ReportUncaughtExceptions = true;
            GoogleAnalytics.Current.Config.Debug = App.DEBUG;
            GoogleAnalytics.Current.InitTracker();

            // current activity plugin (for in-app billing)
            CrossCurrentActivity.Current.Init(this, bundle);
            CrossCurrentActivity.Current.Activity = this;
         
            
            
            
            LoadApplication(new App());

            FirebasePushNotificationManager.ProcessIntent(this, Intent);
        }

        protected override void OnNewIntent(Intent intent)
        {
            //App.haveNotificationBeforeStart = true;
            base.OnNewIntent(intent);

            FirebasePushNotificationManager.ProcessIntent(this, Intent);
        }

        // Field, properties, and method for Video Picker
        public static MainActivity Current { private set; get; }

        public static readonly int PickImageId = 1000;

        public TaskCompletionSource<string> PickImageTaskCompletionSource { set; get; }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            InAppBillingImplementation.HandleActivityResult(requestCode, resultCode, data);
        }

        /*
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == PickImageId)
            {
                if ((resultCode == Result.Ok) && (data != null))
                {
                    // Set the filename as the completion of the Task
                    PickImageTaskCompletionSource.SetResult(data.DataString);
                }
                else
                {
                    PickImageTaskCompletionSource.SetResult(null);
                }
            }
        }
        */
    }

    
}

