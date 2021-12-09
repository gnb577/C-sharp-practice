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
using System.Windows.Forms.DataVisualization.Charting;

using System.Drawing.Drawing2D;

namespace C_최소자승법_RANSAC
{
    public partial class Form1 : Form
    {
        Random r = new Random();
        double[] d_numx;
        double[] d_numy;
        double[,] d_S;
        double[,] d_V;
        double[,] d_D;
        int i_value;
        double[,] d_min_array;
        double[,] d_rev_array;
        double[,] d_array;
        double[] d_unknown;
        double[] d_result;
        double[] d_result2;
        int n_count = 0;
        double n_coefficient = 0;
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

            i_value = Convert.ToInt32(textBox1.Text);
            d_numx = new double[i_value];
            d_numy = new double[i_value];
            d_S = new double[i_value, i_value];
            d_V = new double[i_value, 2];
            d_D = new double[2,2];

            d_min_array = new double[i_value, 2];
            d_rev_array = new double[2, 2];
            d_array = new double[2, 2];
            d_unknown = new double[2];


            d_result = new double[2];
            d_result2 = new double[2];

            System.Array.Clear(d_numx, 0, i_value);
            System.Array.Clear(d_numy, 0, i_value);
            System.Array.Clear(d_min_array, 0, 2 * i_value);
            System.Array.Clear(d_array, 0, 2 * 2);
            System.Array.Clear(d_rev_array, 0, 2*2);
            System.Array.Clear(d_result, 0, 2);
            System.Array.Clear(d_result2, 0, 2);

            chart1.Series.Add("Data");

            for (int i=0;i< i_value; i++)
            {
                d_numx[i] = r.Next(1, i_value);
                d_numy[i] = r.Next(1, i_value);

                d_min_array[i, 0] = d_numx[i];
                d_min_array[i, 1] = 1;


                chart1.Series["Data"].Points.AddXY(d_numx[i], d_numy[i]);
                chart1.Series["Data"].ChartType = SeriesChartType.Point;

                System.Threading.Thread.Sleep(10);
            }
                chart1.Series.Add("LSM");

            for (int k = 0; k < i_value; k++)
            {
                d_array[0, 0] = d_array[0, 0] + d_min_array[k, 0] * d_min_array[k, 0];
                d_array[0, 1] = d_array[0, 1] + d_min_array[k, 0];
                d_array[1, 0] = d_array[1, 0] + d_min_array[k, 0];
                d_array[1, 1] = d_array[1, 1] + 1;
            }

            for(int i=0;i< i_value; i++)
            {
                for(int j=0; j<i_value;j++)
                {
                    
                }
            }

            /*
                    for (int k = 0; k < value; k++)
                    {
                        array[0, 0] = array[0, 0] + min_array[k, 0]*min_array[k,0];
                        array[0, 1] = array[0, 1] + min_array[k, 0];
                        array[1, 0] = array[1, 0] + min_array[k, 0];
                        array[1, 1] = array[1, 1] + 1;
                        result[0] = result[0] + min_array[k, 0] * numy[k];
                        result[1] = result[1] + numy[k];
            
                    }
                        coefficient = 1 /((array[0, 0] * array[1, 1]) - (array[0, 1] * array[1, 0]));
                        rev_array[0, 0] = coefficient * array[1, 1];
                        rev_array[0, 1] = -coefficient * array[0, 1];
                        rev_array[1, 0] = -coefficient * array[1, 0];
                        rev_array[1, 1] = coefficient * array[0, 0];

                        result2[0] = (rev_array[0, 0] * result[0] + rev_array[0, 1] * result[1]);
                        result2[1] = (rev_array[1, 0] * result[0] + rev_array[1, 1] * result[1]);
            
            for(int i=0;i<2*value;i++)
            {
                double y = result2[0] * i + result2[1];
                chart1.Series["LSM"].Points.AddXY(i, y);
                chart1.Series["LSM"].ChartType = SeriesChartType.Line;
            }
            */


        }
    }
}
