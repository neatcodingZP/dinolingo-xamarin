using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace DinoLingo
{
    public static class UI_Sizes
    {
        public static int ScreenHeight { get; set; }
        public static int ScreenWidth { get; set; }
        public static double ScreenHeightX { get; set; } // Xamarin points
        public static double ScreenWidthX { get; set; } // Xamarin sizes
        public static double ScreenHeightX_UNIFORMED_TO_1_78 { get; set; }
        public static double ScreenWidthX_UNIFORMED_TO_1_78 { get; set; }

        public static float ScreenDensity { get; set; }
       

        public static double ScreenWidthAspect { get; set; }
        public static double SmallTextSize { get; set; }
        public static double MediumTextSize { get; set; }
        public static double MicroTextSize { get; set; }
        public static double LeftPadding { get; set; }
        public static double RightPadding { get; set; }
        public static double TopBottomPadding { get; set; }
        public static Thickness MainMargin { get; set; }
        public static Thickness MainMarginPortrait { get; set; }
        public static Thickness MediumPadding { get; set; }

        // some key buttons sizes
        public static double CloseBtnSize { get; set; }


        // big frame sizes
        public static float BigFrameCornerRadius { get; set; }
        public static double BigFrameShadowTranslationX { get; set; }
        public static double BigFrameShadowTranslationY { get; set; }

        // button sizes
        public static float ButtonCornerRadius { get; set; }
        public static double ButtonOutlineWidth { get; set; }
        public static double ButtonShadowTranslationY { get; set; }
        public static Thickness ButtonShadowPadding { get; set; }

        // base button sizes        
        public static double ButtonHeight { get; set; }


        public static void SetAllSizes(int width, int height, float density) {

            Debug.WriteLine("UI_Sizes -> SetAllSizes");
            if (height > width)
            {
                ScreenHeight = width;
                ScreenWidth = height;
            }
            else
            {
                ScreenHeight = height;
                ScreenWidth = width;
            }

            ScreenDensity = 1 / density;
            // ScreenDensity = density;

            ScreenHeightX = ScreenHeight * ScreenDensity;
            ScreenWidthX = ScreenWidth * ScreenDensity;


            ScreenWidthAspect = (double)ScreenWidth / ScreenHeight;

            // normalized screensize

            if (ScreenWidthAspect > 1.78)
            {
                ScreenHeightX_UNIFORMED_TO_1_78 = ScreenHeightX;
                LeftPadding = 30;
                RightPadding = 15;
            }
            else
            {
                ScreenHeightX_UNIFORMED_TO_1_78 = ScreenWidthX / 1.78;
                LeftPadding = RightPadding = 10;                
            }
            ScreenWidthX_UNIFORMED_TO_1_78 = ScreenHeightX_UNIFORMED_TO_1_78 * 1.78;

            TopBottomPadding = 10;
            MainMargin = new Thickness(LeftPadding, TopBottomPadding, RightPadding, TopBottomPadding);
            MainMarginPortrait = new Thickness(TopBottomPadding, LeftPadding, TopBottomPadding, RightPadding);

            MicroTextSize = ScreenHeightX_UNIFORMED_TO_1_78 * 0.035;
            SmallTextSize = ScreenHeightX_UNIFORMED_TO_1_78 * 0.05;
            MediumTextSize = ScreenHeightX_UNIFORMED_TO_1_78 * 0.07;
            CloseBtnSize = ScreenHeightX_UNIFORMED_TO_1_78 * 0.15;

            BigFrameCornerRadius = (float) (ScreenHeightX_UNIFORMED_TO_1_78 * 0.10);
            MediumPadding = new Thickness(BigFrameCornerRadius * 0.25);
            BigFrameShadowTranslationX = ScreenHeightX_UNIFORMED_TO_1_78 * 0.02;
            BigFrameShadowTranslationY = ScreenHeightX_UNIFORMED_TO_1_78 * 0.02;

        // button sizes
        ButtonCornerRadius = (float)ScreenHeightX_UNIFORMED_TO_1_78 *0.03f;
        ButtonOutlineWidth = ScreenHeightX_UNIFORMED_TO_1_78 * 0.01;
        ButtonShadowTranslationY = ScreenHeightX_UNIFORMED_TO_1_78 * 0.01;
        ButtonShadowPadding = new Thickness(0,0,0, ButtonShadowTranslationY);



            Debug.WriteLine($"ScreenWidthX/ScreenHeightX = {ScreenWidthX}/{ScreenHeightX}, ScreenWidthAspect = {ScreenWidthAspect}, ScreenHeightX_UNIFORMED_TO_1_78 = {ScreenHeightX_UNIFORMED_TO_1_78}");
            ButtonHeight = ScreenWidthX_UNIFORMED_TO_1_78 * 0.067;

        }

    }
}
