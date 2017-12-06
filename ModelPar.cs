
namespace LayerCanopyPhotosynthesis
{
//    [System.AttributeUsage(System.AttributeTargets.Class |
//                       System.AttributeTargets.Struct)
//]
    public class ModelPar : System.Attribute
    {
        public string Symbol;
        public string Description;
        public string Units;
        public string Subscript;
        public string Scope;
        public string UnitsTip;
        public string Id;

        public bool C4Only;

        public object value;

        public ModelPar(string id, string _desc, string _sym, string _subscript, string _units, string _scope="", string _unitsTip="", bool _C4Only = false)
        {
            this.Id = id;
            this.Symbol = _sym;
            this.Description = _desc;
            this.Units = _units;
            this.Subscript = _subscript;
            this.Scope = _scope;
            this.UnitsTip = _unitsTip;
            this.C4Only = _C4Only;
            value = null;
        }
    }
}
