
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
        }
    }
}
