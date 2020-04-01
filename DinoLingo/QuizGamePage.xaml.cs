using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace DinoLingo
{
    public partial class QuizGamePage : ContentPage
    {
        QuizViewModel viewModel;  

        public QuizGamePage(INavigation navigation, THEME_NAME ThemeName, ListItemStarsInfo starsInfo)
        {
            InitializeComponent();
            loadingView.BackgroundColor = LoadingView_Logic.GetRandomColor();
            loadingView.Padding = UI_Sizes.MainMargin;
            BindingContext = viewModel = new QuizViewModel(navigation, ThemeName, 
                                                           new View [] {cell_0, cell_1, cell_2, cell_3}, new Image [] {cellImage_0, cellImage_1, cellImage_2, cellImage_3},
                                                           starsInfo);
            
            viewModel.AddVictoryView(absRootLayout);
            viewModel.AddGameOverView(absRootLayout);
            LoadingView_Logic.ShowCloseBtnTimer(LoadingCloseBtn);
        }




        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.OnAppearing(loadingView);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            viewModel.OnDisappearing();
        }

        private void MenuButton_Tapped(object sender, System.EventArgs e)
        {
            viewModel.MenuButton_Tapped(sender, e);
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
            Debug.WriteLine("QuizGamePage -> Dispose");

            Content = null;
            BindingContext = null;
            viewModel = null;
        }

    }
}
