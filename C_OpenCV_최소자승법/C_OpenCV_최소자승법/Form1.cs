using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using OpenCvSharp.Util;
using System.Drawing.Drawing2D;
using ChartDirector;
using OpenCvSharp.ML;
using System.Xml.Schema;
using System.Runtime.CompilerServices;


namespace C_OpenCV_최소자승법
{
    public partial class Form1 : Form
    {
        

        Random r = new Random();
        int i_value;
//------------------------------------------------------------------------------- 공유
        double sum_a = 0;
        double sum_b = 0;

        double[,] MatrixA;
        double[,] MatrixB;
        double[,] Pinv_MatrixA;
        double[,] Matrix_ab;
        double[,] numx;
        double[,] numy;

        Mat d_MatrixA, d_MatrixB, d_Pinv_MatrixA,d_Matrix_ab,d_numx,d_numy;
 //------------------------------------------------------------------------------- 1번 2차원 
        double[,] three_MatrixA;
        double[,] three_Matrix_abcd;
        double[,] s;
        double[,] u;
        double[,] v;
        Mat d_three_MatrixA, d_three_Matrix_abcd, S, U, V;

        //------------------------------------------------------------------------------- 2번 3차원
     
        //-------------------------------------------------------------------------------- 3번 Ransac 임시

        double modelx;
        double modely;
        double modelsx;
        double modelsy;

        double max_rotation_count = 0;
        int total_data_count = 0;
        int sample_data_count = 0;
        int inliers_count = 0;

        double threshold;

        int N = 3;
        double cost = 0;
        double max_cost = 0;

        OpenCvSharp.Point[] total_data;
        OpenCvSharp.Point[] sample_data;
        OpenCvSharp.Point[] inliers;

        Mat MatrixA3;
        Mat MatrixB3;
        Mat Matrix_abc3;
        Mat Matrix_PinvA3;

        double [,]ma3;
        double [,]mb3;
        double [,]mabc3;
        double [,]mpa3;

        double s_a;
        double s_b;
        double s_c;

        bool Find_Same_SampleData(int num)
        {
            for(int i=0;i<N;i++)
            {
                if(sample_data[i].X == total_data[num].X && sample_data[i].Y == total_data[num].Y)
                {
                    return true;
                }
            }
            return false;
        }

        void Get_Sample()
        {

            System.Array.Clear(sample_data,0,sample_data.Length);
            int i = 0;
            while(i != N)
            {
                int j = r.Next(0, total_data.Length);
               
                if (!Find_Same_SampleData(j))
                {
                    sample_data[i] = total_data[j];
                    i++;
                }
            }
        }

        void make_pinv()
        {
            s_a = 0;
            s_b = 0;
            s_c = 0;

            ma3 = new double[sample_data.Length, 3];
            mb3 = new double[sample_data.Length, 1];
            mabc3 = new double[3, 1];
            mpa3 = new double[3, sample_data.Length];

            for (int i = 0; i < sample_data.Length; i++)
            {
                ma3[i, 0] = sample_data[i].X * sample_data[i].X;
                ma3[i, 1] = sample_data[i].X;
                ma3[i, 2] = 1;
                mb3[i, 0] = sample_data[i].Y;
            }
            MatrixA3 = new Mat(sample_data.Length, 3, MatType.CV_64FC1, ma3);
            MatrixB3 = new Mat(sample_data.Length, 1, MatType.CV_64FC1, ma3);

            Matrix_PinvA3 = new Mat(3, sample_data.Length, MatType.CV_64FC1, mpa3);
            Cv2.Invert(MatrixA3, Matrix_PinvA3, DecompTypes.SVD);

            for (int i = 0; i < inliers_count; i++)
            {
                s_a = s_a + Matrix_PinvA3.At<double>(0, i) * MatrixB3.At<double>(i, 0);
                s_b = s_b + Matrix_PinvA3.At<double>(1, i) * MatrixB3.At<double>(i, 0);
                s_c = s_c + Matrix_PinvA3.At<double>(2, i) * MatrixB3.At<double>(i, 0);
            }
        }

        void compute_inlier()
        {
            inliers_count = 0;
            for (int i = 0; i < max_cost; i++)
            {
                double y = s_a * i * i + s_b * i + s_c;
                if (threshold >= Math.Abs((sample_data[i].Y) - y))
                {
                    inliers[inliers_count] = total_data[i];
                    inliers_count++;
                }
            }
        }
        void compute_model()
        {
            cost = 0;
            for (int i = 0; i < total_data.Length; i++)
            {
                double y = s_a * i * i + s_b * i + s_c;
                if(threshold >= Math.Abs((total_data[i].Y)-y))
                {
                    cost++;
                }
            }
        }
    
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

