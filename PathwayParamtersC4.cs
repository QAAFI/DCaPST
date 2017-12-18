
namespace LayerCanopyPhotosynthesis
{
    public class PathwayParametersC4 : PathwayParameters
    {
        public PathwayParametersC4()
            : base()
        {

            StructuralN = 14;
            SLNRatioTop = 1.3;
            SLNAv = 1.3;

            // Kc μbar	
            Kc_P25 = 1210;
            Kc_c = 25.899;
            Kc_b = 7721.915;

            //Ko μbar	
            Ko_P25 = 292000;
            Ko_c = 4.236;
            Ko_b = 1262.93;

            //Kp μbar	
            Kp_P25 = 139;
            Kp_c = 14.644;
            Kp_b = 4366.129;

            //Vcmax/Vomax	-	
            VcMax_VoMax_P25 = 5.401;
            VcMax_VoMax_c = 9.126;
            VcMax_VoMax_b = 2719.478;

            //Vcmax μmol/m2/s*	
            VcMax_c = 31.467;
            VcMax_b = 9381.766;

            //Vpmax μmol/m2/s*	
            VpMax_c = 38.244;
            VpMax_b = 11402.450;

            //Rd μmol/m2/s*	
            Rd_c = 18.715;
            Rd_b = 5579.745;

            //Jmax(Barley, Farquhar 1980)    μmol/m2/s*	
            JMax_TOpt = 32.633;
            JMax_Omega = 15.270;

            //gm(Arabidopsis, Bernacchi 2002)    μmol/m2/s/bar	
            Gm_P25 = 0.55;
            Gm_TOpt = 34.309;
            Gm_Omega = 20.791;

            PsiVc = 0.5;
            PsiJ = 2.4;
            PsiRd = 0;
            PsiVp = 1.0;

            F2 = 0.75;
            F1 = 0.95;

            Fcyc = 0.136;
            CiCaRatio = 0.4;
            CiCaRatioIntercept = 0.84;
            CiCaRatioSlope = -0.19;

            #region Curvilinear Temperature Model

            // Kc µbar	Curvilinear Temperature Model 
            KcP25 = 956.1442;
            KcC = 0.1364;
            KcTMax = 60.0;
            KcTMin = 0.0;
            KcTOpt = 49.8308;

            // Ko µbar	Curvilinear Temperature Model 
            KoP25 = 301308.2522;
            KoC = 0.8423;
            KoTMax = 60.0;
            KoTMin = 0.0;
            KoTOpt = 32.4144;

            //Vcmax µmol/m2/s*	Curvilinear Temperature Model 
            VcMaxC = 0.6958;
            VcTMax = 42.0;
            VcTMin = 0.0;
            VcTOpt = 32.3448;

            //Jmax µmol/m2/s*	Curvilinear Temperature Model 
            JMaxC = 0.6731;
            JTMax = 42.0;
            JTMin = 0.0;
            JTOpt = 32.8418;

            //Vcmax/Vomax	-	Curvilinear Temperature Model
            VcMax_VoMaxP25 = 4.9015;
            VcMax_VoMaxC = 0.4942;
            VcMax_VoMaxTMax = 60.0;
            VcMax_VoMaxTMin = 0.0;
            VcMax_VoMaxTOpt = 44.8955;

            // Kp µbar	-- C4
            KpP25 = 160.1404;
            KpC = 0.6154;
            KpTMax = 60.0;
            KpTMin = 0.0;
            KpTOpt = 41.4914;

            //Vpmax µmol/m2/s*	Curvilinear Temperature Model (C4)
            VpMaxC = 0.5384;
            VpMaxTMax = 45.0;
            VpMaxTMin = 0.0;
            VpMaxTOpt = 35.8650;

            //Rd µmol/m2/s*	
            RdC = 0.4608;
            RdTMax = 60.0;
            RdTMin = 0.0;
            RdTOpt = 43.5930;

            //gm(Arabidopsis, Bernacchi 2002)    µmol/m2/s/bar	
            GmP25 = 0.55;
            GmC = 0.5626;
            GmTMax = 42.7227;
            GmTMin = 0.0;
            GmTOpt = 33.2424;


            #endregion


        }
    }
}
