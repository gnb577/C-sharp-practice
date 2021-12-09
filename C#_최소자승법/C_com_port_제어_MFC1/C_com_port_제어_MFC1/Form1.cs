using OfficeOpenXml;
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
using System.IO.Ports;  //시리얼통신을 위해 추가해줘야 함
using System.Threading;


namespace C_com_port_제어_MFC1
{
    public partial class Form1 : Form
    {
        string led_num;
        string vol_interval;
        string vol_value = "0";
        int vol_interval_int;
        int vol_value_int;

        int channel_flag = 0;

        string root_dir = "D:\\"; //최초 folder를 오픈할 때, 사용하는 초기 위치 값 및 파일을 저장할 공간
        string save_path;

        ExcelPackage pck;                   // excel 기능을 위한 package 생성
        ExcelWorksheet activitiesWorksheet; // excelsheet 생성

        private void CreateSpreadsheet()        //스프레드 시트 생성
        {

            folderBrowserDialog1.SelectedPath = root_dir; // folder 오픈 위치를 지정
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) // folder를 열어
            {
                if (System.IO.Directory.Exists(folderBrowserDialog1.SelectedPath)) // 해당 경로가 존재하는 경우
                {
                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(folderBrowserDialog1.SelectedPath); // 해당 folder의 정보를 스캔
                    save_path = folderBrowserDialog1.SelectedPath;//file_dir는 스캔한 위치에서 더 하위 폴더로 들어가기 위한 "\"를 추가
                }
            }
            string spreadsheetPath = save_path + "\\" + textBox3.Text + ".xlsx"; // 저장 좌표 + 파일명을 지정
            File.Delete(spreadsheetPath);                           //파일 clear하고
            FileInfo spreadsheetInfo = new FileInfo(spreadsheetPath); // 파일 생성

            pck = new ExcelPackage(spreadsheetInfo);                    //excel기능을 활용하기 위한 패키지 생성
            activitiesWorksheet = pck.Workbook.Worksheets.Add(textBox3.Text); // worksheet의 이름 지정
        }


        private void button3_Click(object sender, EventArgs e)
        {
            CreateSpreadsheet();
            activitiesWorksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            activitiesWorksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            pck.Save();
        }

        public Form1()
        {
            InitializeComponent();
            serialPort1.Close();
            for (int i = 1; i <= 13; i++)
            {
                comboBox1.Items.Add(i.ToString());
            }
            for (int i = 0; i <= 2; i++)
            {
                comboBox2.Items.Add((9600*i).ToString());
                
            }
            for (int i = 0; i <= 3; i++)
            {
                comboBox3.Items.Add(i.ToString());
            }
            textBox1.Text = "50";
            textBox2.Text = "3500";
           
        }

        private void button4_Click(object sender, EventArgs e)
        {
            led_num = comboBox3.Text;
            vol_interval = textBox1.Text;
            vol_interval_int = Convert.ToInt32(vol_interval);
            vol_value_int = 0;

            if (!serialPort1.IsOpen)  //시리얼포트가 열려 있지 않으면
            {
                serialPort1.PortName = "COM" + comboBox1.Text;  //콤보박스의 선택된 COM포트명을 시리얼포트명으로 지정
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);  //보레이트 변경이 필요하면 숫자 변경하기
                serialPort1.DataBits = 8;
                serialPort1.StopBits = StopBits.One;
                serialPort1.Parity = Parity.None;
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived); //이것이 꼭 필요하다

