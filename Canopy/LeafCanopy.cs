using System;
using System.Linq;


namespace LayerCanopyPhotosynthesis
{
    public class LeafCanopy
    {
        public PathwayParameters CPath;

        public PathwayParameters C3 { get; set; }
        public PathwayParameters C4 { get; set; }

        public delegate void Notifier();
        //[System.Xml.Serialization.XmlIgnore]
        public Notifier NotifyChanged;

        public delegate void LayerNumberChangeNotifier(int n);
        //[System.Xml.Serialization.XmlIgnore]
        public LayerNumberChangeNotifier LayerNumberChanged;

        protected double _LAI = 5;
        [ModelPar("IlH1M", "Total LAI of the plant", "L", "c", "m2/m2", "", "m2 leaf / m2 ground")]
        public double LAI
        {
            get
            {
                return _LAI;
            }
            set
            {
                _LAI = value;

                CalcLAILayers();
            }
        }

        protected int _nLayers;
        [ModelPar("umPBy", "Number of layers in canopy", "ln", "", "")]
        public int NLayers
        {
            get
            {
                return _nLayers;
            }
            set
            {
                _nLayers = value;
                InitArrays();
                if (LayerNumberChanged != null)
                {
                    LayerNumberChanged(_nLayers);
                }
            }
        }

        [ModelPar("yVwlh", "Number of layers in canopy", "ln", "", "")]
        public double NLayersD
        {
            get
            {
                return _nLayers;
            }
            set
            {
                _nLayers = (int)value;
                InitArrays();

                if (LayerNumberChanged != null)
                {
                    LayerNumberChanged(_nLayers);
                }
            }
        }

        protected double _leafAngle;
        [ModelPar("fV3qT", "Leaf angle (elevation)", "β", "", "°")]
        public double LeafAngle
        {
            get
            {
                return _leafAngle;
            }
            set
            {
                _leafAngle = value;

                for (int i = 0; i < _nLayers; i++)
                {
                    LeafAngles[i] = new Angle(value, AngleType.Deg);
                }
            }
        }

        [ModelVar("xt5rv", "Leaf angle (elevation) of layer(l)", "β", "", "°", "l")]
        public Angle[] LeafAngles { get; set; }

        [ModelVar("6kWOL", "Leaf angle (elevation) of layer(l)", "β", "", "°", "l")]
        public double[] LeafAnglesD
        {
            get
            {
                double[] vals = new double[NLayers];
                for (int i = 0; i < NLayers; i++)
                {
                    vals[i] = LeafAngles[i].Deg;
                }
                return vals;
            }
        }

        [ModelVar("dW3lC", "LAI of layer(l)", "LAI", "", "m2/m2", "l", "m2 leaf / m2 ground")]
        public double[] LAIs { get; set; }

        [ModelVar("GPoT1", "Total leaf nitrogen", "N", "c", "mmol N/m2", "", "")]
        public double TotalLeafNitrogen { get; set; }

        [ModelVar("7VTsD", "Accumulated LAI in each layer", "LAIc(l)", "", "")]
        public double[] LAIAccums { get; set; }

        [ModelVar("ryt1m", "Beam Penetration", "fsun(l)", "", "")]
        public double[] BeamPenetrations { get; set; }

        [ModelVar("202s3", "Sunlit LAI (integrated fun(l))", "LAISun(L)", "", "m2 leaf m-2 ground")]
        public double[] SunlitLAIs { get; set; }

        [ModelVar("gesEM", "Shaded LAI", "LAISh(l)", "", "m2 leaf m-2 ground")]
        public double[] ShadedLAIs { get; set; }

        [ModelVar("XeX9e", "Shadow projection coefficient", "G", "", "", "l,t")]
        public double[] ShadowProjectionCoeffs { get; set; }

        [ModelVar("yZD0V", "Radiation extinction coefficient of canopy", "kb", "", "", "l,t")]
        public double[] BeamExtCoeffs { get; set; }

        [ModelVar("BglXi", "Direct and scattered-direct PAR extinction co-efficient", "kb'", "", "", "l,t")]
        public double[] BeamScatteredBeams { get; set; }

        [ModelVar("yZD0V", "Radiation extinction coefficient of canopy", "kb", "", "", "l,t")]
        public double[] BeamExtCoeffsNIR { get; set; }

        [ModelVar("BglXi", "Direct and scattered-direct PAR extinction co-efficient", "kb'", "", "", "l,t")]
        public double[] BeamScatteredBeamsNIR { get; set; }

        [ModelVar("FZIMS", "Beam and scattered beam", "kb'", "", "")]
        public double Kb_
        {
            get
            {
                if (BeamScatteredBeams.Length > 0)
                {
                    return BeamScatteredBeams[0];
                }
                else
                {
                    return 0;
                }
            }
        }

        [ModelVar("FZIMS", "Beam and scattered beam", "kb'", "", "")]
        public double Kb_NIR
        {
            get
            {
                if (BeamScatteredBeamsNIR.Length > 0)
                {
                    return BeamScatteredBeamsNIR[0];
                }
                else
                {
                    return 0;
                }
            }
        }

        [ModelVar("tp8wi", "Beam radiation extinction coefficient of canopy", "kb", "", "")]
        public double Kb
        {
            get
            {
                if (BeamExtCoeffs.Length > 0)
                {
                    return BeamExtCoeffs[0];
                }
                else
                {
                    return 0;
                }
            }
        }

        [ModelVar("tp8wi", "Beam radiation extinction coefficient of canopy", "kb", "", "")]
        public double KbNIR
        {
            get
            {
                if (BeamExtCoeffsNIR.Length > 0)
                {
                    return BeamExtCoeffsNIR[0];
                }
                else
                {
                    return 0;
                }
            }
        }

        protected double _diffuseExtCoeff = 0.78;
        [ModelPar("naFkz", "Diffuse PAR extinction coefficient", "kd", "", "")]
        public double DiffuseExtCoeff
        {
            get
            {
                return _diffuseExtCoeff;
            }
            set
            {
                _diffuseExtCoeff = value;
                for (int i = 0; i < _nLayers; i++)
                {
                    DiffuseExtCoeffs[i] = value;
                }
            }
        }

