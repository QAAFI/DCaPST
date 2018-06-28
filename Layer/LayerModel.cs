
namespace LayerCanopyPhotosynthesis
{

    public class LayerModel
    {
        NaturalSpline spline;
        public LayerModel(double[] x, double[] y)
        {
            spline = new NaturalSpline(x, y);
        }

        //---------------------------------------------------------------------------
        public double GetValue(double value)
        {
            return spline.GetValue(value);
        }
    }
}
