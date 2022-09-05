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
        MinesweeperGame minesweeperGame = new MinesweeperGame(9, 9, 30);
      
        public Form1()
        {
            InitializeComponent();
            label1CountBombs.Text = $"Мин: {minesweeperGame.CountBombs}";
            label2Timer.Text = time.ToString();
            timer1.Start();           
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            minesweeperGame.Draw(e.Graphics);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            time += TimeSpan.FromSeconds(1);
            label2Timer.Text = time.ToString();
        }
    }
}
