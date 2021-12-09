using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C히스토그램_평활화
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string root_dir = "E:\\"; //최초 folder를 오픈할 때, 사용하는 초기 위치 값 및 파일을 저장할 공간
        string file_dir;

        int sampling_size;
        int blank_value;
        
        int div;
        int row = 0;
        int col = 0;
        int[,] mask;
        double mask_value = 0;

        int[] histogram;
        int[] sum;
        double[] normalize_sum;

        Image image;
        Bitmap ori_map;
        Bitmap re_original_map;
        Bitmap re2_original_map;


        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = root_dir;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                file_dir = openFileDialog1.FileName;
            }

            image = Bitmap.FromFile(file_dir);

            ori_map = new Bitmap(image);
            re_original_map = new Bitmap(image);
            re2_original_map = new Bitmap(image);
            histogram = new int[256];
            sum = new int[256];
            normalize_sum = new double[256];
            System.Array.Clear(histogram, 0, histogram.Length);
            System.Array.Clear(sum,0,sum.Length);
            System.Array.Clear(normalize_sum, 0, normalize_sum.Length);
            sampling_size = 3;
            blank_value = sampling_size / 2;
            
        }

        private void button2_Click(object sender, EventArgs e)
        {

            mask = new int[,] { { 1, 2, 1 }, { 2, 4, 2 }, { 1, 2, 1 } };

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    div = div + mask[i, j];
                }
            }
            for (int i = 0; i < ori_map.Width; i++)
            {
                for (int j = 0; j < ori_map.Height; j++)
                {
                    // pix = ori_map.GetPixel(i, j);
                    row = 0;
                    mask_value = 0;
                    for (int k = i - blank_value; k <= i + blank_value; k++)
                    {
                        col = 0;
                        if (k < 0 || k >= ori_map.Width)
                        { row++; continue; }
                        for (int l = j - blank_value; l <= j + blank_value; l++)
                        {
                            if (l < 0 || l >= ori_map.Height)
                            { col++; continue; }
                            mask_value = mask_value + ori_map.GetPixel(k, l).B * mask[row, col];
                            col++;
                        }
                        row++;
                    }

                    mask_value = mask_value / div;

                    Color m = Color.FromArgb((int)mask_value, (int)mask_value, (int)mask_value);
                    re_original_map.SetPixel(i, j, m);
                }
            }
            pictureBox1.Image = ori_map;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = re_original_map;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            for (int i = 0; i < re_original_map.Width; i++)
            {
                for (int j = 0; j < re_original_map.Height; j++)
                {
                    int a = re_original_map.GetPixel(i, j).B;
                    histogram[a]++;
                }
            }
            sum[0] = histogram[0];
            for (int i = 1; i < 256; i++)
            {
                sum[i] = sum[i-1] + histogram[i];
            }
            for(int i=0;i<256;i++)
            {
                normalize_sum[i] = (double)sum[i] / (ori_map.Width * ori_map.Height) * 255;
            }
            for (int i = 0; i < re_original_map.Width; i++)
            {
                for (int j = 0; j < re_original_map.Height; j++)
                {
                    int pix = re_original_map.GetPixel(i, j).B;
                    mask_value = normalize_sum[pix];
                    Color m = Color.FromArgb((int)mask_value, (int)mask_value, (int)mask_value);
                    re2_original_map.SetPixel(i, j, m);
                }
            }

            pictureBox3.Image = re2_original_map;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
        }
    }
}


/*
  < 잘못 이해한 방식 >
         max_value = re_original_map.GetPixel(0, 0).B;
         min_value = re_original_map.GetPixel(0, 0).B;
         for (int i = 0; i < re_original_map.Width; i++)
         {
             for (int j = 0; j < re_original_map.Height; j++)
             {
                 double a = re_original_map.GetPixel(i, j).B;
                 if (a < min_value)
                 {
                     min_value = a;
                 }
                 if (a > max_value)
                 {
                     max_value = a;
                 }
             }
         }
         mask_rate = 255 / (max_value - min_value);
         for (int i = 0; i < re_original_map.Width; i++)
         {
             for (int j = 0; j < re_original_map.Height; j++)
             {
                 double b = re_original_map.GetPixel(i, j).B;
                 mask_value = (b - min_value) * mask_rate;
                 Color m = Color.FromArgb((int)mask_value, (int)mask_value, (int)mask_value);
                 re2_original_map.SetPixel(i, j, m);

             }
         }
         */
