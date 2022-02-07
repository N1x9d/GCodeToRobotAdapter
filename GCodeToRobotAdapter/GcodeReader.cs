using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GCodeToRobotAdapter
{

    public class GcodeReader
    {
        private bool IsImport = false;
        private string conv;
        private coordinates _current, _previous;
        private gcode_variable _gcode,_prev;
        private List<LineType> lines;
        private float PrevF = 0;
        public string Conv
        {
            get { return conv.ToString(myCIintl); }
            set
            {
                conv = value;
                if (conv.Contains(","))
                    conv = conv.Replace(",", ".");
                if (!conv.Contains("."))
                    conv += ".0";
            }
        }
        LineType ArcStart, ArcEnd, Sets;
        CultureInfo myCIintl = new CultureInfo("es-ES", true);
        private Form1 _form;
        public GcodeReader(Form1 form)
        {
            _form = form;

        }
        public void on_btn_Open_clicked()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "GCode (*.gcode *.gc *.nc) |*.gcode; *.gc' *.nc| Other files (*.*)|*.*";
            if (openFile.ShowDialog() == DialogResult.Cancel)
                return;
            // получаем выбранный файл
            _form.InputFileInfo1 = openFile.FileName;
        }
        public void on_btn_Process_clicked()
        {
            ArcStart.line = _form.ArcStartText;
            ArcStart.tipe = "ArcStart";
            ArcEnd.tipe = "ArcEnd";
            Sets.tipe = "Sets";
            ArcEnd.line = _form.ArcEndText;
            Sets.line = _form.SettingText;
            if (!IsImport)
            {
                _current.x = 0;
                _current.y = 0;
                _current.z = 0;
                _current.e = 0;
                _current.feedrate = 0;
                _current.states = "p";

                lines = new List<LineType>();
                lines.Add(Sets);

                _previous = _current;


                string inputFile = _form.InputFileInfo1;



                StreamReader sr = new StreamReader(inputFile);
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    if (line.Contains("; layer "))
                    {


                    }


                    gcode_process(line);


                }

                sr.Close();
            }
            //QRegularExpression re("(([A-Z])[-]\?\\d+\\.*\\d*)");

            else
            {
                List<LineType> lins2 = new List<LineType>();
                foreach(var ln in lines)
                {
                    switch (ln.tipe)
                    {
                        case "ArcStart":
                            lins2.Add(ArcStart);
                            break;
                        case "ArcEnd":
                            lins2.Add(ArcEnd);
                            break;
                        case "Sets":
                            lins2.Add(Sets);
                            break;
                        case "":
                            lins2.Add(ln);
                            break;
                    }
                }
                lines= lins2;
            }
            if (_form.IsChecked)
                RecalkE();
            StreamWriter sw = new StreamWriter(_form.OutFile);
            foreach (var line in lines)
            {
                foreach(var str in line.line)
                {
                    sw.WriteLine(str);
                }
            }
            sw.WriteLine("G90");
            sw.WriteLine("M83");
            sw.Close();
            MessageBox.Show("done");
            
           

        }
        void gcode_process(string line)
        {
            string lin = line;
            string[] a = lin.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (_gcode.flags!=null && _gcode.flags.Contains('e') && _gcode.command == "G" && _gcode.commandvalue == 1)
                _prev = _gcode;
            _gcode.command = "";
            _gcode.flags = "";
            foreach (var com in a)
            {
                char element = com[0];
                if (element == ';')
                    break;

                float value = float.Parse(com.Substring(1), CultureInfo.InvariantCulture.NumberFormat);
                switch (element)
                {
                    case 'X':
                        _gcode.x = value;
                        _gcode.flags += 'x';
                        break;
                    case 'Y':
                        _gcode.y = value;
                        _gcode.flags += 'y';
                        break;
                    case 'Z':
                        _gcode.z = value;
                        _gcode.flags += 'z';
                        break;
                    case 'E':
                        _gcode.e = value;
                        _gcode.flags += 'e';
                        break;
                    case 'A':
                        _gcode.a = value;
                        _gcode.flags += 'a';
                        break;
                    case 'B':
                        _gcode.b = value;
                        _gcode.flags += 'b';
                        break;
                    case 'C':
                        _gcode.c = value;
                        _gcode.flags += 'c';
                        break;
                    case 'F':
                        _gcode.feedrate = 500;
                        _gcode.flags += 'f';
                        break;
                    case 'G':
                        _gcode.command = element.ToString();
                        _gcode.commandvalue = Convert.ToInt32(value);
                        break;
                    case 'M':
                        _gcode.command = element.ToString();
                        _gcode.commandvalue = Convert.ToInt32(value);
                        break;
                }
            }

            

            if (_gcode.command == "G" && _gcode.commandvalue!= 28)
            {
              
                // Linear move
                //
                float l = 0;
                
                if (_gcode.flags.Contains("e"))
                {
                    _previous = _current;
                    _current.e = _gcode.e;
                    if (_current.e - _previous.e > 0)
                        _current.states = "P";
                    else _current.states = "p";
                    if (_current.states.Contains("p") && _previous.states.Contains("P"))
                    {
                        lines.Add(ArcEnd);
                    }

                    if (_current.states.Contains("P") && _previous.states.Contains("p"))
                    {
                        lines.Add(ArcStart);
                    }
                    


                }
                string[] ln = new string[1];
                ln[0] = line;
                LineType tipLine;
                
                
                ln[0] = line;
                tipLine.line = ln;
                tipLine.tipe = "";
                lines.Add(tipLine);
            }
        }

        public void Import()
        {
            string inputFile = _form.InputFileInfo1;
            lines = new List<LineType>();
            bool markerStart = false;
            LineType lt;
            lt.tipe = "";
            lt.line = null;
            StreamReader sr = new StreamReader(inputFile);
            List<string> strs = new List<string>();
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                try
                {
                    if (line[0] == ';')
                    {
                        if (line.Contains('0'))
                        {
                           
                            if (line.Contains("ArcStart"))
                                lt.tipe = "ArcStart";
                            else if (line.Contains("ArcEnd"))
                                lt.tipe = "ArcEnd";
                            else if (line.Contains("Sets"))
                                lt.tipe = "Sets";

                        }
                        else if(line.Contains('1'))
                        {
                            lt.line = strs.ToArray();
                            strs.Clear();

                            lines.Add(lt);
                            switch (lt.tipe)
                            {
                                case "ArcStart":
                                    _form.ArcStartText = lt.line;
                                    break;
                                case "ArcEnd":
                                    _form.ArcEndText = lt.line;
                                    break;
                                case "Sets":
                                    _form.SettingText = lt.line;
                                    break;
                            }
                            lt.tipe = "";
                            lt.line = null;
                            markerStart = false;
                        }
                        else
                        {
                            strs.Add(line);
                        }

                    }
                    else
                    {
                        strs.Add(line);
                        if (lt.tipe=="")
                        {
                            lt.line = strs.ToArray();
                            strs.Clear();

                            lines.Add(lt);
                        }
                        
                    }
                }
                catch (Exception)
                {
                    strs.Add(line);
                }
                


            }

            sr.Close();
            StreamWriter sw = new StreamWriter(_form.OutFile);
            foreach (var line in lines)
            {
                foreach (var str in line.line)
                {
                    sw.WriteLine();
                }
            }
            sw.Close();
            IsImport = true;
            MessageBox.Show("done");


        }

        private void RecalkE()
        {
            _prev = new gcode_variable();
            _gcode = new gcode_variable();
            var _lines = new List<LineType>(); 
            foreach (var line in lines)
            {
                if(!line.tipe.Contains("ArcStart")|| !line.tipe.Contains("ArcEnd") || !line.tipe.Contains("Sets"))
                {
                    string[] line1 = new string[1];
                    foreach (var ln in line.line)
                    {
                        string[] a = ln.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (_gcode.flags != null && _gcode.flags.Contains('e') && _gcode.command == "G" && _gcode.commandvalue == 1)
                            _prev = _gcode;
                        _gcode.command = "";
                        _gcode.flags = "";
                        foreach (var com in a)
                        {
                            char element = com[0];
                            if (element == ';')
                                break;

                            float value = float.Parse(com.Substring(1), CultureInfo.InvariantCulture.NumberFormat);
                            switch (element)
                            {
                                case 'X':
                                    _gcode.x = value;
                                    _gcode.flags += 'x';
                                    break;
                                case 'Y':
                                    _gcode.y = value;
                                    _gcode.flags += 'y';
                                    break;
                                case 'Z':
                                    _gcode.z = value;
                                    _gcode.flags += 'z';
                                    break;
                                case 'E':
                                    _gcode.e = value;
                                    _gcode.flags += 'e';
                                    break;
                                case 'A':
                                    _gcode.a = value;
                                    _gcode.flags += 'a';
                                    break;
                                case 'B':
                                    _gcode.b = value;
                                    _gcode.flags += 'b';
                                    break;
                                case 'C':
                                    _gcode.c = value;
                                    _gcode.flags += 'c';
                                    break;
                                case 'F':
                                    _gcode.feedrate = 500;
                                    _gcode.flags += 'f';
                                    break;
                                case 'G':
                                    _gcode.command = element.ToString();
                                    _gcode.commandvalue = Convert.ToInt32(value);
                                    break;
                                case 'M':
                                    _gcode.command = element.ToString();
                                    _gcode.commandvalue = Convert.ToInt32(value);
                                    break;
                            }                            
                        }
                        _current.x = 0;
                        _current.y = 0;
                        _current.z = 0;
                        _current.e = 0;
                        _current.feedrate = 0;
                        _current.states = "p";
                        _previous = _current;
                        if (_current.feedrate != 0)
                            PrevF = _current.feedrate;
                        var Nline = "";
                        float l = 0;
                        if (_gcode.e > 0)
                        {
                            l = (float)Math.Sqrt(Math.Pow(_gcode.x - _prev.x, 2) + Math.Pow(_gcode.y - _prev.y, 2) + Math.Pow(_gcode.z - _prev.z, 2));
                            _gcode.e = l / (PrevF * 1000) * _form.GetFeedKoef;
                        }
                        line1[0] = _gcode.getString();
                        

                    }
                    LineType LT;
                    LT.line = line1;
                    LT.tipe = "";
                    _lines.Add(LT);
                }
                else
                {
                    _lines.Add(line);
                }
            }
            lines.Clear();
            lines.AddRange(_lines);

        }

    }
}
