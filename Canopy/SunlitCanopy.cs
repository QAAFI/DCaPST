using System;

namespace LayerCanopyPhotosynthesis
{
    public class SunlitCanopy : SunlitShadedCanopy
    {
        public SunlitCanopy() { }

        public SunlitCanopy(int _nLayers, SSType type) : base(_nLayers, type) { }
        //---------------------------------------------------------------------------------------------------------
        public override void CalcLAI(LeafCanopy canopy, SunlitShadedCanopy counterpart)
        {
            LAIS = new double[canopy.NLayers];
            for (int i = 0; i < canopy.NLayers; i++)
            {

                LAIS[i] = ((i == 0 ? 1 : Math.Exp(-canopy.BeamExtCoeffs[i] * canopy.LAIAccums[i - 1])) -
                    Math.Exp(-canopy.BeamExtCoeffs[i] * canopy.LAIAccums[i])) * 1 / canopy.BeamExtCoeffs[i];
            }
        }
        //---------------------------------------------------------------------------------------------------------
        public override void CalcIncidentRadiation(EnvironmentModel EM, LeafCanopy canopy, SunlitShadedCanopy shaded)
        {
            // Probably redundant
            for (int i = 0; i < _nLayers; i++)
            {
                IncidentIrradiance[i] = EM.DirectRadiationPAR * canopy.PropnInterceptedRadns[i] / LAIS[i] * LAIS[i] +
                     EM.DiffuseRadiationPAR * canopy.PropnInterceptedRadns[i] / (LAIS[i] + shaded.LAIS[i]) * LAIS[i];
            }
        }
        //---------------------------------------------------------------------------------------------------------

        public override void CalcAbsorbedRadiation(EnvironmentModel EM, LeafCanopy canopy, SunlitShadedCanopy shaded)
        {
            CalcAbsorbedRadiationDirect(EM, canopy);
            CalcAbsorbedRadiationDiffuse(EM, canopy);
            CalcAbsorbedRadiationScattered(EM, canopy);

            for (int i = 0; i < _nLayers; i++)
            {
                AbsorbedIrradiance[i] = AbsorbedRadiationDirect[i] + AbsorbedRadiationDiffuse[i] + AbsorbedRadiationScattered[i];
                AbsorbedIrradiancePAR[i] = AbsorbedRadiationDirectPAR[i] + AbsorbedRadiationDiffusePAR[i] + AbsorbedRadiationScatteredPAR[i];
                AbsorbedIrradianceNIR[i] = AbsorbedRadiationDirectNIR[i] + AbsorbedRadiationDiffuseNIR[i] + AbsorbedRadiationScatteredNIR[i];
            }
        }
        //---------------------------------------------------------------------------------------------------------

