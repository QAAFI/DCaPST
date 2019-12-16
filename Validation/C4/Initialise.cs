using System;
using System.Collections.Generic;
using System.Text;

using LayerCanopyPhotosynthesis;

namespace Validation.C4
{
    public static class Initialise
    {
        public static PhotosynthesisModelC4 NewC4()
        {
            double PsiFactor = 0.4;

            var PM = new PhotosynthesisModelC4();
            PM.photoPathway = PhotosynthesisModel.PhotoPathway.C4;
            PM.Initialised = false;
            PM.Canopy.NLayers = 1;

            //Set the parameters
            PM.EnvModel.Initialised = false;

            PM.Canopy.U0 = 1.5;     //U0 - wind speed just above the canopy

            PM.EnvModel.RPAR = 0.5;     //RPAR - Fraction of PAR energy to that of the total solar

            PM.Canopy.Ca = 363;     //Ca - Atmsopheric CO2 partial pressure
            PM.Canopy.OxygenPartialPressure = 210000;     //OxygenPartialPressure - Atmsopheric O2 partial pressure
            PM.EnvModel.ATM = 1.01325;     //Air pressure - Air pressure

            //////// Canopy level ////////
            PM.B = 0.409;     //BiomassConversionCoefficient - CO2-to-biomass conversion efficiency

            PM.Canopy.LeafAngle = 60;     //LeafAngle - Leaf angle (with respect to the horizontal plane)
            PM.Canopy.CPath.SLNRatioTop = 1.3;     //SLNRatioTop - Ratio of the specific leaf nitrogen at the top of the canopy to that of the canopy average
            PM.Canopy.CPath.StructuralN = 14;     //StructuralN - Specific leaf nitrogen below which Vcamx, Jmax and Vpmax are zero
            PM.Canopy.DiffuseExtCoeff = 0.78;     //DiffuseExtCoeff - 
            PM.Canopy.DiffuseExtCoeffNIR = 0.8;     //DiffuseExtCoeffNIR - 
            PM.Canopy.LeafScatteringCoeff = 0.15;     //LeafScatteringCoeff - leaf-level scattering coefficient for PAR
            PM.Canopy.LeafScatteringCoeffNIR = 0.8;     //LeafScatteringCoeffNIR - leaf-level scattering coefficient for NIR
            PM.Canopy.DiffuseReflectionCoeff = 0.036;     //DiffuseReflectionCoeff - canopy-level reflection coefficient for diffuse radiation for PAR
            PM.Canopy.DiffuseReflectionCoeffNIR = 0.389;     //DiffuseReflectionCoeffNIR - canopy-level reflection coefficient for diffuse radiation for NIR
            PM.Canopy.LeafWidth = 0.15;     //LeafWidth - Leaf width
            PM.Canopy.Ku = 1.5;     //Ku - canopy wind speed profile coefficient

            //////// Photosynthesis ////////
            PM.Canopy.CPath.PsiVc = 0.465;     //PsiVc - Slope of linear relationship between Vcmax per leaf area at 25°C and specific leaf nitrogen
            PM.Canopy.CPath.PsiJ = 2.7;     //PsiJ - Slope of linear relationship between Jmax per leaf area at 25°C and specific leaf nitrogen
            PM.Canopy.CPath.PsiRd = 0;     //PsiRd - Slope of linear relationship between Rd per leaf area at 25°C and specific leaf nitrogen
            PM.Canopy.CPath.PsiVp = 1.55;     //PsiVp - Slope of linear relationship between Vpmax per leaf area at 25°C and specific leaf nitrogen
            PM.Canopy.Vpr_l = 120;     //Vpr_l - PEP regeneration rate per unit leaf
            PM.Canopy.Theta = 0.7;     //theta - Empirical curvature factor
            PM.Canopy.F = 0.15;     //F - Spectral correction factor
            PM.Canopy.Alpha = 0.1;     //Alpha - Fraction of PS II activity in the bundle sheath
            PM.Canopy.CPath.Phi = 2;     //Phi - extra energy (ATP) cost required from processes other than the C3 cycle
            PM.Canopy.CPath.X = 0.4;     //X - Fraction of electron transport partitioned to mesophyll chloroplasts
            PM.Canopy.Constant = 0.047;     //Constant - 

            //////// Leaf gas diffusion ////////
            PM.Canopy.CPath.CiCaRatio = 0.45;     //CiCaRatio - ratio of intercellular to air CO2 partial pressure
            PM.Canopy.CPath.PsiGm = 0.0135; // 0.0135 0.0159392789373814;     //PsiGm - Slope of linear relationship between gm per leaf area at 25°C and specific leaf nitrogen
            PM.Canopy.Gbs_CO2 = 0.003;     //Gbs_CO2 - Bundle-sheath conductance for CO2 per leaf area

            //////// Temperature response ////////
            PM.Canopy.CPath.KcP25 = 1210;     //KcP25
            PM.Canopy.CPath.KcTEa = 64200;     //KcEa
            PM.Canopy.CPath.KoP25 = 292000;     //KoP25
            PM.Canopy.CPath.KoTEa = 10500;     //KoEa

            PM.Canopy.CPath.VcTEa = 78000;     //VcmaxEa
            PM.Canopy.CPath.VcMax_VoMaxP25 = 5.51328906454566;     //Vcmax_VomaxP25
            PM.Canopy.CPath.VcMax_VoMaxTEa = 21265.4029552906;     //Vcmax_VomaxEa
            PM.Canopy.CPath.KpP25 = 75;     //KpP25
            PM.Canopy.CPath.KpTEa = 36300;     //KpEa

            PM.Canopy.CPath.VpMaxTEa = 57043.2677590512;     //VpmaxEa

            PM.Canopy.CPath.RdTEa = 46390;     //RdEa

            PM.Canopy.CPath.JTMin = 0;     //JTMin
            PM.Canopy.CPath.JTOpt = 37.8649150880407;     //JTOpt
            PM.Canopy.CPath.JTMax = 55;     //JTMax
            PM.Canopy.CPath.JMaxC = 0.711229539802063;     //JMaxC
            PM.Canopy.CPath.JBeta = 1;     //Jbeta

            PM.Canopy.CPath.GmTMin = 0;     //GmTMin
            PM.Canopy.CPath.GmTOpt = 42;     //GmTOpt
            PM.Canopy.CPath.GmTMax = 55;     //GmTMax
            PM.Canopy.CPath.GmC = 0.462820450976839;     //GmMaxC
            PM.Canopy.CPath.GmBeta = 1;     //Gmbeta

            //////// Constants ////////
            PM.Canopy.Sigma = 0.0000000567;     //
            PM.Canopy.Rcp = 1200;     //Volumetric heat capacity of air

            PM.Canopy.G = 0.066;     //Psychrometric constant
            PM.Canopy.Lambda = 2447000;     //Latent heat of vapourisation of water

            //Set the Psi values using PsiFactor
            PM.Canopy.CPath.PsiVc *= PsiFactor;
            PM.Canopy.CPath.PsiJ *= PsiFactor;
            PM.Canopy.CPath.PsiRd *= PsiFactor;
            PM.Canopy.CPath.PsiVp *= PsiFactor;
            PM.Canopy.CPath.PsiGm *= PsiFactor;

            PM.EnvModel.Initialised = true;
            PM.Initialised = true;

            return PM;
        }
    }
}
