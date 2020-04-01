using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using StoreKit;
using UIKit;
using Xamarin.Forms;
using DinoLingo;
using Xamarin.Forms.Platform.iOS;
using DinoLingo.iOS;

[assembly: Dependency(typeof(RequestReview_IOS))]
namespace DinoLingo.iOS
{
    class RequestReview_IOS : IRequestReview
    {
        public void RequestReview()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 3))
            {
                SKStoreReviewController.RequestReview();
            }
        }
    }
}