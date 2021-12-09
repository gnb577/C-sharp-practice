
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
using DocumentFormat.OpenXml.Bibliography;

namespace C_port_제어_piezo
{
    public partial class Form1 : Form
    {
        string vol_interval;
        string vol_value = "0";
        int vol_interval_int;
        int vol_value_int;
        int last_value_flag;
        


        public Form1()
        {
            InitializeComponent();
            for (int i = 1; i <= 13; i++)
            {
                comboBox1.Items.Add(i.ToString());
            }
            textBox1.Text = "50";
            textBox2.Text = "1000";
        }

     

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            this.Invoke(new EventHandler(MySerialReceived));
        }
        private void MySerialReceived(object s, EventArgs e)  //여기에서 수신 데이타를 사용자의 용도에 따라 처리한다.
        {
            int ReceiveData = serialPort1.ReadByte();  //시리얼 버터에 수신된 데이타를 ReceiveData 읽어오기
            richTextBox2.AppendText(string.Format("{0}\n", ReceiveData));
            richTextBox2.ScrollToCaret();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            vol_interval = textBox1.Text;
            vol_interval_int = Convert.ToInt32(vol_interval);
            vol_value_int = 0;
            last_value_flag = 0;
            richTextBox2.Clear();


            if (!serialPort1.IsOpen)  //시리얼포트가 열려 있지 않으면
            {
                serialPort1.PortName = "COM" + comboBox1.Text;  //콤보박스의 선택된 COM포트명을 시리얼포트명으로 지정
                serialPort1.BaudRate = 19200;  //보레이트 변경이 필요하면 숫자 변경하기
                serialPort1.DataBits = 8;
                serialPort1.StopBits = StopBits.One;
                serialPort1.Parity = Parity.None;
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived); //이것이 꼭 필요하다

                serialPort1.Open();  //시리얼포트 열기

                MessageBox.Show("포트가 열렸습니다.");
                comboBox1.Enabled = false;  //COM포트설정 콤보박스 비활성화
            }

            timer1.Interval = Convert.ToInt32(textBox2.Text);
            serialPort1.WriteLine(string.Format("vset 0\r\n"));

            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            vol_value_int = vol_value_int + vol_interval_int;
            vol_value = Convert.ToString(vol_value_int);


            if (vol_value_int > 1500)
            {
               
                string ReceiveData2 = serialPort1.ReadLine();  //시리얼 버터에 수신된 데이타를 ReceiveData 읽어오기
                                                              //richTextBox2.Text = richTextBox2.Text + string.Format("value: {0}\n", ReceiveData);  //int 형식을 string형식으로 변환하여 출력
                richTextBox2.AppendText(string.Format("vset: {0}\n", ReceiveData2));
              
                serialPort1.Close();
                timer1.Stop();
                timer1.Enabled = false;
                MessageBox.Show("사용한 포트를 닫습니다.");
                return;
            }

            richTextBox1.Text = string.Format("vset {0}\n", vol_value);
            serialPort1.WriteLine(string.Format("vset {0}\r\n", vol_value));
            
            string ReceiveData = serialPort1.ReadLine();  //시리얼 버터에 수신된 데이타를 ReceiveData 읽어오기
                                                          //richTextBox2.Text = richTextBox2.Text + string.Format("value: {0}\n", ReceiveData);  //int 형식을 string형식으로 변환하여 출력
            richTextBox2.AppendText(string.Format("vset: {0}\n", ReceiveData));
            richTextBox2.ScrollToCaret();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            vol_interval = textBox1.Text;
            vol_interval_int = Convert.ToInt32(vol_interval);
            vol_value_int = 0;
            if (!serialPort1.IsOpen)  //시리얼포트가 열려 있지 않으면
            {
                serialPort1.PortName = "COM" + comboBox1.Text;  //콤보박스의 선택된 COM포트명을 시리얼포트명으로 지정
                serialPort1.BaudRate = 19200;  //보레이트 변경이 필요하면 숫자 변경하기
                serialPort1.DataBits = 8;
                serialPort1.StopBits = StopBits.One;
                serialPort1.Parity = Parity.None;
                serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived); //이것이 꼭 필요하다

                serialPort1.Open();  //시리얼포트 열기
                comboBox1.Enabled = false;  //COM포트설정 콤보박스 비활성화
            }

            serialPort1.WriteLine(string.Format("vset {0}\r\n", vol_interval));
            richTextBox1.Text = string.Format("vset {0}\n", vol_interval);
            string ReceiveData = serialPort1.ReadLine();  //시리얼 버터에 수신된 데이타를 ReceiveData 읽어오기
            richTextBox2.Text = richTextBox2.Text + string.Format("-> {0}\n", ReceiveData);  //int 형식을 string형식으로 변환하여 출력
            richTextBox2.ScrollToCaret();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine(string.Format("vset 0\r\n"));
            richTextBox2.Clear();
        }
    }
}
