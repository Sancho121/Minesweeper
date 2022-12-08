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
                Cells[point.X, point.Y].CellState = CellState.ClosedCell;
            }
        }

        public void OpenCell(int x, int y)
        {
            if (!hasBombsOnGameField)
            {
                GenerateBombs(excludedPosition: new MyPoint(x, y));
                hasBombsOnGameField = true;
            }

            switch (this.Cells[x, y].CellState)
            {
                case CellState.ClosedCell:
                    Cells[x, y].CellState = CellState.OpenCell;
                    goto case CellState.OpenCell;
                case CellState.OpenCell:
                    if (Cells[x, y].HasBomb == true)
                    {
                        ProcessDefeat();
                        return;
                    }
                    else
                    {
                        if (Cells[x, y].BombsAroundCell == 0)
                        {
                            OpenAreaAroundCell(x, y);
                        }
                    }
                    break;
                case CellState.FlagCell:
                    return;
            }

            if (CountClosedAndFlagCells() == BombCount)
            {
                ProcessVictory();
            }
        }

        public void PutFlagInCell(int x, int y)
        {
            switch (Cells[x, y].CellState)
            {
                case CellState.ClosedCell:
                    FlagCount++;
                    Cells[x, y].CellState = CellState.FlagCell;
                    break;
                case CellState.FlagCell:
                    FlagCount--;
                    Cells[x, y].CellState = CellState.ClosedCell;
                    break;
            }
        }

        public void SmartOpenCell(int x, int y)
        {
            if (Cells[x, y].CellState == CellState.OpenCell &&
                Cells[x, y].BombsAroundCell == GetPointsAroundCell(x, y).Count(k => Cells[k.X, k.Y].CellState == CellState.FlagCell))
            {
                foreach (var point in GetPointsAroundCell(x, y))
                {
                    switch (this.Cells[point.X, point.Y].HasBomb)
                    {
                        case true when Cells[point.X, point.Y].CellState != CellState.FlagCell:
                            ProcessDefeat();
                            return;
                        case false when Cells[point.X, point.Y].CellState == CellState.ClosedCell:
                            Cells[point.X, point.Y].CellState = CellState.OpenCell;
                            if (Cells[point.X, point.Y].BombsAroundCell == 0)
                            {
                                OpenAreaAroundCell(point.X, point.Y);
                            }
                            break;
                    }
                }

                if (CountClosedAndFlagCells() == BombCount)
                {
                    ProcessVictory();
                }
            }
        }

        private void OpenAreaAroundCell(int x, int y)
        {
            foreach (var point in GetPointsAroundCell(x, y))
            {
                switch (this.Cells[point.X, point.Y].CellState)
                {
                    case CellState.OpenCell:
                        continue;
                    case CellState.FlagCell:
                        continue;
                    case CellState.ClosedCell when Cells[point.X, point.Y].BombsAroundCell == 0:
                        Cells[point.X, point.Y].CellState = CellState.OpenCell;
                        OpenAreaAroundCell(point.X, point.Y);
                        break;
                    default:
                        Cells[point.X, point.Y].CellState = CellState.OpenCell;
                        break;
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
                    Cells[position.X, position.Y].CellState = CellState.OpenCell;
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
                    Cells[position.X, position.Y].CellState = CellState.FlagCell;
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
                .Count(c => c.CellState == CellState.ClosedCell ||
                            c.CellState == CellState.FlagCell);
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