        [ModelVar("4Vt4l", "Diffuse PAR extinction coefficient", "kd", "", "")]
        public double[] DiffuseExtCoeffs { get; set; }

        protected double _diffuseExtCoeffNIR = 0.78;
        [ModelPar("naFkz", "Diffuse PAR extinction coefficient", "kd", "", "")]
        public double DiffuseExtCoeffNIR
        {
            get
            {
                return _diffuseExtCoeffNIR;
            }
            set
            {
                _diffuseExtCoeffNIR = value;
                for (int i = 0; i < _nLayers; i++)
                {
                    DiffuseExtCoeffsNIR[i] = value;
                }
            }
        }

        [ModelVar("4Vt4l", "Diffuse PAR extinction coefficient", "kd", "", "")]
        public double[] DiffuseExtCoeffsNIR { get; set; }

        [ModelVar("ODCVR", "Diffuse and scattered-diffuse PAR extinction coefficient", "kd'", "", "", "l,t")]
        public double[] DiffuseScatteredDiffuses { get; set; }

        [ModelVar("ODCVR", "Diffuse and scattered-diffuse PAR extinction coefficient", "kd'", "", "", "l,t")]
        public double[] DiffuseScatteredDiffusesNIR { get; set; }

        protected double _diffuseScatteredDiffuse = 0.719;
        [ModelPar("sF61n", "Diffuse and scattered diffuse", "kd'", "", "")]
        public double DiffuseScatteredDiffuse
        {
            get
            {
                return _diffuseScatteredDiffuse;
            }
            set
            {
                _diffuseScatteredDiffuse = value;
                for (int i = 0; i < _nLayers; i++)
                {
                    DiffuseScatteredDiffuses[i] = value;
                }
            }
        }

        protected double _D0 = 0.04;
        [ModelPar("N5Imn", "Leaf to Air vapour pressure difference", "Do", "", "")]
        public double D0
        {
            get
            {
                return _D0;
            }
            set
            {
                _D0 = value;
            }
        }

        protected double _leafReflectionCoeff = 0.1;
        [ModelPar("XHAOb", "Leaf reflection coefficient for PAR", "ρ1", "", "")]
        public double LeafReflectionCoeff
        {
            get
            {
                return _leafReflectionCoeff;
            }
            set
            {
                _leafReflectionCoeff = value;
                for (int i = 0; i < _nLayers; i++)
                {
                    LeafReflectionCoeffs[i] = value;
                }
            }
        }

        [ModelVar("RmjoK", "Spectral Correction for J", "ja", "", "")] //
        public double Ja { get; set; } = 0.1;

        [ModelVar("tmH8F", "Leaf reflection coefficient for PAR", "ρ", "l", "")]
        public double[] LeafReflectionCoeffs { get; set; }

        protected double _leafTransmissivity = 0.05;
        [ModelPar("9EEzT", "Leaf transmissivity to PAR", "τ", "1", "")]
        public double LeafTransmissivity
        {
            get
            {
                return _leafTransmissivity;
            }
            set
            {
                _leafTransmissivity = value;
                for (int i = 0; i < _nLayers; i++)
                {
                    LeafTransmissivitys[i] = value;
                }
            }
        }

        [ModelVar("LIt10", "Leaf transmissivity to PAR", "τ", "1", "")]
        public double[] LeafTransmissivitys { get; set; }

        [ModelPar("k1pe7", "Leaf scattering coefficient of PAR", "σ", "", "")]
        public double[] LeafScatteringCoeffs { get; set; }

        protected double _leafScatteringCoeff = 0.15;
        [ModelPar("taH6E", "Leaf scattering coefficient of PAR", "σ", "", "")]
        public double LeafScatteringCoeff
        {
            get
            {
                return _leafScatteringCoeff;
            }
            set
            {
                _leafScatteringCoeff = value;
                for (int i = 0; i < _nLayers; i++)
                {
                    LeafScatteringCoeffs[i] = value;
                }
            }
        }

        [ModelPar("k1pe7", "Leaf scattering coefficient of PAR", "σ", "", "")]
        public double[] LeafScatteringCoeffsNIR { get; set; }

        protected double _leafScatteringCoeffNIR = 0.15;
        [ModelPar("taH6E", "Leaf scattering coefficient of PAR", "σ", "", "")]
        public double LeafScatteringCoeffNIR
        {
            get
            {
                return _leafScatteringCoeffNIR;
            }
            set
            {
                _leafScatteringCoeffNIR = value;
                for (int i = 0; i < _nLayers; i++)
                {
                    LeafScatteringCoeffsNIR[i] = value;
                }
            }
        }

        [ModelVar("mkTm5", "Reflection coefficient of a canopy with horizontal leaves", "ρh", "", "")]
        public double[] ReflectionCoefficientHorizontals { get; set; }

        [ModelVar("mkTm5", "Reflection coefficient of a canopy with horizontal leaves", "ρh", "", "")]
        public double[] ReflectionCoefficientHorizontalsNIR { get; set; }

        [ModelVar("2tQ4l", "Canopy-level reflection coefficient for direct PAR", "ρ", "cb", "", "l,t")]
        public double[] BeamReflectionCoeffs { get; set; }

        [ModelVar("2tQ4l", "Canopy-level reflection coefficient for direct PAR", "ρ", "cb", "", "l,t")]
        public double[] BeamReflectionCoeffsNIR { get; set; }

        protected double _diffuseReflectionCoeff = 0.036;
        [ModelPar("z7i2v", "Canopy-level reflection coefficient for diffuse PAR", "ρ", "cd", "")]
        public double DiffuseReflectionCoeff
        {
            get
            {
                return _diffuseReflectionCoeff;
            }
            set
            {
                _diffuseReflectionCoeff = value;
                for (int i = 0; i < _nLayers; i++)
                {
                    DiffuseReflectionCoeffs[i] = value;
                }
            }
        }

        [ModelVar("ftev5", "Canopy-level reflection coefficient for diffuse PAR", "ρ", "cd", "", "l,t")]
        public double[] DiffuseReflectionCoeffs { get; set; }

