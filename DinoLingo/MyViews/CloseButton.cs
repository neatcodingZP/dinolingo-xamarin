using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DinoLingo.MyViews
{
    public class CloseButton: Image
    {
        public static string CLASS_ID = "CloseBtn";
        public CloseButton()
        {
            ClassId = CLASS_ID;
            HorizontalOptions = LayoutOptions.End;
            VerticalOptions = LayoutOptions.Start;
            WidthRequest = UI_Sizes.CloseBtnSize;
            HeightRequest = UI_Sizes.CloseBtnSize;
            Source = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.COMMON.btn_close.png");
            Aspect = Aspect.AspectFit;
        }
    }
}
