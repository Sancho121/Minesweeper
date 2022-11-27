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
        public readonly int CellSize;
        public Cell[,] Cells;
        private Random random = new Random();
        private MyPoint[] locationBombs;
        private bool isFirstOpenCell = true;
        public event EventHandler Victory = delegate { };
        public event EventHandler Defeat = delegate { };

        public int CountBombs { get; private set; } = 10;
        public int CountFlags { get; private set; } = 0;

        public MinesweeperGame(int gameFieldInCells, int cellSize)
        {
            this.gameFieldInCells = gameFieldInCells;
            this.CellSize = cellSize;
            Cells = new Cell[gameFieldInCells, gameFieldInCells];
            locationBombs = new MyPoint[] { };
        }

        public void Draw(Graphics graphics)
        {
            foreach (var point in GetCellPoints())
            {
                Cells[point.Y, point.X].DrawCell(graphics, point.X, point.Y, CellSize);
            }

            for (int i = 0; i <= gameFieldInCells; i++)
            {
                graphics.DrawLine(Pens.Black, 0, i * CellSize, gameFieldInCells * CellSize, i * CellSize);
                graphics.DrawLine(Pens.Black, i * CellSize, 0, i * CellSize, gameFieldInCells * CellSize);
            }
        }

        private List<MyPoint> GetCellPoints()
        {
            List<MyPoint> cellPoints = new List<MyPoint>();

            for (int y = 0; y < gameFieldInCells; y++)
            {
                for (int x = 0; x < gameFieldInCells; x++)
                {
                    cellPoints.Add(new MyPoint(y, x));                    
                }
            }
            return cellPoints;
        }

        public void Restart()
        {
            isFirstOpenCell = true;
            CountFlags = 0;
            foreach (var point in GetCellPoints())
            {
                Cells[point.Y, point.X] = new Cell();
                Cells[point.Y, point.X].cellState = CellState.ClosedCell;
            }
        }

        public void OpenCell(int y, int x)
        {
            if (isFirstOpenCell)
            {
                isFirstOpenCell = false;
                GenerateBombs(y, x);
                foreach (var point in locationBombs)
                {                    
                    CountBombsAroundCell(point.Y, point.X);
                }
            }

            if (Cells[y, x].cellState == CellState.ClosedCell)
            {
                Cells[y, x].IsVisitCell = true;
                Cells[y, x].cellState = CellState.OpenCell;
            }

            if (Cells[y, x].cellState == CellState.FlagCell)
                return;

            if (Cells[y, x].cellState == CellState.OpenCell && Cells[y, x].IsPresenceBombInCell == true)
            {
                GameLose();
            }

            if (Cells[y, x].cellState == CellState.OpenCell && Cells[y, x].BombsAroundCell == 0)
            {
                OpenAreaAroundCell(y, x);
            }
          
            if (CountClosedAndFlagCells() == CountBombs)
            {
                GameWin();
            }
        }

        private int CountClosedAndFlagCells()
        {
            return this.Cells
                .Cast<Cell>()
                .Count(c => c.cellState == CellState.ClosedCell ||
                            c.cellState == CellState.FlagCell);
        }

        private void GameLose()
        {
            foreach (var location in locationBombs)
            {
                Cells[location.Y, location.X].cellState = CellState.OpenCell;
            }
            this.Defeat(this, EventArgs.Empty);
        }

        private void GameWin()
        {
            foreach (var location in locationBombs)
            {
                Cells[location.Y, location.X].cellState = CellState.FlagCell;
            }
            this.Victory(this, EventArgs.Empty);
        }

        private void OpenAreaAroundCell(int y, int x)
        {
            for (int line = y - 1; line < y + 2; line++)
            {
                for (int column = x - 1; column < x + 2; column++)
                {
                    if (CoordinatesOutsideGameField(line, column) || (line == y && column == x) || Cells[line, column].cellState == CellState.FlagCell)
                        continue;

                    if (Cells[line, column].cellState == CellState.OpenCell)
                        continue;
                  
                    if (Cells[line, column].BombsAroundCell == 0 && Cells[line, column].IsVisitCell == false)
                    {
                        Cells[line, column].IsVisitCell = true;
                        Cells[line, column].cellState = CellState.OpenCell;
                        OpenAreaAroundCell(line, column);
                    }
                    else 
                    {
                        Cells[line, column].cellState = CellState.OpenCell;
                    }
                }
            }
        }

        private void CountBombsAroundCell(int y, int x)
        {
            for (int line = y - 1; line < y + 2; line++)
            {
                for (int column = x - 1; column < x + 2; column++)
                {
                    if (CoordinatesOutsideGameField(line, column) || (line == y && column == x))
                        continue;

                    Cells[line, column].BombsAroundCell++;
                }
            }
        }

        public bool CoordinatesOutsideGameField(int line, int column)
        {
            return column < 0 || line < 0 || line == gameFieldInCells || column == gameFieldInCells;
        }

        public void PutFlagInCell(int y, int x)
        {
            switch (Cells[y, x].cellState)
            {
                case CellState.ClosedCell:
                    CountFlags++;
                    Cells[y, x].cellState = CellState.FlagCell;

                    break;
                case CellState.FlagCell:
                    CountFlags--;
                    Cells[y, x].cellState = CellState.ClosedCell;
                    break;
            }
        }

        private void GenerateBombs(int y, int x)
        {
            locationBombs = GetCellPoints()
                .Where(z => z.Y != y || z.X != x)
                .OrderBy(z => random.NextDouble())
                .Take(CountBombs)
                .ToArray();

            foreach (var location in locationBombs)
            {
                Cells[location.Y, location.X].IsPresenceBombInCell = true;
            }
        }
    }
}
