using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace DinoLingo
{
    public partial class BadgeList_Page : ContentPage
    {
        BadgeList_Page_ViewModel viewModel;
        public BadgeList_Page(INavigation navigation)
        {
            InitializeComponent();
            BindingContext = viewModel = new BadgeList_Page_ViewModel(navigation);
            viewModel.AddCenterView(frameForList);
            viewModel.AddCloseButton(rootRelative);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.OnAppearing();
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
            Debug.WriteLine("BadgeList_Page -> Dispose");
            Content = null;
            BindingContext = null;
            viewModel = null;
        }
    }
}
