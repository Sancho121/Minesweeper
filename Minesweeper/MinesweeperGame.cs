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
        private MyPoint[] bombPositions;
        private bool isFirstCellOpen = true;
        public event EventHandler Victory = delegate { };
        public event EventHandler Defeat = delegate { };

        public int BombCount { get; private set; } = 10;
        public int FlagCount { get; private set; } = 0;

        public MinesweeperGame(int gameFieldInCells, int cellSize)
        {
            this.gameFieldInCells = gameFieldInCells;
            this.CellSize = cellSize;
            Cells = new Cell[gameFieldInCells, gameFieldInCells];
            bombPositions = new MyPoint[] { };
        }

        public void Draw(Graphics graphics)
        {
            foreach (var point in GetAllCellPoints())
            {
                Cells[point.Y, point.X].DrawCell(graphics, point.X, point.Y, CellSize);
            }

            for (int i = 0; i <= gameFieldInCells; i++)
            {
                graphics.DrawLine(Pens.Black, 0, i * CellSize, gameFieldInCells * CellSize, i * CellSize);
                graphics.DrawLine(Pens.Black, i * CellSize, 0, i * CellSize, gameFieldInCells * CellSize);
            }
        }

        private IEnumerable<MyPoint> GetAllCellPoints()
        {
            for (int y = 0; y < gameFieldInCells; y++)
            {
                for (int x = 0; x < gameFieldInCells; x++)
                {
                    yield return new MyPoint(y, x);
                }
            }
        }

        private IEnumerable<MyPoint> GetPointsAroundCell(int y, int x)
        {
            for (int line = y - 1; line < y + 2; line++)
            {
                for (int column = x - 1; column < x + 2; column++)
                {
                    if (IsCoordinatesOutsideGameField(line, column) || (line == y && column == x))
                        continue;

                    yield return new MyPoint(line, column);
                }
            }
        }

        public void Restart()
        {
            isFirstCellOpen = true;
            FlagCount = 0;
            foreach (var point in GetAllCellPoints())
            {
                Cells[point.Y, point.X] = new Cell();
                Cells[point.Y, point.X].cellState = CellState.ClosedCell;
            }
        }

        public void OpenCell(int y, int x)
        {
            if (isFirstCellOpen)
            {
                isFirstCellOpen = false;
                GenerateBombs(excludedPosition: new MyPoint(y, x));
            }

            if (Cells[y, x].cellState == CellState.ClosedCell)
            {
                Cells[y, x].cellState = CellState.OpenCell;
            }

            if (Cells[y, x].cellState == CellState.FlagCell)
                return;

            if (Cells[y, x].cellState == CellState.OpenCell && Cells[y, x].HasBomb == true)
            {
                ProcessDefeat();
                return;
            }

            if (Cells[y, x].cellState == CellState.OpenCell && Cells[y, x].BombsAroundCell == 0)
            {
                OpenAreaAroundCell(y, x);
            }

            if (CountClosedAndFlagCells() == BombCount)
            {
                ProcessVictory();
            }
        }

        public void SmartOpenCell(int y, int x)
        {
            if (Cells[y, x].cellState == CellState.OpenCell &&
                Cells[y, x].BombsAroundCell == GetPointsAroundCell(y, x).Count(k => Cells[k.Y, k.X].cellState == CellState.FlagCell))
            {
                foreach (var point in GetPointsAroundCell(y, x))
                {
                    if (Cells[point.Y, point.X].HasBomb == true && Cells[point.Y, point.X].cellState != CellState.FlagCell)
                    {
                        ProcessDefeat();
                        return;
                    }

                    if (Cells[point.Y, point.X].cellState == CellState.ClosedCell && Cells[point.Y, point.X].HasBomb == false)
                    {
                        Cells[point.Y, point.X].cellState = CellState.OpenCell;

                        if (Cells[point.Y, point.X].BombsAroundCell == 0)
                        {
                            OpenAreaAroundCell(point.Y, point.X);
                        }
                    }
                }
            }
        }

        private int CountClosedAndFlagCells()
        {
            return this.Cells
                .Cast<Cell>()
                .Count(c => c.cellState == CellState.ClosedCell ||
                            c.cellState == CellState.FlagCell);
        }

        private void ProcessDefeat()
        {
            foreach (var position in bombPositions)
            {
                Cells[position.Y, position.X].cellState = CellState.OpenCell;
            }
            this.Defeat(this, EventArgs.Empty);
        }

        private void ProcessVictory()
        {
            foreach (var position in bombPositions)
            {
                Cells[position.Y, position.X].cellState = CellState.FlagCell;
            }
            this.Victory(this, EventArgs.Empty);
        }

        private void OpenAreaAroundCell(int y, int x)
        {
            foreach (var point in GetPointsAroundCell(y, x))
            {
                if (Cells[point.Y, point.X].cellState == CellState.OpenCell ||
                    Cells[point.Y, point.X].cellState == CellState.FlagCell)
                    continue;

                if (Cells[point.Y, point.X].BombsAroundCell == 0 && Cells[point.Y, point.X].cellState == CellState.ClosedCell)
                {
                    Cells[point.Y, point.X].cellState = CellState.OpenCell;
                    OpenAreaAroundCell(point.Y, point.X);
                }
                else
                {
                    Cells[point.Y, point.X].cellState = CellState.OpenCell;
                }
            }
        }

        private void GenerateBombs(MyPoint excludedPosition)
        {
            bombPositions = GetAllCellPoints()
                .Where(z => z.Y != excludedPosition.Y || z.X != excludedPosition.X)
                .OrderBy(z => random.NextDouble())
                .Take(BombCount)
                .ToArray();

            foreach (var position in bombPositions)
            {
                Cells[position.Y, position.X].HasBomb = true;
                CountBombsAroundCell(position.Y, position.X);
            }
        }

        private void CountBombsAroundCell(int y, int x)
        {
            foreach (var point in GetPointsAroundCell(y, x))
            {
                Cells[point.Y, point.X].BombsAroundCell++;
            }
        }

        public bool IsCoordinatesOutsideGameField(int y, int x)
        {
            return x < 0 || y < 0 || y >= gameFieldInCells || x >= gameFieldInCells;
        }

        public void PutFlagInCell(int y, int x)
        {
            switch (Cells[y, x].cellState)
            {
                case CellState.ClosedCell:
                    FlagCount++;
                    Cells[y, x].cellState = CellState.FlagCell;

                    break;
                case CellState.FlagCell:
                    FlagCount--;
                    Cells[y, x].cellState = CellState.ClosedCell;
                    break;
            }
        }       
    }
}
