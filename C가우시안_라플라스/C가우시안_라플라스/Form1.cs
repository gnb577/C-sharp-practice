using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace C가우시안_라플라스
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
       
        int[,] mask;
        int[,] ra_mask;

        int div;

        int row = 0;
        int col = 0;
        double mask_value=0;
        double ra_mask_value = 0;

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

            sampling_size = 3;
            blank_value = sampling_size / 2;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            mask = new int[,]{{1,2,1}, { 2,4,2}, {1,2,1} };
            ra_mask = new int[,] { { 1, 1, 1 }, { 1, -8, 1 }, { 1, 1, 1 } };
            //ra_mask = new int[,] { { 0, 1,0}, { 1, -4, 1 }, { 0, 1, 0 } };
            for (int i=0;i<3;i++)
            {
                for(int j=0;j<3;j++)
                {
                    div = div + mask[i,j];
                }
            }
            for(int i=0;i< ori_map.Width;i++)
            {
                for(int j=0;j<ori_map.Height; j++)
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
            /*
            for (int i = 0; i < ori_map.Width; i++)
            {
                for (int j = 0; j < ori_map.Height; j++)
                {
                    // pix = ori_map.GetPixel(i, j);
                    row = 0;
                    for (int k = i - blank_value; k <= i + blank_value; k++)
                    {
                        col = 0;
                        if (k < 0 || k >= ori_map.Width)
                        { row++; continue; }
                        for (int l = j - blank_value; l <= j + blank_value; l++)
                        {
                            if (l < 0 || l >= ori_map.Height)
                            { col++; continue; }
                            ra_mask_value = ra_mask_value + ori_map.GetPixel(k, l).B * ra_mask[row, col];
                            col++;
                        }
                        row++;
                    }
                    ra_mask_value = Math.Abs(ra_mask_value);
                    if (ra_mask_value >= 255)
                    {
                        ra_mask_value = 255;
                    }
                    Color o = Color.FromArgb((int)ra_mask_value, (int)ra_mask_value, (int)ra_mask_value);
                    re2_original_map.SetPixel(i, j, o);

                }
            }

            pictureBox3.Image = re2_original_map;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            */
   
            for (int i = 0; i < re_original_map.Width; i++)
            {
                for (int j = 0; j < re_original_map.Height; j++)
                {
                    // pix = ori_map.GetPixel(i, j);
                    row = 0;
                    ra_mask_value = 0;
                    for (int k = i - blank_value; k <= i + blank_value; k++)
                    {
                        col = 0;
                        if (k < 0 || k >= re_original_map.Width)
                        { row++; continue; }
                        for (int l = j - blank_value; l <= j + blank_value; l++)
                        {
                            if (l < 0 || l >= re_original_map.Height)
                            { col++; continue; }
                            ra_mask_value = ra_mask_value + re_original_map.GetPixel(k, l).B * ra_mask[row, col];
                            col++;
                        }
                        row++;
                    }
                    ra_mask_value = Math.Abs(ra_mask_value);
                    if(ra_mask_value>= 255)
                    {
                        ra_mask_value = 255;
                    }
                    Color o = Color.FromArgb((int)ra_mask_value, (int)ra_mask_value, (int)ra_mask_value);
                    re2_original_map.SetPixel(i, j, o);

                }
            }
            
            pictureBox3.Image = re2_original_map;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
          
        }
    }
}
