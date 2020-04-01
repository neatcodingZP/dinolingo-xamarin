using System;
using System.Collections.Generic;
using System.Diagnostics;

using Xamarin.Forms;

namespace DinoLingo
{
    public partial class TestFramePage : ContentPage
    {
		public double CornerRadius2 { get; set; }
        public double  StrokeLineWidth2 { get; set; }


		public TestFramePage()
        {
            InitializeComponent();
            BindingContext = this;
            CornerRadius2 = 30;
            StrokeLineWidth2 = 10;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Debug.WriteLine("test appeared");
        }
    }
}
