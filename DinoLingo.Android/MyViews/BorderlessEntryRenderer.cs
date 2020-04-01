using Xamarin.Forms;
using DinoLingo.MyViews;
using Xamarin.Forms.Platform.Android;
using DinoLingo.Droid.MyViews;
using Android.Content;

[assembly: ExportRenderer(typeof(BorderlessEntry), typeof(BorderlessEntryRenderer))]

namespace DinoLingo.Droid.MyViews
{
    public class BorderlessEntryRenderer : EntryRenderer
    {
        public BorderlessEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                Control.Background = null;
            }
        }
    }
}