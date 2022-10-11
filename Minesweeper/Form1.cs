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
    public partial class Form1 : Form
    {
        TimeSpan time = TimeSpan.Zero;
        MinesweeperGame minesweeperGame = new MinesweeperGame(9, 30);        
        Point pointVisualCell = new Point(-1000, -1000);
      
        public Form1()
        {
            InitializeComponent();
            label1CountBombs.Text = $"Мин: {minesweeperGame.CountBombs}";
            label2Timer.Text = time.ToString();
            timer1.Start();
            minesweeperGame.Restart();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            minesweeperGame.Draw(e.Graphics);
            e.Graphics.FillRectangle(Brushes.DarkGreen, pointVisualCell.X * minesweeperGame.cellSize, pointVisualCell.Y * minesweeperGame.cellSize, minesweeperGame.cellSize, minesweeperGame.cellSize);
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
            pictureBox1.Refresh();           
        }
    }
}
