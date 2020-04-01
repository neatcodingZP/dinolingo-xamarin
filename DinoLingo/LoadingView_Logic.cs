using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DinoLingo
{

    static class LoadingView_Logic
    {
        static ImageSource[]DANCING_DINOS_GIF = new ImageSource[] { "terry_dancing.gif", "rexy_dancing.gif" };
        //ImageSource terryGIF = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.GIF.terry_dancing_c.gif");
        //ImageSource rexyGIF = Forms9Patch.ImageSource.FromResource("DinoLingo.Resources.GIF.rexy_dancing_c.gif");  

        static Random randomIndex = new Random();
        static Color[] BACKGROUND_COLORS = new Color[]
        {
            Color.White,
            Color.FromHex("#DCE25D"),
            Color.FromHex("#FFA3E4"),
            Color.FromHex("#AAF2AD"),
            Color.FromHex("#96B2F9"),
            Color.FromHex("#F59C9A"),
            Color.FromHex("#83F7F6"),
            Color.FromHex("#FFB79F"),
            Color.FromHex("#D19FFF"),
            Color.FromHex("#72D0CD"),
            Color.FromHex("#FFFE8F"),
        };

        public static ImageSource GetRandomDancingDinoImg()
        {
            return DANCING_DINOS_GIF[randomIndex.Next(DANCING_DINOS_GIF.Length)];
        }

        public static Color GetRandomColor()
        {
            return BACKGROUND_COLORS[randomIndex.Next(BACKGROUND_COLORS.Length)];
        }

        public static Task ShowCloseBtnTimer(View view)
        {
            int SHOW_CLOSE_BTN_DELAY = 3000;
            return Task.Run(async () => {
                await Task.Delay(SHOW_CLOSE_BTN_DELAY);
                Device.BeginInvokeOnMainThread(()=> {
                    if (view!=null)
                    {
                        view.IsEnabled = view.IsVisible = true;
                    }                    
                });                
            });
        }
    }
}
