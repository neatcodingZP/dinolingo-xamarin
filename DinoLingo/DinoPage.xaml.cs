using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace DinoLingo
{
    public partial class DinoPage : ContentPage
    {
        DinoPage_ViewModel viewModel;
        public DinoPage(INavigation navigation, string post_id)
        {
            InitializeComponent();

            BindingContext = viewModel = new DinoPage_ViewModel(navigation, post_id);
            viewModel.AddCloseButton(rootRelative);
            viewModel.Start();
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
            Debug.WriteLine("DinoPage -> Dispose");
            Content = null;
            BindingContext = null;
            viewModel = null;
        }
    }    
}
