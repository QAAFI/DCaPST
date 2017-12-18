using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerCanopyPhotosynthesis
{

    // Interface declaration.
    [System.Runtime.InteropServices.Guid("14c6b790-8f60-4872-a062-b76f5fa6ab6f")]
    [System.Runtime.InteropServices.ComVisible(true)]
    public interface IApsimC3PhotoLink
    {
        double[] Calc(int DOY, double latitude, double maxT, double minT, double radn, double lai, double SLN, double soilWaterAvail,
            double B, double RootShootRatio, double LeafAngle, double SLNRatioTop, double psiVc, double psiJ, double psiRd,
            double psiFactor, double Ca, double CiCaRatio, double gm25, double structuralN);
    };

    // Interface implementation.
    [System.Runtime.InteropServices.Guid("6ef85006-aad2-4aaf-8a84-e5d6020d9a5c")]
    [System.Runtime.InteropServices.ComVisible(true)]
    public class ApsimC3PhotoLink : IApsimC3PhotoLink
    {
        public void Calc() { } //deliberately empty 

        public double[] Calc(int DOY, double latitude, double maxT, double minT, double radn, double lai, double SLN, double soilWaterAvail,
            double B, double RootShootRatio, double LeafAngle, double SLNRatioTop, double psiVc, double psiJ, double psiRd,
            double psiFactor, double Ca, double CiCaRatio, double gm25, double structuralN) //0 = simple conductance
        {

            LayerCanopyPhotosynthesis.PhotosynthesisModelC3 PM = new LayerCanopyPhotosynthesis.PhotosynthesisModelC3();
            PM.Initialised = false;
            PM.photoPathway = LayerCanopyPhotosynthesis.PhotosynthesisModel.PhotoPathway.C3;

            PM.conductanceModel = LayerCanopyPhotosynthesis.PhotosynthesisModel.ConductanceModel.SIMPLE;
            PM.electronTransportModel = LayerCanopyPhotosynthesis.PhotosynthesisModel.ElectronTransportModel.EMPIRICAL;

            PM.Canopy.NLayers = 1;

            PM.EnvModel.LatitudeD = latitude;
            PM.EnvModel.DOY = DOY;
            PM.EnvModel.MaxT = maxT;
            PM.EnvModel.MinT = minT;
            PM.EnvModel.Radn = radn;  // Check that this changes ratio
            PM.EnvModel.ATM = 1.013;

            PM.Canopy.LAI = lai;
            PM.Canopy.LeafAngle = LeafAngle;
            PM.Canopy.LeafWidth = 0.05;
            PM.Canopy.U0 = 1;
            PM.Canopy.Ku = 0.5;

            PM.Canopy.CPath.SLNAv = SLN;
            PM.Canopy.CPath.SLNRatioTop = SLNRatioTop;
            PM.Canopy.CPath.StructuralN = structuralN;

            PM.Canopy.CPath.PsiVc = psiVc * psiFactor;
            PM.Canopy.CPath.PsiJ = psiJ * psiFactor;
            PM.Canopy.CPath.PsiRd = psiRd * psiFactor;

            PM.Canopy.Rcp = 1200;
            PM.Canopy.G = 0.066;
            PM.Canopy.Sigma = 5.668E-08;
            PM.Canopy.Lambda = 2447000;

            PM.Canopy.θ = 0.7;
            PM.Canopy.F = 0.15;
            PM.Canopy.OxygenPartialPressure = 210000;
            PM.Canopy.Ca = Ca;
            PM.Canopy.CPath.CiCaRatio = CiCaRatio;

            PM.Canopy.Gbs_CO2 = 0.003;
            PM.Canopy.Alpha = 0.1;
            PM.Canopy.X = 0.4;

            PM.Canopy.DiffuseExtCoeff = 0.8;
            PM.Canopy.LeafScatteringCoeff = 0.2;
            PM.Canopy.DiffuseReflectionCoeff = 0.057;

            PM.Canopy.DiffuseExtCoeffNIR = 0.8;
            PM.Canopy.LeafScatteringCoeffNIR = 0.8;
            PM.Canopy.DiffuseReflectionCoeffNIR = 0.389;

            PM.Canopy.CPath.Kc_P25 = 272.38;
            PM.Canopy.CPath.Kc_c = 32.689;
            PM.Canopy.CPath.Kc_b = 9741.4;

            PM.Canopy.CPath.Ko_P25 = 165820;
            PM.Canopy.CPath.Ko_c = 9.574;
            PM.Canopy.CPath.Ko_b = 2853.019;

            PM.Canopy.CPath.VcMax_VoMax_P25 = 4.58;
            PM.Canopy.CPath.VcMax_VoMax_c = 13.241;
            PM.Canopy.CPath.VcMax_VoMax_b = 3945.722;

            PM.Canopy.CPath.VcMax_c = 26.355;
            PM.Canopy.CPath.VcMax_b = 7857.83;
            PM.Canopy.CPath.Rd_c = 18.715;
            PM.Canopy.CPath.Rd_b = 5579.745;

            PM.Canopy.CPath.JMax_TOpt = 28.796;
            PM.Canopy.CPath.JMax_Omega = 15.536;
            PM.Canopy.CPath.Gm_P25 = gm25;
            PM.Canopy.CPath.Gm_TOpt = 34.309;
            PM.Canopy.CPath.Gm_Omega = 20.791;

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

            int startHour = 6;
            int endHour = 18;

            for (int time = startHour; time <= endHour; time++)
            {
                //This run is to get potential water use

                if (time > PM.EnvModel.Sunrise && time < PM.EnvModel.Sunset)
                {
                    PM.Run(time, soilWaterAvail);
                    double sunlitWaterDemand = Math.Min(PM.SunlitAC1.Elambda_[0], PM.SunlitAJ.Elambda_[0]);
                    double shadedWaterDamand = Math.Min(PM.ShadedAC1.Elambda_[0], PM.ShadedAJ.Elambda_[0]);

                    if (double.IsNaN(sunlitWaterDemand))
                    {
                        sunlitWaterDemand = 0;
                    }

                    if (double.IsNaN(shadedWaterDamand))
                    {
                        shadedWaterDamand = 0;
                    }

                    sunlitWaterDemands.Add(sunlitWaterDemand);
                    shadedWaterDemands.Add(shadedWaterDamand);

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
                try
                {
                    sunlitAssimilations.Add(Math.Min(PM.SunlitAC1.A[0], PM.SunlitAJ.A[0]));
                    shadedAssimilations.Add(Math.Min(PM.ShadedAC1.A[0], PM.ShadedAJ.A[0]));
                }
                catch(Exception)
                {
                    sunlitAssimilations.Add(0);
                    shadedAssimilations.Add(0);
                }
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
            for (int time = startHour; time <= endHour; time++)
            {
                double TSupply = hourlyWaterSuppliesmm[time - startHour];
                double sunlitWaterDemand = sunlitWaterDemands[time - startHour];
                double shadedWaterDemand = shadedWaterDemands[time - startHour];

                double totalWaterDemand = sunlitWaterDemand + shadedWaterDemand;

                if (time > PM.EnvModel.Sunrise && time < PM.EnvModel.Sunset)
                {
                    PM.Run(time, soilWaterAvail, hourlyWaterSuppliesmm[time - startHour], sunlitWaterDemand / totalWaterDemand, shadedWaterDemand / totalWaterDemand);
                    double sunlitAssimilation = Math.Min(PM.SunlitAC1.A[0], PM.SunlitAJ.A[0]);
                    double shadedAssimilation = Math.Min(PM.ShadedAC1.A[0], PM.ShadedAJ.A[0]);

                    if (double.IsNaN(sunlitAssimilation))
                    {
                        sunlitAssimilation = 0;
                    }

                    if (double.IsNaN(shadedAssimilation))
                    {
                        shadedAssimilation = 0;
                    }
                    sunlitAssimilations.Add(sunlitAssimilation);
                    shadedAssimilations.Add(shadedAssimilation);


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

            //results[0] = (sunlitAssimilations.Sum() + shadedAssimilations.Sum()) * 3600 / 1000000 * 44 * B * 100 / ((1 + RootShootRatio) * 100);
            results[0] = (sunlitAssimilations.Sum() + shadedAssimilations.Sum()) * 3600 / 1000000 * 44 * B * 100 / ((1) * 100);
            results[1] = hourlyWaterDemandsmm.Sum();
            results[2] = hourlyWaterSuppliesmm.Sum();
            results[3] = interceptedRadn.Sum();

            return results;
        }
    }
}
