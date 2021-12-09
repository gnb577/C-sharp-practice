using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.IO;


namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

      
        Bitmap image1;

         void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = null;
            string file_name = null;
            openFileDialog1.InitialDirectory = "C:\\";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != null)
            {
                //pictureBox1.Image = Bitmap.FromFile(openFileDialog1.FileName)
                image1 = new Bitmap(openFileDialog1.FileName);
                pictureBox1.Image = image1;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                
                //this.ClientSize = pictureBox1.Image.Size;
            }

            timer1.Start();
            timer1.Interval = 1;
        

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            /*
            string X_POS = Cursor.Position.X.ToString();
            string Y_POS = Cursor.Position.Y.ToString();
            textBox1.Text = string.Format("X좌표: {0} Y좌표: {1}", X_POS, Y_POS);
            */
        }

      
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image != null)
            {
            double box_wid = pictureBox1.Width;
            double box_hei = pictureBox1.Height;

            double ori_wid = pictureBox1.Image.Width;
            double ori_hei = pictureBox1.Image.Height;

            double ratio_X = ori_wid / box_wid;
            double ratio_Y = ori_hei / box_hei;

            double X_POS = e.X;
            double Y_POS = e.Y;
            X_POS = X_POS * ratio_X;
            Y_POS = Y_POS * ratio_Y;
            string Real_X_POS = X_POS.ToString();
            string Real_Y_POS = Y_POS.ToString();


            //string X_POS = Cursor.Position.X.ToString();
            //string Y_POS = Cursor.Position.Y.ToString();

            //double ratio_X = 
         
                textBox1.Text = string.Format("X좌표: {0} Y좌표: {1}", (int)X_POS, (int)Y_POS);
                textBox1.Refresh();
            }

        }

        void button2_Click(object sender, EventArgs e)
        {

            FileStream fs = new FileStream("test3.csv", FileMode.Create);
            StreamWriter sw = new StreamWriter(@"E:\test3.csv");

            Color col;
            int gray_col;
            for (int x = 0; x < image1.Height; x++)
            {
                for (int y = 0; y < image1.Width; y++)
                {
                    col = image1.GetPixel(x, y);
                   gray_col = (col.R + col.G + col.B) / 3;
                    sw.Write(gray_col.ToString());
                    sw.Write(",");
                    // file.WriteLine(sCellValues);
                }
                sw.WriteLine();
            }
            sw.Flush();
            /*
               string Init_Directory = @"E:\test.csv";
               string Init_Directory2 = @"E:\";
               string textvalue = "1";
               string textvalue2 = "2";
               string textvalue3 = "3";

               System.IO.File.WriteAllText(Init_Directory, textvalue, Encoding.Default);

               */

            /*
                int[,] c_value = new int[92,112];

                Color col;
                int gray_col;



                for (int x = 0; x < image1.Width; x++)
                {
                    for (int y = 0; y < image1.Height; y++)
                    {
                        col = image1.GetPixel(x, y);
                        gray_col = (col.R + col.G + col.B) / 3;
                        c_value[x, y] = gray_col;
                        sCellValues = gray_col.ToString();
                       // file.WriteLine(sCellValues);
                    }
                }

            */


            /*
            var csv = string.Join("\r\n", lines.Select(words =>
                   string.Join(",", words.Select(word =>
                       "\"" + word.Replace("\"", "\"\"") + "\"")))));
            
            File.WriteAllText("file.csv", csv);
            */


            //string strFilePath = @"C:\Data1.csv";
            //StringBuilder sbOutput = new StringBuilder();





        }

    }
}
