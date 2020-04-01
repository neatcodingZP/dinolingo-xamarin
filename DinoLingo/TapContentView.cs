﻿using System;
using Xamarin.Forms;

namespace DinoLingo
{
    public class TapContentView: ContentView
    {
        #region events
        // --------------------------------------------------------------------------
        //
        // EVENTS
        //
        // --------------------------------------------------------------------------
        public event EventHandler<PointEventArgs> TapEvent;
        public void OnTapEvent(int x, int y)
        {
            var method = TapEvent;
            if (method != null)
            {
                method(this, new PointEventArgs(x, y));
            }
        }
        #endregion
    }
}
