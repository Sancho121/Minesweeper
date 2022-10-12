using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Minesweeper
{
    class Cell
    {
        public CellState cellState;

        private Image imageFlag = Properties.Resources.flag;

        public void DrawCells(Graphics graphics, int x, int y, int cellSize)
        {
            if (cellState == CellState.ClosedCell)
            {
                graphics.FillRectangle(Brushes.Aqua, x * cellSize, y * cellSize, cellSize, cellSize);
            }
            if (cellState == CellState.FlagCell)
            {
                graphics.DrawImage(imageFlag, x * cellSize, y * cellSize);
            }
        }
    }
}
