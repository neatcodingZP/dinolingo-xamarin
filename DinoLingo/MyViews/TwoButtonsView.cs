using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DinoLingo.MyViews
{
    public class TwoButtonsView: AbsoluteLayout
    {
        static int ERROR_PREFIX = 260;

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

        AbsoluteLayout dialogLayout_;

        public Action OnButton1Clicked;
        public Action OnButton2Clicked;

        public TwoButtonsView(string header, string button_1, string button_2) : this(header, button_1, button_2, 
            UI_Sizes.ScreenHeightX_UNIFORMED_TO_1_78 * 0.75 * 1.0,
            UI_Sizes.ScreenHeightX_UNIFORMED_TO_1_78 * 0.75,
            UI_Sizes.ScreenHeightX_UNIFORMED_TO_1_78 * 0.75 * 0.33 * 0.3)
        { }

        public TwoButtonsView(string header, string button_1, string button_2, double width, double height, double fontSize)
        {

            SetLayoutBounds(this, new Rectangle(0.5, 0.5, 1, 1));
            SetLayoutFlags(this, AbsoluteLayoutFlags.All);
            BackgroundColor = Color.FromRgba(0, 0, 0, 0.75);

            
            // create dialog layout
            AbsoluteLayout dialogLayout = new AbsoluteLayout();
            SetLayoutBounds(dialogLayout, new Rectangle(0.5, 0.5, 1, 1));
            SetLayoutFlags(dialogLayout, AbsoluteLayoutFlags.All);
            dialogLayout_ = dialogLayout;

            Frame shadowFrame = new Frame
            {
                CornerRadius = UI_Sizes.BigFrameCornerRadius,
                BorderColor = Color.Transparent,
                HasShadow = false,
                BackgroundColor = MyColors.ShadowLoginColor,
                TranslationX = UI_Sizes.BigFrameShadowTranslationX,
                TranslationY = UI_Sizes.BigFrameShadowTranslationX
            };
            SetLayoutBounds(shadowFrame, new Rectangle(0.5, 0.5, width, height));
            SetLayoutFlags(shadowFrame, AbsoluteLayoutFlags.PositionProportional);
            dialogLayout.Children.Add(shadowFrame);

            Frame mainFrame = new Frame
            {
                CornerRadius = UI_Sizes.BigFrameCornerRadius,
                BorderColor = Color.Transparent,
                HasShadow = false,
                BackgroundColor = MyColors.YellowColorLight,
                Padding = 10
            };
            SetLayoutBounds(mainFrame, new Rectangle(0.5, 0.5, width, height));
            SetLayoutFlags(mainFrame, AbsoluteLayoutFlags.PositionProportional);

            // add grid to main frame
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
           

            // add top text
            MyViews.MyLabel headerLabel = new MyViews.MyLabel()
            {
                Text = header,
                FontSize = fontSize,
                TextColor = MyColors.BlueTextLoginColor,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };
            grid.Children.Add(headerLabel, 0, 0);
                        

            // add button 1
            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (sender, e) =>
            {
                View view = (View)sender;
                OnButtonTapped(view);
            };

            // button1
            MyViews.ButtonWithImage button1 = new MyViews.ButtonWithImage(
                "button1",
                new Rectangle(0.5, 0.5, 0.75, 1.0),
                "DinoLingo.Resources.UI.btnblue_x2.png",
                button_1,
                fontSize * 0.75,
                MyColors.ButtonBlueTextColor,
                tapGestureRecognizer
                );
            grid.Children.Add(button1, 0, 1);

            // button2
            MyViews.ButtonWithImage button2 = new MyViews.ButtonWithImage(
                "button2",
                new Rectangle(0.5, 0.5, 0.75, 1.0),
                "DinoLingo.Resources.UI.btnblue_x2.png",
                button_2,
                fontSize * 0.75,
                MyColors.ButtonBlueTextColor,
                tapGestureRecognizer
                );
            grid.Children.Add(button2, 0, 2);


            dialogLayout.Children.Add(mainFrame);
            mainFrame.Content = grid;


            Children.Add(dialogLayout);

            IsEnabled = IsVisible = false;
            InputTransparent = true;
            dialogLayout.TranslationY = -UI_Sizes.ScreenHeightX;

        }

        async void OnButtonTapped(View view)
        {
            if (IsAnimating) return;
            IsAnimating = true;
            await AnimateView(view, 250);

            if (view.ClassId == "button1")
            {
                OnButton1Clicked?.Invoke();                
            }
            else if (view.ClassId == "button2")
            {
                OnButton2Clicked?.Invoke();
            }
            
        }

        public Task AnimateView(View view, uint time)
        {
            return Task.Run(async () =>
            {
                try
                {
                    await view.ScaleTo(0.8, time / 2);
                    await view.ScaleTo(1.0, time / 2);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("TwoButtonsView -> AnimateView -> ex:" + ex.Message);
                }
                return;
            });
        }

        public async void Show()
        {
            IsVisible = true;
            InputTransparent = false;

            await Task.Delay(200);

            await dialogLayout_.TranslateTo(0, 0, 700);
            IsEnabled = true;

            Debug.WriteLine("TwoButtonsView -> IsEnabled = true");
        }

        public async void Hide()
        {  
            await dialogLayout_.TranslateTo(0, -UI_Sizes.ScreenHeightX, 700);
            IsEnabled = false;
            IsVisible = false;
            InputTransparent = true;
            IsAnimating = false;
            Debug.WriteLine("TwoButtonsView -> IsEnabled = false");
        }

        public void Dispose()
        {
            BindingContext = null;
            dialogLayout_ = null;            
            animLock = null;
        }
    }
}