        void CalcAbsorbedRadiationDirect(EnvironmentModel EM, LeafCanopy canopy)
        {
            for (int i = 0; i < _nLayers; i++)
            {

                AbsorbedRadiationDirect[i] = (1 - canopy.LeafScatteringCoeffs[i]) * EM.DirectRadiationPAR *
                    ((i == 0 ? 1 : Math.Exp(-canopy.BeamExtCoeffs[i] * canopy.LAIAccums[i - 1])) -
                    Math.Exp(-canopy.BeamExtCoeffs[i] * canopy.LAIAccums[i]));

                AbsorbedRadiationDirectPAR[i] = (1 - canopy.LeafScatteringCoeffs[i]) * canopy.DirectPAR *
                    ((i == 0 ? 1 : Math.Exp(-canopy.BeamExtCoeffs[i] * canopy.LAIAccums[i - 1])) -
                    Math.Exp(-canopy.BeamExtCoeffs[i] * canopy.LAIAccums[i]));

                AbsorbedRadiationDirectNIR[i] = (1 - canopy.LeafScatteringCoeffsNIR[i]) * canopy.DirectNIR *
                    ((i == 0 ? 1 : Math.Exp(-canopy.BeamExtCoeffsNIR[i] * canopy.LAIAccums[i - 1])) -
                    Math.Exp(-canopy.BeamExtCoeffsNIR[i] * canopy.LAIAccums[i]));
            }
        }
        //---------------------------------------------------------------------------------------------------------
        void CalcAbsorbedRadiationDiffuse(EnvironmentModel EM, LeafCanopy canopy)
        {
            for (int i = 0; i < _nLayers; i++)
            {
                AbsorbedRadiationDiffuse[i] = (1 - canopy.DiffuseReflectionCoeffs[i]) * EM.DiffuseRadiationPAR *
                    ((i == 0 ? 1 : Math.Exp(-(canopy.DiffuseScatteredDiffuses[i] + canopy.BeamExtCoeffs[i]) * canopy.LAIAccums[i - 1])) -
                    Math.Exp(-(canopy.DiffuseScatteredDiffuses[i] + canopy.BeamExtCoeffs[i]) * canopy.LAIAccums[i])) * (canopy.DiffuseScatteredDiffuses[i] /
                    (canopy.DiffuseScatteredDiffuses[i] + canopy.BeamExtCoeffs[i]));

                AbsorbedRadiationDiffusePAR[i] = (1 - canopy.DiffuseReflectionCoeffs[i]) * canopy.DiffusePAR *
                    ((i == 0 ? 1 : Math.Exp(-(canopy.DiffuseScatteredDiffuses[i] + canopy.BeamExtCoeffs[i]) * canopy.LAIAccums[i - 1])) -
                    Math.Exp(-(canopy.DiffuseScatteredDiffuses[i] + canopy.BeamExtCoeffs[i]) * canopy.LAIAccums[i])) * (canopy.DiffuseScatteredDiffuses[i] /
                    (canopy.DiffuseScatteredDiffuses[i] + canopy.BeamExtCoeffs[i]));

                AbsorbedRadiationDiffuseNIR[i] = (1 - canopy.DiffuseReflectionCoeffsNIR[i]) * canopy.DiffuseNIR *
                    ((i == 0 ? 1 : Math.Exp(-(canopy.DiffuseScatteredDiffusesNIR[i] + canopy.BeamExtCoeffsNIR[i]) * canopy.LAIAccums[i - 1])) -
                    Math.Exp(-(canopy.DiffuseScatteredDiffusesNIR[i] + canopy.BeamExtCoeffsNIR[i]) * canopy.LAIAccums[i])) * (canopy.DiffuseScatteredDiffusesNIR[i] /
                    (canopy.DiffuseScatteredDiffusesNIR[i] + canopy.BeamExtCoeffsNIR[i]));
            }
        }
        //---------------------------------------------------------------------------------------------------------
        public override void CalcConductanceResistance(PhotosynthesisModel PM, LeafCanopy canopy)
        {
            for (int i = 0; i < canopy.NLayers; i++)
            {
                Gbh[i] = 0.01 * Math.Pow((canopy.Us[i] / canopy.LeafWidth), 0.5) *
                    (1 - Math.Exp(-1 * (0.5 * canopy.Ku + canopy.Kb) * canopy.LAI)) / (0.5 * canopy.Ku + canopy.Kb);
            }

            base.CalcConductanceResistance(PM, canopy);
        }
        //---------------------------------------------------------------------------------------------------------
        public override void CalcMaxRates(LeafCanopy canopy, SunlitShadedCanopy counterpart, PhotosynthesisModel PM)
        {
            CalcRubiscoActivity25(canopy, counterpart, PM);
            CalcElectronTransportRate25(canopy, counterpart, PM);
            CalcRdActivity25(canopy, counterpart, PM);
            CalcPRate25(canopy, counterpart, PM);
            CalcGmRate25(canopy, counterpart, PM);


        }
        //---------------------------------------------------------------------------------------------------------

