using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           timer1.Start();
           timer1.Interval = 1;
            
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            /*
           //string file_name = null;
           openFileDialog1.FileName = null;
           openFileDialog1.InitialDirectory = "C:\\";
           if(openFileDialog1.ShowDialog() == DialogResult.OK)
           {
               //file_name = openFileDialog1.FileName;
               pictureBox1.Image = new Bitmap(openFileDialog1.FileName);
               timer1.Start();
               timer1.Interval = 1;
               //this.ClientSize = pictureBox1.Image.Size;
           }
          else if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
           {
               return;
           }

 */
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string X = Cursor.Position.X.ToString();
            string Y = Cursor.Position.Y.ToString();
            // string.Format("X좌표 {0} Y좌표 {1}", a, b);
            텍스트.Text = string.Format("X좌표 {0}\n Y좌표 {1}", X, Y);
            //textBox2.Refresh();
        }
       
    }
}
