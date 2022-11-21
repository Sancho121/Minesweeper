using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Text;

namespace Minesweeper
{
    class Cell
    {
        public CellState cellState;

        private Image imageFlag = Properties.Resources.flag;
        private Bitmap imageBomb = Properties.Resources.bomb;
        private Font font = new Font(FontFamily.GenericSansSerif, 18);
        public bool isPresenceBombInCell;
        public int bombsAroundCell;
        public bool isVisitCell;

        private Dictionary<int, Brush> brush = new Dictionary<int, Brush>()
        {
            {1, Brushes.Blue },
            {2, Brushes.Green },
            {3, Brushes.Red } ,
            {4, Brushes.DarkRed },
            {5, Brushes.DarkBlue },
            {6, Brushes.DarkRed },
            {7, Brushes.DarkRed },
            {8, Brushes.DarkRed },
        };

        public void DrawCells(Graphics graphics, int x, int y, int cellSize)
        {
            if (bombsAroundCell > 0)
            {                
                graphics.DrawString($"{bombsAroundCell}", font, brush[bombsAroundCell], x * cellSize, y * cellSize);
            }
            if (cellState == CellState.ClosedCell)
            {
                graphics.FillRectangle(Brushes.Aqua, x * cellSize, y * cellSize, cellSize, cellSize);
            }
            if (cellState == CellState.FlagCell)
            {
                graphics.DrawImage(imageFlag, x * cellSize, y * cellSize);
            }
            if (cellState == CellState.OpenCell && isPresenceBombInCell == true)
            {
                graphics.DrawImage(imageBomb, x * cellSize, y * cellSize);
            }
        }
    }
}
