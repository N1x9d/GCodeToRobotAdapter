using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace GCodeToRobotAdapter
{
    public partial class Form1 : Form
    {
        string InputFileInfo;
        string outFile;
        private GcodeReader GCode;
        public Form1()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            InitializeComponent();
            GCode = new GcodeReader(this);
            try
            {
                bool ast,ae,set;
                ast = false;
                ae = false;
                set = false;
                StreamReader sr = new StreamReader("SaveData.dat");
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    switch (line)
                    {
                        case "#AS":
                            ast = !ast;
                            break;
                        case "#AE":
                            ae = !ae;
                            break;
                        case "#ST":
                            set = !set;
                            break;
                        default:
                            if (ast)
                                textBox4.Text += line+ System.Environment.NewLine;
                            if (ae)
                                textBox5.Text += line + System.Environment.NewLine;
                            if (set)
                                textBox3.Text += line + System.Environment.NewLine;
                            break;
                    }
                    

                }
                sr.Close();
            }
            catch
            {

            }
        }
        public string[] ArcStartText { get { return addMarker(textBox4.Lines,";ArcStart");  } set { textBox4.Lines = value; } }
        public string[] ArcEndText { get { return addMarker(textBox5.Lines,";ArcEnd"); } set { textBox5.Lines = value; } }
        public string[] SettingText { get { return addMarker(textBox3.Lines,";Sets"); } set { textBox3.Lines = value; } }
        public string InputFileInfo1 
        { 
            get => InputFileInfo; 
            
            set 
            { 
                InputFileInfo = value; 
                textBox1.Text = InputFileInfo;
                var a = InputFileInfo;
                a = a.Substring(0, a.LastIndexOf("."));
                OutFile = a + "1.gcode";
            } 
        }
        public bool IsChecked { get =>  checkBox1.Checked; }
        public int GetHSpeed {
            get  
            {
                if (checkBox3.Checked)
                    return int.Parse(textBox9.Text);
                else
                    return 0;
            }
        }

        public int GetSpeed
        {
            get
            {
                if (checkBox3.Checked)
                    return int.Parse(textBox7.Text);
                else
                    return 0;
            }
        }

        public float GetFeedKoef 
        {
            get 
            {
                if (checkBox1.Checked)
                    return float.Parse(textBox6.Text);
                else
                    return 1;
            }
             
        }
        public string OutFile { get => outFile; set { outFile = value; textBox2.Text = outFile; } }

        private void button1_Click(object sender, EventArgs e)
        {
            GCode.on_btn_Open_clicked();
        }
        private String[] addMarker(string[] InpLine, string Marker)
        {
            List<string> str = new List<string>();
            str.Add(Marker+'0');
            str.AddRange(InpLine);
            str.Add(Marker+'1');
            return str.ToArray();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            GCode.on_btn_Process_clicked();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GCode.Import();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            outFile = textBox2.Text;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox6.Enabled = true;
            }
            else
            {
                textBox6.Enabled = false;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBox3.Checked)
            {
                textBox7.Enabled = true;
                textBox9.Enabled = true;
            }
            else
            {
                textBox7.Enabled = false;
                textBox9.Enabled = false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StreamWriter sw = new StreamWriter("SaveData.dat");
            var lines = new List<string>();
            lines.Clear();
            lines.AddRange(textBox3.Lines);
            sw.WriteLine("#ST");
            foreach(var ln in lines)
            {
                sw.WriteLine(ln);
            }
            sw.WriteLine("#ST");
            lines.Clear();
            lines.AddRange(textBox4.Lines);
            sw.WriteLine("#AS");
            foreach (var ln in lines)
            {
                sw.WriteLine(ln);
            }
            sw.WriteLine("#AS");
            lines.Clear();
            lines.AddRange(textBox5.Lines);
            sw.WriteLine("#AE");
            foreach (var ln in lines)
            {
                sw.WriteLine(ln);
            }
            sw.WriteLine("#AE");
            sw.Close();

        }
    }
}