            MatrixA = new double[i_value, 2];
            MatrixB = new double[i_value, 1];
            Pinv_MatrixA = new double[2, i_value];
            Matrix_ab = new double[2, 1];
            numx = new double[i_value, 1];
            numy = new double[i_value, 1];
            sum_a = 0;
            sum_b = 0;

            System.Array.Clear(numx, 0, i_value);
            System.Array.Clear(numy, 0, i_value);
            System.Array.Clear(MatrixA, 0, 2 * i_value);
            System.Array.Clear(Pinv_MatrixA, 0, 2 * i_value);
            System.Array.Clear(Matrix_ab, 0, 2);
            System.Array.Clear(MatrixB, 0, i_value);

            chart1.Series.Add("Data");
            chart1.Series["Data"].ChartType = SeriesChartType.Point;

            for (int i = 0; i < i_value; i++)
            {
                numx[i, 0] = r.Next(1, i_value);
                numy[i, 0] = r.Next(1, i_value);
                MatrixA[i, 0] = numx[i, 0];
                MatrixA[i, 1] = 1;
                chart1.Series["Data"].Points.AddXY(numx[i, 0], numy[i, 0]);
                System.Threading.Thread.Sleep(3);
            }

            d_numx = new Mat(i_value, 1, MatType.CV_64FC1, numx);
            d_numy = new Mat(i_value, 1, MatType.CV_64FC1, numy);
            d_MatrixA = new Mat(i_value, 2, MatType.CV_64FC1, MatrixA);
            d_Pinv_MatrixA = new Mat(2, i_value, MatType.CV_64FC1, Pinv_MatrixA);
            d_MatrixB = new Mat(i_value, 1, MatType.CV_64FC1, MatrixB);


            Cv2.Invert(d_MatrixA, d_Pinv_MatrixA, DecompTypes.SVD);

            for (int i = 0; i < i_value; i++)
            {
                sum_a = sum_a + d_Pinv_MatrixA.At<double>(0, i) * d_numy.At<double>(i, 0);
                sum_b = sum_b + d_Pinv_MatrixA.At<double>(1, i) * d_numy.At<double>(i, 0);

            }

            Matrix_ab[0, 0] = sum_a;
            Matrix_ab[1, 0] = sum_b;
            d_Matrix_ab = new Mat(2, 1, MatType.CV_64FC1, Matrix_ab);
            chart1.Series.Add("LSM");
            chart1.Series["LSM"].ChartType = SeriesChartType.Line;