        protected double _diffuseReflectionCoeffNIR = 0.036;
        [ModelPar("z7i2v", "Canopy-level reflection coefficient for diffuse PAR", "ρ", "cd", "")]
        public double DiffuseReflectionCoeffNIR
        {
            get
            {
                return _diffuseReflectionCoeffNIR;
            }
            set
            {
                _diffuseReflectionCoeffNIR = value;
                for (int i = 0; i < _nLayers; i++)
                {
                    DiffuseReflectionCoeffsNIR[i] = value;
                }
            }
        }

        [ModelVar("ftev5", "Canopy-level reflection coefficient for diffuse PAR", "ρ", "cd", "", "l,t")]
        public double[] DiffuseReflectionCoeffsNIR { get; set; }

        [ModelVar("fcTBH", "Proportion of intercepted radiation", "F(l)", "", "")]
        public double[] PropnInterceptedRadns { get; set; }

        [ModelVar("gZLKz", "Proportion of intercepted radiation Accumulated", "F(c)", "", "")]
        public double[] PropnInterceptedRadnsAccum { get; set; }

        [ModelVar("264dv", "Proportion of intercepted radiation Accumulated", "F(c)", "", "")]
        public double Fc
        {
            get
            {
                return PropnInterceptedRadns.Sum();
            }
        }

        [ModelVar("pcSTq", "Total absorbed radiation for the canopy", "Iabs", "", "μmol m-2 ground s-1")]
        public double[] AbsorbedRadiation { get; set; }

        [ModelVar("pcSTq", "Total absorbed radiation for the canopy", "Iabs", "", "μmol m-2 ground s-1")]
        public double[] AbsorbedRadiationNIR { get; set; }

        [ModelVar("pcSTq", "Total absorbed radiation for the canopy", "Iabs", "", "μmol m-2 ground s-1")]
        public double[] AbsorbedRadiationPAR { get; set; }

        [ModelPar("", "", "'", "", "", "")]
        public double Rcp { get; set; } = 1200;

        [ModelPar("", "", "'", "", "", "")]
        public double Sigma { get; set; } = 5.67E-08;//5.668E-08;

        [ModelPar("", "", "'", "", "", "")]
        public double Lambda { get; set; } = 2447000;

        [ModelVar("", "", "'", "", "", "")]
        public double S { get; set; }

        [ModelVar("", "", "'", "", "", "")]
        public double ES { get; set; }

        [ModelVar("", "", "'", "", "", "")]
        public double ES1 { get; set; }

        [ModelVar("", "", "'", "", "", "")]
        public double Radi { get; set; }

        [ModelVar("", "", "'", "", "", "")]
        public double Rair { get; set; }

        [ModelVar("", "", "'", "", "", "")]
        public double DirectPAR;

        [ModelVar("", "", "'", "", "", "")]
        public double DiffusePAR;

        [ModelVar("", "", "'", "", "", "")]
        public double DirectNIR;

        [ModelVar("", "", "'", "", "", "")]
        public double DiffuseNIR;

        [ModelVar("", "", "'", "", "", "")]
        public double[] Gbh;

        [ModelVar("aW5lJ", "Constant", "C", "", "", "")]
        public double Constant { get; set; } = 0.047;
        //-----------------------------------------------------------------------
        public LeafCanopy(int numlayers, double lai)
        {
            _nLayers = numlayers;
            InitArrays();

            CalcLAILayers(lai);
        }
        //-----------------------------------------------------------------------
        public LeafCanopy(int numlayers, double lai, double leafangle)
        {
            _nLayers = numlayers;
            InitArrays();
            for (int i = 0; i < _nLayers; i++)
            {
                LeafAngles[i] = new Angle(leafangle, AngleType.Deg);
            }

            CalcLAILayers(lai);
        }
        //-----------------------------------------------------------------------
        public LeafCanopy(int numlayers, double lai, double[] leafangles)
        {
            _nLayers = numlayers;
            InitArrays();
            for (int i = 0; i < _nLayers; i++)
            {
                LeafAngles[i] = new Angle(leafangles[i], AngleType.Deg);
            }

            CalcLAILayers(lai);
        }
        //-----------------------------------------------------------------------
        public LeafCanopy()
        {
            _nLayers = 5;
            InitArrays();

            C3 = new PathwayParametersC3();
            C4 = new PathwayParametersC4();

            CPath = C3;
        }
        //-----------------------------------------------------------------------
        public void PhotoPathwayChanged(PhotosynthesisModel.PhotoPathway pathway)
        {
            if (pathway == PhotosynthesisModel.PhotoPathway.C3)
            {
                CPath = C3;
            }
            else
            {
                CPath = C4;
            }
        }
        //-----------------------------------------------------------------------

