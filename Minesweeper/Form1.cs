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
        TimeSpan elapsedTime = TimeSpan.Zero;
        MinesweeperGame minesweeperGame = new MinesweeperGame(9, 30);
        Point pointHighlightedCell = new Point(-100, -100);
        Rectangle HighlightedCell;
        bool isPressedLeftMouseButton;
        bool isPressedRightMouseButton;

        public Form1()
        {
            InitializeComponent();
            minesweeperGame.Victory += OnVictory;
            minesweeperGame.Defeat += OnDefeat;
            minesweeperGame.Restart();
            bombCountLabel.Text = $"Мин: {minesweeperGame.BombCount}";
            elapsedTimeLabel.Text = elapsedTime.ToString();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            minesweeperGame.Draw(e.Graphics);

            if (minesweeperGame.IsCoordinatesOutsideGameField(pointHighlightedCell.Y, pointHighlightedCell.X))
                return;
            if (minesweeperGame.Cells[pointHighlightedCell.Y, pointHighlightedCell.X].cellState == CellState.ClosedCell)
            {
                e.Graphics.FillRectangle(Brushes.DarkGreen, HighlightedCell);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            elapsedTime += TimeSpan.FromSeconds(1);
            elapsedTimeLabel.Text = elapsedTime.ToString();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (pointHighlightedCell.X != e.Location.X / minesweeperGame.CellSize ||
                pointHighlightedCell.Y != e.Location.Y / minesweeperGame.CellSize)
            {
                pointHighlightedCell.X = e.Location.X / minesweeperGame.CellSize;
                pointHighlightedCell.Y = e.Location.Y / minesweeperGame.CellSize;

                HighlightedCell = new Rectangle(
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

            if (minesweeperGame.IsCoordinatesOutsideGameField(cellY, cellX))
                return;

            timer1.Start();

            if (e.Button == MouseButtons.Left)
            {
                minesweeperGame.OpenCell(cellY, cellX);
            }

            if (e.Button == MouseButtons.Right)
            {
                minesweeperGame.PutFlagInCell(cellY, cellX);

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
                minesweeperGame.SmartOpenCell(cellY, cellX);
            }

            isPressedRightMouseButton = false;
            isPressedLeftMouseButton = false;
            pictureBox1.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {          
            minesweeperGame.Restart();
            timer1.Stop();
            elapsedTime = TimeSpan.Zero;
            elapsedTimeLabel.Text = elapsedTime.ToString();
            bombCountLabel.Text = $"Мин: {minesweeperGame.BombCount}";
            this.pictureBox1.Refresh();
        }

        public void OnDefeat(object sender, EventArgs e)
        {
            timer1.Stop();
            this.pictureBox1.Refresh();
            MessageBox.Show("луз");
            button1_Click(this, new EventArgs());
        }

        public void OnVictory(object sender, EventArgs e)
        {
            timer1.Stop();
            this.pictureBox1.Refresh();
            MessageBox.Show("вин");
            button1_Click(this, new EventArgs());
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
    }
}
