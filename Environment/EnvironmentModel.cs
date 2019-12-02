using System;
using System.Collections.Generic;
using Utilities;

namespace LayerCanopyPhotosynthesis
{
    public class EnvironmentModel
    {
        //Location variables
        protected Angle _latitude;
        protected Angle _longitude;

        //Daily Variables
        protected double _DOY;
        protected double _maxT;
        protected double _minT;
        protected double Sg;
        protected double _vpd;
        protected double _maxRH = -1;
        protected double _minRH;
        protected double _xLag = 1.8;
        protected double _yLag = 2.2;
        protected double _zLag = 1;
        protected double _ATM = 1.01325;

        //protected HourlyMet hm;

        public delegate void Notify();

        //[System.Xml.Serialization.XmlIgnore]
        public Notify notify;

        //Timestep variables
        //-------------------------------------------------------------------------
        public EnvironmentModel()
        {
            _latitude = new Angle();
            _longitude = new Angle();

            SolarDeclination = new Angle();

            SolarElevation = new Angle();

            //hm = new HourlyMet();
        }
        //-------------------------------------------------------------------------
        public EnvironmentModel(double Latitude, double Longitude, int NumberLayers)
            : this()
        {
            _latitude = new Angle(Latitude, AngleType.Deg);

            _longitude = new Angle(Longitude, AngleType.Deg);
        }
        //-------------------------------------------------------------------------
        public EnvironmentModel(double Latitude, int NumberLayers)
            : this()
        {
            _latitude = new Angle(Latitude, AngleType.Deg);

            _longitude = new Angle(90, AngleType.Deg);
        }

        //---------------------------------------------------------------------------
        protected int CalcStandardLongitude(double longitude)
        {
            return (int)((longitude / 15) + 0.5) * 15;
        }
        //------------------------------------------------------------------------------------------------
        // Properties
        //------------------------------------------------------------------------------------------------
        [ModelPar("MjNwl", "Longitude", "Le", "", "°")]
        public Angle Longitude
        {
            get { return _longitude; }
            set
            {
                _longitude = value;
                //OnPropertyChanged("longitude");
            }
        }

        [ModelPar("hjFzy", "Latitude", "Lat", "", "°")]
        public Angle Latitude
        {
            get { return _latitude; }
            set
            {
                _latitude = value;
                //run();
                //OnPropertyChanged("latitude");
                if (notify != null)
                {
                    notify();
                }
            }
        }

        [ModelPar("vmCd9", "Latitude", "Lat", "", "°")]
        public double LatitudeD
        {
            get { return _latitude.Deg; }
            set
            {
                _latitude = new Angle(value, AngleType.Deg);
                //run();
                //OnPropertyChanged("latitude");
                if (notify != null)
                {
                    notify();
                }
            }
        }

        [ModelPar("AhhfK", "Absolute Temperature", "", "°C", "")]
        public double AbsoluteTemperature = 273;

        [ModelPar("Dmrd4", "Fraction of PAR energy to that of the total solar", "Lat", "", "°")]
        public double RPAR { get; set; } = 0.5;

        [ModelPar("AlMfK", "Day of year", "Day", "", "")]
        public double DOY
        {
            get { return _DOY; }
            set
            {
                _DOY = value;
                //run();
                //OnPropertyChanged("DOY");
                if (notify != null)
                {
                    // notify();
                }
            }
        }

        [ModelPar("ArrfK", "Atmospheric pressure", "ATM", "", "")]
        public double ATM
        {
            get { return _ATM; }
            set
            {
                _ATM = value;
            }
        }

        [ModelPar("5hHkF", "Maximum air temperature for the day", "T", "a_max", "°C")]
        public double MaxT
        {
            get { return _maxT; }
            set
            {
                _maxT = value;
                //run();
                //OnPropertyChanged("maxT");
                if (notify != null)
                {
                    // notify();
                }
            }
        }

        [ModelPar("lI6iy", "Minimum air temperature for the day", "T", "a_min", "°C")]
        public double MinT
        {
            get { return _minT; }
            set
            {
                _minT = value;
                //run();
                //OnPropertyChanged("minT");
                if (notify != null)
                {
                    // notify();
                }
            }
        }

