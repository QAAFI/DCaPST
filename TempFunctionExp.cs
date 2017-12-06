using System;

namespace LayerCanopyPhotosynthesis
{
    public class TempFunctionExp
    {
        public TempFunctionExp() { }

        public static double Val(double temp,  double P25, double c, double b)
        {
            return P25 * Math.Exp(c - b / (temp + 273));
        }
    }
}
