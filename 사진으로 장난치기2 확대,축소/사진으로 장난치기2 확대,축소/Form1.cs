using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;




namespace 사진으로_장난치기2_확대_축소
{
    public partial class Form1 : Form
    {

       private Bitmap img;
        private double ratio = 1.0F;
        private Point imgPoint;
        private Rectangle imgRect;
        private Point clickPoint;



        public Form1()
        {
            InitializeComponent();

            pictureBox1.MouseWheel += new MouseEventHandler(pictureBox1_MouseWheel);
           img = new Bitmap(@"E:\01.bmp");
            pictureBox1.Image = new Bitmap(@"E:\01.bmp");
           pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            imgPoint = new Point(pictureBox1.Width / 2, pictureBox1.Height / 2);
            imgRect = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
            ratio = 1.0;
            clickPoint = imgPoint;

            pictureBox1.Refresh();
            pictureBox1.Invalidate();
            
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {

            int lines = e.Delta * SystemInformation.MouseWheelScrollLines / 120;
            PictureBox pb = (PictureBox)sender;

            if (lines > 0)
            {
                ratio *= 1.1F;
                if (ratio > 100.0) ratio = 100.0;
            }
            else if (lines < 0)
            {
                ratio *= 0.9F;
                if (ratio < 1) ratio = 1;
            }

            imgRect.Width = (int)Math.Round(pictureBox1.Width * ratio);
            imgRect.Height = (int)Math.Round(pictureBox1.Height * ratio);
            imgRect.X = (int)Math.Round(pb.Width / 2 - imgPoint.X * ratio);
            imgRect.Y = (int)Math.Round(pb.Height / 2 - imgPoint.Y * ratio);

            pictureBox1.Refresh();
            pictureBox1.Invalidate();
        }

       

        private void pictureBox1_Paint_1(object sender, PaintEventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                // e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                e.Graphics.DrawImage(pictureBox1.Image, imgRect);
                pictureBox1.Focus();
            }
        }
    }
}