        [ModelPar("RyHP0", "Daily min RH", "RHMin", "", "%")]
        public double MinRH
        {
            get { return _minRH; }
            set
            {
                _minRH = value;
                //run();
                //OnPropertyChanged("minT");
                if (notify != null)
                {
                    // notify();
                }
            }
        }

        [ModelPar("CjkRT", "Daily max RH", "RHMax", "", "%")]
        public double MaxRH
        {
            get { return _maxRH; }
            set
            {
                _maxRH = value;
                //run();
                //OnPropertyChanged("minT");
                if (notify != null)
                {
                    //notify();
                }
            }
        }

        [ModelPar("vWD3R", "X Lag", "xLag", "", "")]
        public double XLag
        {
            get { return _xLag; }
            set
            {
                _xLag = value;
                //run();
                if (notify != null)
                {
                    //notify();
                }
            }
        }

        [ModelPar("r5C7r", "Y Lag", "yLag", "", "")]
        public double YLag
        {
            get { return _yLag; }
            set
            {
                _yLag = value;
                //run();
                if (notify != null)
                {
                    //notify();
                }
            }
        }

        [ModelPar("JKzC3", "Z Lag", "zLag", "", "")]
        public double ZLag
        {
            get { return _zLag; }
            set
            {
                _zLag = value;
                //run();
                if (notify != null)
                {
                    //notify();
                }
            }
        }

        [ModelPar("sdkGj", "Daily VPD", "vpd", "", "kPa")]
        public double VPD
        {
            get { return _vpd; }
            set
            {
                _vpd = value;
                //OnPropertyChanged("vpd");
                if (notify != null)
                {
                    notify();
                }
            }
        }

        [ModelPar("goJzH", "Daily solar radiation reaching Earth's surface", "S", "g", "MJ/m2/day", "", "m2 of ground")]
        public double Radn
        {
            get { return Sg; }
            set
            {
                Sg = value;
                CalcSolarGeometry();
                _atmTransmissionRatio = Sg / So;

                CalcRatios(Sg / So);
                //run();
                if (notify != null)
                {
                    //    notify();
                }
            }
        }

        [ModelPar("uMZ2C", "Total daily incident solar radiation", "S", "g", "MJ/m2/day", "", "m2 of ground")]
        public double Sg2
        {
            get
            {
                double r = 0;
                for (int i = 6; i <= 18; i++)
                {
                    //r += Ios.value(i) * ratios.value(i) * 3600;
                    r += Ios.Value(i) * 3600;
                }
                return r;
            }
        }

        [ModelVar("drKhV", "Solar declination angle", "δ", "", "radian")]
        public Angle SolarDeclination { get; set; }

        [ModelVar("EwOhQ", "Solar elevation angle", "β", "", "radian")]
        public Angle SolarElevation { get; set; }

        [ModelVar("UHXoS", "Standard longitude of time zone", "Ls", "", "degree")]
        public double StandardLongitude { get; set; }

        #region Solar Geometry
        private double _solarConstant = 1360;
        [ModelPar("7eMZa", "Solar constant", "I0", "", "J m-2 s-1")]
        public double SolarConstant
        {
            get { return _solarConstant; }
            set
            {
                _solarConstant = value;
                //OnPropertyChanged("solarConstant"); 
            }
        }

        protected double _atmTransmissionRatio = 0.75;
        [ModelPar("AoRO2", "Atmospheric transmission coefficient", "Ratio", "", "")]
        public double AtmTransmissionRatio
        {
            get { return _atmTransmissionRatio; }
            set
            {
                _atmTransmissionRatio = value;

                CalcSolarGeometry();

                Sg = So * _atmTransmissionRatio;

                CalcRatios(_atmTransmissionRatio);

                //run();

                if (notify != null)
                {
                    //     notify();
                }
                //OnPropertyChanged("atmTransmissionCoeff");
            }
        }

