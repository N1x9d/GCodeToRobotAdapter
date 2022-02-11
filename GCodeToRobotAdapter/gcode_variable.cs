namespace GCodeToRobotAdapter
{
    struct gcode_variable
    {
        public float x, y, z, e, a, b, c;
        public int feedrate;
        public string command;
        public int commandvalue;
        public string flags;
        
        public string getString()
        {
            var res = command + commandvalue;
            if (flags.Contains("x"))
                res += " X" + x;
            if (flags.Contains("y"))
                res += " Y" + y;
            if (flags.Contains("z"))
                res += " Z" + z;
            if (flags.Contains("e"))
                res += " E" + e;
            if(feedrate!=0 && commandvalue==1)
                res += " F" + feedrate;
            return res;
        }

        public void Nulification()
        {
            x = 0;
            y = 0;
            z =0;
            e = 0;
            a = 0;
            c = 0;
                b = 0;

        }
    }
}
