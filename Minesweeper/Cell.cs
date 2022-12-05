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

        private static Image flagImage = Properties.Resources.flag;
        private static Image bombImage = Properties.Resources.bomb;
        private static Font font = new Font(FontFamily.GenericSansSerif, 18);
        public bool HasBomb;
        public int BombsAroundCell;

        private static Dictionary<int, Brush> brushByBombCount = new Dictionary<int, Brush>()
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

        public void DrawCell(Graphics graphics, int x, int y, int cellSize)
        {
            switch (this.cellState)
            {
                case CellState.ClosedCell:
                    graphics.FillRectangle(Brushes.Aqua, x * cellSize, y * cellSize, cellSize, cellSize);
                    break;
                case CellState.FlagCell:
                    graphics.DrawImage(flagImage, x * cellSize, y * cellSize);
                    break;
                case CellState.OpenCell:
                    if (this.HasBomb)
                    {
                        graphics.DrawImage(bombImage, x * cellSize, y * cellSize);
                    }
                    else
                    {
                        if(this.BombsAroundCell > 0)
                        {
                            graphics.DrawString(BombsAroundCell.ToString(), font, brushByBombCount[BombsAroundCell], x * cellSize + 4, y * cellSize);
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();                      
            }
        }
    }
}
