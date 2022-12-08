using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    partial class Form1 : Form
    {
        private TimeSpan elapsedTime = TimeSpan.Zero;
        private MinesweeperGame minesweeperGame = new MinesweeperGame(9, 30);
        private Point pointHighlightedCell = new Point(-100, -100);
        private Rectangle highlightedCell;
        private bool isPressedLeftMouseButton;
        private bool isPressedRightMouseButton;

        public Form1()
        {
            InitializeComponent();
            minesweeperGame.Victory += OnVictory;
            minesweeperGame.Defeat += OnDefeat;
            minesweeperGame.Restart();
            bombCountLabel.Text = $"Мин: {minesweeperGame.BombCount}";
            elapsedTimeLabel.Text = elapsedTime.ToString();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int cellX = e.Location.X / minesweeperGame.CellSize;
            int cellY = e.Location.Y / minesweeperGame.CellSize;

            if (pointHighlightedCell.X != cellX ||
                pointHighlightedCell.Y != cellY)
            {
                pointHighlightedCell.X = cellX;
                pointHighlightedCell.Y = cellY;

                highlightedCell = new Rectangle(
                pointHighlightedCell.X * minesweeperGame.CellSize,
                pointHighlightedCell.Y * minesweeperGame.CellSize,
                minesweeperGame.CellSize,
                minesweeperGame.CellSize
                );

                pictureBox1.Refresh();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            int cellX = e.Location.X / minesweeperGame.CellSize;
            int cellY = e.Location.Y / minesweeperGame.CellSize;

            if (minesweeperGame.IsCoordinatesOutsideGameField(cellX, cellY))
                return;

            timer1.Start();

            if (e.Button == MouseButtons.Left)
            {
                minesweeperGame.OpenCell(cellX, cellY);
            }

            if (e.Button == MouseButtons.Right)
            {
                minesweeperGame.PutFlagInCell(cellX, cellY);

                if (minesweeperGame.FlagCount <= minesweeperGame.BombCount)
                {
                    bombCountLabel.Text = $"Мин: {minesweeperGame.BombCount - minesweeperGame.FlagCount}";
                }
                else
                {
                    bombCountLabel.Text = $"Мин: {0}";
                }               
            }

            if (e.Button == MouseButtons.Middle ||
               (isPressedRightMouseButton == true && e.Button == MouseButtons.Left) ||
               (isPressedLeftMouseButton == true && e.Button == MouseButtons.Right))
            {
                minesweeperGame.SmartOpenCell(cellX, cellY);
            }

            isPressedRightMouseButton = false;
            isPressedLeftMouseButton = false;
            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isPressedLeftMouseButton = true;
            }
            if (e.Button == MouseButtons.Right)
            {
                isPressedRightMouseButton = true;
            }
        }

        private void restartButton_Click(object sender, EventArgs e)
        {          
            minesweeperGame.Restart();
            timer1.Stop();
            elapsedTime = TimeSpan.Zero;
            elapsedTimeLabel.Text = elapsedTime.ToString();
            bombCountLabel.Text = $"Мин: {minesweeperGame.BombCount}";
            this.pictureBox1.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            minesweeperGame.Draw(e.Graphics);

            if (minesweeperGame.IsCoordinatesOutsideGameField(pointHighlightedCell.X, pointHighlightedCell.Y))
                return;
            if (minesweeperGame.Cells[pointHighlightedCell.X, pointHighlightedCell.Y].CellState == CellState.ClosedCell)
            {
                e.Graphics.FillRectangle(Brushes.DarkGreen, highlightedCell);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            elapsedTime += TimeSpan.FromSeconds(1);
            elapsedTimeLabel.Text = elapsedTime.ToString();
        }
        
        private void OnDefeat(object sender, EventArgs e)
        {
            timer1.Stop();
            this.pictureBox1.Refresh();
            MessageBox.Show("луз");
            restartButton_Click(this, EventArgs.Empty);
        }

        private void OnVictory(object sender, EventArgs e)
        {
            timer1.Stop();
            this.pictureBox1.Refresh();
            MessageBox.Show("вин");
            restartButton_Click(this, EventArgs.Empty);
        }      
    }
}
