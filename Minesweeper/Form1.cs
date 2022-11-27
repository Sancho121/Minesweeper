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
        TimeSpan time = TimeSpan.Zero;
        MinesweeperGame minesweeperGame = new MinesweeperGame(9, 30);
        Point pointVisualCell = new Point();
        Rectangle visualCell;

        public Form1()
        {
            InitializeComponent();
            minesweeperGame.Victory += Win;
            minesweeperGame.Defeat += Lose;
            minesweeperGame.Restart();
            label1CountBombs.Text = $"Мин: {minesweeperGame.CountBombs}";
            label2Timer.Text = time.ToString();

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            minesweeperGame.Draw(e.Graphics);

            if (minesweeperGame.CoordinatesOutsideGameField(pointVisualCell.Y, pointVisualCell.X))
                return;
            if (minesweeperGame.Cells[pointVisualCell.Y, pointVisualCell.X].cellState == CellState.ClosedCell)
            {
                e.Graphics.FillRectangle(Brushes.DarkGreen, visualCell);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {                      
            time += TimeSpan.FromSeconds(1);
            label2Timer.Text = time.ToString();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            pointVisualCell.X = e.Location.X / minesweeperGame.CellSize;
            pointVisualCell.Y = e.Location.Y / minesweeperGame.CellSize;

            visualCell = new Rectangle(
                pointVisualCell.X * minesweeperGame.CellSize,
                pointVisualCell.Y * minesweeperGame.CellSize,
                minesweeperGame.CellSize,
                minesweeperGame.CellSize
                );

            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            int cellX = e.Location.X / minesweeperGame.CellSize;
            int cellY = e.Location.Y / minesweeperGame.CellSize;


            if (minesweeperGame.CoordinatesOutsideGameField(cellY, cellX))
                return;

            timer1.Start();

            if (e.Button == MouseButtons.Left)
            {
                minesweeperGame.OpenCell(cellY, cellX);
            }

            if (e.Button == MouseButtons.Right)
            {
                minesweeperGame.PutFlagInCell(cellY, cellX);

                if (minesweeperGame.CountFlags <= minesweeperGame.CountBombs)
                {
                    label1CountBombs.Text = $"Мин: {minesweeperGame.CountBombs - minesweeperGame.CountFlags}";
                }
                else
                {
                    label1CountBombs.Text = $"Мин: {0}";
                }               
            } 
            
            if (e.Button == MouseButtons.Middle)
            {
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {          
            minesweeperGame.Restart();
            timer1.Stop();
            time = TimeSpan.Zero;
            label2Timer.Text = time.ToString();
            label1CountBombs.Text = $"Мин: {minesweeperGame.CountBombs}";
            this.pictureBox1.Refresh();
        }

        public void Lose(object sender, EventArgs e)
        {
            timer1.Stop();
            this.pictureBox1.Refresh();
            MessageBox.Show("луз");
            button1_Click(this, new EventArgs());
        }

        public void Win(object sender, EventArgs e)
        {
            timer1.Stop();
            this.pictureBox1.Refresh();
            MessageBox.Show("вин");
            button1_Click(this, new EventArgs());
        }
    }
}
