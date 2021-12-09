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

namespace missed_log_찾기
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            int counter = 0;
            string line;
            string cmpline = comboBox1.SelectedItem as string;
            //string cmpline = "Missed";

            /*
            string []cmpline = { "Missed Frames: ", "FrameRate", "Snap done", "Tact Time"} ;

            var list = new List<string>();
            list.AddRange(cmpline);
            */


            System.IO.StreamReader file = new System.IO.StreamReader(@"E:\mapping.txt",System.Text.Encoding.Default);

            //FileStream fs2 = new FileStream("mappingtest2.txt", FileMode.Create);
            StreamWriter sw2 = new StreamWriter(@"E:\mappingtest2.txt", false, System.Text.Encoding.Default);

            // Read the file and display it line by line.  
           
            cmpline = comboBox1.SelectedItem as string;
            while ((line = file.ReadLine()) != null)
            {
                if (line.Contains(cmpline))
                { 
                    sw2.WriteLine(line);
                    Console.WriteLine(line);
                }
                counter++;
            }

            
            file.Close();
            sw2.Close();
          
        }


        /*
private void button1_Click(object sender, EventArgs e)
{
int counter = 0;
string line;
string cmpline = "Missed Frames: ";

FileStream fs = new FileStream("mappingtest.txt", FileMode.Create);
StreamWriter sw = new StreamWriter(@"E:\mappingtest.txt");

// Read the file and display it line by line.  
System.IO.StreamReader file = new System.IO.StreamReader(@"E:\mapping.txt");
while ((line = file.ReadLine()) != null)
{
bool stringcmp = line.Contains(cmpline);
if(stringcmp == true)
{
  var replacement = line.Replace("Missed", ",Missed");
  replacement = replacement.Replace(":", ",");
  sw.WriteLine(replacement);
}
counter++;
}

file.Close();
System.Console.WriteLine("There were {0} lines.", counter);
// Suspend the screen.  
System.Console.ReadLine();
}
*/
    }
}
