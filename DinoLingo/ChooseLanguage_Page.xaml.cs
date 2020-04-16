using DinoLingo.ScreenOrientations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace DinoLingo
{    
    public partial class ChooseLanguage_Page : ContentPage
    {

        ChooseLanguage_ViewModel viewModel;

        public ChooseLanguage_Page(Login_Response.Login loginResult)
        {
            Debug.WriteLine("ChooseLanguagePage -> ");
            InitializeComponent();

            // iOS Solution for a ModalPage (popup) which is not fullscreen
            On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.FullScreen);

            FlowDirection = Translate.FlowDirection_;
            BindingContext = viewModel = new ChooseLanguage_ViewModel (loginResult, Navigation);
            mainGrid.Padding = UI_Sizes.MainMargin;
            shadowFrame.TranslationX = UI_Sizes.BigFrameShadowTranslationX;
            shadowFrame.TranslationY = UI_Sizes.BigFrameShadowTranslationY;
            CloseBtn.WidthRequest = CloseBtn.HeightRequest = UI_Sizes.CloseBtnSize;
        }

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            viewModel.OnItemTapped(sender, e);            
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            viewModel.OnItemSelected(sender, e);
        }

        private async void MenuButton_Tapped(object sender, System.EventArgs e)
        {
            viewModel.MenuButton_Tapped(sender, e);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Send((ContentPage)this, "ForcePortrait");
            ScreenOrientation.Instance.ForcePortrait();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            if (Parent == null)
            {
                viewModel.Dispose();
                Dispose();
                GC.Collect();
                MemoryLeak.TrackMemory();
            }
        }

        void Dispose()
        {
            Debug.WriteLine("ChooseLanguage_Page -> Dispose");
            Content = null;
            BindingContext = null;
            viewModel = null;
        }
    }
}
