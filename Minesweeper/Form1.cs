﻿using System;
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
        //MinesweeperGame minesweeperGame = new MinesweeperGame(9, 30);
        MinesweeperGame minesweeperGame;
        Point pointVisualCell = new Point();
        Rectangle visualCell;

        public Form1()
        {
            InitializeComponent();
            minesweeperGame = new MinesweeperGame(9, 30, this);
            minesweeperGame.Restart();
            minesweeperGame.GenerateBombs();
            label1CountBombs.Text = $"Мин: {minesweeperGame.CountBombs}";
            label2Timer.Text = time.ToString();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            minesweeperGame.Draw(e.Graphics);
            if (minesweeperGame.cells[pointVisualCell.Y, pointVisualCell.X].cellState == CellState.ClosedCell)
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
            pointVisualCell.X = e.Location.X / minesweeperGame.cellSize;
            pointVisualCell.Y = e.Location.Y / minesweeperGame.cellSize;

            visualCell = new Rectangle(
                pointVisualCell.X * minesweeperGame.cellSize,
                pointVisualCell.Y * minesweeperGame.cellSize,
                minesweeperGame.cellSize,
                minesweeperGame.cellSize
                );
            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            timer1.Start();            
            if (e.Button == MouseButtons.Left)
            {
                minesweeperGame.OpenCell(pointVisualCell.X, pointVisualCell.Y);
            }

            if (e.Button == MouseButtons.Right)
            {
                minesweeperGame.PutFlagInCell(pointVisualCell.X, pointVisualCell.Y);

                if (minesweeperGame.CountBombs >= 0)
                {
                    label1CountBombs.Text = $"Мин: {minesweeperGame.CountBombs}";
                }
                else
                {
                    label1CountBombs.Text = $"Мин: {0}";
                }               
            }          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            minesweeperGame.Restart();
            minesweeperGame.GenerateBombs();
            timer1.Stop();
            time = TimeSpan.Zero;
            label2Timer.Text = time.ToString();
            label1CountBombs.Text = $"Мин: {minesweeperGame.CountBombs}";
            this.pictureBox1.Refresh();
        }

        public void Lose()
        {
            timer1.Stop();
            this.pictureBox1.Refresh();
            MessageBox.Show("луз");
            button1_Click(this, new EventArgs());
        }
    }
}
