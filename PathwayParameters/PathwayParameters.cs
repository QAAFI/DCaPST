
namespace LayerCanopyPhotosynthesis
{
    public class PathwayParameters
    {
        public PathwayParameters()
        {
        }

        [ModelPar("GUUtv", "Base leaf nitrogen content at or below which PS = 0", "N", "b", "mmol N/m2", "", "m2 leaf")]
        public double StructuralN { get; set; } = 25;
        [ModelPar("6awks", "Ratio of SLNo to SLNav", "SLN", "ratio_top", "")]
        public double SLNRatioTop { get; set; } = 1.32;
        [ModelPar("cNPBJ", "Average SLN over the canopy", "SLN", "av", "g N/m2", "", "m2 leaf")]
        public double SLNAv { get; set; } = 1.45;
        //Parmas based on C3
        [ModelPar("36Sr7", "Ratio of Ci to Ca for the sunlit and shade leaf fraction", "Ci/Ca", "", "")]
        public double CiCaRatio { get; set; } = 0.7;
        [ModelPar("WE2QN", "Intercept of linear relationship of Ci/Ca ratio to VPD", "b", "", "")]
        public double CiCaRatioIntercept { get; set; } = 0.90;
        [ModelPar("jC0xB", "Slope of linear relationship of Ci/Ca ratio to VPD", "a", "", "1/kPa")]
        public double CiCaRatioSlope { get; set; } = -0.12;
        [ModelPar("uk6BV", "Fraction of electrons at PSI that follow cyclic transport around PSI", "f", "cyc", "")]
        public double Fcyc { get; set; } = 0;
        [ModelPar("3WXTb", "Slope of the linear relationship between Rd_l and N(L) at 25°C with intercept = 0", "psi", "Rd", "mmol/mol N/s")]
        public double PsiRd { get; set; } = 0.0175;
        [ModelPar("l2mwD", "Slope of the linear relationship between Vcmax_l and N(L) at 25°C with intercept = 0", "psi", "Vc", "mmol/mol N/s")]
        public double PsiVc { get; set; } = 1.75;
        //[ModelPar("I0uh7", "Slope of the linear relationship between J2max_l and N(L) at 25°C with intercept = 0", "psi", "J2", "mmol/mol N/s")]
        //public double PsiJ2 { get; set; } = 2.43;
        [ModelPar("JwwUu", "Slope of the linear relationship between Jmax_l and N(L) at 25°C with intercept = 0", "psi", "J", "mmol/mol N/s")]
        public double PsiJ { get; set; } = 3.20;
        [ModelPar("pYisy", "Slope of the linear relationship between Vpmax_l and N(L) at 25°C with interception = 0", "psi", "Vp", "mmol/mol N/s", "", "", true)]
        public double PsiVp { get; set; } = 3.39;
        [ModelPar("psyGM", "Slope of the linear relationship between GM and SLN at 25°C", "psi", "Gm", "mmol/mol N/s", "", "", true)]
        public double PsiGm { get; set; } = 0.005296;

        [ModelPar("cGmso", "Intercept of the relationship between GM and SLN at 25°C", "psi", "Gm", "mmol/mol N/s", "", "", true)]
        public double CGm { get; set; } = 0;

        [ModelPar("tuksS", "Fraction of electron transport partitioned to mesophyll chlorplast", "x", "", "", "", "", true)]
        public double X { get; set; } = 0.4;

        #region KineticParams
       
        [ModelPar("THGeL", "Quantum efficiency of PSII e- folow on PSII-absorbed light basis at the strictly limiting light level", "ɸ", "2(LL)", "")]
        public double F2 = 0.75;
        [ModelPar("6RyTa", "Quantum efficiency of PSI e- flow at the strictly limiting light level", "ɸ", "1(LL)", "")]
        public double F1 = 0.95;
        [ModelPar("rRJ12", "extra energy (ATP) cost required from processes other than the C3 cycle", "Fi", "(LL)", "")]
        public double Phi = 2;

        #region Curvilinear Temperature Model

        // Kc μbar	Curvilinear Temperature Model
        [ModelPar("KcP25", "", "", "", "")]
        public double KcP25 { get; set; } = 267.9295;
        //[ModelPar("KcC", "", "K", "c", "μbar")]
        //public double KcC { get; set; } = 0.1403;
        //[ModelPar("KcTmax", "", "", "", "")]
        //public double KcTMax { get; set; } = 60.0;
        [ModelPar("KcTmin", "", "", "", "")]
        public double KcTEa { get; set; } = 0.0;
        //[ModelPar("KcTopt", "", "", "", "")]
        //public double KcTOpt { get; set; } = 50.0115;

        // Ko μbar	Curvilinear Temperature Model 
        [ModelPar("KoP25", "", "", "", "")]
        public double KoP25 { get; set; } = 164991.8069;
        //[ModelPar("KoC", "", "K", "c", "μbar")]
        //public double KoC { get; set; } = 0.7230;
        //[ModelPar("KoTmax", "", "", "", "")]
        //public double KoTMax { get; set; } = 60.0;
        [ModelPar("KoTmin", "", "", "", "")]
        public double KoTEa { get; set; } = 0.0;
        //[ModelPar("KoTopt", "", "", "", "")]
        //public double KoTOpt { get; set; } = 37.9682;

