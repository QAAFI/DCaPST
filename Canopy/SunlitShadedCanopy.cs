using System;
using System.Linq;

namespace LayerCanopyPhotosynthesis
{
    public enum SSType { Ac1, Ac2, Aj };

    public enum TranspirationMode { limited, unlimited };

    public class SunlitShadedCanopy
    {
        public SSType type;
        public int _nLayers;

        public double CmTolerance = 1;
        public double leafTempTolerance = 0.5;

        [ModelVar("yeYEA", "LAI of the sunlit and shade leaf fractions", "LAI", "", "m2/m2", "l,t", "m2 leaf/m2 ground")]
        public double[] LAIS { get; set; }
        [ModelVar("aRNmW", "Irradiance incident on leaves", "I", "inc", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] IncidentIrradiance { get; set; }
        [ModelVar("wKFLV", "PAR absorbed by the sunlit and shade leaf fractions", "I", "abs", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] AbsorbedIrradiance { get; set; }
        [ModelVar("0PWTP", "Direct-beam absorbed by leaves", "Integrate over LAIsun", "", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] AbsorbedRadiationDirect { get; set; }
        [ModelVar("OsYZA", "Diffuse absorbed by leaves", "Integrate over LAIsun", "", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] AbsorbedRadiationDiffuse { get; set; }
        [ModelVar("jRJiA", "Scattered-beam absorbed by leaves", "Integrate over LAIsun", "", "μmol m-2 ground s-1")]
        public double[] AbsorbedRadiationScattered { get; set; }
        [ModelVar("wKFLV", "PAR absorbed by the sunlit and shade leaf fractions", "I", "abs", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] AbsorbedIrradianceNIR { get; set; }
        [ModelVar("0PWTP", "Direct-beam absorbed by leaves", "Integrate over LAIsun", "", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] AbsorbedRadiationDirectNIR { get; set; }
        [ModelVar("OsYZA", "Diffuse absorbed by leaves", "Integrate over LAIsun", "", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] AbsorbedRadiationDiffuseNIR { get; set; }
        [ModelVar("jRJiA", "Scattered-beam absorbed by leaves", "Integrate over LAIsun", "", "μmol m-2 ground s-1")]
        public double[] AbsorbedRadiationScatteredNIR { get; set; }
        [ModelVar("wKFLV", "PAR absorbed by the sunlit and shade leaf fractions", "I", "abs", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] AbsorbedIrradiancePAR { get; set; }
        [ModelVar("0PWTP", "Direct-beam absorbed by leaves", "Integrate over LAIsun", "", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] AbsorbedRadiationDirectPAR { get; set; }
        [ModelVar("OsYZA", "Diffuse absorbed by leaves", "Integrate over LAIsun", "", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] AbsorbedRadiationDiffusePAR { get; set; }
        [ModelVar("jRJiA", "Scattered-beam absorbed by leaves", "Integrate over LAIsun", "", "μmol m-2 ground s-1")]
        public double[] AbsorbedRadiationScatteredPAR { get; set; }
        [ModelVar("hwe6f", "Vcmax for the sunlit and shade leaf fractions  @ 25°", "V", "c_max", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] VcMax25 { get; set; }
        [ModelVar("nX8u7", "Vcmax for the sunlit and shade leaf fractions  @ T°", "V", "c_max", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] VcMaxT { get; set; }
        [ModelVar("GGrx7", "Gm @ 25°", "G", "m_25", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] Gm25 { get; set; }
        [ModelVar("Fl7rY", "Gm @ T", "G", "m_t", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] GmT { get; set; }
        [ModelVar("y1rt7", "Maximum rate of P activity-limited carboxylation @ 25°", "V", "p_max", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] VpMax25 { get; set; }
        [ModelVar("Bl7oY", "Maximum rate of Rubisco carboxylation", "V", "p_max", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] VpMaxT { get; set; }
        [ModelVar("2cAQn", "J2max for the sunlit and shade leaf fractions @ 25°", "J2", "max", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] J2Max25 { get; set; }
        [ModelVar("UMOz0", "J2max for the sunlit and shade leaf fractions @ T°", "J2", "max", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] J2MaxT { get; set; }
        [ModelVar("lAx_5b", "Jmax for the sunlit and shade leaf fractions @ 25°", "J", "max", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] JMax25 { get; set; }
        [ModelVar("Gx6ir", "Maximum rate of electron transport", "J", "max", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] JMaxT { get; set; }
        [ModelVar("xMC5L", "Temperature of sunlit and shade leaf fractions", "T", "l", "°C", "l,t")]
        public double[] LeafTemp { get; set; }
        [ModelVar("xMC5L", "Temperature of sunlit and shade leaf fractions", "T", "l", "°C", "l,t")]
        public double[] LeafTemp_1 { get; set; }
        [ModelVar("xMC5L", "Temperature of sunlit and shade leaf fractions", "T", "l", "°C", "l,t")]
        public double[] LeafTemp_2 { get; set; }
        [ModelVar("VOep5", "Leaf-to-air vapour pressure deficit for sunlit and shade leaf fractions", "VPD", "al", "kPa", "l,t")]
        public double[] VPD { get; set; }
        [ModelVar("YC5yq", "", "", "", "")]
        public double[] FVPD { get; set; }
        [ModelVar("N14in", "PEP regeneration rate at Temp", "V", "pr", "μmol/m2/s", "l,t")]
        public double[] Vpr { get; set; }
        [ModelVar("opoaH", "Leaf boundary layer conductance for CO2 for the sunlit and shade fractions", "g", "b_CO2", "mol/m2/s", "l,t", "mol H20, m2 ground")]
        public double[] Gb_CO2 { get; set; }
        [ModelVar("L3FnM", "Leakiness", "ɸ", "", "", "l,t")]
        public double[] F { get; set; }
        [ModelVar("VBqDL", "Half the reciprocal of Sc/o", "γ*", "", "", "l,t")]
        public double[] G_ { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double[] LeafTemp__ { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double[] Cm__ { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double[] Cm_1 { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double[] Cm_2 { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double[] Gsw { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double[] Rsw { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double[] ES1 { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double[] VPD_la { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double[] Elambda_ { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double[] TDelta { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double[] Elambda { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double[] E_PC { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double[] Gbh { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double[] Gbw { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double[] Rbw { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double[] Rbh { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double[] Gbw_m { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double[] GbCO2 { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double[] GsCO2 { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double[] Gsh { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double XX { get; set; }
        //Numeric soution variables
        [ModelVar("", "", "", "", "", "")]
        public double x_1 { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double x_2 { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double x_3 { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double x_4 { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double x_5 { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double x_6 { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double x_7 { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double x_8 { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double x_9 { get; set; }

        [ModelVar("", "", "", "", "", "")]
        public double p { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double q { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double m { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double t { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double b { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double j { get; set; }
        [ModelVar("", "", "", "", "", "")]
        public double e { get; set; } 
        [ModelVar("", "", "", "", "", "")]
        public double R { get; set; }




        [ModelVar("34Dses", "Water Use", "", "E", "mm/hr", "")]
        public double[] WaterUse { get; set; }
        [ModelVar("htr6De", "Water Use", "", "E", "mols/s", "")]
        public double[] WaterUseMolsSecond { get; set; }

        public double[] Cb { get; set; }

        #region Instaneous Photosynthesis
        [ModelVar("0svKg", "Rate of electron transport of sunlit and shade leaf fractions", "J", "", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] J { get; set; }
        [ModelVar("pmvS3", "Rate of e- transport through PSII", "J2", "", "μmol/m2/s", "l,t")]
        //public double[] J2 { get; set; }
        //[ModelVar("Q7w4j", "CO2 compensation point in the absence of Rd for the sunlit and shade leaf fractions for layer l", "Γ*", "", "μbar", "l,t")]
        public double[] r_ { get; set; }
        [ModelVar("LL1b6", "Relative CO2/O2 specificity factor for Rubisco", "S", "c/o", "bar/bar", "l,t")]
        public double[] ScO { get; set; }
        [ModelVar("ZLENJ", "Kc for the sunlit and shade leaf fractions for layer l", "K", "c", "μbar", "l,t")]
        public double[] Kc { get; set; }
        [ModelVar("TMWa9", "Ko for the sunlit and shade leaf fractions for layer l", "K", "o", "μbar", "l,t")]
        public double[] Ko { get; set; }
        [ModelVar("P0HgR", "", "", "", "", "")]
        public double[] VcVo { get; set; }
        [ModelVar("hv6R5", "Effective Michaelis-Menten constant", "K'", "", "μbar")]
        public double[] K_ { get; set; }
        [ModelVar("AyR6r", "Rd for the sunlit and shade leaf fractions @ 25°", "Rd @ 25", "", "μmol/m2/s1", "l,t", "m2 ground")]
        public double[] Rd25 { get; set; }
        [ModelVar("hJOu6", "Rd for the sunlit and shade leaf fractions @ T°", "R", "d", "μmol/m2/s1", "l,t", "m2 ground")]
        public double[] RdT { get; set; }
        [ModelVar("CZFJx", "min(Aj, Ac) of sunlit and shade leaf fractions for layer l", "A", "", "μmol CO2/m2/s", "l,t", "m2 ground")]
        public double[] A { get; set; }
        [ModelVar("YFblo", "Leaf stomatal conductance for CO2 for the sunlit and shade fractions", "g", "s_CO2", "mol/m2/s", "l,t", "mol H20, m2 ground")]
        public double[] gs_CO2 { get; set; }
        [ModelVar("XrAH2", "Leaf mesophyll conductance for CO2 for the sunlit and shade fractions", "g", "m_CO2", "mol/m2/s", "l,t", "mol H20, m2 ground")]
        public double[] gm_CO2T { get; set; }
        [ModelVar("cozpi", "Intercellular CO2 partial pressure for the sunlit and shade leaf fractions", "C", "i", "μbar", "l,t")]
        public double[] Ci { get; set; }
        [ModelVar("8VmlI", "Chloroplastic CO2 partial pressure for the sunlit and shade leaf fractions", "C", "c", "μbar", "l,t")]
        public double[] Cc { get; set; }
        #endregion

        #region Conductance and Resistance parameters

        #endregion

        #region Leaf temperature from Penman-Monteith combination equation (isothermal form)
        [ModelVar("B2QqH", "Laten heat of vaporizatin for water * evaporation rate", "lE", "", "J s-1 kg-1 * kg m-2")]
        public double[] lE { get; set; }
        [ModelVar("G3rwq", "Slope of curve relating saturating water vapour pressure to temperature", "s", "", "kPa K-1")]
        public double[] s { get; set; }
        [ModelVar("rJSil", "Net isothermal radiation absorbed by the leaf", "Rn", "", "J s-1 m-2")]
        public double[] Rn { get; set; }
        [ModelVar("be7vB", "Absorbed short-wave radiation (PAR + NIR)", "Sn", "", "J s-1 m-2")]
        public double[] Sn { get; set; }
        [ModelVar("KbcZq", "Outgoing long-wave radiation", "R↑", "", "J s-1m-2")]
        public double[] R_ { get; set; }
        [ModelVar("EYrht", "Saturated water vapour pressure @ Tl", "es(Tl)", "", "")]
        public double[] es { get; set; }
        [ModelVar("ii3Kr", "Leaf stomatal resistance for H2O", "rs_H2O", "", "s m-1")]
        public double[] rs_H2O { get; set; }
        [ModelVar("a4Pbr", "", "gs", "", "m s-1")]
        public double[] gs { get; set; }
        [ModelVar("XenBm", "Michealise-Menten constant of PEPc for CO2", "K", "p", "μbar", "l,t", "", true)]
        public double[] Kp { get; set; }
        [ModelVar("09u65", "(Mesophyll oxygen partial pressure for sunlit and shade leaf fractions,", "O", "m", "μbar", "l,t")]
        public double[] Om { get; set; }
        [ModelVar("85YYK", "Intercellular oxygen partial pressure for sunlit and shade leaf fractions", "O", "i", "μbar", "l,t")]
        public double[] Oi { get; set; }
        [ModelVar("nIyeA", "Ci to Ca ratio", "Ci/Ca", "ratio", "", "l,t")]
        public double[] CiCaRatio { get; set; }
        #endregion

        [ModelVar("s4Vg0", "Oxygen partial pressure at the oxygenating site of Rubisco in the chloroplast for sunlit and shade leaf fractions", "O", "c", "μbar", "l,t")]
        public double[] Oc { get; set; }
        [ModelVar("mmxWN", "Mesophyll CO2 partial pressure for the sunlit and shade leaf fractions", "C", "m", "μbar", "l,t", "", true)]
        public double[] Cm { get; set; }
        [ModelVar("7u3JF", "Chloroplast or BS CO2 partial pressuer", "Cbs", "", "μbar")]
        public double[] Cbs { get; set; }
        [ModelVar("JiUIt", "Rate of PEPc", "V", "p", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] Vp { get; set; }
        [ModelVar("FCzCY", "Gross canopy CO2 assimilation per second", "Ag", "", "μmol CO2 m-2 ground s-1")]
        public double[] Ag { get; set; }
        [ModelVar("a2eBP", "", "", "", "")]
        public double[] L { get; set; }
        [ModelVar("iuUvg", "Mitochondrial respiration occurring in the mesophyll", "R", "m", "μmol/m2/s", "l,t", "m2 ground")]
        public double[] Rm { get; set; }
        [ModelVar("oem3o", "Conductance to CO2 leakage from the bundle sheath to mesophyll", "g", "bsCO2", "mol/m2/s", "l,t", "mol of H20, m2 leaf", true)]
        public double[] Gbs { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //---------------------------------------------------------------------------------------------------------
        public SunlitShadedCanopy() { }
        //---------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nLayers"></param>
        /// <param name="type"></param>
        public SunlitShadedCanopy(int nLayers, SSType type)
        {
            //_nLayers = nLayers;
            this.type = type;
            InitArrays(nLayers);
        }
        //---------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nLayers"></param>
        public void InitArrays(int nLayers)
        {
            _nLayers = nLayers;

            IncidentIrradiance = new double[nLayers];
            AbsorbedRadiationDirect = new double[nLayers];
            AbsorbedRadiationDiffuse = new double[nLayers];
            AbsorbedRadiationScattered = new double[nLayers];
            AbsorbedIrradiance = new double[nLayers];

            AbsorbedIrradianceNIR = new double[nLayers];
            AbsorbedRadiationDirectNIR = new double[nLayers];
            AbsorbedRadiationDiffuseNIR = new double[nLayers];
            AbsorbedRadiationScatteredNIR = new double[nLayers];

            AbsorbedIrradiancePAR = new double[nLayers];
            AbsorbedRadiationDirectPAR = new double[nLayers];
            AbsorbedRadiationDiffusePAR = new double[nLayers];
            AbsorbedRadiationScatteredPAR = new double[nLayers];

            VcMax25 = new double[nLayers];
            VcMaxT = new double[nLayers];
            //J2Max25 = new double[nLayers];
            //J2MaxT = new double[nLayers];
            JMaxT = new double[nLayers];
            JMax25 = new double[nLayers];
            VpMax25 = new double[nLayers];
            VpMaxT = new double[nLayers];

            Gm25 = new double[nLayers];
            GmT = new double[nLayers];

            //J2 = new double[nLayers];
            J = new double[nLayers];
            r_ = new double[nLayers];
            ScO = new double[nLayers];
            Kc = new double[nLayers];
            Ko = new double[nLayers];
            VcVo = new double[nLayers];
            K_ = new double[nLayers];
            Rd25 = new double[nLayers];
            RdT = new double[nLayers];
            A = new double[nLayers];
            gs_CO2 = new double[nLayers];
            gm_CO2T = new double[nLayers];
            Ci = new double[nLayers];
            Cc = new double[nLayers];

            lE = new double[nLayers];
            s = new double[nLayers];
            Rn = new double[nLayers];
            Sn = new double[nLayers];
            es = new double[nLayers];
            rs_H2O = new double[nLayers];
            gs = new double[nLayers];

            LeafTemp = new double[nLayers];
            es = new double[nLayers];
            s = new double[nLayers];
            gs = new double[nLayers];
            rs_H2O = new double[nLayers];
            lE = new double[nLayers];
            Sn = new double[nLayers];
            R_ = new double[nLayers];
            G_ = new double[nLayers];

            Rn = new double[nLayers];
            VPD = new double[nLayers];
            FVPD = new double[nLayers];
            Gb_CO2 = new double[nLayers];

            Vpr = new double[nLayers];

            //C4 variables 
            Oc = new double[nLayers];
            Cm = new double[nLayers];
            Cc = new double[nLayers];
            Vp = new double[nLayers];
            Ag = new double[nLayers];
            Rm = new double[nLayers];
            L = new double[nLayers];
            Gbs = new double[nLayers];
            F = new double[nLayers];

            Kp = new double[nLayers];
            Om = new double[nLayers];
            Oi = new double[nLayers];
            CiCaRatio = new double[nLayers];

            LeafTemp__ = new double[nLayers];
            LeafTemp_1 = new double[nLayers];
            LeafTemp_2 = new double[nLayers];
            Cm__ = new double[nLayers];
            Cm_1 = new double[nLayers];
            Cm_2 = new double[nLayers];

            Gsw = new double[nLayers];
            Rsw = new double[nLayers];
            ES1 = new double[nLayers];
            VPD_la = new double[nLayers];
            Elambda_ = new double[nLayers];
            TDelta = new double[nLayers];
            Elambda = new double[nLayers];
            E_PC = new double[nLayers];

            Gbh = new double[nLayers];
            Gbw = new double[nLayers];

            Rbw = new double[nLayers];
            Rbh = new double[nLayers];
            Gbw_m = new double[nLayers];
            GbCO2 = new double[nLayers];

            GsCO2 = new double[nLayers];
            Gsw = new double[nLayers];
            Gsh = new double[nLayers];

            Cb = new double[nLayers];

            WaterUse = new double[nLayers];
            WaterUseMolsSecond = new double[nLayers];
        }

        //---------------------------------------------------------------------------------------------------------
        public virtual void Run(int nlayers, PhotosynthesisModel PM, SunlitShadedCanopy counterpart)
        {
            _nLayers = nlayers;
            InitArrays(_nLayers);
            CalcIncidentRadiation(PM.EnvModel, PM.Canopy, counterpart);
            CalcAbsorbedRadiation(PM.EnvModel, PM.Canopy, counterpart);
            CalcMaxRates(PM.Canopy, counterpart, PM);

        }
        //---------------------------------------------------------------------------------------------------------
        public virtual void CalcLAI(LeafCanopy canopy, SunlitShadedCanopy counterpart) { }
        //---------------------------------------------------------------------------------------------------------
        public virtual void CalcIncidentRadiation(EnvironmentModel EM, LeafCanopy canopy, SunlitShadedCanopy counterpart) { }
        //---------------------------------------------------------------------------------------------------------
        public virtual void CalcAbsorbedRadiation(EnvironmentModel EM, LeafCanopy canopy, SunlitShadedCanopy counterpart) { }
        //---------------------------------------------------------------------------------------------------------
        public virtual void CalcMaxRates(LeafCanopy canopy, SunlitShadedCanopy counterpart, PhotosynthesisModel EM) { }
        //---------------------------------------------------------------------------------------------------------
        public double SolveQuadratic(double a, double b, double d)
        {
            return (-1 * Math.Pow((Math.Pow(a, 2) - 4 * b), 0.5) - a) / (2 * d);
        }
        ////---------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PM"></param>
        /// <param name="canopy"></param>
        public virtual void CalcConductanceResistance(PhotosynthesisModel PM, LeafCanopy canopy)
        {
            for (int i = 0; i < canopy.NLayers; i++)
            {
                //gbh[i] = 0.01 * Math.Pow((canopy.us[i] / canopy.leafWidth), 0.5) *
                //    (1 - Math.Exp(-1 * (0.5 * canopy.ku + canopy.kb) * canopy.LAI)) / (0.5 * canopy.ku + canopy.kb);

                Gbw[i] = Gbh[i] / 0.92; //This 0.92 changed from 0.93 29/06

                Rbh[i] = 1 / Gbh[i];

                Rbw[i] = 1 / Gbw[i];

                Gbw_m[i] = PM.EnvModel.ATM * PM.Canopy.Rair * Gbw[i];

                GbCO2[i] = Gbw_m[i] / 1.37;
            }
        }
        //---------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PM"></param>
        /// <param name="canopy"></param>
        public virtual void DoWaterInteraction(PhotosynthesisModel PM, LeafCanopy canopy, TranspirationMode mode)
        {
            for (int i = 0; i < canopy.NLayers; i++)
            {
                Rn[i] = AbsorbedIrradiancePAR[i] + AbsorbedIrradianceNIR[i];

                double BnUp = 8 * canopy.Sigma * Math.Pow((PM.EnvModel.GetTemp(PM.Time) + PM.EnvModel.AbsoluteTemperature), 3) * (LeafTemp__[i] - PM.EnvModel.GetTemp(PM.Time));  //This should be: HEnergyBalance - * (LeafTemp__[i]-PM.EnvModel.GetTemp(PM.Time));
                double VPTLeaf = 0.61365 * Math.Exp(17.502 * LeafTemp__[i] / (240.97 + LeafTemp__[i]));
                double VPTAir = 0.61365 * Math.Exp(17.502 * PM.EnvModel.GetTemp(PM.Time) / (240.97 + PM.EnvModel.GetTemp(PM.Time)));
                double VPTAir_1 = 0.61365 * Math.Exp(17.502 * (PM.EnvModel.GetTemp(PM.Time) + 1) / (240.97 + (PM.EnvModel.GetTemp(PM.Time) + 1)));
                double VPTMin = 0.61365 * Math.Exp(17.502 * PM.EnvModel.MinT / (240.97 + PM.EnvModel.MinT));

                double s = VPTAir_1 - VPTAir;
                VPD_la[i] = VPTLeaf - VPTMin;

                double Wl = VPTLeaf / (PM.EnvModel.ATM * 100) * 1000;
                double Wa = VPTMin / (PM.EnvModel.ATM * 100) * 1000;

                if (mode == TranspirationMode.unlimited)
                {

                    double a_var_gsCO2 = 1 / GbCO2[i];
                    double d_var_gsCO2 = (Wl - Wa) / (1000 - (Wl + Wa) / 2) * (canopy.Ca + Ci[i]) / 2;
                    double e_var_gsCO2 = A[i];
                    double f_var_gsCO2 = canopy.Ca - Ci[i];
                    double m_var_gsCO2 = 1.37; //Constant
                    double n_var_gsCO2 = 1.6;  //Constant
                    double a_lump_gsCO2 = e_var_gsCO2 * a_var_gsCO2 * m_var_gsCO2 + e_var_gsCO2 * a_var_gsCO2 * n_var_gsCO2 + d_var_gsCO2 * m_var_gsCO2 * n_var_gsCO2 - f_var_gsCO2 * m_var_gsCO2;
                    double b_lump_gsCO2 = e_var_gsCO2 * m_var_gsCO2 * (e_var_gsCO2 * Math.Pow(a_var_gsCO2, 2) * n_var_gsCO2 + a_var_gsCO2 * d_var_gsCO2 * m_var_gsCO2 * n_var_gsCO2 - a_var_gsCO2 * f_var_gsCO2 * n_var_gsCO2);
                    double c_lump_gsCO2 = -a_lump_gsCO2;
                    double d_lump_gsCO2 = m_var_gsCO2 * A[i];

                    GsCO2[i] = 2 * d_lump_gsCO2 / (Math.Pow((Math.Pow(a_lump_gsCO2, 2) - 4 * b_lump_gsCO2), 0.5) - a_lump_gsCO2);
                    double Gtw = 1 / (1 / (1.37 * GbCO2[i]) + 1 / (1.6 * GsCO2[i])); //Are these the same constansts shown above
                    double GtCO2 = 1 / (1 / GbCO2[i] + 1 / GsCO2[i]);

                    //DO NOT DELETE - FILLED ARE OF SPREADSHEET
                    //double EMolsPerSecond = Gtw * (Wl - Wa) / (1000 - (Wl + Wa) / 2);
                    //double EMmPerHour = EMolsPerSecond * 18 / 1000 * 3600;

                    //double LambdaEEnergyBalance = 44100 * EMolsPerSecond; //44100 should be a parameter..discuss with AW
                    //double HEnergyBalance = 8 * canopy.Sigma * Math.Pow((PM.EnvModel.GetTemp(PM.Time) + PM.EnvModel.AbsoluteTemperature), 3);

                    //TDelta[i] = (Rn[i] - LambdaEEnergyBalance) / (HEnergyBalance + canopy.Rcp / Rbh[i]);

                    double rtw = canopy.Rair / Gtw * PM.EnvModel.ATM;

                    double a_lump_lambdaE = s * (Rn[i] - BnUp) + VPD_la[i] * canopy.Rcp / Rbh[i];
                    double b_lump_lambdaE = s + canopy.G * (rtw) / Rbh[i];
                    double lambdaE = a_lump_lambdaE / b_lump_lambdaE;
                    double EKgPerSecond = lambdaE / canopy.Lambda;
                    WaterUseMolsSecond[i] = EKgPerSecond / 18 * 1000;
                    WaterUse[i] = EKgPerSecond * 3600;

                    double a_lump_deltaT = canopy.G * (Rn[i] - BnUp) * rtw / canopy.Rcp - VPD_la[i];
                    double d_lump_deltaT = s + canopy.G * rtw / Rbh[i];
                    TDelta[i] = a_lump_deltaT / d_lump_deltaT;

                    VPD_la[i] = 0.61365 * Math.Exp(17.502 * LeafTemp__[i] / (240.97 + LeafTemp__[i])) - 0.61365 * Math.Exp(17.502 * PM.EnvModel.MinT / (240.97 + PM.EnvModel.MinT));

                    LeafTemp[i] = PM.EnvModel.GetTemp(PM.Time) + TDelta[i];
                }

                else
                {
                    WaterUseMolsSecond[i] = WaterUse[i] / 18 * 1000 / 3600;
                    double EKgPerSecond = WaterUseMolsSecond[i] * 18 / 1000;

                    double rtw = (s * (Rn[i] - BnUp) + VPD_la[i] * canopy.Rcp / Rbh[i] - canopy.Lambda * EKgPerSecond * s) * Rbh[i] / (canopy.Lambda * EKgPerSecond * canopy.G);
                    Rsw[i] = rtw - Rbw[i];

                    GsCO2[i] = canopy.Rair * PM.EnvModel.ATM / Rsw[i] / 1.6;

                    double GtCO2 = 1 / (1 / GbCO2[i] + 1 / GsCO2[i]);

                    double a_lump_deltaT = canopy.G * (Rn[i] - BnUp) * rtw / canopy.Rcp - VPD_la[i];
                    double d_lump_deltaT = s + canopy.G * rtw / Rbh[i];

                    TDelta[i] = a_lump_deltaT / d_lump_deltaT;

                    //DO NOT DELETE - FILLED ARE OF SPREADSHEET
                    //double LambdaEEnergyBalance = 44100 * EMolsPerSecond; //44100 should be a parameter..discuss with AW
                    //double HEnergyBalance = 8 * canopy.Sigma * Math.Pow((PM.EnvModel.GetTemp(PM.Time) + PM.EnvModel.AbsoluteTemperature), 3);

                    //double Tdelta = (Rn[i] - LambdaEEnergyBalance) / (HEnergyBalance + canopy.Rcp / Rbh[i]); //This is not used anywhere

                    //double Gtw = EMolsPerSecond * (1000 - (Wl + Wa) / 2) / (Wl - Wa);

                    //GsCO2[i] = 1 / ((1 / Gtw - 1 / (1.37 * GbCO2[i])) * 1.6);

                    //GtCO2 = 1 / (1 / GbCO2[i] + 1 / GsCO2[i]);

                    LeafTemp[i] = PM.EnvModel.GetTemp(PM.Time) + TDelta[i];
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public double CalcAssimilation()
        {
            double _a, _b, _d;

            _a = b * R * x_2 * x_9 - b * t * x_1 * x_9 - j * p + j * q * R - j * q * x_1 - e * j * x_2 - j * x_3 + m * x_8 - p * x_4 * x_8 + q * R * x_4 * x_8 - q * x_1 * x_4 * x_8 + R * x_6 * x_8 - x_1 * x_6 * x_8 - x_5 * x_8 + x_7 * x_8;
            _d = -b * x_2 * x_9 + j * q + q * x_4 * x_8 + x_6 * x_8;
            _b = _d * (-j * p * R + j * p * x_1 - e * j * R * x_2 - j * R * x_3 - e * j * t * x_1 + m * R * x_8 - m * x_1 * x_8 - p * R * x_4 * x_8 + p * x_1 * x_4 * x_8 - R * x_7 * x_8 + x_1 * x_5 * x_8 - x_1 * x_7 * x_8);

            return SolveQuadratic(_a, _b, _d); //Eq (A55)
        }
        //---------------------------------------------------------------------------------------------------------
    }
}
