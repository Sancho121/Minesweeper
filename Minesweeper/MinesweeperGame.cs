using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Minesweeper
{
    class MinesweeperGame
    {
        private readonly int gameFieldInCells;
        public readonly int cellSize;
        public Cell[,] cells;
        private Random random = new Random();

        public int CountBombs { get; private set; } = 10;

        public MinesweeperGame(int gameFieldInCells, int cellSize)
        {
            this.gameFieldInCells = gameFieldInCells;
            this.cellSize = cellSize;
            cells = new Cell[gameFieldInCells, gameFieldInCells];
        }

        public void Draw(Graphics graphics)
        {
            foreach (var point in GetCellPoints())
            {
                cells[point.y, point.x].DrawCells(graphics, point.x, point.y, cellSize);
            }

            for (int i = 0; i <= gameFieldInCells; i++)
            {
                graphics.DrawLine(Pens.Black, 0, i * cellSize, gameFieldInCells * cellSize, i * cellSize);
                graphics.DrawLine(Pens.Black, i * cellSize, 0, i * cellSize, gameFieldInCells * cellSize);
            }
        }

        private List<(int y, int x)> GetCellPoints()
        {
            List<(int y, int x)> cellPoints = new List<(int y, int x)>();

            for (int y = 0; y < gameFieldInCells; y++)
            {
                for (int x = 0; x < gameFieldInCells; x++)
                {
                    cellPoints.Add((y, x));
                }
            }
            return cellPoints;
        }

        public void Restart()
        {
            CountBombs = 10;
            foreach (var point in GetCellPoints())
            {
                cells[point.y, point.x] = new Cell();
                cells[point.y, point.x].cellState = CellState.ClosedCell;
            }
        }

        public void OpenCell(int x, int y)
        {
            if (cells[y, x].cellState == CellState.ClosedCell)
            {
                cells[y, x].cellState = CellState.OpenCell;
            }

            if (cells[y, x].cellState == CellState.FlagCell)
                return;
        }

        public void PutFlagInCell(int x, int y)
        {
            if (cells[y, x].cellState == CellState.ClosedCell)
            {
                CountBombs--;
                cells[y, x].cellState = CellState.FlagCell;

            }
            else if (cells[y, x].cellState == CellState.FlagCell)
            {
                CountBombs++;
                cells[y, x].cellState = CellState.ClosedCell;
            }
        }

        public void GenerateBombs()
        {
            foreach (var point in GetCellPoints()
                .OrderBy(x => random.NextDouble())
                .Take(CountBombs))
            {
                cells[point.y, point.x].isPresenceBombInCell = true;
            }
        }
    }
}
