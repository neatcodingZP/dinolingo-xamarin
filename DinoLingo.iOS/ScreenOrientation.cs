using System;
using System.Threading;
using Foundation;

using UIKit;
using System.Drawing;
using UIKit;
using CoreGraphics;
using DinoLingo.ScreenOrientations;
using Xamarin.Forms;

[assembly: Dependency(typeof(DinoLingo.iOS.ScreenOrientation))]
namespace DinoLingo.iOS
{
    public class ScreenOrientation: IScreenOrientation
    {
        public void ForceLandscape()
        {
            AppDelegate appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
            appDelegate.orientation = 1;

            UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.LandscapeRight), new NSString("orientation"));            
            
        }

        public void ForcePortrait()
        {
            AppDelegate appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
            appDelegate.orientation = 0;

            UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)UIInterfaceOrientation.Portrait), new NSString("orientation"));           
            
        }
                
    }
}