        protected void InitArrays()
        {
            LeafAngles = new Angle[_nLayers];
            LAIs = new double[_nLayers];
            LAIAccums = new double[_nLayers];
            BeamPenetrations = new double[_nLayers];
            SunlitLAIs = new double[_nLayers];
            ShadedLAIs = new double[_nLayers];
            ShadowProjectionCoeffs = new double[_nLayers];
            BeamExtCoeffs = new double[_nLayers];
            BeamScatteredBeams = new double[_nLayers];
            DiffuseExtCoeffs = new double[_nLayers];
            DiffuseScatteredDiffuses = new double[_nLayers];
            LeafReflectionCoeffs = new double[_nLayers];
            LeafTransmissivitys = new double[_nLayers];
            ReflectionCoefficientHorizontals = new double[_nLayers];
            BeamReflectionCoeffs = new double[_nLayers];
            DiffuseReflectionCoeffs = new double[_nLayers];
            LeafScatteringCoeffs = new double[_nLayers];
            PropnInterceptedRadns = new double[_nLayers];
            PropnInterceptedRadnsAccum = new double[_nLayers];
            AbsorbedRadiation = new double[_nLayers];

            BeamExtCoeffsNIR = new double[_nLayers];
            BeamScatteredBeamsNIR = new double[_nLayers];
            DiffuseExtCoeffsNIR = new double[_nLayers];
            DiffuseScatteredDiffusesNIR = new double[_nLayers];
            ReflectionCoefficientHorizontalsNIR = new double[_nLayers];
            BeamReflectionCoeffsNIR = new double[_nLayers];
            DiffuseReflectionCoeffsNIR = new double[_nLayers];
            LeafScatteringCoeffsNIR = new double[_nLayers];
            AbsorbedRadiationNIR = new double[_nLayers];
            AbsorbedRadiationPAR = new double[_nLayers];

            Us = new double[_nLayers];
            Rb_Hs = new double[_nLayers];
            Rb_H2Os = new double[_nLayers];
            Rb_CO2s = new double[_nLayers];
            Rts = new double[_nLayers];
            Rb_H_LAIs = new double[_nLayers];
            Rb_H2O_LAIs = new double[_nLayers];
            //boundryLayerConductance = new double[_nLayers];

            Ac = new double[_nLayers];
            Acgross = new double[_nLayers];

            BiomassC = new double[_nLayers];

            //Nitrogen variables
            LeafNs = new double[_nLayers];

            VcMax25 = new double[_nLayers];
            J2Max25 = new double[_nLayers];
            JMax25 = new double[_nLayers];
            Rd25 = new double[_nLayers];
            VpMax25 = new double[_nLayers];

            Gm25 = new double[_nLayers];

            LeafWidths = new double[_nLayers];

            Gbh = new double[_nLayers];

            LAI = LAI;
            LeafNTopCanopy = LeafNTopCanopy;

            LeafAngle = LeafAngle;
            LeafReflectionCoeff = LeafReflectionCoeff;
            LeafTransmissivity = LeafTransmissivity;
            DiffuseExtCoeff = DiffuseExtCoeff;
            DiffuseReflectionCoeff = DiffuseReflectionCoeff;
            LeafWidth = LeafWidth;

            DiffuseScatteredDiffuse = DiffuseScatteredDiffuse;

        }
        //-----------------------------------------------------------------------
        void CalcLAILayers(double lai)
        {
            LAI = lai;
        }
        //-----------------------------------------------------------------------
        void CalcLAILayers()
        {
            double LAITotal = 0;

            for (int i = 0; i < _nLayers; i++)
            {
                LAIs[i] = LAI / _nLayers;
                LAITotal += LAIs[i];

                LAIAccums[i] = LAITotal;
            }
        }
        //-----------------------------------------------------------------------
        public void CalcCanopyStructure(double sunAngleRadians)
        {
            for (int i = 0; i < _nLayers; i++)
            {
                //Shadow projection coefficient
                Angle θ0 = new Angle((LeafAngles[i].Rad > sunAngleRadians ? Math.Acos(1 / Math.Tan(LeafAngles[i].Rad) * Math.Tan(sunAngleRadians)) : -1), AngleType.Rad);
                if (θ0.Rad == -1)
                {
                    ShadowProjectionCoeffs[i] = Math.Cos(LeafAngles[i].Rad) * Math.Sin(sunAngleRadians);
                }
                else
                {
                    ShadowProjectionCoeffs[i] = 2 / Math.PI * Math.Sin(LeafAngles[i].Rad) * Math.Cos(sunAngleRadians) *
                        Math.Sin(θ0.Rad) + ((1 - θ0.Deg / 90) * Math.Cos(LeafAngles[i].Rad) * Math.Sin(sunAngleRadians));
                }

                //leafScatteringCoeffs[i] = leafReflectionCoeffs[i] + leafTransmissivitys[i];
                if (sunAngleRadians > 0)
                {
                    BeamExtCoeffs[i] = ShadowProjectionCoeffs[i] / Math.Sin(sunAngleRadians);
                    BeamExtCoeffsNIR[i] = ShadowProjectionCoeffs[i] / Math.Sin(sunAngleRadians);

                }
                else
                {
                    BeamExtCoeffs[i] = 0;
                    BeamExtCoeffsNIR[i] = 0;

                }
                BeamPenetrations[i] = Math.Exp(-1 * BeamExtCoeffs[i] * LAIAccums[i]);

                BeamScatteredBeams[i] = BeamExtCoeffs[i] * Math.Pow(1 - LeafScatteringCoeffs[i], 0.5);

                DiffuseScatteredDiffuses[i] = DiffuseExtCoeffs[i] * Math.Pow(1 - LeafScatteringCoeffs[i], 0.5);

                ReflectionCoefficientHorizontals[i] = (1 - Math.Pow(1 - LeafScatteringCoeffs[i], 0.5)) / (1 + Math.Pow(1 - LeafScatteringCoeffs[i], 0.5));

                BeamReflectionCoeffs[i] = 1 - Math.Exp(-2 * ReflectionCoefficientHorizontals[i] * BeamExtCoeffs[i] / (1 + BeamExtCoeffs[i]));

                //NIR
                BeamScatteredBeamsNIR[i] = BeamExtCoeffsNIR[i] * Math.Pow(1 - LeafScatteringCoeffsNIR[i], 0.5);

                DiffuseScatteredDiffusesNIR[i] = DiffuseExtCoeffsNIR[i] * Math.Pow(1 - LeafScatteringCoeffsNIR[i], 0.5);

                ReflectionCoefficientHorizontalsNIR[i] = (1 - Math.Pow(1 - LeafScatteringCoeffsNIR[i], 0.5)) / (1 + Math.Pow(1 - LeafScatteringCoeffsNIR[i], 0.5));

                BeamReflectionCoeffsNIR[i] = 1 - Math.Exp(-2 * ReflectionCoefficientHorizontalsNIR[i] * BeamExtCoeffsNIR[i] / (1 + BeamExtCoeffsNIR[i]));

                PropnInterceptedRadnsAccum[i] = 1 - Math.Exp(-BeamExtCoeffs[i] * LAIAccums[i]);

                PropnInterceptedRadns[i] = PropnInterceptedRadnsAccum[i] - (i == 0 ? 0 : PropnInterceptedRadnsAccum[i - 1]);
            }
        }
        //-----------------------------------------------------------------------
        void CalcAbsorbedRadiation(EnvironmentModel em)
        {
            // double[] radiation = new double[_nLayers];

            for (int i = 0; i < _nLayers; i++)
            {
                //radiation[i] = (1 - beamReflectionCoeffs[i]) * beamScatteredBeams[i] * em.directRadiation * Math.Exp(-beamScatteredBeams[i] * LAIAccums[i]) +
                //(1 - diffuseReflectionCoeffs[i]) * diffuseScatteredDiffuses[i] * em.diffuseRadiation * Math.Exp(-diffuseScatteredDiffuses[i] * LAIAccums[i]);
                AbsorbedRadiation[i] = (1 - BeamReflectionCoeffs[i]) * em.DirectRadiationPAR * ((i == 0 ? 1 : Math.Exp(-BeamScatteredBeams[i] * LAIAccums[i - 1])) - Math.Exp(-BeamScatteredBeams[i] * LAIAccums[i])) +
                    (1 - DiffuseReflectionCoeffs[i]) * em.DiffuseRadiationPAR * ((i == 0 ? 1 : Math.Exp(-DiffuseScatteredDiffuses[i] * LAIAccums[i - 1])) - Math.Exp(-DiffuseScatteredDiffuses[i] * LAIAccums[i]));

                // Calculate PAR and NIR in terms of energy
                DirectPAR = em.DirectRadiation * 0.5 * 1000000;
                DiffusePAR = em.DiffuseRadiation * 0.5 * 1000000;
                DirectNIR = em.DirectRadiation * 0.5 * 1000000;
                DiffuseNIR = em.DiffuseRadiation * 0.5 * 1000000;


                AbsorbedRadiationPAR[i] = (1 - BeamReflectionCoeffs[i]) * DirectPAR * ((i == 0 ? 1 : Math.Exp(-BeamScatteredBeams[i] * LAIAccums[i - 1])) - Math.Exp(-BeamScatteredBeams[i] * LAIAccums[i])) +
                    (1 - DiffuseReflectionCoeffs[i]) * DiffusePAR * ((i == 0 ? 1 : Math.Exp(-DiffuseScatteredDiffuses[i] * LAIAccums[i - 1])) - Math.Exp(-DiffuseScatteredDiffuses[i] * LAIAccums[i]));

                AbsorbedRadiationNIR[i] = (1 - BeamReflectionCoeffsNIR[i]) * DirectNIR * ((i == 0 ? 1 : Math.Exp(-BeamScatteredBeamsNIR[i] * LAIAccums[i - 1])) - Math.Exp(-BeamScatteredBeamsNIR[i] * LAIAccums[i])) +
                    (1 - DiffuseReflectionCoeffsNIR[i]) * DiffuseNIR * ((i == 0 ? 1 : Math.Exp(-DiffuseScatteredDiffusesNIR[i] * LAIAccums[i - 1])) - Math.Exp(-DiffuseScatteredDiffusesNIR[i] * LAIAccums[i]));
            }

            //TrapezoidLayer.integrate(_nLayers, absorbedRadiation, radiation, LAIs);
        }
        //-----------------------------------------------------------------------