        //Vcmax μmol/m2/s*	Curvilinear Temperature Model 
        //[ModelPar("VcMaxC", "", "V", "c_max", "μmol/m2/s")]
        //public double VcMaxC { get; set; } = 0.2352;
        //[ModelPar("VcTmax", "", "", "", "")]
        //public double VcTMax { get; set; } = 60.0;
        [ModelPar("VcTmin", "", "", "", "")]
        public double VcTEa { get; set; } = 0.0;
        //[ModelPar("VcTopt", "", "", "", "")]
        //public double VcTOpt { get; set; } = 48.2470;

        //Jmax μmol/m2/s*	Curvilinear Temperature Model 
        [ModelPar("JMaxC", "", "V", "c_max", "μmol/m2/s")]
        public double JMaxC { get; set; } = 0.7991;
        [ModelPar("JTmax", "", "", "", "")]
        public double JTMax { get; set; } = 42.9922;
        [ModelPar("JTmin", "", "", "", "")]
        public double JTMin { get; set; } = 0.0;
        [ModelPar("JTopt", "", "", "", "")]
        public double JTOpt { get; set; } = 31.2390;
        [ModelPar("JBetat", "", "", "", "")]
        public double JBeta { get; set; } = 1;

        //Vcmax/Vomax	-	Curvilinear Temperature Model
        [ModelPar("VcMax.VoMaxP25", "", "", "", "")]
        public double VcMax_VoMaxP25 { get; set; } = 4.1672;
        //[ModelPar("VcMax.VoMaxC", "", "Vc_max/Vo_max", "", "")]
        //public double VcMax_VoMaxC { get; set; } = 0.4242;
        //[ModelPar("VcMax.VoMaxTmax", "", "", "", "")]
        //public double VcMax_VoMaxTMax { get; set; } = 60.0;
        [ModelPar("VcMax.VoMaxTmin", "", "", "", "")]
        public double VcMax_VoMaxTEa { get; set; } = 0.0;
        //[ModelPar("VcMax.VoMaxTopt", "", "", "", "")]
        //public double VcMax_VoMaxTOpt { get; set; } = 45.0364;

        // Kp μbar	-- C4
        [ModelPar("KpP25", "", "", "", "")]
        public double KpP25 { get; set; } = 160.1404;
        //[ModelPar("KpC", "", "K", "p", "μbar")]
        //public double KpC { get; set; } = 0.6154;
        //[ModelPar("KpTmax", "", "", "", "")]
        //public double KpTMax { get; set; } = 60.0;
        [ModelPar("KpTmin", "", "", "", "")]
        public double KpTEa { get; set; } = 0.0;
        //[ModelPar("KpTopt", "", "", "", "")]
        //public double KpTOpt { get; set; } = 41.4914;

        //Vpmax μmol/m2/s*	Curvilinear Temperature Model (C4)
        //[ModelPar("VpMaxC", "", "V", "p_max", "μmol/m2/s", "", "", true)]
        //public double VpMaxC { get; set; } = 0.5384;
        //[ModelPar("VpMaxTmax", "", "", "", "")]
        //public double VpMaxTMax { get; set; } = 45.0;
        [ModelPar("VpMaxTmin", "", "", "", "")]
        public double VpMaxTEa { get; set; } = 0.0;
        //[ModelPar("VpMaxTopt", "", "", "", "")]
        //public double VpMaxTOpt { get; set; } = 35.8650;

        //Rd μmol/m2/s*	
        //[ModelPar("RdC", "", "R", "d", "μmol/m2/s")]
        //public double RdC { get; set; } = 0.4608;
        //[ModelPar("RdTmax", "", "", "", "")]
        //public double RdTMax { get; set; } = 60.0;
        [ModelPar("RdTmin", "", "", "", "")]
        public double RdTEa { get; set; } = 0.0;
        //[ModelPar("RdTopt", "", "", "", "")]
        //public double RdTOpt { get; set; } = 43.5930;

        //gm(Arabidopsis, Bernacchi 2002)    μmol/m2/s/bar	
        //[ModelPar("gmP25", "", "", "", "")]
        //public double GmP25 { get; set; } = 0.55;
        [ModelPar("gmC", "", "R", "d", "μmol/m2/s")]
        public double GmC { get; set; } = 0.5626;
        [ModelPar("GmTmax", "", "", "", "")]
        public double GmTMax { get; set; } = 42.7227;
        [ModelPar("GmTmin", "", "", "", "")]
        public double GmTMin { get; set; } = 0.0;
        [ModelPar("GmTopt", "", "", "", "")]
        public double GmTOpt { get; set; } = 33.2424;
        [ModelPar("GmBeta", "", "", "", "")]
        public double GmBeta { get; set; } = 1;

        [ModelPar("Beta", "Shape Factor", "", "", "")]
        public double beta = 1;

        #endregion

        #endregion
    }
}
