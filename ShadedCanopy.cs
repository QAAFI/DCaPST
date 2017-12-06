
using System;

namespace LayerCanopyPhotosynthesis
{
    public class ShadedCanopy : SunlitShadedCanopy
    {
        public ShadedCanopy(int _nLayers, SSType type) : base(_nLayers, type) { }
        public ShadedCanopy() { }
        //---------------------------------------------------------------------------------------------------------
        public override void CalcLAI(LeafCanopy canopy, SunlitShadedCanopy sunlit)
        {
            LAIS = new double[canopy.NLayers];

            for (int i = 0; i < canopy.NLayers; i++)
            {
                LAIS[i] = canopy.LAIs[i] - sunlit.LAIS[i];
            }
        }
        //----------------------------------------------------------------------
        public override void CalcConductanceResistance(PhotosynthesisModel PM, LeafCanopy canopy)
        {
            for (int i = 0; i < canopy.NLayers; i++)
            {
                double sunlitGbh = 0.01 * Math.Pow((canopy.Us[i] / canopy.LeafWidth), 0.5) *
                    (1 - Math.Exp(-1 * (0.5 * canopy.Ku + canopy.Kb) * canopy.LAI)) / (0.5 * canopy.Ku + canopy.Kb);

                Gbh[i] = canopy.Gbh[i] - sunlitGbh;
            }

            base.CalcConductanceResistance(PM, canopy);
        }
        //----------------------------------------------------------------------
        public override void CalcIncidentRadiation(EnvironmentModel EM, LeafCanopy canopy, SunlitShadedCanopy sunlit)
        {
            for (int i = 0; i < _nLayers; i++)
            {
                IncidentIrradiance[i] = EM.DiffuseRadiationPAR * canopy.PropnInterceptedRadns[i] / (sunlit.LAIS[i] + LAIS[i]) *
                    LAIS[i] + 0.15 * (sunlit.IncidentIrradiance[i] * sunlit.LAIS[i]) / (LAIS[i] + (i < (_nLayers - 1) ? LAIS[i + 1] : 0)) * LAIS[i]; //+ *((E70*E44)/(E45+F45)
            }
        }
        //----------------------------------------------------------------------
        public override void CalcAbsorbedRadiation(EnvironmentModel EM, LeafCanopy canopy, SunlitShadedCanopy sunlit)
        {
            for (int i = 0; i < _nLayers; i++)
            {
                AbsorbedIrradiance[i] = canopy.AbsorbedRadiation[i] - sunlit.AbsorbedIrradiance[i];

                AbsorbedIrradianceNIR[i] = canopy.AbsorbedRadiationNIR[i] - sunlit.AbsorbedIrradianceNIR[i];

                AbsorbedIrradiancePAR[i] = canopy.AbsorbedRadiationPAR[i] - sunlit.AbsorbedIrradiancePAR[i];
            }
        }
        //----------------------------------------------------------------------
        public void CalcRubiscoActivity25(LeafCanopy canopy, SunlitShadedCanopy sunlit, PhotosynthesisModel PM)
        {
            for (int i = 0; i < _nLayers; i++)
            {
                VcMax25[i] = canopy.VcMax25[i] - sunlit.VcMax25[i];
            }
        }
        //----------------------------------------------------------------------
        public void CalcRdActivity25(LeafCanopy canopy, SunlitShadedCanopy sunlit, PhotosynthesisModel PM)
        {
            for (int i = 0; i < _nLayers; i++)
            {
                Rd25[i] = canopy.Rd25[i] - sunlit.Rd25[i];
            }
        }
        //---------------------------------------------------------------------------------------------------------
        public void CalcElectronTransportRate25(LeafCanopy canopy, SunlitShadedCanopy sunlit, PhotosynthesisModel PM)
        {
            for (int i = 0; i < _nLayers; i++)
            {
                J2Max25[i] = canopy.J2Max25[i] - sunlit.J2Max25[i];
                JMax25[i] = canopy.JMax25[i] - sunlit.JMax25[i];

            }
        }

        //---------------------------------------------------------------------------------------------------------
        public void CalcPRate25(LeafCanopy canopy, SunlitShadedCanopy sunlit, PhotosynthesisModel PM)
        {
            for (int i = 0; i < _nLayers; i++)
            {
                VpMax25[i] = canopy.VpMax25[i] - sunlit.VpMax25[i];
            }
        }
        //---------------------------------------------------------------------------------------------------------
       
        public override void CalcMaxRates(LeafCanopy canopy, SunlitShadedCanopy counterpart, PhotosynthesisModel PM)
        {
            CalcRubiscoActivity25(canopy, counterpart, PM);
            CalcElectronTransportRate25(canopy, counterpart, PM);
            CalcRdActivity25(canopy, counterpart, PM);
            CalcPRate25(canopy, counterpart, PM);
        }
    }
}

