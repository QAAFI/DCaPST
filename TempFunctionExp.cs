using System;

namespace LayerCanopyPhotosynthesis
{
    public class TempFunctionExp
    {
        public TempFunctionExp() { }

        //public static double Val(double temp, double P25, double c, double b)
        //{
        //    return P25 * Math.Exp(c - b / (temp + 273));
        //}
    }

    public class TemperatureFunction
    {
        public TemperatureFunction() { }
        public static double Val(double temp, double P25, double c, double tMax,  double tMin, double tOpt, double beta)
        {
            double alpha = Math.Log(2) / (Math.Log((tMax - tMin) / (tOpt - tMin)));
            double numerator = 2 * Math.Pow((temp - tMin), alpha) * Math.Pow((tOpt - tMin), alpha) - Math.Pow((temp - tMin), 2 * alpha);
            double denominator = Math.Pow((tOpt - tMin), 2 * alpha);
            double funcT = P25 * Math.Pow(numerator / denominator, beta) / c;

            return funcT;
        }
    }
}
