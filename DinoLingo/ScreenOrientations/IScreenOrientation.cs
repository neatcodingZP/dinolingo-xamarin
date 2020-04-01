using System;
using System.Collections.Generic;
using System.Text;

namespace DinoLingo.ScreenOrientations
{
    public interface IScreenOrientation
    {
        void ForceLandscape();
        void ForcePortrait();        
    }
}