        protected double[] _ratios_;
        [ModelPar("a1Pb4", "", "", "", "")]
        public double[] Ratios_
        {
            get { return _ratios_; }
            set
            {
                _ratios_ = value;

                CalcRatios();

                //run();

                if (notify != null)
                {
                    //          notify();
                }
                //OnPropertyChanged("atmTransmissionCoeff");
            }
        }

        [ModelVar("ob6iX", "Hour angle of sun", "W2", "", "radian")]
        public Angle HourAngle { get; set; }

        [ModelVar("3ywo3", "Sunset hour angle of sun", "W1", "", "radian")]
        public Angle SunsetAngle { get; set; }

        [ModelVar("XidUS", "Day angle", "Γd", "", "")]
        public Angle DayAngle { get; set; }

        [ModelVar("a4REK", "Solar elevation", "α", "", "°", "t")]
        public Angle SunAngle { get; set; }

        [ModelVar("RTS4t", "Solar elevation", "α", "", "°", "t")]
        public double SunAngleD
        {
            get
            {
                return SunAngle.Deg;
            }
        }

        [ModelVar("CBMOF", "Zenith angle", "Z", "", "radian")]
        public Angle ZenithAngle { get; set; }

        [ModelVar("nEqsb", "Radius vector", "R1", "", "")]
        public double RadiusVector { get; set; }

        [ModelVar("t2rN0", "Day length", "L1", "", "h")]
        public double DayLength { get; set; }

        [ModelVar("4lcLb", "", "tfrac", "", "")]
        public double TFrac { get; set; }

        [ModelVar("JfTav", "Daily extra-terrestrial irradiance", "So", "", "MJ/m2/day")]
        public double So { get; set; }

        [ModelVar("eaehB", "Sunrise", "", "", "")]
        public double Sunrise { get; set; }

        [ModelVar("0yWWK", "Sunset", "", "", "")]
        public double Sunset { get; set; }

        private bool _initilised = false;
        public bool Initialised
        {
            get
            {
                return _initilised;
            }
            set
            {
                _initilised = value;
                if (_initilised)
                {
                    Run();
                }
            }
        }

        public void CalcRatios(double ratio)
        {
            _ratios_ = new double[24];

            for (int i = 0; i < 23; i++)
            {
                _ratios_[i] = ratio;
            }

            CalcRatios();
        }


        public void CalcRatios()
        {
            double[] times = new double[24];

            for (int i = 0; i < 23; i++)
            {
                times[i] = i;
            }

            Ratios = new TableFunction(times, _ratios_);
        }

