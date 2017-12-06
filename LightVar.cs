
namespace LayerCanopyPhotosynthesis
{
    class LightVar
    {
        private double _sunlit;
        private double _shaded;

        public double Sunlit
        {
            get { return _sunlit; }
            set { _sunlit = value; }
        }

        public double Shaded
        {
            get { return _shaded; }
            set { _shaded = value; }
        }
    }
}