            for (int i = 0; i < i_value * 2; i++)
            {
                chart1.Series["LSM"].Points.AddXY(i, Matrix_ab[0, 0] * i + Matrix_ab[1, 0]);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RanSeries chart2 = new RanSeries(0);
            double[] xData;
            double[] yData;
            double[] zData;

            i_value = Convert.ToInt32(richTextBox1.Text);

            three_MatrixA = new double[i_value, 4];
            three_Matrix_abcd = new double[4, 1];
            s = new double[4, 1];
            u = new double[i_value, 4];
            v = new double[4, 4];
            xData = new double[i_value];
            yData = new double[i_value];
            zData = new double[i_value];



            for (int i = 0; i < i_value; i++)
            {

                three_MatrixA[i, 0] = r.Next(1, i_value);
                xData[i] = three_MatrixA[i, 0];
                three_MatrixA[i, 1] = r.Next(1, i_value);
                yData[i] = three_MatrixA[i, 1];
                three_MatrixA[i, 2] = r.Next(1, i_value);
                zData[i] = three_MatrixA[i, 2];
                three_MatrixA[i, 3] = 1;

            }


            ThreeDScatterChart c = new ThreeDScatterChart(360, 300);
            c.addTitle("3D 분산 형 차트 (1)", "Times New Roman Italic", 2);
            c.setPlotRegion(175, 140, 180, 180, 135);
            c.addScatterGroup(xData, yData, zData, "", ChartDirector.Chart.GlassSphere2Shape, 11,
            ChartDirector.Chart.SameAsMainColor);
            winChartViewer1.Chart = c;


            d_three_MatrixA = new Mat(i_value, 4, MatType.CV_64FC1, three_MatrixA);
            S = new Mat(4, 1, MatType.CV_64FC1, s);
            U = new Mat(i_value, 4, MatType.CV_64FC1, u);
            V = new Mat(4, 4, MatType.CV_64FC1, v);


            Cv2.SVDecomp(d_three_MatrixA, S, U, V);
            three_Matrix_abcd[0, 0] = V.At<double>(0, 3);
            three_Matrix_abcd[1, 0] = V.At<double>(1, 3);
            three_Matrix_abcd[2, 0] = V.At<double>(2, 3);
            three_Matrix_abcd[3, 0] = V.At<double>(3, 3);

            double[] zzzzz = new double[yData.Length * xData.Length];
            for (int i = 0; i < yData.Length; i++)
            {
                double yy = yData[i];
                for (int j = 0; j < xData.Length; j++)
                {
                    double xx = xData[j];
                    zzzzz[i * xData.Length + j] = (1 / three_Matrix_abcd[2, 0]) * (-(three_Matrix_abcd[0, 0] * xx) - (three_Matrix_abcd[0, 0] * yy) - three_Matrix_abcd[3, 0]);
                }
            }

            SurfaceChart d = new SurfaceChart(360, 300);
            d.setPlotRegion(175, 140, 180, 180, 135);
            d.setData(xData, yData, zzzzz);
            c.makeChart().merge(d.makeChart(), 0, 0, 0, 150);
            winChartViewer1.Chart = c;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            chart1.Series.Clear();
            chart1.Legends[0].Enabled = false;
            chart1.ChartAreas[0].AxisX.Title = "X";      //chart를 통해 그래프를 그리기 위한 초기 세팅
            chart1.ChartAreas[0].AxisY.Title = "Y";
           
            richTextBox2.Clear();
            

            i_value = Convert.ToInt32(richTextBox1.Text);
            N = Convert.ToInt32(richTextBox3.Text);
            threshold = Convert.ToDouble(richTextBox4.Text);

            max_cost = 0;
            max_rotation_count = Math.Log(1 - 0.99) / Math.Log(1 - Math.Pow(0.75, N));

            
            if (i_value < N)
            {
                return;
            }

            total_data = new OpenCvSharp.Point[i_value];
            sample_data = new OpenCvSharp.Point[N];
            inliers = new OpenCvSharp.Point[i_value];

            chart1.Series.Add("something");
            chart1.Series["something"].ChartType = SeriesChartType.Point;

            for(int i=0;i<i_value;i++)
            {
                total_data[i].X = i;
                total_data[i].Y = -(i * i) + 200;
                if (i >= i_value / 3 && i <= (i_value / 3) + (i_value / 5))
                {
                    total_data[i].Y = total_data[i].Y + 1500;
                }
                chart1.Series["something"].Points.AddXY(total_data[i].X, total_data[i].Y);
            }


            for(int i=0;i<(int)max_rotation_count;i++)
            {

                Get_Sample();
                make_pinv();
                compute_model();

                if(max_cost <cost)
                {
                    max_cost = cost;
                   compute_inlier();
                }
            }

            ma3 = new double[inliers_count, 3];
            mb3 = new double[inliers_count,1];
            mabc3 = new double[3,1];
            mpa3 = new double[3, inliers_count];

            for (int i = 0; i < inliers_count; i++)
            {
                ma3[i, 0] = inliers[i].X * inliers[i].X;
                ma3[i, 1] = inliers[i].X;
                ma3[i, 2] = 1;
                mb3[i,0] = inliers[i].Y;
            }
            richTextBox2.AppendText("inlie_cnt: " + inliers_count + "\n");
            richTextBox2.AppendText("inlie_cnt: " + max_cost + "\n");
            MatrixA3 = new Mat(inliers_count, 3, MatType.CV_64FC1, ma3);
            MatrixB3 = new Mat(inliers_count, 1, MatType.CV_64FC1, ma3);
           
            Matrix_PinvA3 = new Mat(3, inliers_count, MatType.CV_64FC1, mpa3);
            Cv2.Invert(MatrixA3, Matrix_PinvA3,DecompTypes.SVD);

            sum_a = 0;
            sum_b = 0;
            double sum_c = 0;

            for (int i = 0; i < inliers_count; i++)
            {
                sum_a = sum_a + Matrix_PinvA3.At<double>(0, i) * MatrixB3.At<double>(i, 0);
                sum_b = sum_b + Matrix_PinvA3.At<double>(1, i) * MatrixB3.At<double>(i, 0);
                sum_c = sum_c + Matrix_PinvA3.At<double>(2, i) * MatrixB3.At<double>(i, 0);
            }

            mabc3[0, 0] = sum_a;
            mabc3[1, 0] = sum_b;
            mabc3[2, 0] = sum_c;
            Matrix_abc3 = new Mat(3, 1, MatType.CV_64FC1,mabc3);

            chart1.Series.Add("inliers");
            chart1.Series["inliers"].ChartType = SeriesChartType.Spline;
            
            
            for (int i=0;i<i_value;i++)
            {
                double y = (sum_a * i * i) + (sum_b * i) + sum_c;
                chart1.Series["inliers"].Points.AddXY(i, y);

            }

        }

       

       
    }
}