        void CalcAbsorbedRadiationScattered(EnvironmentModel EM, LeafCanopy canopy)
        {
            for (int i = 0; i < _nLayers; i++)
            {
                if (canopy.BeamScatteredBeams[i] + canopy.BeamExtCoeffs[i] == 0)
                {
                    AbsorbedRadiationScattered[i] = 0;
                }
                else
                {
                    AbsorbedRadiationScattered[i] = EM.DirectRadiationPAR * (((1 - canopy.BeamReflectionCoeffs[i]) *
                         ((i == 0 ? 1 : Math.Exp(-(canopy.BeamExtCoeffs[i] + canopy.BeamScatteredBeams[i]) * canopy.LAIAccums[i - 1])) -
                         Math.Exp(-(canopy.BeamExtCoeffs[i] + canopy.BeamScatteredBeams[i]) * canopy.LAIAccums[i])) *
                         (canopy.BeamScatteredBeams[i] / (canopy.BeamScatteredBeams[i] + canopy.BeamExtCoeffs[i]))) -
                         ((1 - canopy.LeafScatteringCoeffs[i]) *
                          ((i == 0 ? 1 : Math.Exp(-2 * canopy.BeamExtCoeffs[i] * canopy.LAIAccums[i - 1])) -
                          Math.Exp(-2 * canopy.BeamExtCoeffs[i] * canopy.LAIAccums[i])) / 2));
                }

                if (canopy.BeamScatteredBeams[i] + canopy.BeamExtCoeffs[i] == 0)
                {
                    AbsorbedRadiationScattered[i] = 0;
                }
                else
                {
                    AbsorbedRadiationScatteredPAR[i] = canopy.DirectPAR * (((1 - canopy.BeamReflectionCoeffs[i]) *
                         ((i == 0 ? 1 : Math.Exp(-(canopy.BeamExtCoeffs[i] + canopy.BeamScatteredBeams[i]) * canopy.LAIAccums[i - 1])) -
                         Math.Exp(-(canopy.BeamExtCoeffs[i] + canopy.BeamScatteredBeams[i]) * canopy.LAIAccums[i])) *
                         (canopy.BeamScatteredBeams[i] / (canopy.BeamScatteredBeams[i] + canopy.BeamExtCoeffs[i]))) -
                         ((1 - canopy.LeafScatteringCoeffs[i]) *
                          ((i == 0 ? 1 : Math.Exp(-2 * canopy.BeamExtCoeffs[i] * canopy.LAIAccums[i - 1])) -
                          Math.Exp(-2 * canopy.BeamExtCoeffs[i] * canopy.LAIAccums[i])) / 2));
                }

                if (canopy.BeamScatteredBeamsNIR[i] + canopy.BeamExtCoeffsNIR[i] == 0)
                {
                    AbsorbedRadiationScatteredNIR[i] = 0;
                }
                else
                {
                    AbsorbedRadiationScatteredNIR[i] = canopy.DirectNIR * (((1 - canopy.BeamReflectionCoeffsNIR[i]) *
                         ((i == 0 ? 1 : Math.Exp(-(canopy.BeamExtCoeffsNIR[i] + canopy.BeamScatteredBeamsNIR[i]) * canopy.LAIAccums[i - 1])) -
                         Math.Exp(-(canopy.BeamExtCoeffsNIR[i] + canopy.BeamScatteredBeamsNIR[i]) * canopy.LAIAccums[i])) *
                         (canopy.BeamScatteredBeamsNIR[i] / (canopy.BeamScatteredBeamsNIR[i] + canopy.BeamExtCoeffsNIR[i]))) -
                         ((1 - canopy.LeafScatteringCoeffsNIR[i]) *
                          ((i == 0 ? 1 : Math.Exp(-2 * canopy.BeamExtCoeffsNIR[i] * canopy.LAIAccums[i - 1])) -
                          Math.Exp(-2 * canopy.BeamExtCoeffsNIR[i] * canopy.LAIAccums[i])) / 2));
                }
            }
        }
        //----------------------------------------------------------------------
        public void CalcRubiscoActivity25(LeafCanopy canopy, SunlitShadedCanopy shaded, PhotosynthesisModel PM)
        {
            for (int i = 0; i < _nLayers; i++)
            {
                VcMax25[i] = canopy.LAI * canopy.CPath.PsiVc * (canopy.LeafNTopCanopy - canopy.CPath.StructuralN) *
                   ((i == 0 ? 1 : Math.Exp(-(canopy.BeamExtCoeffs[i] + canopy.NAllocationCoeff / canopy.LAI) * canopy.LAIAccums[i - 1])) -
                   Math.Exp(-(canopy.BeamExtCoeffs[i] + canopy.NAllocationCoeff / canopy.LAI) * canopy.LAIAccums[i])) /
                   (canopy.NAllocationCoeff + canopy.BeamExtCoeffs[i] * canopy.LAI);
            }
        }
        //----------------------------------------------------------------------
        public void CalcRdActivity25(LeafCanopy canopy, SunlitShadedCanopy shaded, PhotosynthesisModel PM)
        {
            for (int i = 0; i < _nLayers; i++)
            {
                Rd25[i] = canopy.LAI * canopy.CPath.PsiRd * (canopy.LeafNTopCanopy - canopy.CPath.StructuralN) *
                    ((i == 0 ? 1 : Math.Exp(-(canopy.BeamExtCoeffs[i] + canopy.NAllocationCoeff / canopy.LAI) * canopy.LAIAccums[i - 1])) -
                    Math.Exp(-(canopy.BeamExtCoeffs[i] + canopy.NAllocationCoeff / canopy.LAI) * canopy.LAIAccums[i])) /
                    (canopy.NAllocationCoeff + canopy.BeamExtCoeffs[i] * canopy.LAI);
            }
        }
        //---------------------------------------------------------------------------------------------------------
        public void CalcElectronTransportRate25(LeafCanopy canopy, SunlitShadedCanopy shaded, PhotosynthesisModel PM)
        {
            for (int i = 0; i < _nLayers; i++)
            {
                //J2Max25[i] = canopy.LAI * canopy.CPath.PsiJ2 * (canopy.LeafNTopCanopy - canopy.CPath.StructuralN) *
                //    ((i == 0 ? 1 : Math.Exp(-(canopy.BeamExtCoeffs[i] + canopy.NAllocationCoeff / canopy.LAI) * canopy.LAIAccums[i - 1])) -
                //    Math.Exp(-(canopy.BeamExtCoeffs[i] + canopy.NAllocationCoeff / canopy.LAI) * canopy.LAIAccums[i])) /
                //    (canopy.NAllocationCoeff + canopy.BeamExtCoeffs[i] * canopy.LAI);

                JMax25[i] = canopy.LAI * canopy.CPath.PsiJ * (canopy.LeafNTopCanopy - canopy.CPath.StructuralN) *
                    ((i == 0 ? 1 : Math.Exp(-(canopy.BeamExtCoeffs[i] + canopy.NAllocationCoeff / canopy.LAI) * canopy.LAIAccums[i - 1])) -
                    Math.Exp(-(canopy.BeamExtCoeffs[i] + canopy.NAllocationCoeff / canopy.LAI) * canopy.LAIAccums[i])) /
                    (canopy.NAllocationCoeff + canopy.BeamExtCoeffs[i] * canopy.LAI);
            }
        }