        #region Nitrogen
        protected double _leafNTopCanopy = 137;
        [ModelPar("fgMsM", "Leaf N at canopy top", "N", "0", "mmol N/m2", "", "m2 leaf")]
        public double LeafNTopCanopy
        {
            get { return _leafNTopCanopy; }
            set { _leafNTopCanopy = value; }
        }

        protected double _NAllocationCoeff = 0.713;
        [ModelPar("s8zoc", "Coefficient of leaf N allocation in canopy", "k", "n", "")]
        public double NAllocationCoeff
        {
            get { return _NAllocationCoeff; }
            set { _NAllocationCoeff = value; }
        }

        protected double _Vpr_l = 80;
        [ModelPar("J2u5N", "PEP regeneration rate per unit leaf area at 25°C", "V", "pr_l", "μmol/m2/s", "", "m2 leaf", true)]
        public double Vpr_l
        {
            get { return _Vpr_l; }
            set { _Vpr_l = value; }
        }

        [ModelVar("xj9ot", "Total canopy N", "Nc", "", "mmol N m-2 ground")]
        public double Nc { get; set; }

        [ModelVar("HY9Qj", "Average canopy N", "Nc_av", "", "mmol N m-2 ground")]
        public double NcAv { get; set; }

        [ModelPar("ZW890", "Empirical spectral correction factor", "f", "", "")]
        public double F { get; set; } = 0.15;

        [ModelVar("nGIyH", "Leaf nitrogen distribution", "Nl", "", "g N m-2 leaf")]
        public double[] LeafNs { get; set; }

        [ModelVar("cVhgB", "Maximum rate of Rubisco carboxylation @ 25", "V", "c_Max@25°", "μmol/m2/s")]
        public double[] VcMax25 { get; set; }

        [ModelVar("MdQHB", "Maximum rate of electron transport  @ 25", "J2Max", "", "μmol/m2/s")]
        public double[] J2Max25 { get; set; }

        [ModelVar("zp4O3", "Maximum rate of electron transport  @ 25", "J", "Max@25°", "μmol/m2/s")]
        public double[] JMax25 { get; set; }

        [ModelVar("pbYnG", "Leaf day respiration @ 25°", "R", "d@25°", "μmol/m2/s")]
        public double[] Rd25 { get; set; }

        [ModelVar("WfOpd", "Maximum rate of P activity-limited carboxylation for the canopy @ 25", "V", "p_Max@25°", "μmol/m2/s", "", "", true)]
        public double[] VpMax25 { get; set; }

        [ModelVar("W4emd", "Gm @ 25", "G", "m@25°", "μmol/m2/s", "", "", true)]
        public double[] Gm25 { get; set; }

        protected double _θ2 = 0.7;
        [ModelPar("rClzy", "Convexity factor for response of J2 to absorbed PAR", "θ", "2", "")]
        public double θ2
        {
            get { return _θ2; }
            set { _θ2 = value; }
        }




        [ModelVar("glKdy", "SLN at canopy top", "SLNo", "", "")]
        public double SLNTop { get; set; }


        //protected double _fpseudo = 0.1;
        //[ModelPar("uz8TM", "Fraction of electrons at PSI that follow pseudocyclic transport", "f", "pseudo", "")]
        //public double FPseudo
        //{
        //    get { return _fpseudo; }
        //    set { _fpseudo = value; }
        //}

        //[ModelVar("beFC7", "Quantum efficiency of PSII e- transport under strictly limiting light", "α2", "2", "", "LL")]
        //public double A2 { get; set; }

        public double CalcSLN(double LAIAc, double structuralN)
        {
            return (LeafNTopCanopy - structuralN) * Math.Exp(-NAllocationCoeff *
                     LAIAc / LAIs.Sum()) + structuralN;
        }

