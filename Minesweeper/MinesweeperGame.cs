using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Minesweeper
{
    class MinesweeperGame
    {
        private readonly int gameFieldHeightInCells;
        private readonly int gameFieldWidthInCells;
        private readonly int cellSize;

        public MinesweeperGame(int gameFieldHeightInCells, int gameFieldWidthInCells, int cellSize)
        {
            this.gameFieldHeightInCells = gameFieldHeightInCells;
            this.gameFieldWidthInCells = gameFieldWidthInCells;
            this.cellSize = cellSize;
        }

        public void Draw(Graphics graphics)
        {
            for (int y = 0; y <= gameFieldHeightInCells; y++)
            {
                graphics.DrawLine(Pens.Black, 0, y * cellSize, gameFieldWidthInCells * cellSize, y * cellSize);
            }

            for (int x = 0; x <= gameFieldWidthInCells; x++)
            {
                graphics.DrawLine(Pens.Black, x * cellSize, 0, x * cellSize, gameFieldHeightInCells * cellSize);
            }
        }
    }
}
