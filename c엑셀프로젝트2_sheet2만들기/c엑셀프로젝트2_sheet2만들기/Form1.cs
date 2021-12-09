using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms.DataVisualization.Charting;


namespace c엑셀프로젝트2_sheet2만들기
{
    public partial class Form1 : Form
    {
        static Excel.Application excelApp = null;
        static Excel.Workbook workBook = null;
        static Excel.Worksheet workSheet = null;
        Excel.Range range = null;

        const string rootdir = @"E:\new_folder\";
        string dir;
        string idx;
        string cmpline = "QualityControl";

        
      


        string[] files;
        string[] refiles = new string[3000];
        int count_refiles = 0;
        
        string []distance_idx = new string[3000];
        string[] force_idx = new string[3000];

        double[] distance_num = new double[10000];
        double[] force_num = new double[10000];


        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < 2000; i++)
            {
                refiles[i] = null;
            }
        }
        int csv_cnt = -1;
        private void button1_Click(object sender, EventArgs e)
        {
            string[] cmplist = { "Chip", "Measurement", "Max Force" }; //list를 통해 걸러낼 문장(item)을 선택
            var list = new List<string>();
            list.AddRange(cmplist); //해당 리스트를 리스트를 생성해서 추가
            StringBuilder sb = new StringBuilder(); //string []sb랑 비슷한 느낌(malloc으로 할당된?)
            StringBuilder sb2 = new StringBuilder(); //string []sb랑 비슷한 느낌(malloc으로 할당된?)
            string line;

            string path2 = Path.Combine(rootdir, "Excel5.xlsx"); // 엑셀 파일 저장 경로 
            excelApp = new Excel.Application(); // 엑셀 어플리케이션 생성
            workBook = excelApp.Workbooks.Add(); // 워크북 추가 
            workSheet = workBook.Worksheets.get_Item(1) as Excel.Worksheet; // 엑셀 첫번째 워크시트 가져오기


            chart1.Series["Series1"].Points.Clear();
            folderBrowserDialog1.SelectedPath = rootdir;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                files = System.IO.Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.csv");
                dir = folderBrowserDialog1.SelectedPath + "\\";
            }

            int row_cnt = 2;
            int col_cnt = 2;
            //int csv_cnt = -1;
            int first_line_check = 0;
            for (int i = 0; i < files.Length; i++)
            {
                
                bool stringcmp = files[i].Contains(cmpline); //csv파일 중 뽑고 싶은 것만 뽑기 위해 contains 사용

                if (stringcmp == true) continue;
                else
                {
                    
                    
                    refiles[count_refiles] = files[i];
                    System.IO.StreamReader readfile = new System.IO.StreamReader(refiles[count_refiles], System.Text.Encoding.Default);
                    count_refiles++;
                    row_cnt = 3;
                    col_cnt = 2;
                    int cnt = 0;
                    first_line_check = 0;
                    csv_cnt++;
                    chart1.Series.Add("Series"+ csv_cnt + 1);
                    // workSheet.Cells[3, col_cnt - 1] = "Index";
                    // workSheet.Cells[3, col_cnt] = "Time[s]";
                    // workSheet.Cells[3, col_cnt + 1] = "Dist[um]";
                    // workSheet.Cells[3, col_cnt + 2] = "Force[g]";
                    chart1.Series[csv_cnt].Points.Clear();
                    while ((line = readfile.ReadLine()) != null)
                    {

                        if (line.Contains("Index"))
                        { cnt = 1; col_cnt = 2; first_line_check = 0; }
                        if (cnt == 0)
                        {
                            foreach (string s in list)
                            {
                                if (line.Contains(s))
                                {
                                    var replacement = line.Replace("Chip,","");
                                    var replacement2 = replacement.Replace("MeasurementBump,", "");
                                    var replacement3 = replacement2.Replace("Max Force, , ", "");
                                    //if(line.Contains("MeasurementBump,"))
                                    //{
                                    //    replacement3 = replacement3.Insert(4, ",");
                                    //}

                                    workSheet.Cells[2, col_cnt + csv_cnt * 5 + 1] = replacement3;
                                    col_cnt++;
                                }
                            }
                        }
                        else
                        {
                            string[] split1 = line.Split(new string[] { "," }, StringSplitOptions.None);
                            foreach (string k in split1)
                            {
                                workSheet.Cells[row_cnt, col_cnt + csv_cnt*5] = k;
                                col_cnt++;
                                if (first_line_check == 1)
                                {
                                    
                                    if (col_cnt == 5)
                                    {
                                        //int a = Convert.ToInt32(k);
                                        distance_num[row_cnt - 3] = Convert.ToDouble(k);
                                        textBox1.Text = distance_num[row_cnt - 3].ToString();
                                    }
                                    if (col_cnt == 6)
                                    {
                                        force_num[row_cnt - 3] = Convert.ToDouble(k);
                                    }
                                   
                                }
                                
                            }
                            if (first_line_check == 1)
                            {
                                chart1.Series[csv_cnt].Points.AddXY(distance_num[row_cnt - 3], force_num[row_cnt - 3]);
                                chart1.Series[csv_cnt].ChartType = SeriesChartType.Line;
                            }
                            first_line_check = 1;
                            row_cnt++;
                            col_cnt = 2;
                            
                        }
                    }
                    //row_cnt++;
                    // col_cnt= col_cnt+3;

                    
                }

               
              
            }

            workSheet.Columns.AutoFit(); // 열 너비 자동 맞춤
            workBook.SaveAs(path2, Excel.XlFileFormat.xlWorkbookDefault); // 엑셀 파일 저장
            workBook.Close(true);
            excelApp.Quit();
        }


    }


     
 }

