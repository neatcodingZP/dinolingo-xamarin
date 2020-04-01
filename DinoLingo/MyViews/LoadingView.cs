using FFImageLoading.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DinoLingo.MyViews
{
    public class LoadingView: Grid
    {
        public CachedImage mainImage_;
        public CloseButton closeBtn_;

        public LoadingView()
        {
            Padding = UI_Sizes.MainMargin;
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(10, GridUnitType.Star) });
            RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            CachedImage mainImage = new CachedImage
            {
                Aspect = Aspect.AspectFit,
            };
            mainImage_ = mainImage;
            Children.Add(mainImage, 0, 1);            

            CloseButton closeBtn = new CloseButton();
            closeBtn_ = closeBtn;
            closeBtn.IsEnabled = closeBtn.IsVisible = false;
            Children.Add(closeBtn, 0, 0);
            Grid.SetRowSpan(closeBtn, 3);
        }

        public async Task AnimateUp(uint time)
        {
            Debug.WriteLine("LoadingView -> AnimateUp");
            await Task.Delay((int) (time * 0.2));
            await this.TranslateTo(0, -App.Current.MainPage.Height, (uint) (time * 0.8));
            IsEnabled = IsVisible = false;
            InputTransparent = true;
            Debug.WriteLine("LoadingView -> AnimateUp -> finished");
        }

        
        public void Dispose()
        {
            mainImage_ = null;
            closeBtn_ = null;
        }
    }
}
