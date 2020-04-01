using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace DinoLingo
{
    public partial class BookPage : ContentPage
    {
        BookPage_ViewModel viewModel;
        public BookPage(INavigation navigation, string book_id)
        {
            InitializeComponent();
            loadingView.BackgroundColor = LoadingView_Logic.GetRandomColor();
            loadingView.Padding = UI_Sizes.MainMargin;

            viewModel = new BookPage_ViewModel(navigation, loadingView, BottomTextFrame, BottomTextScrollView, book_id);
            BindingContext = viewModel;

            LoadingView_Logic.ShowCloseBtnTimer(LoadingCloseBtn);
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (viewModel != null) viewModel.OnDisappearing();
        }

        public void MenuButton_Tapped(object sender, System.EventArgs e)
        {
            Debug.WriteLine("MenuButton_Tapped");
            viewModel.MenuButton_Tapped(sender, e);
        }

        
        void OnPositionSelected(object sender, CarouselView.FormsPlugin.Abstractions.PositionSelectedEventArgs e)
        {
            Debug.WriteLine("BookPage -> Position " + e.NewValue + " selected.");
            viewModel.OnPositionSelected(sender, e);
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
            Debug.WriteLine("BookPage -> Dispose");

            Content = null;
            BindingContext = null;
            viewModel = null;
        }
    }
}
