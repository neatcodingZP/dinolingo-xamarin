using System;
using System.Collections.Generic;
using System.Diagnostics;
using FormsVideoLibrary;
using Xamarin.Forms;

namespace DinoLingo
{
    public partial class VideoPage : ContentPage
    {
        

        VideoPage_ViewModel viewModel;

        public VideoPage(INavigation navigation, string id, ListItemStarsInfo starsInfo)
        {
            InitializeComponent();
            BindingContext = viewModel = new VideoPage_ViewModel(navigation, id, loadingView, rootRelative, starsInfo);

            viewModel.AddCloseButton(rootRelative);
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

        void Handle_UpdateStatus(object sender, System.EventArgs e)
        {
            if (viewModel != null) viewModel.Handle_UpdateStatus(videoPlayer, sender, e);
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
            Debug.WriteLine("VideoPage -> Dispose");

            Content = null;
            BindingContext = null;
            viewModel = null;
        }
    }
}
