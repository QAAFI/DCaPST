using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerCanopyPhotosynthesis
{
    
    // Interface declaration.
    [System.Runtime.InteropServices.Guid("8f9e78bf-de86-4151-868b-db5c23608eba")]
    [System.Runtime.InteropServices.ComVisible(true)]
    public interface IApsimC4PhotoLink
    {
        double[] Calc(string[] paramNames, double[] paramValues, double DOY, double latitude, double maxT, double minT, double radn, double lai, double SLN, double soilWaterAvail,
            double B, double RootShootRatio, double LeafAngle, double SLNRatioTop, double psiVc, double psiJ, double psiRd, double psiVp,
            double psiFactor, double Ca, double CiCaRatio, double gbs, double gm25, double Vpr, double structuralN);
        void Setup(string[] paramNames, double[] paramValues);
    };
    
    // Interface implementation.
    [System.Runtime.InteropServices.Guid("5657ad55-ddca-4d7e-a2bc-1b0d119a85e2")]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class ApsimC4PhotoLink : IApsimC4PhotoLink
    {
        public void Setup(string[] paramNames, double[] paramValues)
        {
            LayerCanopyPhotosynthesis.PhotosynthesisModelC4 PM = new LayerCanopyPhotosynthesis.PhotosynthesisModelC4();
            PM.Canopy.CPath.CiCaRatio = paramValues[4];
        }
        public void Calc() { } //deliberately empty 
        public double[] Calc(string[] paramNames, double[] paramValues, double DOY, double latitude, double maxT, double minT, double radn, double lai, double SLN, double soilWaterAvail,
            double RootShootRatio, double B, double LeafAngle, double SLNRatioTop, double psiVc, double psiJ, double psiRd, double psiVp, 
            double psiFactor, double Ca, double CiCaRatio, double gbs, double gm25, double Vpr, double structuralN) //0 = simple conductance
        {
            Dictionary<string, double> parameters = new Dictionary<string, double>();
            for(int i=0;i<paramNames.Count();i++)
            {
                parameters.Add(paramNames[i], paramValues[i]);
            }

            LayerCanopyPhotosynthesis.PhotosynthesisModelC4 PM = new LayerCanopyPhotosynthesis.PhotosynthesisModelC4();
            PM.Initialised = false;
            PM.photoPathway = LayerCanopyPhotosynthesis.PhotosynthesisModel.PhotoPathway.C4;

            PM.conductanceModel = LayerCanopyPhotosynthesis.PhotosynthesisModel.ConductanceModel.SIMPLE;
            PM.electronTransportModel = LayerCanopyPhotosynthesis.PhotosynthesisModel.ElectronTransportModel.EMPIRICAL;

            PM.Canopy.NLayers = 1;

            PM.EnvModel.LatitudeD = latitude;
            PM.EnvModel.DOY = (int)DOY;
            PM.EnvModel.MaxT = maxT;
            PM.EnvModel.MinT = minT;
            PM.EnvModel.Radn = radn;  // Check that this changes ratio
            PM.EnvModel.ATM = 1.013;

            PM.Canopy.LAI = lai;
            PM.Canopy.LeafAngle = LeafAngle;
            PM.Canopy.LeafWidth = 0.05;
            PM.Canopy.U0 = 1;
            PM.Canopy.Ku = 0.5;

            PM.Canopy.CPath.CiCaRatio = parameters["CiCaRatio"];

            PM.Canopy.CPath.SLNAv = SLN;
            PM.Canopy.CPath.SLNRatioTop = parameters["SLNRatioTop"];
            PM.Canopy.CPath.StructuralN = parameters["structuralN"];

            double psiFact  = parameters["psiFactor"];
            PM.Canopy.CPath.PsiVc = parameters["psiVc"] * psiFact;
            PM.Canopy.CPath.PsiJ = parameters["psiJ"] * psiFact;
            PM.Canopy.CPath.PsiRd = parameters["psiRd"] * psiFact;
            PM.Canopy.CPath.PsiVp = parameters["psiVp"] * psiFact;

            PM.Canopy.Rcp = 1200;
            PM.Canopy.G = 0.066;
            PM.Canopy.Sigma = 5.668E-08;
            PM.Canopy.Lambda = 2447000;

            PM.Canopy.θ = 0.7;
            PM.Canopy.F = 0.15;
            PM.Canopy.OxygenPartialPressure = 210000;
            PM.Canopy.Ca = parameters["Ca"];

            PM.Canopy.Gbs_CO2 = 0.003;
            PM.Canopy.Alpha = 0.1;
            PM.Canopy.X = 0.4;
            
            PM.Canopy.DiffuseExtCoeff = 0.8;
            PM.Canopy.LeafScatteringCoeff = 0.2;
            PM.Canopy.DiffuseReflectionCoeff = 0.057;

            PM.Canopy.DiffuseExtCoeffNIR = 0.8;
            PM.Canopy.LeafScatteringCoeffNIR = 0.8;
            PM.Canopy.DiffuseReflectionCoeffNIR = 0.389;

            PM.Canopy.CPath.Kc_P25 = 1210;
            PM.Canopy.CPath.Kc_c = 25.899;
            PM.Canopy.CPath.Kc_b = 7721.915;
            PM.Canopy.CPath.Ko_P25 = 292000;
            PM.Canopy.CPath.Ko_c = 4.236;
            PM.Canopy.CPath.Ko_b = 1262.93;
            PM.Canopy.CPath.VcMax_VoMax_P25 = 5.401;
            PM.Canopy.CPath.VcMax_VoMax_c = 9.126;
            PM.Canopy.CPath.VcMax_VoMax_b = 2719.478;
            PM.Canopy.CPath.VcMax_c = 31.467;
            PM.Canopy.CPath.VcMax_b = 9381.766;
            PM.Canopy.CPath.Rd_c = 0;
            PM.Canopy.CPath.Rd_b = 0;
            PM.Canopy.CPath.Kp_P25 = 139;
            PM.Canopy.CPath.Kp_c = 14.644;
            PM.Canopy.CPath.Kp_b = 4366.129;
            PM.Canopy.CPath.VpMax_c = 38.244;
            PM.Canopy.CPath.VpMax_b = 11402.45;

            PM.Canopy.CPath.JMax_TOpt = 32.633;
            PM.Canopy.CPath.JMax_Omega = 15.27;
            PM.Canopy.CPath.Gm_TOpt = 34.309;
            PM.Canopy.CPath.Gm_Omega = 20.791;

            PM.Canopy.Gbs_CO2 = parameters["gbs"];
            PM.Canopy.CPath.Gm_P25 = parameters["gm25"];
            PM.Canopy.Vpr_l = parameters["Vpr"];

            PM.EnvModel.Initilised = true;
            PM.EnvModel.Run();

            PM.Initialised = true;

            List<double> sunlitWaterDemands = new List<double>();
            List<double> shadedWaterDemands = new List<double>();
            List<double> hourlyWaterDemandsmm = new List<double>();
            List<double> hourlyWaterSuppliesmm = new List<double>();
            List<double> sunlitAssimilations = new List<double>();
            List<double> shadedAssimilations = new List<double>();
            List<double> interceptedRadn = new List<double>();

            for (int time = 6; time <= 18; time++)
            {
                //This run is to get potential water use

                if (time > PM.EnvModel.Sunrise && time < PM.EnvModel.Sunset)
                {  
                    PM.Run(time, soilWaterAvail);
                    sunlitWaterDemands.Add(Math.Min(Math.Min(PM.SunlitAC1.Elambda_[0], PM.sunlitAC2.Elambda_[0]), PM.SunlitAJ.Elambda_[0]));
                    shadedWaterDemands.Add(Math.Min(Math.Min(PM.ShadedAC1.Elambda_[0], PM.shadedAC2.Elambda_[0]), PM.ShadedAJ.Elambda_[0]));

                    sunlitWaterDemands[sunlitWaterDemands.Count - 1] = Math.Max(sunlitWaterDemands.Last(), 0);
                    shadedWaterDemands[shadedWaterDemands.Count - 1] = Math.Max(shadedWaterDemands.Last(), 0);

                    hourlyWaterDemandsmm.Add((sunlitWaterDemands.Last() + shadedWaterDemands.Last()) / PM.Canopy.Lambda * 1000 * 0.001 * 3600);
                    hourlyWaterSuppliesmm.Add(hourlyWaterDemandsmm.Last());
                }
                else
                {
                    sunlitWaterDemands.Add(0);
                    shadedWaterDemands.Add(0);
                    hourlyWaterDemandsmm.Add(0);
                    hourlyWaterSuppliesmm.Add(0);
                }

                sunlitAssimilations.Add(0);
                shadedAssimilations.Add(0);
            }

            double maxHourlyT = hourlyWaterSuppliesmm.Max();

            while (hourlyWaterSuppliesmm.Sum() > soilWaterAvail)
            {
                maxHourlyT *= 0.99;
                for (int i = 0; i < hourlyWaterSuppliesmm.Count; i++)
                {
                    if (hourlyWaterSuppliesmm[i] > maxHourlyT)
                    {
                        hourlyWaterSuppliesmm[i] = maxHourlyT;
                    }
                }
            }


            sunlitAssimilations.Clear();
            shadedAssimilations.Clear();


            //Now that we have our hourly supplies we can calculate again
            for (int time = 6; time <= 18; time++)
            {
                double TSupply = hourlyWaterSuppliesmm[time - 6];
                double sunlitWaterDemand = sunlitWaterDemands[time - 6];
                double shadedWaterDemand = shadedWaterDemands[time - 6];

                double totalWaterDemand = sunlitWaterDemand + shadedWaterDemand;

                if (time > PM.EnvModel.Sunrise && time < PM.EnvModel.Sunset)
                {
                    PM.Run(time, soilWaterAvail, hourlyWaterSuppliesmm[time - 6], sunlitWaterDemand / totalWaterDemand, shadedWaterDemand / totalWaterDemand);
                    sunlitAssimilations.Add(Math.Min(Math.Min(PM.SunlitAC1.A[0], PM.sunlitAC2.A[0]), PM.SunlitAJ.A[0]));
                    shadedAssimilations.Add(Math.Min(Math.Min(PM.ShadedAC1.A[0], PM.shadedAC2.A[0]), PM.ShadedAJ.A[0]));


                    sunlitAssimilations[sunlitAssimilations.Count - 1] = Math.Max(sunlitAssimilations.Last(), 0);
                    shadedAssimilations[shadedAssimilations.Count - 1] = Math.Max(shadedAssimilations.Last(), 0);

                    double propIntRadn = PM.Canopy.PropnInterceptedRadns.Sum();
                    interceptedRadn.Add(PM.EnvModel.TotalIncidentRadiation * propIntRadn * 3600);
                    interceptedRadn[interceptedRadn.Count - 1] = Math.Max(interceptedRadn.Last(), 0);
                }
                else
                {
                    sunlitAssimilations.Add(0);
                    shadedAssimilations.Add(0);
                    interceptedRadn.Add(0);
                }
            }
            double[] results = new double[4];

            results[0] = (sunlitAssimilations.Sum() + shadedAssimilations.Sum()) * 3600 / 1000000 * 44 * B * 100 / ((1 + RootShootRatio) * 100);
            results[1] = hourlyWaterDemandsmm.Sum();
            results[2] = hourlyWaterSuppliesmm.Sum();
            results[3] = interceptedRadn.Sum();

            return results;
        }
    }
}
