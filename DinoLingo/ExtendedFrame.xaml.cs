using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace DinoLingo
{
    public partial class ExtendedFrame : Frame
    {
        public static readonly BindableProperty OutLineWidthProperty =
            BindableProperty.Create("OutLineWidth", typeof(double), typeof(ExtendedFrame), 0.0);

        public double OutLineWidth
        {
            get { return (double)GetValue(OutLineWidthProperty); }
            set { SetValue(OutLineWidthProperty, value); }
        }


        public ExtendedFrame()
        {
            InitializeComponent();
        }
    }
}
