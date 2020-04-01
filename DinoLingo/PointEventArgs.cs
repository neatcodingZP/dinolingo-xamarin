using System;
namespace DinoLingo
{
    public class PointEventArgs : EventArgs
    {
        #region properties & fields
        // --------------------------------------------------------------------------
        // 
        // PROPERTIES & FIELDS
        //
        // --------------------------------------------------------------------------
        public int X { get; set; }
        public int Y { get; set; }
        #endregion

        #region constructors
        // --------------------------------------------------------------------------
        // 
        // CONSTRUCTORS
        //
        // --------------------------------------------------------------------------
        public PointEventArgs(int x, int y)
        {
            X = x;
            Y = y;
        }
        #endregion
    }
}
