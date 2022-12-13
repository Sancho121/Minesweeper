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
            foreach (MyPoint point in GetAllCellPoints())
            {
                Cells[point.X, point.Y] = new Cell();
                Cells[point.X, point.Y].CellState = CellState.ClosedCell;
            }
        }

        public void OpenCell(int x, int y)
        {
            Cell cell = Cells[x, y];

            if (cell.CellState == CellState.OpenCell || 
                cell.CellState == CellState.FlagCell)
                return;

            if (!hasBombsOnGameField)
            {
                GenerateBombs(excludedPosition: new MyPoint(x, y));
                hasBombsOnGameField = true;
            }

            cell.CellState = CellState.OpenCell;

            if (cell.HasBomb)
            {
                ProcessDefeat();
                return;
            }

            if (Cells[x, y].BombsAroundCell == 0)
                OpenAreaAroundCell(x, y);

            if (CountClosedAndFlagCells() == BombCount)
                ProcessVictory();           
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
            Cell openedCell = Cells[x, y];

            if (openedCell.CellState == CellState.ClosedCell ||
                openedCell.CellState == CellState.FlagCell)
                return;

            MyPoint[] adjacentPoints = GetPointsAroundCell(x, y).ToArray();

            int flagsAroundCell = adjacentPoints.Count(point => Cells[point.X, point.Y].CellState == CellState.FlagCell);

            if (flagsAroundCell == openedCell.BombsAroundCell)
            {
                foreach (MyPoint point in adjacentPoints)
                {
                    Cell cell = Cells[point.X, point.Y];
                    
                    if (cell.CellState == CellState.ClosedCell)
                    {
                        cell.CellState = CellState.OpenCell;

                        if (cell.HasBomb)
                        {
                            ProcessDefeat();
                            return;
                        }

                        if (cell.BombsAroundCell == 0)
                            OpenAreaAroundCell(point.X, point.Y);
                    }
                }

                if (CountClosedAndFlagCells() == BombCount)
                    ProcessVictory();
            }
        }

        private void OpenAreaAroundCell(int x, int y)
        {
            foreach (MyPoint point in GetPointsAroundCell(x, y))
            {
                Cell cell = Cells[point.X, point.Y];

                if (cell.CellState == CellState.ClosedCell)
                {
                    cell.CellState = CellState.OpenCell;

                    if (cell.BombsAroundCell == 0)
                        OpenAreaAroundCell(point.X, point.Y);
                }
            }
        }

        public void Draw(Graphics graphics)
        {
            foreach (MyPoint point in GetAllCellPoints())
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
            return x < 0 || 
                   y < 0 || 
                   y >= gameFieldInCells || 
                   x >= gameFieldInCells;
        }

        private void ProcessDefeat()
        {
            foreach (MyPoint point in GetAllCellPoints())
            {
                if (Cells[point.X, point.Y].HasBomb == true)
                {
                    Cells[point.X, point.Y].CellState = CellState.OpenCell;
                }
            }
            this.Defeat(this, EventArgs.Empty);
        }

        private void ProcessVictory()
        {
            foreach (MyPoint point in GetAllCellPoints())
            {
                if (Cells[point.X, point.Y].HasBomb == true)
                {
                    Cells[point.X, point.Y].CellState = CellState.FlagCell;
                }
            }
            this.Victory(this, EventArgs.Empty);
        }

        private void GenerateBombs(MyPoint excludedPosition)
        {
            var bombPositions = GetAllCellPoints()
                .Where(point => point.X != excludedPosition.X || 
                                point.Y != excludedPosition.Y)
                .OrderBy(z => Guid.NewGuid())
                .Take(BombCount);

            foreach (MyPoint position in bombPositions)
            {
                Cells[position.X, position.Y].HasBomb = true;
                CountBombsAroundCell(position.X, position.Y);
            }
        }

        private void CountBombsAroundCell(int x, int y)
        {
            foreach (MyPoint point in GetPointsAroundCell(x, y))
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
