using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DinoLingo.ScreenOrientations;
using Xamarin.Forms;

[assembly: Dependency(typeof(DinoLingo.Droid.ScreenOrientation))]
namespace DinoLingo.Droid
{
    public class ScreenOrientation: IScreenOrientation
    {
        public void ForceLandscape()
        {
            var activity = (Activity)Forms.Context;
            //activity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
        }

        public void ForcePortrait()
        {
            var activity = (Activity)Forms.Context;
            //activity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Portrait;
        }
    }
}