                serialPort1.Open();  //시리얼포트 열기
                comboBox1.Enabled = false;  //COM포트설정 콤보박스 비활성화
            }

            serialPort1.WriteLine(string.Format("led {0} {1}\r\n",led_num , vol_interval));
            richTextBox1.Text = string.Format("led {0} {1}\n", led_num, vol_interval);
            string ReceiveData = serialPort1.ReadLine();  //시리얼 버터에 수신된 데이타를 ReceiveData 읽어오기
            richTextBox2.Text = richTextBox2.Text + string.Format("-> {0}\n", ReceiveData);  //int 형식을 string형식으로 변환하여 출력
            richTextBox2.ScrollToCaret();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //button1
            led_num = comboBox3.Text;
            vol_interval = textBox1.Text;
            vol_interval_int = Convert.ToInt32(vol_interval);
            vol_value_int = 0;
            richTextBox2.Clear();

            if (!serialPort1.IsOpen)  //시리얼포트가 열려 있지 않으면
            {
                serialPort1.PortName = "COM"+comboBox1.Text;  //콤보박스의 선택된 COM포트명을 시리얼포트명으로 지정
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);  //보레이트 변경이 필요하면 숫자 변경하기
                serialPort1.DataBits = 8;
                serialPort1.StopBits = StopBits.One;
                serialPort1.Parity = Parity.None;
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived); //이것이 꼭 필요하다

                serialPort1.Open();  //시리얼포트 열기

                MessageBox.Show("포트가 열렸습니다.");
                comboBox1.Enabled = false;  //COM포트설정 콤보박스 비활성화
            }

            timer1.Interval =  Convert.ToInt32(textBox2.Text);
            serialPort1.WriteLine(string.Format("led {0} {1}\r\n", led_num, "0"));
           
            timer1.Start();
            

        }
 

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            this.Invoke(new EventHandler(MySerialReceived));
        }
        private void MySerialReceived(object s, EventArgs e)  //여기에서 수신 데이타를 사용자의 용도에 따라 처리한다.
        {
            int ReceiveData = serialPort1.ReadByte();  //시리얼 버터에 수신된 데이타를 ReceiveData 읽어오기
            //richTextBox2.Text = richTextBox2.Text + string.Format("{0}\n", ReceiveData);  //int 형식을 string형식으로 변환하여 출력
            richTextBox2.AppendText(string.Format("{0}\n", ReceiveData));
            richTextBox2.ScrollToCaret();
        }
       
     
        private void timer1_Tick(object sender, EventArgs e)
        {

            vol_value_int = vol_value_int + vol_interval_int;
            vol_value = Convert.ToString(vol_value_int);


            if (vol_value_int > 1000)
            {
                serialPort1.WriteLine(string.Format("led {0} {1}\r\n", led_num, "0"));
                serialPort1.Close();
                timer1.Stop();
                timer1.Enabled = false;
                MessageBox.Show("사용한 포트를 닫습니다.");
                return;
            }

  
                serialPort1.WriteLine(string.Format("led {0} {1}\r\n", led_num, vol_value));
                richTextBox1.Text = string.Format("led {0} {1}\n", led_num, vol_value);
                string ReceiveData = serialPort1.ReadLine();  //시리얼 버터에 수신된 데이타를 ReceiveData 읽어오기
                //richTextBox2.Text = richTextBox2.Text + string.Format("value: {0}\n", ReceiveData);  //int 형식을 string형식으로 변환하여 출력
                richTextBox2.AppendText(string.Format("value: {0}\n", ReceiveData));
                richTextBox2.ScrollToCaret();


        }

        private void button2_Click(object sender, EventArgs e)
        {

            vol_interval = textBox1.Text;
            vol_interval_int = Convert.ToInt32(vol_interval);
            vol_value_int = vol_interval_int;
            led_num = "0";
            vol_value = Convert.ToString(vol_value_int);
            channel_flag = 0;
            richTextBox2.Clear();
            if (!serialPort1.IsOpen)  //시리얼포트가 열려 있지 않으면
            {
                serialPort1.PortName = "COM" + comboBox1.Text;  //콤보박스의 선택된 COM포트명을 시리얼포트명으로 지정
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);  //보레이트 변경이 필요하면 숫자 변경하기
                serialPort1.DataBits = 8;
                serialPort1.StopBits = StopBits.One;
                serialPort1.Parity = Parity.None;
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived); //이것이 꼭 필요하다

                serialPort1.Open();  //시리얼포트 열기

                MessageBox.Show("포트가 열렸습니다.");
                comboBox1.Enabled = false;  //COM포트설정 콤보박스 비활성화
            }



            timer3.Interval = Convert.ToInt32(textBox2.Text);
            serialPort1.WriteLine(string.Format("led {0} {1}\r\n", "0", "0"));
            serialPort1.WriteLine(string.Format("led {0} {1}\r\n", "1", "0"));
            serialPort1.WriteLine(string.Format("led {0} {1}\r\n", "2", "0"));
            serialPort1.WriteLine(string.Format("led {0} {1}\r\n", "3", "0"));
            timer3.Start();

        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            
            if (channel_flag == 4)
            {
                vol_value_int = vol_value_int + vol_interval_int;
                vol_value = Convert.ToString(vol_value_int);
                channel_flag = 0;
            }

            if (vol_value_int > 1000)
            {
                serialPort1.WriteLine(string.Format("led {0} {1}\r\n", led_num, "0"));
                serialPort1.Close();
                timer3.Stop();
                timer3.Enabled = false;
                MessageBox.Show("사용한 포트를 닫습니다.");

                return;
            }

            serialPort1.WriteLine(string.Format("led {0} {1}\r\n", channel_flag, vol_value));
            richTextBox1.Text = string.Format("led {0} {1}\n", channel_flag, vol_value);
            string ReceiveData = serialPort1.ReadLine();  //시리얼 버터에 수신된 데이타를 ReceiveData 읽어오기
            richTextBox2.Text = richTextBox2.Text + string.Format("value: {0}\n", ReceiveData);  //int 형식을 string형식으로 변환하여 출력
            channel_flag++;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine(string.Format("led {0} {1}\r\n", "0", "0"));
            serialPort1.WriteLine(string.Format("led {0} {1}\r\n", "1", "0"));
            serialPort1.WriteLine(string.Format("led {0} {1}\r\n", "2", "0"));
            serialPort1.WriteLine(string.Format("led {0} {1}\r\n", "3", "0"));
            
            richTextBox2.Clear();
            
           
            
        }
    }
}
