using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DinoLingo
{
    public class LanguageList_ViewCell: ViewCell
    {
        public LanguageList_ViewCell()
        {
            MyViews.MyLabel langLabel = new MyViews.MyLabel()
            {
                Text = Translate.GetString("choose_language"),
                FontSize = UI_Sizes.MediumTextSize * 0.8,
                TextColor = MyColors.BlueTextLoginColor,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };


        }        
    }
}