        public double _B = Math.Round(30.0 / 44 * 0.6, 3);
        [ModelVar("Vm5Ix", "Biomass conversion efficiency ", "B", "", "", "")]
        public double B
        {
            get { return _B; }
            set { _B = value; }
        }

        //-----------------------------------------------------------------------
        void CalcLeafNitrogenDistribution(PhotosynthesisModel PM)
        {
            //-------------This is only when coupled with Apsim----------------------------------------
            //-------------Otherwise use parameters----------------------------------------------------
            if (PM.nitrogenModel == PhotosynthesisModel.NitrogenModel.APSIM)
            {
                SLNTop = PM.Canopy.CPath.SLNAv * PM.Canopy.CPath.SLNRatioTop;

                LeafNTopCanopy = SLNTop * 1000 / 14;

                NcAv = PM.Canopy.CPath.SLNAv * 1000 / 14;

                NAllocationCoeff = -1 * Math.Log((NcAv - PM.Canopy.CPath.StructuralN) / (LeafNTopCanopy - PM.Canopy.CPath.StructuralN)) * 2;
            }
            //-------------This is only when coupled with Apsim----------------------------------------
            else
            {
                SLNTop = LeafNTopCanopy / 1000 * 14;

                NcAv = (LeafNTopCanopy - PM.Canopy.CPath.StructuralN) * Math.Exp(-0.5 * NAllocationCoeff) + PM.Canopy.CPath.StructuralN;

                PM.Canopy.CPath.SLNAv = NcAv / 1000 * 14;

                PM.Canopy.CPath.SLNRatioTop = SLNTop / PM.Canopy.CPath.SLNAv;
            }

            for (int i = 0; i < _nLayers; i++)
            {
                LeafNs[i] = CalcSLN(LAIAccums[i], PM.Canopy.CPath.StructuralN);

                VcMax25[i] = LAI * CPath.PsiVc * (LeafNTopCanopy - PM.Canopy.CPath.StructuralN) * (
                    (i == 0 ? 1 : Math.Exp(-NAllocationCoeff * LAIAccums[i - 1] / LAI)) -
                    Math.Exp(-NAllocationCoeff * LAIAccums[i] / LAI)) / NAllocationCoeff;

                //J2Max25[i] = LAI * CPath.PsiJ2 * (LeafNTopCanopy - PM.Canopy.CPath.StructuralN) * (
                //    (i == 0 ? 1 : Math.Exp(-NAllocationCoeff * LAIAccums[i - 1] / LAI)) -
                //    Math.Exp(-NAllocationCoeff * LAIAccums[i] / LAI)) / NAllocationCoeff;

                JMax25[i] = LAI * CPath.PsiJ * (LeafNTopCanopy - PM.Canopy.CPath.StructuralN) * (
                   (i == 0 ? 1 : Math.Exp(-NAllocationCoeff * LAIAccums[i - 1] / LAI)) -
                   Math.Exp(-NAllocationCoeff * LAIAccums[i] / LAI)) / NAllocationCoeff;

                Rd25[i] = LAI * CPath.PsiRd * (LeafNTopCanopy - PM.Canopy.CPath.StructuralN) * (
                    (i == 0 ? 1 : Math.Exp(-NAllocationCoeff * LAIAccums[i - 1] / LAI)) -
                    Math.Exp(-NAllocationCoeff * LAIAccums[i] / LAI)) / NAllocationCoeff;

                VpMax25[i] = LAI * CPath.PsiVp * (LeafNTopCanopy - PM.Canopy.CPath.StructuralN) * (
                    (i == 0 ? 1 : Math.Exp(-NAllocationCoeff * LAIAccums[i - 1] / LAI)) -
                    Math.Exp(-NAllocationCoeff * LAIAccums[i] / LAI)) / NAllocationCoeff;

                //Gm25[i] = CPath.PsiGm * (NcAv - PM.Canopy.CPath.StructuralN) + CPath.CGm;
                Gm25[i] = LAI* CPath.PsiGm * (LeafNTopCanopy - PM.Canopy.CPath.StructuralN) * (
                   (i == 0 ? 1 : Math.Exp(-NAllocationCoeff * LAIAccums[i - 1] / LAI)) -
                   Math.Exp(-NAllocationCoeff * LAIAccums[i] / LAI)) / NAllocationCoeff;
            }
        }

        #endregion


        #region InstantaneousPhotosynthesis
        [ModelPar("AweVY", "", "k2(LL)", "", "")]
        public double K2 { get; set; } = 0.284;

        [ModelPar("OlCWb", "Convexity factor for response of J to PAR", "θ", "", "")]
        public double Theta { get; set; } = 0.7;

        [ModelPar("4N7O4", "Oxygen partial pressure inside leaves", "O", "l", "μbar")]
        public double OxygenPartialPressure { get; set; } = 210000;

        [ModelPar("ojS8u", "Ratio of leaf respiration to PS Rubisco capacity", "Rd/Vcmax", "", "-")]
        public double RespirationRubiscoRatio { get; set; } = 0;

        [ModelPar("aGpUj", "Ambient air CO2 partial pressure", "C", "a", "μbar")]
        public double Ca { get; set; } = 380;

        [ModelPar("wUyRh", "Chloroplast CO2 partial pressure initial guess", "CcInit", "", "μbar")]
        public double CcInit { get; set; } = 100;

        [ModelVar("ATmtA", "Instantaneous net canopy Assimilation", "Ac (gross)", "", "μmol CO2 m-2 ground s-1")]
        public double[] InstantaneousAssimilation { get; set; }

        /// <summary>
        /// Factor relating linear e- flow to the ATP production portion of J
        /// </summary>
        public double z { get; set; }

        /// <summary>
        /// Fraction of the e- flow out of PSI that proceeds via cyclic e- flow
        /// </summary>
        public double fcyc { get; set; }
        #endregion

        #region Daily canopy biomass accumulation
        [ModelVar("XiJxc", "Ac", "Ac", "", "g CO2 m-2 ground s-1")]
        public double[] Ac { get; set; }
        [ModelVar("SA871", "Ac gross", "Ac", "", "g CO2 m-2 ground s-1")]
        public double[] Acgross { get; set; }

        [ModelPar("lkgr9", "", "", "", "")]
        public double HexoseToCO2 { get; set; }= 0.681818182;
        
