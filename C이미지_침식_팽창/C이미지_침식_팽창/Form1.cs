using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace C이미지_침식_팽창
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string root_dir = "E:\\"; //최초 folder를 오픈할 때, 사용하는 초기 위치 값 및 파일을 저장할 공간
        string file_dir;
     

        int img_width;
        int img_height;

        int sampling_size;
        int blank_value;
        

        Color pix, max_value, min_value;
        int[,] ori_img_value;
        int[,] re_img_value;
       

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
                textBox1.Text = file_dir;
            }

            image = Bitmap.FromFile(file_dir);

            ori_map = new Bitmap(image);
            re_original_map = new Bitmap(image);
            re2_original_map = new Bitmap(image);

            img_width = ori_map.Width;
            img_height = ori_map.Height;

            sampling_size = 3;
            blank_value = sampling_size / 2;
           
            ori_img_value = new int[img_width, img_height];
            re_img_value = new int[img_width, img_height];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < img_width; i++)
            {
                for (int j = 0; j < img_height; j++)
                {
                    pix = ori_map.GetPixel(i, j);
                    max_value = pix;
                    for (int k = i - blank_value; k <= i + blank_value; k++)
                    {
                        if (k < 0 || k >= img_width) continue;
                        for (int l = j - blank_value; l <= j + blank_value; l++)
                        {
                            if (l < 0 || l >= img_height) continue;
                            if (k == i && l == j) continue;
                            if (max_value.B < ori_map.GetPixel(k, l).B)
                            {
                                max_value = ori_map.GetPixel(k, l);
                            }
                        }
                    }
                    re_original_map.SetPixel(i, j, max_value);
                }
            }
            pictureBox1.Image = ori_map;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = re_original_map;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;

            for (int i = 0; i < img_width; i++)
            {
                for (int j = 0; j < img_height; j++)
                {
                    pix = ori_map.GetPixel(i, j);
                    min_value = pix;
                    for (int k = i - blank_value; k <= i + blank_value; k++)
                    {
                        if (k < 0 || k >= img_width) continue;
                        for (int l = j - blank_value; l <= j + blank_value; l++)
                        {
                            if (l < 0 || l >= img_height) continue;
                            if (k == i && l == j) continue;
                            if (min_value.B > ori_map.GetPixel(k, l).B)
                            {
                                min_value = ori_map.GetPixel(k, l);
                            }
                        }
                    }
                    re2_original_map.SetPixel(i, j, min_value);
                }
            }
            pictureBox3.Image = re2_original_map;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
        }

    }
}
    
