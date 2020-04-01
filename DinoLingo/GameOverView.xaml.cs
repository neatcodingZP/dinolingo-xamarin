using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DinoLingo
{
    public partial class GameOverView : ContentView
    {
        public double DialogHeight { get; set; }
        public double DialogWidth { get; set; }
        public double DialogFontSize { get; set; }

        public double BtnCornerRadius { get; set; }
        public double BtnCornerRadiusInner { get; set; }
        public double BtnShadowTranslationY { get; set; }
        public Thickness StrokeLineMarginBtn { get; set; }
        public double BtnTextSize { get; set; }

        public bool isAnimating = false;


        INavigation navigation;
        Action OnClose;
        Action OnRestart;
        double startY;

        public void Dispose()
        {
            Content = null;
            BindingContext = null;
            OnClose = OnRestart = null;
            navigation = null;
        }

        public GameOverView(INavigation navigation, Action onClose, Action onRestart)
        {
            InitializeComponent();
            BindingContext = this;
            this.navigation = navigation;
            this.BackgroundColor = Color.FromRgba(0, 0, 0, 0.75);
            OnClose = onClose;
            OnRestart = onRestart;

            CloseBtn.WidthRequest = CloseBtn.HeightRequest = UI_Sizes.CloseBtnSize;
            shadowFrame.CornerRadius = mainFrame.CornerRadius = UI_Sizes.BigFrameCornerRadius;
            shadowFrame.TranslationX = UI_Sizes.BigFrameShadowTranslationX;
            shadowFrame.TranslationY = UI_Sizes.BigFrameShadowTranslationY;

            BtnCornerRadius = UI_Sizes.ButtonCornerRadius;
            BtnCornerRadiusInner = BtnCornerRadius - UI_Sizes.ButtonOutlineWidth;
            BtnShadowTranslationY = UI_Sizes.ButtonShadowTranslationY;
            StrokeLineMarginBtn = new Thickness(UI_Sizes.ButtonOutlineWidth);
            BtnTextSize = UI_Sizes.SmallTextSize;
        }

        public async void Button_Tapped(object sender, System.EventArgs e)
        {
            if (isAnimating) return;
            isAnimating = true;

            View view = sender as View;
            if (view.ClassId == "CloseBtn")
            {
                OnClose();
                await AnimateImage(view, 250);
                await navigation.PopModalAsync();
            }
            else if (view.ClassId == "RestartBtn")
            {
                Debug.WriteLine("restart tapped");
                await AnimateImage(view, 250);
                OnRestart();
            }
            isAnimating = false;
        }

        public Task AnimateImage(View view, uint time)
        {
            
            return Task.Run(async () =>
            {
                try
                {
                    await view.ScaleTo(0.8, time / 2);
                    await view.ScaleTo(1.0, time / 2);
                }
                catch
                {

                }               
                return;
            });
        }


        public async void AnimateDown()
        {
            isAnimating = true;
            IsVisible = true;
            InputTransparent = false;
            await gameoverDialog.TranslateTo(0, 0, 700);
            IsEnabled = true;
            isAnimating = false;
        }

        public async void AnimateUp()
        {
            IsEnabled = false;
            await gameoverDialog.TranslateTo(0, startY, 700);
            IsVisible = false;
            InputTransparent = true;
        }

        public void SetSizesAndStartY(double height, double widtFactor, double fontFactor, double translationY)
        {
            DialogHeight = height;
            DialogWidth = height * widtFactor;
            DialogFontSize = UI_Sizes.SmallTextSize; //height * fontFactor;
            Debug.WriteLine("DialogFontSize= " + DialogFontSize);
            startY = translationY; 
            gameoverDialog.TranslationY = translationY;
        }
    }
}