        //---------------------------------------------------------------------------------------------------------
        public void CalcPRate25(LeafCanopy canopy, SunlitShadedCanopy shaded, PhotosynthesisModel PM)
        {
            for (int i = 0; i < _nLayers; i++)
            {
                VpMax25[i] = canopy.LAI * canopy.CPath.PsiVp * (canopy.LeafNTopCanopy - canopy.CPath.StructuralN) *
                    ((i == 0 ? 1 : Math.Exp(-(canopy.BeamExtCoeffs[i] + canopy.NAllocationCoeff / canopy.LAI) * canopy.LAIAccums[i - 1])) -
                    Math.Exp(-(canopy.BeamExtCoeffs[i] + canopy.NAllocationCoeff / canopy.LAI) * canopy.LAIAccums[i])) /
                    (canopy.NAllocationCoeff + canopy.BeamExtCoeffs[i] * canopy.LAI);
            }
        }
        //---------------------------------------------------------------------------------------------------------

        public void CalcGmRate25(LeafCanopy canopy, SunlitShadedCanopy shaded, PhotosynthesisModel PM)
        {
            for (int i = 0; i < _nLayers; i++)
            {
                Gm25[i] = canopy.LAI * canopy.CPath.PsiGm * (canopy.LeafNTopCanopy - canopy.CPath.StructuralN) *
                    ((i == 0 ? 1 : Math.Exp(-(canopy.BeamExtCoeffs[i] + canopy.NAllocationCoeff / canopy.LAI) * canopy.LAIAccums[i - 1])) -
                    Math.Exp(-(canopy.BeamExtCoeffs[i] + canopy.NAllocationCoeff / canopy.LAI) * canopy.LAIAccums[i])) /
                    (canopy.NAllocationCoeff + canopy.BeamExtCoeffs[i] * canopy.LAI);
            }
        }
    }
}
