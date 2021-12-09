using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;


namespace 최소자승법
{
    public partial class Form1 : Form
    {
        Random r = new Random();
        Mat w, u, vt;
        int i_value;
       // double[] d_numx;
       // double[] d_numy;
       // double[,] d_MatrixA;
       // double[,] d_rev_MatrixA;
       // double[,] d_Square_MatrixA;
       // double[,] d_Pinv_MatrixA;
       // double[,] d_Matrix_ab;
       // double[,] d_MatrixB;
        Mat m_Square_MatrixA, m_Pinv_MatrixA;
        Mat d_numx,d_numy;
        Mat d_MatrixA, d_rev_MatrixA, d_Square_MatrixA, d_Pinv_MatrixA;
        Mat d_Matrix_ab;
        Mat d_MatrixB;
        double[] numx;
        double[] numy;
        double[,] MatrixA;
        double[,] rev_MatrixA;
        double[,] Square_MatrixA;
        double[,] Pinv_MatrixA;
        double[,] Matrix_ab;
        double[,] MatrixB;


        public Form1()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

            chart1.Series.Clear();
            chart1.Legends[0].Enabled = false;
            chart1.ChartAreas[0].AxisX.Title = "X";      //chart를 통해 그래프를 그리기 위한 초기 세팅
            chart1.ChartAreas[0].AxisY.Title = "Y";
            i_value = Convert.ToInt32(richTextBox1.Text);

            numx = new double[i_value];
            numy = new double[i_value];
            MatrixA = new double[i_value,2];
            rev_MatrixA = new double[2,i_value];
            Pinv_MatrixA = new double[2,2];
            Matrix_ab = new double[2,1];
            MatrixB = new double[2, 1];

            //d_Square_MatrixA.Set<double>(0~4, 0<-건드리지말것, 4);
            
            Square_MatrixA = new double[2,2];
            
            
            d_Matrix_ab = new Mat(2, 1, MatType.CV_32F, Matrix_ab);
            d_MatrixB = new Mat(2, 1, MatType.CV_32F, MatrixB);
            //d_numx.SetTo(Scalar(0));
            // System.Array.Clear(d_numx, 0, i_value);
            // System.Array.Clear(d_numy, 0, i_value);
            // System.Array.Clear(d_MatrixA, 0, 2 * i_value);
            // System.Array.Clear(d_rev_MatrixA, 0, 2 * i_value);
            // System.Array.Clear(d_Square_MatrixA, 0, 2 * 2);
            // System.Array.Clear(d_Pinv_MatrixA, 0, 2 * 2);
            // System.Array.Clear(d_Matrix_ab, 0, 2);
            // System.Array.Clear(d_MatrixB, 0, i_value);

            // Cv2.Invert(d_MatrixA, d_MatrixpinvA, DecompTypes.SVD);
            chart1.Series.Add("Data");
            d_MatrixA = new Mat(i_value, 2, MatType.CV_32F, MatrixA);
            for (int i = 0; i < i_value; i++)
            {

                //numx[i] = r.Next(1, i_value);
                //numy[i] = r.Next(1, i_value);
                numx[i] = i + 1;
                numy[i] = i + 2;
                MatrixA[i, 0] = numx[i];
                MatrixA[i, 1] = 1;
                //d_MatrixA.Set<double>(i, 0, numx[i]);
                chart1.Series["Data"].ChartType = SeriesChartType.Point;

                System.Threading.Thread.Sleep(10);
            }

            d_numx = new Mat(i_value, 1, MatType.CV_32F, numx);
            d_numy = new Mat(i_value, 1, MatType.CV_32F, numy);
            d_MatrixA = new Mat(i_value, 2, MatType.CV_32F, MatrixA);
            
            
            for(int i=0;i<i_value;i++)
            {
                for(int j=0;j<2;j++)
                {
                   rev_MatrixA[j,i]=  MatrixA[i,j];
                }
            }
            d_rev_MatrixA = new Mat(2, i_value, MatType.CV_32F, rev_MatrixA);

            for (int k = 0; k < i_value; k++)
            {
                Square_MatrixA[0, 0] = Square_MatrixA[0, 0] + MatrixA[k, 0] * MatrixA[k, 0];
                Square_MatrixA[0, 1] = Square_MatrixA[0, 1] + MatrixA[k, 0];
                Square_MatrixA[1, 0] = Square_MatrixA[1, 0] + MatrixA[k, 0];
                Square_MatrixA[1, 1] = Square_MatrixA[1, 1] + 1;
                MatrixB[0,0] = MatrixB[0,0] + MatrixA[k, 0] * numy[k];
                MatrixB[1,0] = MatrixB[1,0] + numy[k];
            }

            d_Square_MatrixA = new Mat(2, 2, MatType.CV_32F, Square_MatrixA);
            d_Pinv_MatrixA = new Mat(2, 2, MatType.CV_32F, Pinv_MatrixA);

            double[,] d_w = new double[i_value, 2];
            double[,] d_u = new double[i_value, i_value];
            double[,] d_vt = new double[2, 2];
            w = new Mat(i_value, 2, MatType.CV_32F, d_w);
            u = new Mat(i_value, i_value, MatType.CV_32F, d_u);
            vt = new Mat(2, 2, MatType.CV_32F, d_vt);
            SVD.Compute(d_Square_MatrixA, w, u, vt,0);

            Cv2.Invert(d_Square_MatrixA, d_Pinv_MatrixA, DecompTypes.SVD);
            //Cv2.Invert(d_Square_MatrixA, d_Pinv_MatrixA, DecompTypes.





            chart1.Series.Add("LSM");


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