        //---------------------------------------------------------------------------
        public double GetTemp(double time)
        {
            return Temps.Value(time - 1);
        }
        //---------------------------------------------------------------------------
        public double GetVPD(double time)
        {
            return VPDs.Value(time);
        }
        //---------------------------------------------------------------------------
        Angle CalcSolarDeclination(int DoY)
        {
            return new Angle(23.45 * Math.Sin(2 * Math.PI * (284 + DoY) / 365), AngleType.Deg);
        }
        //---------------------------------------------------------------------------
        Angle CalcHourAngle(double timeOfDay)
        {
            return new Angle((timeOfDay - 12) * 15, AngleType.Deg);
        }
        //---------------------------------------------------------------------------
        Angle CalcDayAngle(int DoY)
        {
            return new Angle(2 * Math.PI * (DoY - 1) / 365, AngleType.Rad);
        }
        //---------------------------------------------------------------------------
        protected double CalcDayLength(double latitudeRadians, double solarDecRadians)
        {
            SunsetAngle = new Angle(Math.Acos(-1 * Math.Tan(latitudeRadians) * Math.Tan(solarDecRadians)), AngleType.Rad);
            return (SunsetAngle.Deg / 15) * 2;
        }
        //---------------------------------------------------------------------------
        double CalcSunAngle(double hour, double latitudeRadians, double solarDecRadians, double dayLength)
        {
            return Math.Asin(Math.Sin(latitudeRadians) * Math.Sin(solarDecRadians) +
                           Math.Cos(latitudeRadians) * Math.Cos(solarDecRadians) * Math.Cos((Math.PI / 12.0) * dayLength * (TFrac - 0.5))); //Daylength can be taken out of this last bit
        }
        //---------------------------------------------------------------------------
        double CalcZenithAngle(double latitudeRadians, double solarDeclinationRadians, double hourAngleRadians)
        {
            return Math.Acos(Math.Sin(solarDeclinationRadians) * Math.Sin(latitudeRadians) + Math.Cos(solarDeclinationRadians) * Math.Cos(latitudeRadians) * Math.Cos(hourAngleRadians));
        }
        //---------------------------------------------------------------------------
        double CalcDailyExtraTerrestrialRadiation(double latitudeRadians, double sunsetAngleRadians, double solarDecRadians, int DoY) // solar radiation
        {
            RadiusVector = 1.0 / Math.Sqrt(1 + (0.033 * Math.Cos(360.0 * DoY / 365.0)));

            return ((24.0 / Math.PI) * (3600.0 * SolarConstant / Math.Pow(RadiusVector, 2)) * (sunsetAngleRadians * Math.Sin(latitudeRadians) * Math.Sin(solarDecRadians) +
               +Math.Sin(sunsetAngleRadians) * Math.Cos(latitudeRadians) * Math.Cos(solarDecRadians)) / 1000000.0);
        }
        //---------------------------------------------------------------------------
        public void CalcSolarGeometry()
        {
            SolarDeclination = CalcSolarDeclination((int)DOY);

            DayLength = CalcDayLength(Latitude.Rad, SolarDeclination.Rad);

            DayAngle = CalcDayAngle((int)DOY);

            Sunrise = 12 - DayLength / 2;

            Sunset = 12 + DayLength / 2;

            So = CalcDailyExtraTerrestrialRadiation(Latitude.Rad, SunsetAngle.Rad, SolarDeclination.Rad, (int)DOY);

            //_atmTransmissionRatio = radn / ETRadiation;

        }
        //---------------------------------------------------------------------------
        public void CalcSolarGeometryTimestep(double hour)
        {
            TFrac = (hour - Sunrise) / DayLength;

            HourAngle = CalcHourAngle(hour);

            SunAngle = new Angle(CalcSunAngle(hour, Latitude.Rad, SolarDeclination.Rad, DayLength), AngleType.Rad);

        }
        //---------------------------------------------------------------------------
        public void Run()
        {
            ConversionFactor = 2413.0 / 1360.0;

            CalcSolarGeometry();

            //_atmTransmissionRatio = _radn / ETRadiation;
            //_atmTransmissionRatio = 0.75;

            Sg = _atmTransmissionRatio * So;

            if (Ratios == null)
            {
                CalcRatios(Sg / So);
            }
            // calcETRadns();
            CalcIncidentRadns();
            CalcDiffuseRadns();
            CalcDirectRadns();
            CalcTemps();
            CalcSVPs();
            CalcRHs();
            CalcVPDs();

            ConvertRadiationsToPAR();
        }
        //---------------------------------------------------------------------------
        private void ConvertRadiationsToPAR()
        {
            List<double> io_par = new List<double>();
            List<double> idiff_par = new List<double>();
            List<double> idir_par = new List<double>();
            List<double> time = new List<double>();

            for (int i = 0; i < 24; i++)
            {
                time.Add(i);

                idiff_par.Add(Idiffs.Value(i) * RPAR * 4.25 * 1E6);
                idir_par.Add(Idirs.Value(i) * RPAR * 4.56 * 1E6);

                io_par.Add(idiff_par[i] + idir_par[i]);
            }
            Ios_PAR = new TableFunction(time.ToArray(), io_par.ToArray(), false);
            Idiffs_PAR = new TableFunction(time.ToArray(), idiff_par.ToArray(), false);
            Idirs_PAR = new TableFunction(time.ToArray(), idir_par.ToArray(), false);
        }

        public void Run(double time)
        {
            CalcSolarGeometry();
            CalcSolarGeometryTimestep(time);
            CalcIncidentRadiation(time);

            // run();
            // notify();
        }
        //---------------------------------------------------------------------------
        #endregion

