using System;
using System.Collections.Generic;
using System.Text;

namespace Minesweeper
{
    class MyPoint
    {
        public int Y { get; set; }
        public int X { get; set; }

        public MyPoint(int Y, int X)
        {
            this.Y = Y;
            this.X = X;
        }
    }
}