        [ModelPar("TCFyz", "Biomass to hexose ratio", "", "Biomass:hexose", "g biomass/g hexose")]
        public double BiomassToHexose { get; set; } = 0.75;
        
        [ModelPar("ynLXn", "Maintenance and growth respiration to hexose ratio", "", "Respiration:hexose", "g hexose/g CO2")]
        public double MaintenanceRespiration { get; set; } = 0.075;

        [ModelVar("vUYQG", "Total biomass accumulation", "BiomassC", "", "g biomass m-2 ground hr-1")]
        public double TotalBiomassC { get; set; }

        [ModelVar("ZKlcU", "Total biomass accumulation", "BiomassC", "", "g biomass m-2 ground hr-1")]
        public double[] BiomassC { get; set; }

        [ModelVar("sAF5H", "", "", "", "")]
        public double Sco { get; set; }
        #endregion

        #region Conductance and Resistance Parameters
        [ModelVar("fJ3nt", "Leaf boundary layer resistance for heat", "rb_H", "", "s m-1")]
        public double[] Rb_Hs { get; set; }

        [ModelVar("am5HZ", "Leaf boundary layer resistance for H2O", "rb_H2O", "", "s m-1")]
        public double[] Rb_H2Os { get; set; }

        [ModelVar("zjhMW", "Leaf boundary layer resistance for CO2", "rb_CO2", "", "s m-1")]
        public double[] Rb_CO2s { get; set; }


        protected double _gs0_CO2 = 0.01;
        [ModelPar("zpNXx", "Residual stomatal conductance of CO2", "g", "s_CO2", "mol/m2/s", "", "mol H2O, m2 leaf")]
        public double Gs0_CO2
        {
            get { return _gs0_CO2; }
            set { _gs0_CO2 = value; }
        }


        protected double _gs_CO2 = 0.3;
        [ModelPar("jpiir", "Stomatal conductance of CO2", "g", "s_CO2", "mol/m2/s", "", "mol H2O, m2 leaf")]
        public double Gs_CO2
        {
            get { return _gs_CO2; }
            set { _gs_CO2 = value; }
        }


        protected double _gm_0 = 0;
        [ModelPar("7J9FU", "", "gm_0", "", "")]
        public double Gm_0
        {
            get { return _gm_0; }
            set { _gm_0 = value; }
        }

        protected double _gm_delta = 1.35;
        [ModelPar("WTRZb", "", "gm_delta", "", "")]
        public double Gm_delta
        {
            get { return _gm_delta; }
            set { _gm_delta = value; }
        }

        protected double _a = 74.7;
        [ModelPar("LUm53", "Empirical coefficient of the impact function of VDPla", "a", "", "")]
        public double A
        {
            get { return _a; }
            set { _a = value; }
        }

        protected double _Do = 0.04;
        [ModelPar("n1lDz", "Emprical coefficient for fvpd", "D", "o", "kPa")]
        public double Do
        {
            get { return _Do; }
            set { _Do = value; }
        }

        [ModelVar("7uBqi", "Molar density of air", "ρa", "", "mol m-3")]
        public double Ra { get; set; }
        #endregion

        #region Leaf temperature from Penman-Monteith combination equation (isothermal form)
        [ModelPar("XVJhn", "Energy conversion ratio", "", "", "J s-1 m-2 : mmol m-2 s-1")]
        public double EnergyConvRatio { get; set; } = 0.208;

        [ModelPar("zJyMY", "Stefan-Boltzmann constant", "Bz", "", "J s-1 K-4")]
        public double Bz { get; set; } = 5.67038E-08;

        [ModelPar("baxMC", "Latent heat of vaporization of water vapour", "l", "", "MJ kg-1")]
        public double L { get; set; } = 2.26;

        [ModelPar("6mb4O", "Mwratio", "", "", "")]
        public double MwRatio { get; set; } = 0.622;

        [ModelPar("tpm6l", "Atmospheric pressure", "p", "", "kPa")]
        public double P { get; set; } = 101.325;

        [ModelPar("sc9d8", "Crop height", "H", "", "m")]
        public double Height { get; set; } = 1.5;

        [ModelPar("iggg1", "Wind speed at canopy top", "u", "0", "m/s")]
        public double U0 { get; set; } = 2;

        [ModelPar("Sj4Gm", "Extinction coefficient for wind speed", "ku", "", "")]
        public double Ku { get; set; } = 0.5;

        protected double _leafWidth = 0.1;
        [ModelPar("8cdc4", "Leaf width", "w", "l", "m")]
        public double LeafWidth
        {
            get { return _leafWidth; }
            set
            {
                _leafWidth = value;
                for (int i = 0; i < NLayers; i++)
                {
                    LeafWidths[i] = _leafWidth;
                }
            }
        }

        [ModelVar("DulsD", "Leaf width", "wl", "", "m", "l")]
        public double[] LeafWidths { get; set; }

        [ModelVar("D4mwj", "Air density	rair (weight)", "", "", "kg m-3")]
        public double AirDensity { get; set; }
        //{
        //    get { return _airDensity; }
        //    set { _airDensity = value; }
        //}

        [ModelPar("7i0In", "Specific heat of air", "cp", "", "J kg-1 K-1")]
        public double Cp { get; set; } = 1000;

        [ModelPar("H7wDs", "Vapour pressure of air", "Vair", "", "kPa")]
        public double Vair { get; set; } = 1.6;

        [ModelVar("yp5fX", "", "fvap", "", "")]
        public double FVap { get; set; }
        [ModelVar("553hc", "", "fclear", "", "")]
        public double FClear { get; set; }
        [ModelVar("WRc27", "Saturated water vapour pressure @ Ta", "es(Ta)", "", "")]
        public double ES_Ta { get; set; }
        [ModelVar("2wZdI", "Turbulence resistance (same for heat, CO2 and H2O)", "rt", "", "s m-1 (LAIsun,sh)")]
        public double[] Rts { get; set; }
        [ModelVar("Qn31e", "Leaf boundary layer resistance for heat", "rb_H", "", "s m-1")]
        public double[] Rb_H_LAIs { get; set; }
        [ModelVar("efoaj", "Leaf boundary layer resistance for H2O", "rb_H2O", "", "s m-1")]
        public double[] Rb_H2O_LAIs { get; set; }
        [ModelVar("LpNUY", "Wind speed", "u", "", "m/s", "l")]
        public double[] Us { get; set; }
        [ModelVar("c0pfL", "Leaf-to-air vapour pressure difference", "Da", "", "kPa")]
        public double Da { get; set; }
        [ModelVar("WYVVq", "Psychrometric constant", "g", "", "kPa K-1")]
        public double G { get; set; } = 0.066;
        [ModelVar("M0Rdv", "Half the reciprocal of Sc/o", "", "γ*", "")]
        public double G_ { get; set; }

