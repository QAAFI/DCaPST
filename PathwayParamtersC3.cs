
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
            PsiVc = 1.6;
            PsiJ = 2.7;
            PsiVp = 3.39;
            X = 0.4;

            
            F2 = 0.75;
            F1 = 0.95;

            #region Curvilinear Temperature Model

            // Kc µbar	Curvilinear Temperature Model 
            KcP25 = 273.422964228666;
            KcTMin = 80990;

            // Ko µbar	Curvilinear Temperature Model 
            KoP25 = 165824.064155384;
            KoTMin = 23720;

            VcTMin = 65330;

            //Jmax µmol/m2/s*	Curvilinear Temperature Model 
            JMaxC = 0.813461238591984;
            JTMax = 42;
            JTMin = 0.0;
            JTOpt = 30;
            JBeta = 1;

            VcMax_VoMaxP25 = 4.62505056717461;
            VcMax_VoMaxTMin = 32818.0987474166;
   
            RdTMin = 46390;

            //gm(Arabidopsis, Bernacchi 2002)    µmol/m2/s/bar	
            GmP25 = 0.55;
            GmC = 0.626018046615251;
            GmTMax = 45;
            GmTMin = 0.0;
            GmTOpt = 35;
            GmBeta = 1;


            #endregion


        }
    }
}
