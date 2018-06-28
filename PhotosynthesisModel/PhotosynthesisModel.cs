using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LayerCanopyPhotosynthesis
{
    public class PhotosynthesisModel : NotifyablePropertyClass
    {
        public enum PhotoPathway { C3, C4 };
        public enum NitrogenModel { APSIM, EXPONENTIAL };
        public enum ElectronTransportModel { EMPIRICAL, EXTENDED };
        public enum ConductanceModel { SIMPLE, DETAILED };

        public List<string> Parameters;
        public List<string> Variables;
        public List<string> SunlitShadedVariables;
        public List<string> CanopyVariables;

        #region Global
        public EnvironmentModel EnvModel { get; set; }
        public LeafCanopy Canopy { get; set; }
        public SunlitShadedCanopy Sunlit { get; set; }
        public SunlitShadedCanopy Shaded { get; set; }

        public SunlitCanopy SunlitAC1;
        public SunlitCanopy SunlitAJ;
        public ShadedCanopy ShadedAC1;
        public ShadedCanopy ShadedAJ;

        public delegate void Notify();
        public delegate void NotifyBool(bool flag);
        //[System.Xml.Serialization.XmlIgnore]
        public Notify NotifyFinish;
        //[System.Xml.Serialization.XmlIgnore]
        public NotifyBool NotifyStart;
        //[System.Xml.Serialization.XmlIgnore]
        public Notify NotifyFinishDay;
        //[System.Xml.Serialization.XmlIgnore]
        public Notify NotifyNewParSet;


        protected double _time = 10.0;
        protected double _timeStep = 1;

        //[System.Xml.Serialization.XmlIgnore]
        public bool Initialised = false;

        //[System.Xml.Serialization.XmlIgnore]
        public List<double> Instants;

        //[System.Xml.Serialization.XmlIgnore]
        public List<double> Ass;

        public List<double> Ios = new List<double>();
        public List<double> Idirs = new List<double>();
        public List<double> Idiffs = new List<double>();

        [ModelVar("SLAC88", "Time step", "t", "", "")]
        public List<double> SunlitACs { get; set; } = new List<double>();
        [ModelVar("SLAJ73", "", "", "", "")]
        public List<double> SunlitAJs { get; set; } = new List<double>();
        [ModelVar("SSAC11", "", "", "", "")]
        public List<double> ShadedACs { get; set; } = new List<double>();
        [ModelVar("SSAK00", "", "", "", "")]
        public List<double> ShadedAJs { get; set; } = new List<double>();

        // [ModelVar("CCs324", "", "", "", "")]
        // public List<double> Ccs { get; set; } = new List<double>();

        public int MinTime = 6;
        public int MaxTime = 18;

        [ModelVar("4UytK", "Time step", "t", "", "")]
        public double TimeStep
        {
            get { return _timeStep; }
            set
            {
                _timeStep = value;
                OnPropertyChanged("timeStep");
                //this.runDaily();
            }
        }

        [ModelVar("xEdBc", "Time of day", "t", "", "")]
        public double Time
        {
            get { return _time; }
            set
            {
                _time = value;
                //OnPropertyChanged("time");
                //this.run();
            }
        }

        [ModelVar("Vs9lY", "Ambient air Temperature", "T", "a", "°C", "t")]
        //[System.Xml.Serialization.XmlIgnore]
        public double Temp
        {
            get { return EnvModel.GetTemp(Time); }
        }
        //[System.Xml.Serialization.XmlIgnore]
        [ModelVar("hXaSv", "Vapour pressure deficit", "VPD", "", "kPa", "t")]
        public double VPD
        {
            get { return EnvModel.GetVPD(Time); }
        }

        [ModelVar("DTXwi", "Total daily intercepted solar radiation", "RAD", "DAY", "MJ/m2/day", "", "m2 ground")]
        public double InterceptedRadn { get; set; }

        [ModelVar("ewC3T", "Daily canopy solar radiation extinction coefficient", "k", "", "")]
        public double k { get; set; }

        [ModelVar("qRXR7", "Daily biomass accumulated by canopy", "BIO", "Total,DAY", "g/m2", "", "m2 ground")]
        public double DailyBiomass { get; set; }

        [ModelVar("qR227", "Daily above ground biomass accumulated by canopy", "BIO", "Shoot,DAY", "g/m2", "", "m2 ground")]
        public double DailyBiomassShoot { get; set; }

        [ModelVar("DDDR7", "Proportion of above ground biomass to total plant biomass", "P", "Shoot", "", "", "")]
        public double P_AG { get; set; } = 0.8;

        [ModelVar("vtoDc", "ASunlitMAx", "A", "", "")]
        public double AShadedMax { get; set; }

        [ModelVar("Ti3HI", "ASunlitMAx", "A", "", "")]
        public double ASunlitMax { get; set; }

        [ModelVar("8XLI6", "Daily canopy CO2 assimilation", "A", "c_DAY", "mmol/m2/day", "", "m2 ground")]
        public double A { get; set; }

        [ModelVar("ThUS8", "Above ground Radiation Use Efficiency for the day", "RUE", "Shoot,DAY", "g/MJ", "", "g biomass")]
        public double RUE { get; set; }

        public double B { get; set; }

        public double PsiFactor { get; set; } = 1;

        #region Model switches and delegates
        private PhotoPathway _photoPathway = PhotoPathway.C3;
        [ModelPar("0gDem", "Photosynthetic pathway", "", "", "")]
        public PhotoPathway photoPathway
        {
            get
            {
                return _photoPathway;
            }
            set
            {
                _photoPathway = value;
                if (photoPathwayChanged != null)
                {
                    photoPathwayChanged(_photoPathway);
                }
                if (Initialised)
                {
                    //this.runDaily();
                }
            }
        }

        public delegate void PhotoPathwayNotifier(PhotoPathway p);
        //[System.Xml.Serialization.XmlIgnore]
        public PhotoPathwayNotifier photoPathwayChanged;

        private NitrogenModel _nitrogenModel = NitrogenModel.APSIM;
        [ModelPar("3GxZk", "Nitrogen Model", "", "", "")]
        public NitrogenModel nitrogenModel
        {
            get
            {
                return _nitrogenModel;
            }
            set
            {
                _nitrogenModel = value;
                if (nitrogenModelChanged != null)
                {
                    nitrogenModelChanged();
                }
                if (Initialised)
                {
                    //this.runDaily();
                }
            }
        }

        public delegate void NitrogenModelNotifier();
        //[System.Xml.Serialization.XmlIgnore]
        public NitrogenModelNotifier nitrogenModelChanged;

        private ElectronTransportModel _electronTransportModel = ElectronTransportModel.EXTENDED;
        //[ModelPar("Ndx07", "Electron Transport Model", "", "")]
        public ElectronTransportModel electronTransportModel
        {
            get
            {
                return _electronTransportModel;
            }
            set
            {
                _electronTransportModel = value;
                if (electronTransportModelChanged != null)
                {
                    electronTransportModelChanged();
                }
                if (Initialised)
                {
                    //this.runDaily();
                }
            }
        }

        public delegate void ElectronTransportModelNotifier();
        //[System.Xml.Serialization.XmlIgnore]
        public ElectronTransportModelNotifier electronTransportModelChanged;

        private ConductanceModel _conductanceModel = ConductanceModel.DETAILED;
        //[ModelPar("pvSkJ", "Photosynthetic pathway", "P", "")]
        public ConductanceModel conductanceModel
        {
            get
            {
                return _conductanceModel;
            }
            set
            {
                _conductanceModel = value;
                if (conductanceModelChanged != null)
                {
                    conductanceModelChanged();
                }
                if (Initialised)
                {
                    //this.runDaily();
                }
            }
        }

        public delegate void ConductanceModelNotifier();
        //[System.Xml.Serialization.XmlIgnore]
        public ConductanceModelNotifier conductanceModelChanged;
        #endregion

        public int Count;
        //---------------------------------------------------------------------------
        public PhotosynthesisModel()
        {
            EnvModel = new EnvironmentModel();
            Canopy = new LeafCanopy();
            Canopy.NLayers = 1;
            //sunlit = new SunlitCanopy();
            //shaded = new ShadedCanopy();

            //canopy.notifyChanged += runDaily;
            photoPathwayChanged += Canopy.PhotoPathwayChanged;

            //envModel.notify += runDaily;
            Parameters = new List<string>();
            Variables = new List<string>();
            SunlitShadedVariables = new List<string>();
            CanopyVariables = new List<string>();

            //canopy.layerNumberChanged += sunlit.initArrays;
            //canopy.layerNumberChanged += shaded.initArrays;
        }
        //---------------------------------------------------------------------------
        public void Run()
        {
            if (Initialised)
            {
                Run(true);
            }
            //runDaily();
        }
        //---------------------------------------------------------------------------
        public virtual void Run(double time, double swAvail, double maxHourlyT = -1, double sunlitPC = 0, double shadedPC = 0)
        {
            this.Time = time;
            if (maxHourlyT == -1)
            {
                Run(false, swAvail);
            }
            else
            {
                Run(false, swAvail, maxHourlyT, sunlitPC, shadedPC);
            }
        }

        public virtual double[] RunApsim(int DOY, double latitude, double maxT, double minT, double radn, double lai, double SLN, double soilWaterAvail, double RootShootRatio) { return null; }
        
        //---------------------------------------------------------------------------
        public virtual void Run(bool sendNotification, double swAvail = 0, double maxHourlyT = -1, double sunlitFraction = 0, double shadedFraction = 0)
        {
            
        }
        #endregion
    }
}
