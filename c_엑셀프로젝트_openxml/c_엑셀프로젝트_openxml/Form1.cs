using OfficeOpenXml;
//using OfficeOpenXml.Drawing;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Spreadsheet;
//using DocumentFormat.OpenXml;
//using DocumentFormat.OpenXml.Drawing.Charts;
//using System.Xml;
using System.Windows.Forms.DataVisualization.Charting;
//using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;


// 주의! EPPLUS 패키지를 추가해야함
// Excel 패키지를 사용해야 함
//ExcelPackage.LicenseContext = LicenseContext.Commercial;를 해주어야 되는 데, 이게 안되는 경우 app.config에서 
//  <appSettings>
//    <!--The license context used-->
//    <add key = "EPPlus:ExcelPackage.LicenseContext" value="Commercial" />
//  </appSettings> //이 부분을 추가해 주어야 excelpackage license가 commercial(상업)용임을 지정할 수 있음

namespace c_엑셀프로젝트_openxml
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        string root_dir = "D:\\"; //최초 folder를 오픈할 때, 사용하는 초기 위치 값 및 파일을 저장할 공간
        
        string[] folders; // folder명을 저장할 배열
        int folder_counter = 0; //folder의 갯수

        string[] file;      //file을 보관할 배열
        string file_dir;    //file의 위치를 보관
        string save_path;

        string cmpline = "QualityControl"; // csv파일 중 quality control 파일 제외, **차후 quality control을 받지 않는 방법 모색 예정
        string line;    //file에서 읽은 1줄을 보관할 변수

        int row_cnt = 3;    // sampling 값의 초기 행 좌표
        int col_cnt = 2;    //sampling 값의 초기 열 좌표
        int csv_cnt = -1;   //sampling한 csv 파일의 번호(갯수)를 나타내는 변수

        int Index_check = 0; //Index 값이 나오는 기점으로 sampling 방식을 달리할 flag 변수
        int first_check = 0; //Index 값 출몰 시점은 string값, 나머지는 숫자값 따라서 다른 추출 방식을 사용할 flag 변수

        double distance_value; //그래프의 x좌표인 distance 값을 보관할 공간
        double force_value;  //그래프의 y좌표인 force 값을 보관할 공간

        ExcelPackage pck;                   // excel 기능을 위한 package 생성
        ExcelWorksheet activitiesWorksheet, activitiesWorksheet2; // excelsheet 생성

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = root_dir; // folder 오픈 위치를 지정
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) // folder를 열어
            {
                if (System.IO.Directory.Exists(folderBrowserDialog1.SelectedPath)) // 해당 경로가 존재하는 경우
                {
                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(folderBrowserDialog1.SelectedPath); // 해당 folder의 정보를 스캔
                    file_dir = folderBrowserDialog1.SelectedPath + "\\"; //file_dir는 스캔한 위치에서 더 하위 폴더로 들어가기 위한 "\"를 추가
                    foreach (var folder in di.GetDirectories()) // 하위 폴더 갯수를 count
                    {
                        folder_counter++;
                    }

                    folders = new string[folder_counter]; //그 만큼 folder 배열의 크기를 할당
                    folder_counter = 0;                    //folder 배열 검색을 위해 초기화 한번 실행

                    foreach (var folder in di.GetDirectories()) // 모든 folder를 뒤져서 folder배열의 해당 폴더의 이름을 저장
                    {
                        folders[folder_counter] = folder.Name;
                        folder_counter++;
                    }
                }
            }
            folder_counter = 0;
        }


        public static Image resizeImage(Image image, string dir)
        {

            if (image != null)
            {
                Bitmap croppedBitmap = new Bitmap(dir + ".topimage"); // 파일명을 이용한 원본 파일 불러오기
                Bitmap originalmap = croppedBitmap;
                croppedBitmap = croppedBitmap.Clone(
                        new RectangleF(image.Width / 2 - 160, image.Height / 2 - 160, 320, 320),
                        System.Drawing.Imaging.PixelFormat.DontCare); // 자르기
                return croppedBitmap;
            }
            else
            {
                return image;
            }
        }

        private void CreateSpreadsheet()        //스프레드 시트 생성
        {
            folderBrowserDialog2.SelectedPath = root_dir; // folder 오픈 위치를 지정
            if (folderBrowserDialog2.ShowDialog() == DialogResult.OK) // folder를 열어
            {
                if (System.IO.Directory.Exists(folderBrowserDialog2.SelectedPath)) // 해당 경로가 존재하는 경우
                {
                    System.IO.DirectoryInfo di2 = new System.IO.DirectoryInfo(folderBrowserDialog2.SelectedPath); // 해당 folder의 정보를 스캔
                    save_path = folderBrowserDialog2.SelectedPath;//file_dir는 스캔한 위치에서 더 하위 폴더로 들어가기 위한 "\"를 추가
                }
            }
            string spreadsheetPath = save_path +"\\"+ "PUTTER_Shear_data1.xlsx"; // 저장 좌표 + 파일명을 지정
            File.Delete(spreadsheetPath);                           //파일 clear하고
            FileInfo spreadsheetInfo = new FileInfo(spreadsheetPath); // 파일 생성

            pck = new ExcelPackage(spreadsheetInfo);                    //excel기능을 활용하기 위한 패키지 생성
            activitiesWorksheet = pck.Workbook.Worksheets.Add("RAW DATA"); // worksheet의 이름 지정
            activitiesWorksheet2 = pck.Workbook.Worksheets.Add("Sheet1"); // worksheet의 이름 지정
            activitiesWorksheet.View.FreezePanes(2, 2);                    // 초기 위치를 뜻하는 듯?
            activitiesWorksheet2.View.FreezePanes(3, 3);
        }



        private void button2_Click(object sender, EventArgs e)
        {
            CreateSpreadsheet();                                    //스프레드 시트 생성
            chart1.Series.Clear();                                  //최초에 존재하는 차트를 날림 (지움)
            chart1.Legends[0].Enabled = false;
            chart1.ChartAreas[0].AxisX.Title = "Distance[um]";      //chart를 통해 그래프를 그리기 위한 초기 세팅
            chart1.ChartAreas[0].AxisY.Title = "Force[g]";

            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = 45;
            chart1.ChartAreas[0].AxisX.Interval = 10;
            chart1.ChartAreas[0].AxisX.IntervalOffset = 0;

            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Maximum = 25;
            chart1.ChartAreas[0].AxisY.Interval = 10;
            chart1.ChartAreas[0].AxisY.IntervalOffset = 0;

            activitiesWorksheet2.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            activitiesWorksheet2.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
           
            activitiesWorksheet2.Cells[2, 3].Value = "위치";
            activitiesWorksheet2.Cells[2, 4].Value = "Name";
            activitiesWorksheet2.Cells[2, 5].Value = "Max Force";
            activitiesWorksheet2.Cells["F2:N2"].Merge = true;
            activitiesWorksheet2.Cells[2, 6].Value = "Graph";
            activitiesWorksheet2.Cells["O2:R2"].Merge = true;
            activitiesWorksheet2.Cells[2, 15].Value = "Image";

            for (int l = 0; l < folders.Length; l++) //폴더 탐색
            {
                file = System.IO.Directory.GetFiles(file_dir + folders[l], "*.csv"); //전체 폴더의 하위 폴더를 탐색

                string[] cmplist = { "Chip", "Measurement", "Max Force" }; //list를 통해 걸러낼 문장(item)을 선택
                var list = new List<string>();
                list.AddRange(cmplist); //해당 리스트를 리스트를 생성해서 추가 
               

                string folder_name = folders[l];// chart legend를 위해 folder_name을 활용 예정

               
                for (int i = 0; i < file.Length; i++) //파일 탐색
                {

                    if (file[i].Contains(cmpline)) continue; // quality control csv가 나온경우 
                    else                                     // 원하는 CSV 파일이 나온 경우
                    {

                        string file_name = Path.GetFileName(file[i]);   //파일이름을 추출
                        chart1.Series.Add(folder_name + ")" + file_name);   //chart legend를 폴더명 + 파일명으로 나타냄

                        csv_cnt++;                                      //csv파일의 갯수 count -> 그것으로 excel 파일의 열 위치를 지정
                        row_cnt = 3;                                    //행 초기화
                        col_cnt = 2;                                    //열 초기화 -> csv_cnt와 같이 사용
                        first_check = 0;                                //Index line 출현 이후, 처음은 value가 아니기 때문에 flag로 구분 
                        Index_check = 0;                                //Index line 출현 여부를 flag로 구분


                        activitiesWorksheet2.Cells[3 + csv_cnt * 10, 3, 3 + (csv_cnt + 1) * 10 - 1, 3].Merge = true; //셀 병합
                        activitiesWorksheet2.Cells[3 + csv_cnt * 10, 4, 3 + (csv_cnt + 1) * 10 - 1, 4].Merge = true;
                        activitiesWorksheet2.Cells[3 + csv_cnt * 10, 5, 3 + (csv_cnt + 1) * 10 - 1, 5].Merge = true;
                        activitiesWorksheet2.Cells[3 + csv_cnt * 10, 6, 3 + (csv_cnt + 1) * 10 - 1, 14].Merge = true;
                        activitiesWorksheet2.Cells[3 + csv_cnt * 10, 15, 3 + (csv_cnt + 1) * 10 - 1, 18].Merge = true;


                        System.IO.StreamReader readfile = new System.IO.StreamReader(file[i], System.Text.Encoding.Default); //해당 csv파일을 읽어옴


                        while ((line = readfile.ReadLine()) != null) 
                        {
                            if (line.Contains("Index"))//"Index"전까지는 원하는 값만 추출, Index 이후로는 모든 제원을 추출하기 위함
                            {
                                Index_check = 1; //Index 탐색 여부
                                col_cnt = 2; // 앞에서 Chip,Measurement, Max Force를 처리하느라 col_cnt를 사용했기 때문에 초기화
                                first_check = 0; // 첫줄 index, time등은 convert처리가 필요없기 때문에 체크용

                                var pic2 = activitiesWorksheet2.Drawings.AddPicture("FD" + csv_cnt, Bitmap.FromFile(file_dir + folder_name + "\\" + Path.GetFileNameWithoutExtension(file[i]) + "_FDgraph.png"));
                                var pic3 = activitiesWorksheet2.Drawings.AddPicture("TOP" + csv_cnt, resizeImage(Bitmap.FromFile(file_dir + folder_name + "\\" + Path.GetFileNameWithoutExtension(file[i]) + ".topimage"), file_dir + folder_name + "\\" + Path.GetFileNameWithoutExtension(file[i])));
                                pic2.SetPosition(3 + csv_cnt * 10, 0, 6, 0);
                                pic2.SetSize(40);
                                pic3.SetPosition(3 + csv_cnt * 10, 0, 15, 0);
                                pic3.SetSize(40);
                            }

                            if (Index_check == 0) //Index 전에는 chip , measure, max만 추출
                            {
                                foreach (string s in list)  //list의 문장들과 비교해
                                {
                                    if (line.Contains(s))   //list의 문장이 존재하는 경우 
                                    {
                                        var replacement = line.Replace("Chip,", "");
                                        var replacement2 = replacement.Replace("MeasurementBump,", "");
                                        var replacement3 = replacement2.Replace("Max Force, , ", "");
                                        
                                        activitiesWorksheet.Cells[2, col_cnt + csv_cnt * 5 + 1].Value = replacement3; //그 값을 sheet에 추가함
                                        activitiesWorksheet2.Cells[3 + csv_cnt * 10, col_cnt + 1].Value = replacement3;


                                        col_cnt++;
                                    }
                                }

                            }
                            else //Index이후는 모든 라인을 추출
                            {
                                string[] split1 = line.Split(new string[] { "," }, StringSplitOptions.None); //,를 기준으로 값을 분할해서 index,time,distance,force값을 저장
                                foreach (string k in split1)
                                {
                                    activitiesWorksheet.Cells[row_cnt, col_cnt + csv_cnt * 5].Value = k;
                                    col_cnt++;

                                    if (first_check == 1) // 첫번째 줄은 모든 index, time, dist등 string요소 -> 나머지는 double요소 이기때문에 첫줄을 생략하기 위한 체크
                                    {                   // 첫번째 이외의 나머지는 convert함수를 통해 string을 double로 변환시켜주어야 원하는 데이터를 얻을 수 있음
                                        if (col_cnt == 5) //dist값을 저장할 건데 행이 4부터 시작이니까 -3을 해줌
                                        {
                                            distance_value= Convert.ToDouble(k);
                                        }
                                        if (col_cnt == 6)   //force값을 저장할 건데 행이 4부터 시작이니까 -3을 해줌
                                        {
                                            force_value = Convert.ToDouble(k);
                                        }
                                    }

                                }
                                if (first_check == 1)// 첫번째 줄은 모든 index, time, dist등 string요소 -> 나머지는 double요소 이기때문에 첫줄을 생략하기 위한 체크
                                {                   // 첫번째 이외의 나머지는 convert함수를 통해 string을 double로 변환시켜주어야 원하는 데이터를 얻을 수 있음
                                    chart1.Series[folder_name + ")" + file_name].Points.AddXY(distance_value, force_value);
                                    chart1.Series[folder_name + ")" + file_name].ChartType = SeriesChartType.Line;
                                }
                                first_check = 1;
                                row_cnt++;
                                col_cnt = 2;
                            }

                        }
                    }

                }
                pck.Save(); // 한 파일 내용이 끝날 때 마다 내용을 저장 --> 맨 마지막에 한번 만 해도 상관은 없음
            }
            this.chart1.SaveImage(root_dir + "\\mychart.png", ChartImageFormat.Png); // chart 완성후 이미지 저장
            var img = Image.FromFile(root_dir + "\\mychart.png");                   //그 이미지를 불러와 drawing 함
                                                                                    // var img2 = chart1;
                                                                                    // var pic2 = activitiesWorksheet.Drawings.AddChart("CSV_Image2", chart1); //* 차트 이미지를 바로 불러올 수 있을 까??
            var pic = activitiesWorksheet.Drawings.AddPicture("CSV_Image", img);
            pic.SetPosition(10, 0, 10, 0);
            pic.SetSize(50);
            pck.Save();
            MessageBox.Show("all done");
        }
    }
}
