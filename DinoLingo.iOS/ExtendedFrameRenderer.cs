using Xamarin.Forms;
using DinoLingo;
using Xamarin.Forms.Platform.iOS;
using DinoLingo.iOS;
using UIKit;
using System.ComponentModel;
using System.Diagnostics;

[assembly: ExportRenderer(typeof(ExtendedFrame), typeof(ExtendedFrameRenderer))]
namespace DinoLingo.iOS
{
    public class ExtendedFrameRenderer: FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs <Frame> e) {  
            base.OnElementChanged(e);  

            //Layer.BorderColor = UIColor.White.CGColor;
            
            
            //Layer.CornerRadius = UI_Sizes.ButtonCornerRadius;
            //Layer.MasksToBounds = false;  
            //Layer.ShadowOffset = new CGSize(-2, 2);  
            //Layer.ShadowRadius = 5;  
            //Layer.ShadowOpacity = 0.4 f;  

            if (Element != null)
            {
                Layer.MasksToBounds = true;
                Layer.BorderColor = UIColor.White.CGColor;
                Layer.CornerRadius = (float)(Element as ExtendedFrame).CornerRadius;
                Layer.BorderWidth = (float)(Element as ExtendedFrame).OutLineWidth;
                Debug.WriteLine("Layer.BorderWidth = " + Layer.BorderWidth);
            }

        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            Layer.BorderWidth = (float)(Element as ExtendedFrame).OutLineWidth; 
        }
    }

}
