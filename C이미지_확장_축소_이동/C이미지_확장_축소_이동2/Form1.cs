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

namespace C이미지_확장_축소_이동2
{
    public partial class Form1 : Form
    {
        private Point LastPoint;
        Image image;
        Bitmap ori_map;
        Bitmap re_original_map;
        Bitmap re2_original_map;
        private double ratio = 1.0F;
        private Point imgPoint;
        private Rectangle imgRect;
        private Point clickPoint;

        string root_dir = "E:\\"; //최초 folder를 오픈할 때, 사용하는 초기 위치 값 및 파일을 저장할 공간
        string file_dir;

        int count = 0;
        int count2 = 0;
        int count3 = 0;
        public Form1()
        {
            InitializeComponent();

            
                openFileDialog1.InitialDirectory = root_dir;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    file_dir = openFileDialog1.FileName;
                }
             image = new Bitmap(file_dir);

            ori_map = new Bitmap(image);
            re_original_map = new Bitmap(image);
            re2_original_map = new Bitmap(image);

         
            pictureBox1.MouseWheel += new MouseEventHandler(pictureBox1_MouseWheel);
            pictureBox1.MouseDown += new MouseEventHandler(pictureBox1_MouseDown);
            pictureBox1.MouseMove += new MouseEventHandler(pictureBox1_MouseMove);

           // pictureBox1.Image = new Bitmap(image);
            //pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;

            
            
            
            imgPoint = new Point(pictureBox1.Width / 2, pictureBox1.Height / 2);
            imgRect = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
    
            ratio = 1.0;
            clickPoint = imgPoint;
            count2++;
            //imgPoint = clickPoint;
            pictureBox1.Refresh();
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        { 
            if (e.Button == MouseButtons.Left)
            {
                count3++;
                clickPoint = new Point(e.X, e.Y);
                //richTextBox2.Text = string.Format("X: {0}\n Y: {1}", e.X, e.Y);
            }
            pictureBox1.Invalidate();
            
        }

        
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            
            if (e.Button == MouseButtons.Left)
            {
                imgRect.X = imgRect.X + (int)Math.Round((double)(e.X - clickPoint.X)/10 );
                if (imgRect.X >= 0) imgRect.X = 0;
                if (Math.Abs(imgRect.X) >= Math.Abs(imgRect.Width - pictureBox1.Width)) imgRect.X = -(imgRect.Width - pictureBox1.Width);
                imgRect.Y = imgRect.Y + (int)Math.Round((double)(e.Y - clickPoint.Y)/10);
                if (imgRect.Y >= 0) imgRect.Y = 0;
                if (Math.Abs(imgRect.Y) >= Math.Abs(imgRect.Height - pictureBox1.Height)) imgRect.Y = -(imgRect.Height - pictureBox1.Height);
            }
            else
            {
                LastPoint = e.Location;
            }

            pictureBox1.Invalidate();
           
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {

            int lines = e.Delta * SystemInformation.MouseWheelScrollLines / 120;
            PictureBox pb = (PictureBox)sender;

            if (lines > 0)
            {
                ratio *= 1.1F;
                if (ratio > 10.0) ratio = 10.0;
            }
            else if (lines < 0)
            {
                ratio *= 0.9F;
                if (ratio < 0.1) ratio = 0.1;
            }

            imgRect.Width = (int)Math.Round(pictureBox1.Width * ratio);
            imgRect.Height = (int)Math.Round(pictureBox1.Height * ratio);
            imgRect.X = (int)Math.Round(pb.Width / 2 - imgPoint.X * ratio);
            imgRect.Y = (int)Math.Round(pb.Height / 2 - imgPoint.Y * ratio);
            richTextBox2.Text = string.Format("pb Width : {0}\n pb Height: {1}", pb.Width, pb.Height);

            pictureBox1.Refresh();
            pictureBox1.Invalidate();
        }

       
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (image != null)
            {
                // e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                count++;
                e.Graphics.DrawImage(image, imgRect);
                richTextBox1.Text = string.Format("imgPoint: {0}\n imgRect: {1}\n X: {2}\n Y: {3}\n Ratio : {4}\n imgwidth : {5}\n imgheight : {6}", imgPoint, imgRect, imgRect.X, imgRect.Y, ratio,image.Width,image.Height);
                pictureBox1.Focus();
            }
        }

       
    }
}


 