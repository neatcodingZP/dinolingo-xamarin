
using Xamarin.Forms;
using DinoLingo;
using Xamarin.Forms.Platform.iOS;
using DinoLingo.iOS;
using UIKit;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(BorderlessEntry), typeof(BorderlessEntryRenderer))]
namespace DinoLingo.iOS
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