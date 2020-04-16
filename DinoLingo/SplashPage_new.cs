using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace DinoLingo
{
    public class SplashPage_new: ContentPage
    {        

        public SplashPage_new()
        {
            Image image = new Image
            {
                Source = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.launch_icon_512x512.png"),
                WidthRequest = 200,
                HeightRequest = 200,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Aspect = Aspect.AspectFit,
            };
            Content = image;

            /*
            var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            Debug.WriteLine("SplashPage_new() ->  ci = " + ci.Name);
            */
        }            
           
        protected override void OnParentSet()
        {
            base.OnParentSet();
            if (Parent == null)
            {               
                Dispose();
                GC.Collect();
                MemoryLeak.TrackMemory();
            }
        }

        void Dispose()
        {
            Debug.WriteLine("SplashPage_new -> Dispose");
            (Content as Image).Source = null;
            Content = null;
            BindingContext = null;            
        }
    }
}
