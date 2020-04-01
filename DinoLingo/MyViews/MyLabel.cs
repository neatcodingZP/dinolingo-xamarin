using Xamarin.Forms;

/*
 *** Label with custom font *** 
 */
namespace DinoLingo.MyViews
{
    class MyLabel: Label
    {
        public MyLabel()
        {            
            FontAttributes = Translate.fontAttributes;
            FontFamily = Translate.fontFamily;
        }
    }
}
