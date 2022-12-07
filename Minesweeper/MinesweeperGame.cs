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
        private bool hasBombsOnGameField = false;
        public event EventHandler Victory = delegate { };
        public event EventHandler Defeat = delegate { };

        public int BombCount { get; private set; } = 10;
        public int FlagCount { get; private set; } = 0;

        public MinesweeperGame(int gameFieldInCells, int cellSize)
        {
            this.gameFieldInCells = gameFieldInCells;
            this.CellSize = cellSize;
            Cells = new Cell[gameFieldInCells, gameFieldInCells];
        }

        public void Restart()
        {
            hasBombsOnGameField = false;
            FlagCount = 0;
            foreach (var point in GetAllCellPoints())
            {
                Cells[point.X, point.Y] = new Cell();
                Cells[point.X, point.Y].cellState = CellState.ClosedCell;
            }
        }

        public void OpenCell(int x, int y)
        {
            if (!hasBombsOnGameField)
            {
                GenerateBombs(excludedPosition: new MyPoint(x, y));
                hasBombsOnGameField = true;
            }
         
            if (Cells[x, y].cellState == CellState.ClosedCell)
            {
                Cells[x, y].cellState = CellState.OpenCell;
            }

            if (Cells[x, y].cellState == CellState.FlagCell)
                return;

            if (Cells[x, y].cellState == CellState.OpenCell && Cells[x, y].HasBomb == true)
            {
                ProcessDefeat();
                return;
            }

            if (Cells[x, y].cellState == CellState.OpenCell && Cells[x, y].BombsAroundCell == 0)
            {
                OpenAreaAroundCell(x, y);
            }

            if (CountClosedAndFlagCells() == BombCount)
            {
                ProcessVictory();
            }
        }

        public void PutFlagInCell(int x, int y)
        {
            switch (Cells[x, y].cellState)
            {
                case CellState.ClosedCell:
                    FlagCount++;
                    Cells[x, y].cellState = CellState.FlagCell;
                    break;
                case CellState.FlagCell:
                    FlagCount--;
                    Cells[x, y].cellState = CellState.ClosedCell;
                    break;
            }
        }

        public void SmartOpenCell(int x, int y)
        {
            if (Cells[x, y].cellState == CellState.OpenCell &&
                Cells[x, y].BombsAroundCell == GetPointsAroundCell(x, y).Count(k => Cells[k.X, k.Y].cellState == CellState.FlagCell))
            {
                foreach (var point in GetPointsAroundCell(x, y))
                {
                    if (Cells[point.X, point.Y].HasBomb == true && Cells[point.X, point.Y].cellState != CellState.FlagCell)
                    {
                        ProcessDefeat();
                        return;
                    }

                    if (Cells[point.X, point.Y].cellState == CellState.ClosedCell && Cells[point.X, point.Y].HasBomb == false)
                    {
                        Cells[point.X, point.Y].cellState = CellState.OpenCell;

                        if (CountClosedAndFlagCells() == BombCount)
                        {
                            ProcessVictory();
                            return;
                        }

                        if (Cells[point.X, point.Y].BombsAroundCell == 0)
                        {
                            OpenAreaAroundCell(point.X, point.Y);
                        }
                    }
                }
            }
        }

        private void OpenAreaAroundCell(int x, int y)
        {
            foreach (var point in GetPointsAroundCell(x, y))
            {
                if (Cells[point.X, point.Y].cellState == CellState.OpenCell ||
                    Cells[point.X, point.Y].cellState == CellState.FlagCell)
                    continue;

                if (Cells[point.X, point.Y].BombsAroundCell == 0 && Cells[point.X, point.Y].cellState == CellState.ClosedCell)
                {
                    Cells[point.X, point.Y].cellState = CellState.OpenCell;
                    OpenAreaAroundCell(point.X, point.Y);
                }
                else
                {
                    Cells[point.X, point.Y].cellState = CellState.OpenCell;
                }
            }
        }

        public void Draw(Graphics graphics)
        {
            foreach (var point in GetAllCellPoints())
            {
                Cells[point.X, point.Y].DrawCell(graphics, point.X, point.Y, CellSize);
            }

            for (int i = 0; i <= gameFieldInCells; i++)
            {
                graphics.DrawLine(Pens.Black, 0, i * CellSize, gameFieldInCells * CellSize, i * CellSize);
                graphics.DrawLine(Pens.Black, i * CellSize, 0, i * CellSize, gameFieldInCells * CellSize);
            }
        }

        public bool IsCoordinatesOutsideGameField(int x, int y)
        {
            return x < 0 || y < 0 || y >= gameFieldInCells || x >= gameFieldInCells;
        }

        private void ProcessDefeat()
        {
            foreach (var position in GetAllCellPoints())
            {
                if (Cells[position.X, position.Y].HasBomb == true)
                {
                    Cells[position.X, position.Y].cellState = CellState.OpenCell;
                }
            }
            this.Defeat(this, EventArgs.Empty);
        }

        private void ProcessVictory()
        {
            foreach (var position in GetAllCellPoints())
            {
                if (Cells[position.X, position.Y].HasBomb == true)
                {
                    Cells[position.X, position.Y].cellState = CellState.FlagCell;
                }
            }
            this.Victory(this, EventArgs.Empty);
        }

        private void GenerateBombs(MyPoint excludedPosition)
        {
            var bombPositions = GetAllCellPoints()
                .Where(z => z.X != excludedPosition.X || z.Y != excludedPosition.Y)
                .OrderBy(z => random.NextDouble())
                .Take(BombCount);

            foreach (var position in bombPositions)
            {
                Cells[position.X, position.Y].HasBomb = true;
                CountBombsAroundCell(position.X, position.Y);
            }
        }

        private void CountBombsAroundCell(int x, int y)
        {
            foreach (var point in GetPointsAroundCell(x, y))
            {
                Cells[point.X, point.Y].BombsAroundCell++;
            }
        }

        private int CountClosedAndFlagCells()
        {
            return this.Cells
                .Cast<Cell>()
                .Count(c => c.cellState == CellState.ClosedCell ||
                            c.cellState == CellState.FlagCell);
        }

        private IEnumerable<MyPoint> GetAllCellPoints()
        {
            for (int x = 0; x < gameFieldInCells; x++)
            {
                for (int y = 0; y < gameFieldInCells; y++)
                {
                    yield return new MyPoint(x, y);
                }
            }
        }

        private IEnumerable<MyPoint> GetPointsAroundCell(int x, int y)
        {
            for (int column = x - 1; column < x + 2; column++)
            {
                for (int line = y - 1; line < y + 2; line++)
                {
                    if (IsCoordinatesOutsideGameField(column, line) || (column == x && line == y))
                        continue;

                    yield return new MyPoint(column, line);
                }
            }
        }           
    }
}
