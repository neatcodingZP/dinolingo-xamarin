using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;
using DinoLingo.ScreenOrientations;

namespace DinoLingo
{
    public partial class ReportPage : ContentPage
    {
        ReportPage_ViewModel viewModel;
        public ReportPage(INavigation navigation)
        {
            InitializeComponent();
            FlowDirection = Translate.FlowDirection_;
            BindingContext = viewModel = new ReportPage_ViewModel(navigation);

            Thickness margin = new Thickness(5);
            if (UI_Sizes.ScreenWidthAspect > 1.78)
            {
                margin = new Thickness(25, 5, 15, 5);
            }
            rootRelative.Margin = margin;

            viewModel.AddCenterView(rootRelative);
            viewModel.AddCloseButton(rootRelative);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.OnAppearing();

            MessagingCenter.Send((ContentPage)this, "ForcePortrait");
            ScreenOrientation.Instance.ForcePortrait();
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
            Debug.WriteLine("ReportPage -> Dispose");

            Content = null;
            BindingContext = null;
            viewModel = null;
        }
    }
}
