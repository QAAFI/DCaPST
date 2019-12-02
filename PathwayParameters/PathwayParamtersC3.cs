
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
            CiCaRatioIntercept = 0.70;
            CiCaRatioSlope = 0;

            Fcyc = 0;

            PsiRd = 0;
            PsiVc = 1.1;
            PsiJ = 1.85;
            PsiVp = 1;
            PsiGm = 0.00529573590096286;

            X = 0.4;

            
            F2 = 0.75;
            F1 = 0.95;

            #region Curvilinear Temperature Model

            // Kc µbar	Curvilinear Temperature Model 
            KcP25 = 273.422964228666;
            KcTEa = 93720;

            // Ko µbar	Curvilinear Temperature Model 
            KoP25 = 165824.064155384;
            KoTEa = 33600;

            VcTEa = 65330;

            //Jmax µmol/m2/s*	Curvilinear Temperature Model 
            JMaxC = 0.911017958600129;
            JTMax = 45;
            JTMin = 0.0;
            JTOpt = 30;
            JBeta = 1;

            VcMax_VoMaxP25 = 4.59217066521612;
            VcMax_VoMaxTEa = 35713.1987127717;
   
            RdTEa = 46390;

            //gm(Arabidopsis, Bernacchi 2002)    µmol/m2/s/bar	
            //GmP25 = 0.55;
            GmC = 0.875790608584141;
            GmTMax = 45;
            GmTMin = 0.0;
            GmTOpt = 29.2338417788683;
            GmBeta = 1;

            CGm = 0;


            #endregion


        }
    }
}
