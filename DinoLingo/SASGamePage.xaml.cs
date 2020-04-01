using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace DinoLingo
{
    public partial class SASGamePage : ContentPage
    {
        SASViewModel viewModel;

        int gameIndex;

        public SASGamePage()
        {
            InitializeComponent();
            loadingView.BackgroundColor = LoadingView_Logic.GetRandomColor();
            loadingView.Padding = UI_Sizes.MainMargin;
            LoadingView_Logic.ShowCloseBtnTimer(LoadingCloseBtn);
        }

        public SASGamePage (INavigation navigation, THEME_NAME ThemeName, int gameIndex, ListItemStarsInfo starsInfo): this() {
            Debug.WriteLine("public SASGamePage (INavigation navigation, THEME_NAME ThemeName): this()");
            this.gameIndex = gameIndex;
            viewModel = new SASViewModel(navigation, ThemeName, gameIndex, absoluteRoot, totalLayout, loadingView, starsInfo);
            BindingContext = viewModel;
        }

        private void OnBackClicked(object sender, System.EventArgs e)
        {
            Debug.WriteLine("QuizGame ---> OnBackClicked");
            viewModel.navigation.PopModalAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.OnAppearing();
        }

        public async void MenuButton_Tapped(object sender, System.EventArgs e)
        {
            viewModel.MenuButton_Tapped(sender, e);
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();
            viewModel.OnClose();

            if (gameIndex == 1) // it is animals game 2  
            {
                //await Navigation.PopToRootAsync(true);
                Debug.WriteLine("first PopModalAsync");
				Navigation.PopModalAsync();
            }           
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
            Debug.WriteLine("SASGamePage -> Dispose");

            Content = null;
            BindingContext = null;
            viewModel = null;
        }
    }
}
