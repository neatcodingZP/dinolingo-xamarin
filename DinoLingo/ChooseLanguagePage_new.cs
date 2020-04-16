using DinoLingo.ScreenOrientations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;

using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace DinoLingo
{
    class ChooseLanguagePage_new: ContentPage
    {
        static int ERROR_PREFIX = 40;

        bool isAnimating;
        Object animLock = new Object();
        public bool IsAnimating
        {
            get
            {
                if (animLock != null) lock (animLock)
                    {
                        return isAnimating;
                    }
                else return true;
            }
            set
            {
                if (animLock != null) lock (animLock)
                    {
                        isAnimating = value;
                    }
            }
        }
        bool IsDownloading = false;

        Xamarin.Forms.ListView mainList;

        public ChooseLanguagePage_new(Login_Response.Login loginResult)
        {
            Debug.WriteLine("ChooseLanguagePage_new -> ");
            FlowDirection = Translate.FlowDirection_;

            // iOS Solution for a ModalPage (popup) which is not fullscreen
            On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.FullScreen);

            var absLayout = new AbsoluteLayout() { BackgroundColor = MyColors.BackgroundBlueColor };

            Forms9Patch.Image patternImage = new Forms9Patch.Image
            {
                Source = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.pattern.png"),
                Fill = Forms9Patch.Fill.Tile,
            };
            AbsoluteLayout.SetLayoutBounds(patternImage, new Rectangle(0.5, 0.5, 1, 1));
            AbsoluteLayout.SetLayoutFlags(patternImage, AbsoluteLayoutFlags.All);

            // add grid to main frame
            var grid = new Grid()
            {
                Padding = UI_Sizes.MainMargin
            };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(3, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(6, GridUnitType.Star) });
            AbsoluteLayout.SetLayoutBounds(grid, new Rectangle(0.5, 0.5, 1, 1));
            AbsoluteLayout.SetLayoutFlags(grid, AbsoluteLayoutFlags.All);

            Image rainbowImage = new Image
            {
                Source = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.rainbow_big.png"),
                Aspect = Aspect.AspectFit,                
            };
            grid.Children.Add(rainbowImage, 0, 0);

            var gridForText = new Grid();
            gridForText.RowDefinitions.Add(new RowDefinition { Height = new GridLength(3, GridUnitType.Star) });
            gridForText.RowDefinitions.Add(new RowDefinition { Height = new GridLength(7, GridUnitType.Star) });

            MyViews.MyLabel chooseLangText = new MyViews.MyLabel()
            {                
                Text = Translate.GetString("choose_language"),
                FontSize = UI_Sizes.MediumTextSize * 0.8,
                TextColor = MyColors.BlueTextLoginColor,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };
            gridForText.Children.Add(chooseLangText, 0, 1);
            grid.Children.Add(gridForText, 0, 0);

            Image chooseLangDino = new Image
            {
                Source = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.UI.choose_lang_dino.png"),
                WidthRequest = UI_Sizes.ScreenHeightX * 0.5,
                Aspect = Aspect.AspectFit,
            };
            grid.Children.Add(rainbowImage, 0, 1);

            // abs layout for the list
            var absLayoutForList = new AbsoluteLayout();
            //add 2 frames
            Frame shadowFrame = new Frame
            {
                CornerRadius = UI_Sizes.BigFrameCornerRadius,
                BorderColor = Color.Transparent,
                HasShadow = false,
                BackgroundColor = MyColors.ShadowLoginColor,
                TranslationX = UI_Sizes.BigFrameShadowTranslationX,
                TranslationY = UI_Sizes.BigFrameShadowTranslationX
            };
            AbsoluteLayout.SetLayoutBounds(shadowFrame, new Rectangle(0, 0, 0.95, 0.95));
            AbsoluteLayout.SetLayoutFlags(shadowFrame, AbsoluteLayoutFlags.All);

            Frame mainFrame = new Frame
            {
                CornerRadius = UI_Sizes.BigFrameCornerRadius,
                BorderColor = Color.Transparent,
                HasShadow = false,
                BackgroundColor = Color.White,
                Padding = new Thickness(
                    UI_Sizes.BigFrameCornerRadius * 0.27, UI_Sizes.BigFrameCornerRadius * 0.35,                
                    UI_Sizes.BigFrameCornerRadius * 0.27, UI_Sizes.BigFrameCornerRadius * 0.35),
            };
            AbsoluteLayout.SetLayoutBounds(mainFrame, new Rectangle(0, 0, 0.95, 0.95));
            AbsoluteLayout.SetLayoutFlags(mainFrame, AbsoluteLayoutFlags.All);

            Xamarin.Forms.ListView mainList_ = new Xamarin.Forms.ListView()
            {
                HasUnevenRows = true,
                IsEnabled = !IsDownloading,
                SeparatorVisibility = SeparatorVisibility.None,  
            };
            mainList = mainList_;
            mainList_.ItemTapped += OnItemTapped;
            mainList_.ItemSelected += OnItemSelected;



            grid.Children.Add(absLayoutForList, 1, 0);
            Grid.SetRowSpan(absLayoutForList, 2);

            absLayout.Children.Add(patternImage);
            absLayout.Children.Add(grid);

            Content = absLayout;
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            
        }

        private async void MenuButton_Tapped(object sender, System.EventArgs e)
        {
            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Send((ContentPage)this, "ForcePortrait");
            ScreenOrientation.Instance.ForcePortrait();
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
            Debug.WriteLine("ChooseLanguagePage_new -> Dispose");
            Content = null;
            BindingContext = null;
            animLock = null;
        }
    }
}
