using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace DinoLingo
{
    public partial class SlideView : ContentView
    {


        public static readonly BindableProperty IsSlideOpenProperty =
            BindableProperty.Create<SlideView, bool>(p => p.IsSlideOpen, false, BindingMode.TwoWay, propertyChanged: SlideOpenClose);

        public static readonly BindableProperty WasSlideOpenProperty =
            BindableProperty.Create<SlideView, bool>(p => p.WasSlideOpen, false, BindingMode.TwoWay, propertyChanged: WasSlideOpenChange);

        public static readonly BindableProperty IsSlideVisisbleProperty =
            BindableProperty.Create<SlideView, bool>(p => p.IsSlideVisisble, false, BindingMode.TwoWay, propertyChanged: SlideVisibleSwitch);

        public static readonly BindableProperty CurrentPositionProperty =
            BindableProperty.Create<SlideView, double>(p => p.CurrentPosition, 0, BindingMode.TwoWay, propertyChanged: CurrentPositionChanged);
        
        public static double Size = 200;

        public static bool WasOpened = false;

        public SlideView()
        {
            InitializeComponent();

        }

        public bool IsSlideOpen
        {
            get { return (bool)GetValue(IsSlideOpenProperty); }
            set { SetValue(IsSlideOpenProperty, value); }

        }

        public bool WasSlideOpen
        {
            get { return (bool)GetValue(IsSlideOpenProperty); }
            set { SetValue(IsSlideOpenProperty, value); }
        }

        public bool IsSlideVisisble
        {
            get { return (bool)GetValue(IsSlideVisisbleProperty); }
            set { SetValue(IsSlideVisisbleProperty, value); }
        }

        public double CurrentPosition
        {
            get { return (double)GetValue(CurrentPositionProperty); }
            set { SetValue(CurrentPositionProperty, value); }
        }

        private static void CurrentPositionChanged(BindableObject bindableObject, double oldValue, double newValue)
        {
            (bindableObject as SlideView).TranslationX = newValue;

            if (!(bindableObject as SlideView).IsVisible && (bindableObject as SlideView).TranslationX > -Size) {
                (bindableObject as SlideView).IsVisible = true;
                (bindableObject as SlideView).IsSlideVisisble = true;
            }
            Debug.WriteLine("CurrentPosition = newValue, TranslationX =" + (bindableObject as SlideView).TranslationX); 
        }

        private static void SlideVisibleSwitch(BindableObject bindableObject, bool oldValue, bool newValue)
        {
            if (newValue)
            {
                newValue = false;
            }

            else
            {
                newValue = true;
            }
        }

        private static void WasSlideOpenChange(BindableObject bindableObject, bool oldValue, bool newValue)
        {
            Debug.WriteLine("WasSlide Changed, (bindableObject as SlideView).WasSlideOpen = " + (bindableObject as SlideView).WasSlideOpen);
        }


        private static async void SlideOpenClose(BindableObject bindableObject, bool oldValue, bool newValue)
        {
            
            if (newValue)
            {
                (bindableObject as SlideView).IsVisible = true;
                (bindableObject as SlideView).IsSlideVisisble = true;
                await (bindableObject as SlideView).TranslateTo(0, 0, 250, Easing.SinInOut);
                (bindableObject as SlideView).CurrentPosition = 0;
                (bindableObject as SlideView).WasSlideOpen = true;
                WasOpened = true;
                newValue = false;
                Debug.WriteLine("CurrentPosition = 0, TranslationX =" + (bindableObject as SlideView).TranslationX);
            }

            else
            {
                await (bindableObject as SlideView).TranslateTo(-Size -1, 0, 250, Easing.SinInOut);
                //(bindableObject as SlideView).TranslationX = -Size -1;
                (bindableObject as SlideView).IsVisible = false;
                (bindableObject as SlideView).IsSlideVisisble = false;
                (bindableObject as SlideView).CurrentPosition = -Size - 1;
                (bindableObject as SlideView).WasSlideOpen = false;
                WasOpened = false;
                newValue = true;

                Debug.WriteLine("CurrentPosition = -Size-1, TranslationX =" + (bindableObject as SlideView).TranslationX);
            }

            Debug.WriteLine("IsSlide Changed, (bindableObject as SlideView).IsSlideOpen = " + (bindableObject as SlideView).IsSlideOpen);
            Debug.WriteLine("(bindableObject as SlideView).WasSlideOpen = " + (bindableObject as SlideView).WasSlideOpen);
        }

       
    }
}
