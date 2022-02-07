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
            var res = command + command;
            if (flags.Contains("x"))
                res += " X" + x;
            if (flags.Contains("y"))
                res += " Y" + y;
            if (flags.Contains("z"))
                res += " Z" + z;
            if (flags.Contains("e"))
                res += " E" + e;
            if(feedrate!=0)
                res += " F" + feedrate;
            return res;
        }
    }
}
