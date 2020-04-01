using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace DinoLingo
{
    public partial class BackButtonView : ContentView
    {
        
        public BackButtonView()
        {
            InitializeComponent();
        }


        public void AddToTopRight(RelativeLayout totalLayout, double btn_height_size, double padding, Action<View> OnClickedClose)
        {
            totalLayout.Children.Add(this,
                Constraint.RelativeToParent((parent) =>
                {
                return parent.Width - parent.Height * btn_height_size - padding;    // установка координаты X
                }),
                Constraint.RelativeToParent((parent) =>
                {
                    return parent.Height * 0 + padding;   // установка координаты Y
                }),

                Constraint.RelativeToParent((parent) =>
                {
                return parent.Height * btn_height_size;
                }), // установка ширины
                Constraint.RelativeToParent((parent) =>
                {
                return parent.Height * btn_height_size;
                })
                );

            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (sender, e) =>
            {
                BackButtonView view = (BackButtonView)sender;
                if (view != null) OnClickedClose(view);

            };
            this.GestureRecognizers.Add(tapGestureRecognizer);
        }

        public void Dispose()
        {
            Content = null;
            BindingContext = null;
        }
    }
}