        #region Incident Radiation
        protected double _fracDiffuseATM = 0.1725;
        [ModelPar("nU58A", "Fraction of diffuse irradiance in the atmosphere", "", "", "")]
        public double FracDiffuseATM
        {
            get { return _fracDiffuseATM; }
            set
            {
                _fracDiffuseATM = value;
                //OnPropertyChanged("fracDiffuseATM"); 
            }
        }

        [ModelVar("oh5kh", "Total PAR at canopy top ", "I", "o", "μmol PAR/m2/s", "t", "m2 of ground")]
        public double TotalIncidentRadiation { get; set; }

        public double  DirectRadiation { get; set; }

        public double DiffuseRadiation { get; set; }

        [ModelVar("LuYdw", "Direct PAR at canopy top", "I", "dir", "μmol PAR/m2/s", "t", "m2 of ground")]
        public double DirectRadiationPAR { get; set; }

        [ModelVar("aqmpo", "Diffuse PAR at canopy top", "I", "dif", "μmol PAR/m2/s", "t", "m2 of ground")]
        public double DiffuseRadiationPAR { get; set; }

        [ModelVar("8WKRv", "Total PAR at canopy top", "I", "o", "μmol PAR/m2/s", "t", "m2 of ground")]
        public double TotalRadiationPAR { get; set; }

        [ModelVar("4HlHU", "", "", "", "")]
        public double ConversionFactor { get; set; }

        //---------------------------------------------------------------------------

        public void CalcIncidentRadiation(double hour)
        {
            TotalIncidentRadiation = Ios.Value(hour);

            DirectRadiation = Idirs.Value(hour);
            DiffuseRadiation = Idiffs.Value(hour);

            TotalRadiationPAR = Ios_PAR.Value(hour);
            DiffuseRadiationPAR = Idiffs_PAR.Value(hour);
            DirectRadiationPAR = Idirs_PAR.Value(hour);

            //double S0 = calcExtraTerestrialRadiation2(hour);

            //totalIncidentRadiation = S0 * ratio / conversionFactor * 1E12;
            ////totalIncidentRadiation = calcTotalIncidentRadiation(hour, dayLength, sunrise);
            ////diffuseRadiation = calcDiffuseRadiation(sunAngle.rad) / conversionFactor * 10E12;
            //diffuseRadiation = fracDiffuseATM * solarConstant * Math.Sin(sunAngle.rad) / 1000000 / conversionFactor * 1E12;

            //if (diffuseRadiation > totalIncidentRadiation)
            //{
            //    diffuseRadiation = totalIncidentRadiation;
            //}

            ////totalIncidentRadiation = totalIncidentRadiation / conversionFactor * 10E12;
            //directRadiation = (totalIncidentRadiation - diffuseRadiation);
        }

        //---------------------------------------------------------------------------
        public double CalcInstantaneousIncidentRadiation(double hour)
        {

            // return So * ratios.value(hour) * (1 + Math.Sin(2 * Math.PI * (hour - sunrise) / dayLength + 1.5 * Math.PI)) / 
            //     (dayLength * 3600);

            return (So * Ratios.Value(hour) * Math.PI * Math.Sin(Math.PI * (hour - Sunrise) / DayLength)) / (2 * DayLength * 3600);

        }
        //---------------------------------------------------------------------------
        public double CalcExtraTerestrialRadiation2(double hour, bool external = false)
        {
            if (external)
            {
                CalcSolarGeometryTimestep(hour);
            }

            Angle hourAngle2 = new Angle(0.25 * Math.Abs((hour - 12) * 60) / (180 / Math.PI), AngleType.Rad);

            //zenithAngle = new Angle(calcZenithAngle(latitude.rad, solarDeclination.rad, hourAngle.rad), AngleType.Rad);

            ZenithAngle = new Angle(CalcZenithAngle(Latitude.Rad, SolarDeclination.Rad, hourAngle2.Rad), AngleType.Rad);

            return (SolarConstant / Math.Pow(RadiusVector, 2) * Math.Cos(ZenithAngle.Rad) / 1000000);
        }

