using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace DinoLingo
{
    public partial class TestViewPerformance : ContentPage
    {
        public ImageSource Source { get; set; }

        public TestViewPerformance()
        {
            InitializeComponent();
            BindingContext = this;
            Source = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.COMMON.heart_color.png");
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("Test page appeared");

        }
    }
}
