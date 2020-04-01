using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DinoLingo.ScreenOrientations
{
    public static class ScreenOrientation
    {
        static IScreenOrientation _instance;
        public static IScreenOrientation Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = DependencyService.Get<IScreenOrientation>();
                }

                return _instance;
            }
        }
    }
}
