
namespace LayerCanopyPhotosynthesis
{
    public class PathwayParametersC3 : PathwayParameters
    {
        public PathwayParametersC3() : base()
        {
            StructuralN = 25;
            SLNRatioTop = 1.32;
            SLNAv = 1.45;

            //Parmas based on C3
            CiCaRatio = 0.7;
            CiCaRatioIntercept = 0.90;
            CiCaRatioSlope = -0.12;
            Fcyc = 0;
            PsiRd = 0.0175;
            PsiVc = 1.75;
            PsiJ2 = 2.43;
            PsiJ = 3.20;
            PsiVp = 3.39;
            X = 0.4;

            Kc_P25 = 272.38;
            Kc_c = 32.689;
            Kc_b = 9741.400;
            Kp_P25 = 139;
            Kp_c = 14.644;
            Kp_b = 4366.129;

            Ko_P25 = 165820;
            Ko_c = 9.574;
            Ko_b = 2853.019;
            VcMax_VoMax_P25 = 4.580;
            VcMax_VoMax_c = 13.241;
            VcMax_VoMax_b = 3945.722;
            VcMax_c = 26.355;
            VcMax_b = 7857.830;
            VpMax_c = 26.355;
            VpMax_b = 7857.830;

            //Rd μmol/m2/s*	
            Rd_c = 18.715;
            Rd_b = 5579.745;

            JMax_TOpt = 28.796;
            JMax_Omega = 15.536;

            Gm_P25 = 0.55;
            Gm_TOpt = 34.309;
            Gm_Omega = 20.791;

            F2 = 0.75;
            F1 = 0.95;

            #region Curvilinear Temperature Model

            // Kc µbar	Curvilinear Temperature Model 
            KcP25 = 267.9295;
            KcC = 0.1403;
            KcTMax = 60.0;
            KcTMin = 0.0;
            KcTOpt = 50.0115;

            // Ko µbar	Curvilinear Temperature Model 
            KoP25 = 164991.8069;
            KoC = 0.7230;
            KoTMax = 60.0;
            KoTMin = 0.0;
            KoTOpt = 37.9682;

            //Vcmax µmol/m2/s*	Curvilinear Temperature Model 
            VcMaxC = 0.2352;
            VcTMax = 60.0;
            VcTMin = 0.0;
            VcTOpt = 48.2470;

            //Jmax µmol/m2/s*	Curvilinear Temperature Model 
            JMaxC = 0.7991;
            JTMax = 42.9922;
            JTMin = 0.0;
            JTOpt = 31.2390;

            //Vcmax/Vomax	-	Curvilinear Temperature Model
            VcMax_VoMaxP25 = 4.1672;
            VcMax_VoMaxC = 0.4242;
            VcMax_VoMaxTMax = 60.0;
            VcMax_VoMaxTMin = 0.0;
            VcMax_VoMaxTOpt = 45.0364;

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
