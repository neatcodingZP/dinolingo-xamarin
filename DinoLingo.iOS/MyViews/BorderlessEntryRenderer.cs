
using Xamarin.Forms;
using DinoLingo.MyViews;
using Xamarin.Forms.Platform.iOS;
using DinoLingo.iOS.MyViews;
using UIKit;
using System.ComponentModel;
using Foundation;

[assembly: ExportRenderer(typeof(BorderlessEntry), typeof(BorderlessEntryRenderer))]
namespace DinoLingo.iOS.MyViews
{
    public class BorderlessEntryRenderer : EntryRenderer
    {

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            Control.KeyboardType = UIKeyboardType.Default;
            Control.AutocorrectionType = UITextAutocorrectionType.No;

            Control.Layer.BorderWidth = 0;
            Control.BorderStyle = UITextBorderStyle.None;

            

        }
    }
}