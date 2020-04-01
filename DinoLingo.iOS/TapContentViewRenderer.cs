using System;
using Xamarin.Forms;
using DinoLingo;
using Xamarin.Forms.Platform.iOS;
using DinoLingo.iOS;
using UIKit;
using CoreGraphics;


[assembly: ExportRenderer(typeof(TapContentView), typeof(TapContentViewRenderer))]
namespace DinoLingo.iOS
{
    public class TapContentViewRenderer: ViewRenderer<TapContentView, UIView>
    {
        #region properties & fields
        // ---------------------------------------------------------------------------
        //
        // PROPERTIES & FIELDS
        //
        // ---------------------------------------------------------------------------
        private UIView nativeElement;
        private TapContentView formsElement;
        #endregion

        #region methods
        // ---------------------------------------------------------------------------
        //
        // METHODS
        //
        // ---------------------------------------------------------------------------

        //
        // Set up the custom renderer. In this case, that means set up the gesture
        // recognizer.
        //
        protected override void OnElementChanged(ElementChangedEventArgs<TapContentView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                nativeElement = new UIView ();
                SetNativeControl(nativeElement);
            }

            if (e.OldElement != null)
            {
                // Unsubscribe

            }
            if (e.NewElement != null)
            {
                // Subscribe

            }

            if (e.NewElement != null)
            {
                // Grab the Xamarin.Forms control (not native)
                formsElement = e.NewElement as TapContentView;
                // Grab the native representation of the Xamarin.Forms control
                nativeElement = Control as UIView;
                // Set up a tap gesture recognizer on the native control
                if (nativeElement != null) nativeElement.UserInteractionEnabled = true;

                UITapGestureRecognizer tgr = new UITapGestureRecognizer(TapHandler);
                nativeElement.AddGestureRecognizer(tgr);
            }
        }

        //
        // Respond to taps.
        //
        public void TapHandler(UITapGestureRecognizer tgr)
        {
            CGPoint touchPoint = tgr.LocationInView(nativeElement);
            formsElement.OnTapEvent((int)touchPoint.X, (int)touchPoint.Y);
        }
        #endregion
    }
}