        void CalcConductanceResistance(PhotosynthesisModel PM)
        {
            for (int i = 0; i < _nLayers; i++)
            {
                //Intercepted radiation

                //Saturated vapour pressure
                ES = PM.EnvModel.CalcSVP(PM.EnvModel.GetTemp(PM.Time));
                ES1 = PM.EnvModel.CalcSVP(PM.EnvModel.GetTemp(PM.Time) + 1);
                S = (ES1 - ES) / ((PM.EnvModel.GetTemp(PM.Time) + 1) - PM.EnvModel.GetTemp(PM.Time));


                //Wind speed
                //us[i] = u0 * Math.Exp(-ku * (i + 1));
                Us[i] = U0;

                Rair = PM.EnvModel.ATM * 100000 / (287 * (PM.EnvModel.GetTemp(PM.Time) + 273)) * 1000 / 28.966;

                Gbh[i] = 0.01 * Math.Pow((Us[i] / LeafWidth), 0.5) *
                    (1 - Math.Exp(-0.5 * Ku * LAI)) / (0.5 * Ku);

                //Boundary layer
                //rb_Hs[i] = 100 * Math.Pow((leafWidths[i] / us[i]), 0.5);
                //rb_H2Os[i] = 0.93 * rb_Hs[i];

                //rb_CO2s[i] = 1.37 * rb_H2Os[i];

                //rts[i] = 0.74 * Math.Pow(Math.Log(2 - 0.7 * Height) / (0.1 * Height), 2) / (0.16 * us[i]) / LAIs[i];
                //rb_H_LAIs[i] = rb_Hs[i] / LAIs[i];
                //rb_H2O_LAIs[i] = rb_H2Os[i] / LAIs[i];

            }
        }

        public void CalcLeafTemperature(PhotosynthesisModel PM, EnvironmentModel EM)
        {
            //double airTemp = EM.getTemp(PM.time);

            //fvap = 0.56 - 0.079 * Math.Pow(10 * Vair, 0.5);

            //fclear = 0.1 + 0.9 * Math.Max(0, Math.Min(1, (EM.atmTransmissionRatio - 0.2) / 0.5));

            //g = (cp * Math.Pow(10, -6)) * p / (l * mwRatio);

            //es_Ta = 5.637E-7 * Math.Pow(airTemp, 4) + 1.728E-5 * Math.Pow(airTemp, 3) + 1.534E-3 *
            //    Math.Pow(airTemp, 2) + 4.424E-2 * airTemp + 6.095E-1;

            //a2 = CPath.F2 * (1 - CPath.fcyc) / (CPath.F2 / CPath.F1 + (1 - CPath.fcyc));

        }

        #endregion


        public void CalcCanopyBiomassAccumulation(PhotosynthesisModel PM)
        {
            //for (int i = 0; i < nLayers; i++)
            //{
            //    //TODO -- Rename / refactor variables to reflect units and time
            //    //TODO -- calculate biomass using B after daily A has ben summed (ie 1 calculation per day)
            //    //TODO -- check that we are only calculating between dawn and dusk
            //    //TODO -- use floor and cieling on dusk and dawn to calculate assimilation times

            //    Ac[i] = (PM.sunlit.A[i] + PM.shaded.A[i]) * 3600; // Rename (Acan, hour)  (umolCo2/m2/s)

            //    // Acgross[i] = Ac[i] * Math.Pow(10, -6) * 44; // (gCo2/m2/s)

            //    biomassC[i] = Ac[i] * 44 * B * Math.Pow(10, -6); // Hourly Biomass
            //}
            //totalBiomassC = biomassC.Sum();
        }



        public void Run(PhotosynthesisModel PM, EnvironmentModel EM)
        {
            //calcCanopyStructure(EM.sunAngle.rad);

            CalcAbsorbedRadiation(EM);
            CalcLeafNitrogenDistribution(PM);
            CalcConductanceResistance(PM);
            CalcLeafTemperature(PM, EM);
            CalcTotalLeafNitrogen(PM);
        }


        public void CalcTotalLeafNitrogen(PhotosynthesisModel PM)
        {
            TotalLeafNitrogen = LAI * ((LeafNTopCanopy - PM.Canopy.CPath.StructuralN) * (1 - Math.Exp(-NAllocationCoeff)) / NAllocationCoeff + PM.Canopy.CPath.StructuralN);
        }
        //////////////////////////////////////////////////////////////////////////////////
        //-----------C4 Section ----------------------------------------------------------
        //////////////////////////////////////////////////////////////////////////////////

        [ModelPar("L8PzL", "Fraction of PS II activity in the bundle sheath", "α", "", "")]
        public double Alpha { get; set; } = 0.1;
       
        [ModelPar("pE0qz", "Conductance to CO2 leakage from the bundle sheath to mesophyll", "g", "bs_CO2", "mol/m2/s", "", "mol of H20, m2 leaf", true)]
        public double Gbs_CO2 { get; set; } = 0.003;
       
        [ModelPar("iJHqO", "Fraction of electron transport operating in the Q-cycle", "f", "q", "")]
        public double FQ { get; set; } = 1;

        //[ModelPar("EdOLY", "Number of protons, generated by the electron transport chain, required to produce one ATP", "h", "", "")]
        //public double H { get; set; } = 3;

        [ModelPar("fChzp", "Fraction of electron transport partitioned to mesophyll chloroplasts", "x", "", "")]
        public double X { get; set; } = 0.4;

        //[ModelVar("3sYmP", "Mesophyll oxygen partial pressure", "Om", "", "μbar")]  //Om==Oi
        //public double Om { get; set; }
    }
}
