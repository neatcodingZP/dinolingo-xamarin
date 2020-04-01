
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DinoLingo
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MyLabel : Label
	{
		public MyLabel ()
		{
			InitializeComponent ();
            FontAttributes = Translate.fontAttributes;
            FontFamily = Translate.fontFamily;
		}
	}
}