        //---------------------------------------------------------------------------
        void CalcIncidentRadns()
        {
            List<double> ios = new List<double>();
            //List<double> ets = new List<double>();

            List<double> time = new List<double>();

            int preDawn = (int)Math.Floor(12 - DayLength / 2.0);
            int postDusk = (int)Math.Ceiling(12 + DayLength / 2.0);

            for (int i = 0; i < 24; i++)
            {
                time.Add(i);

                //ios.Add(ETs.value(i) * ratios.value(i));
                if (i > preDawn && i < postDusk)
                {
                    ios.Add(Math.Max(CalcInstantaneousIncidentRadiation(i), 0));
                }
                else
                {
                    ios.Add(0);
                }

                //ets.Add(ios[i] / 0.75);
            }
            Ios = new TableFunction(time.ToArray(), ios.ToArray(), false);
            //ETs = new TableFunction(time.ToArray(), ets.ToArray(), false);
        }
        //---------------------------------------------------------------------------
        void CalcDiffuseRadns()
        {
            List<double> diffs = new List<double>();
            List<double> ets = new List<double>();
            List<double> time = new List<double>();

            for (int i = 0; i < 24; i++)
            {
                time.Add(i);
                CalcSolarGeometryTimestep(i);

                diffs.Add(Math.Max(FracDiffuseATM * SolarConstant * Math.Sin(SunAngle.Rad) / 1000000, 0));
                // ets.Add(diffs[i] / fracDiffuseATM);

                if (diffs[i] > Ios.Value(i))
                {
                    diffs[i] = Ios.Value(i);
                }
            }
            Idiffs = new TableFunction(time.ToArray(), diffs.ToArray(), false);
            // ETs = new TableFunction(time.ToArray(), ets.ToArray(), false);
        }
        //---------------------------------------------------------------------------
        void CalcDirectRadns()
        {
            List<double> dirs = new List<double>();
            List<double> time = new List<double>();

            for (int i = 0; i < 24; i++)
            {
                time.Add(i);
                dirs.Add(Ios.Value(i) - Idiffs.Value(i));
            }
            Idirs = new TableFunction(time.ToArray(), dirs.ToArray(), false);
        }
        //---------------------------------------------------------------------------

        double CalcTotalIncidentRadiation(double hour, double dayLength, double sunrise)
        {
            return Radn * (1 + Math.Sin(2 * Math.PI * (hour - sunrise) / dayLength + 1.5 * Math.PI)) / (dayLength * 3600);
        }
        //---------------------------------------------------------------------------

        double CalcDiffuseRadiation(double sunAngleRadians)
        {
            return Math.Min(Math.Sin(sunAngleRadians) * SolarConstant * FracDiffuseATM / 1000000, TotalIncidentRadiation);
        }

        //---------------------------------------------------------------------------
        #endregion
        //[System.Xml.Serialization.XmlIgnore]
        public TableFunction Ios { get; set; }
        //[System.Xml.Serialization.XmlIgnore]
        public TableFunction Idirs { get; set; }
        //[System.Xml.Serialization.XmlIgnore]
        public TableFunction Idiffs { get; set; }
        //[System.Xml.Serialization.XmlIgnore]
        public TableFunction Ios_PAR { get; set; }
        //[System.Xml.Serialization.XmlIgnore]
        public TableFunction Idirs_PAR { get; set; }
        //[System.Xml.Serialization.XmlIgnore]
        public TableFunction Idiffs_PAR { get; set; }
        //[System.Xml.Serialization.XmlIgnore]
        public TableFunction ETs { get; set; }
        //[System.Xml.Serialization.XmlIgnore]
        public TableFunction Ratios { get; set; }

        //[System.Xml.Serialization.XmlIgnore]
        public TableFunction Temps { get; set; }
        //[System.Xml.Serialization.XmlIgnore]
        public TableFunction Radns { get; set; }
        //[System.Xml.Serialization.XmlIgnore]
        public TableFunction SVPs { get; set; }
        //[System.Xml.Serialization.XmlIgnore]
        public TableFunction VPDs { get; set; }
        //[System.Xml.Serialization.XmlIgnore]
        public TableFunction RHs { get; set; }



