using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C오츠이진화
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
        int[] histogram;
        int[] sum;
       
        int back_sum = 0;
        int for_sum = 0;
        double back_weight = 0;
        double for_weight = 0;
        double back_mean = 0;
        double for_mean = 0;
        double back_variance = 0;
        double for_variance = 0;
        double variance = 0;

        int min_idx = 0;
        double min_variance = 9999999;
        double threshold = 0;

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
          
            System.Array.Clear(histogram, 0, histogram.Length);
            System.Array.Clear(sum, 0, sum.Length);
  
            sampling_size = 3;
            blank_value = sampling_size / 2;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < ori_map.Width; i++)
            {
                for (int j = 0; j < ori_map.Height; j++)
                {
                    int a = ori_map.GetPixel(i, j).B;
                    histogram[a]++;
                }
            }
            for (int o = 1; o < 255; o++)
            {
                back_sum = 0;
                back_weight = 0;
                back_mean = 0;
                back_variance = 0;

                for_mean = 0;
                for_weight = 0;
                for_sum = 0;
                for_variance = 0;

                for (int i = 0; i < o; i++)
                {
                    back_weight = back_weight + histogram[i];
                    back_mean = back_mean + i * histogram[i];
                    back_sum = back_sum + histogram[i];
                }
                if (back_sum == 0) continue;
                back_weight = back_weight / (ori_map.Width * ori_map.Height);
                back_mean = back_mean / back_sum;

                for (int i = o; i < 256; i++)
                {
                    for_weight = for_weight + histogram[i];
                    for_mean = for_mean + i * histogram[i];
                    for_sum = for_sum + histogram[i];
                }
                for_weight = for_weight / (ori_map.Width * ori_map.Height);
                for_mean = for_mean / for_sum;

                for (int i = 0; i < o; i++)
                {
                    back_variance = back_variance + (Math.Pow((i - back_mean), 2) * histogram[i]);
                }
                back_variance = back_variance / back_sum;

                for (int i = o; i < 256; i++)
                {
                    for_variance = for_variance + (Math.Pow((i - for_mean), 2) * histogram[i]);
                }
                for_variance = for_variance / for_sum;
                
                variance = for_weight * for_variance + back_weight * back_variance;
                if (min_variance > variance)
                {
                    min_variance = variance;
                    min_idx = o;
                }
            }
            threshold = min_idx;

            for(int i=0;i < ori_map.Width;i++)
            {
                for(int j=0;j<ori_map.Height;j++)
                {
                    int col = ori_map.GetPixel(i, j).B;
                    if(col>= threshold)
                    {
                        col = 255;
                    }
                    else
                    {
                        col = 0;
                    }
                    Color m = Color.FromArgb(col, col, col);
                    re_original_map.SetPixel(i, j, m);
                }
            }

            pictureBox1.Image = ori_map;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = re_original_map;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
        }
    }
}
