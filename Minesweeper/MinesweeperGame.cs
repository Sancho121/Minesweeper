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
        public List<(int y, int x)> locationBombs = new List<(int y, int x)>();
        Form1 mainForm;
        private bool isFirstOpenCell = true;

        public int CountBombs { get; private set; } = 10;
        public int CountFlags { get; private set; } = 0;

        public MinesweeperGame(int gameFieldInCells, int cellSize, Form1 form)
        {
            this.gameFieldInCells = gameFieldInCells;
            this.cellSize = cellSize;
            cells = new Cell[gameFieldInCells, gameFieldInCells];
            mainForm = form;
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
            isFirstOpenCell = true;
            CountBombs = 10;
            CountFlags = 0;
            foreach (var point in GetCellPoints())
            {
                cells[point.y, point.x] = new Cell();
                cells[point.y, point.x].cellState = CellState.ClosedCell;
            }
        }

        public void OpenCell(int y, int x)
        {
            if (isFirstOpenCell)
            {
                isFirstOpenCell = false;
                GenerateBombs(y, x);
                foreach (var point in GetCellPoints())
                {                    
                    CountBombsAroundCell(point.y, point.x);
                }
            }

            if (cells[y, x].cellState == CellState.ClosedCell)
            {
                cells[y, x].isVisitCell = true;
                cells[y, x].cellState = CellState.OpenCell;
            }

            if (cells[y, x].cellState == CellState.FlagCell)
                return;

            if (cells[y, x].cellState == CellState.OpenCell && cells[y, x].isPresenceBombInCell == true)
            {
                GameLose();
            }

            if (cells[y, x].cellState == CellState.OpenCell && cells[y, x].bombsAroundCell == 0)
            {
                OpenAreaAroundCell(y, x);
            }

            FindNumberClosedAndFlagCells(out int countClosedCells, out int countFlagCells);
            if (countClosedCells + countFlagCells == CountBombs)
            {
                GameWin();
            }
        }

        private void FindNumberClosedAndFlagCells(out int countClosedCells, out int countFlagCells)
        {
            var y = cells.Cast<Cell>();
            
            countClosedCells = y.Count(c => c.cellState == CellState.ClosedCell);

            countFlagCells = y.Count(c => c.cellState == CellState.FlagCell);
        }

        private void GameLose()
        {
            foreach (var location in locationBombs)
            {
                cells[location.y, location.x].cellState = CellState.OpenCell;
            }
            mainForm.Lose();
        }

        private void GameWin()
        {
            foreach (var location in locationBombs)
            {
                cells[location.y, location.x].cellState = CellState.FlagCell;
            }
            mainForm.Win();
        }

        private void OpenAreaAroundCell(int y, int x)
        {
            for (int column = y - 1; column < y + 2; column++)
            {
                for (int line = x - 1; line < x + 2; line++)
                {
                    if (CoordinatesOutsideGameField(column, line) || (column == y && line == x) || cells[column, line].cellState == CellState.FlagCell)
                        continue;

                    if (cells[column, line].cellState == CellState.OpenCell)
                        continue;
                  
                    if (cells[column, line].bombsAroundCell == 0 && cells[column, line].isVisitCell == false)
                    {
                        cells[column, line].isVisitCell = true;
                        cells[column, line].cellState = CellState.OpenCell;
                        OpenAreaAroundCell(column, line);
                    }
                    else 
                    {
                        cells[column, line].cellState = CellState.OpenCell;
                    }
                }
            }
        }

        private void CountBombsAroundCell(int y, int x)
        {
            for (int column = y - 1; column < y + 2; column++)
            {
                for (int line = x - 1; line < x + 2; line++)
                {
                    if (CoordinatesOutsideGameField(column, line) || (column == y && line == x))
                        continue;

                    if (cells[column, line].isPresenceBombInCell == true)
                    {
                        cells[y, x].bombsAroundCell++;
                    }
                }
            }
        }

        public bool CoordinatesOutsideGameField(int column, int line)
        {
            return line < 0 || column < 0 || column == gameFieldInCells || line == gameFieldInCells;
        }

        public void PutFlagInCell(int y, int x)
        {
            if (cells[y, x].cellState == CellState.ClosedCell)
            {
                CountFlags++;
                cells[y, x].cellState = CellState.FlagCell;

            }
            else if (cells[y, x].cellState == CellState.FlagCell)
            {
                CountFlags--;
                cells[y, x].cellState = CellState.ClosedCell;
            }
        }

        private void GenerateBombs(int y, int x)
        {
            locationBombs = GetCellPoints()
                .Where(z => z != (y, x))
                .OrderBy(z => random.NextDouble())
                .Take(CountBombs)
                .ToList();

            //locationBombs = new List<(int y, int x)> { (0, 0), (1, 1), (2, 2), (3, 3), (4, 4), (5, 5), (6, 6), (7, 7), (8, 8), (0, 1) };
            //locationBombs = new List<(int y, int x)> { (1, 1)};

            //locationBombs = new List<(int y, int x)> { (4, 0), (4, 1), (4, 2), (4, 3), (5, 3), (6, 3), (7, 3), (8, 3) };

            foreach (var location in locationBombs)
            {
                cells[location.y, location.x].isPresenceBombInCell = true;
            }
        }
    }
}