        public virtual void DoUpdate() { }
        //------------------------------------------------------------------------------------------------
        public double CalcSVP(double TAir)
        {
            // calculates SVP at the air temperature
            //return 6.1078 * Math.Exp(17.269 * TAir / (237.3 + TAir)) * 0.1;
            return 610.7 * Math.Exp(17.4 * TAir / (239 + TAir)) / 1000;
        }
        //------------------------------------------------------------------------------------------------
        protected void CalcSVPs()
        {
            // calculates SVP at the air temperature
            List<double> svp = new List<double>();
            List<double> time = new List<double>();

            for (int i = 0; i < 24; i++)
            {
                svp.Add(CalcSVP(Temps.YVals[i]));
                time.Add(i);
            }
            SVPs = new TableFunction(time.ToArray(), svp.ToArray(), false);
        }
        //------------------------------------------------------------------------------------------------
        protected void CalcRHs()
        {
            List<double> time = new List<double>();
            List<double> rh = new List<double>();
            // calculate relative humidity
            double wP;
            if (MaxRH < 0.0 || MinRH < 0.0)
            {
                wP = CalcSVP(_minT) / 100 * 1000 * 90;         // svp at Tmin
            }
            else
            {
                wP = (CalcSVP(_minT) / 100 * 1000 * MaxRH + CalcSVP(_maxT) / 100 * 1000 * MinRH) / 2.0;
            }

            for (int i = 0; i < 24; i++)
            {
                rh.Add(wP / (10 * SVPs.YVals[i]));
            }

            RHs = new TableFunction(time.ToArray(), rh.ToArray(), false);

        }
        //------------------------------------------------------------------------------------------------
        protected void CalcVPDs()
        {
            List<double> time = new List<double>();
            List<double> vpd = new List<double>();

            double AVP = CalcSVP(MinT); // Actual vapour pressure

            for (int i = 0; i < 24; i++)
            {
                //AD - Have checked. This formula is equivalent to above
                //vpd.Add((1 - (rhs.yVals[i] / 100)) * svps.yVals[i]);
                vpd.Add(SVPs.YVals[i] - AVP);
                time.Add(i);
            }

            VPDs = new TableFunction(time.ToArray(), vpd.ToArray(), false);
        }
        //------------------------------------------------------------------------------------------------
        public void CalcTemps()
        {
            List<double> hours = new List<double>();
            List<double> temperatures = new List<double>();

            Angle aDelt = new Angle(23.45 * Math.Sin(2 * Math.PI * (284 + _DOY) / 365), AngleType.Deg);
            Angle temp2 = new Angle(Math.Acos(-Math.Tan(Latitude.Rad) * Math.Tan(aDelt.Rad)), AngleType.Rad);

            double dayLength = (temp2.Deg / 15) * 2;
            double nightLength = (24.0 - dayLength);                     // night hours
            // determine if the hour is during the day or night
            double t_mint = 12.0 - dayLength / 2.0 + ZLag;           // corrected dawn - GmcL
                                                                     //time of Tmin ??
            double t_suns = 12.0 + dayLength / 2.0;                  // sundown

            for (int t = 1; t <= 24; t++)
            {
                double hr = t;
                double temperature;

                if (hr >= t_mint && hr < t_suns)         //day
                {
                    double m = 0;
                    m = hr - t_mint;
                    temperature = (MaxT - MinT) * Math.Sin((Math.PI * m) / (dayLength + 2 * XLag)) + MinT;
                }
                else                             // night
                {
                    double n = 0;
                    if (hr > t_suns)
                    {
                        n = hr - t_suns;
                    }
                    if (hr < t_mint)
                    {
                        n = (24.0 - t_suns) + hr;
                    }
                    double m_suns = dayLength - ZLag;
                    double T_suns = (MaxT - MinT) * Math.Sin((Math.PI * m_suns) / (dayLength + 2 * XLag)) + MinT;
                    temperature = MinT + (T_suns - MinT) * Math.Exp(-YLag * n / nightLength);
                }
                hours.Add(hr - 1);
                temperatures.Add(temperature);
            }
            Temps = new TableFunction(hours.ToArray(), temperatures.ToArray(), false);
        }
    }
}
