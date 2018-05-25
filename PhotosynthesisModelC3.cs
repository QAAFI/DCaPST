using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C3MethodExtensions;

namespace LayerCanopyPhotosynthesis
{
    public class PhotosynthesisModelC3 : PhotosynthesisModel
    {
        public PhotosynthesisModelC3() : base() { }

        public override void Run(bool sendNotification, double swAvail = 0, double maxHourlyT = -1, double sunlitFraction = 0, double shadedFraction = 0)
        {
            if (!Initialised)
            {
                return;
            }

            if (sendNotification && NotifyStart != null)
            {
                NotifyStart(false);
            }

            EnvModel.Run(this.Time);
            Canopy.CalcCanopyStructure(this.EnvModel.SunAngle.Rad);

            SunlitAC1 = new SunlitCanopy(Canopy.NLayers, SSType.AC1);
            SunlitAJ = new SunlitCanopy(Canopy.NLayers, SSType.AJ);
            ShadedAC1 = new ShadedCanopy(Canopy.NLayers, SSType.AC1);
            ShadedAJ = new ShadedCanopy(Canopy.NLayers, SSType.AJ);

            double temp = EnvModel.GetTemp(Time);

            if (temp > Canopy.CPath.JTMax || temp < Canopy.CPath.JTMin || temp > Canopy.CPath.GmTMax || temp < Canopy.CPath.GmTMin)
            {
                ZeroVariables();
                return;
            }

            if (EnvModel.Ios.Value(this.Time) <= (0 + double.Epsilon))
            {
                ZeroVariables();
                return;
            }


            SunlitAC1.CalcLAI(this.Canopy, ShadedAC1);
            SunlitAJ.CalcLAI(this.Canopy, ShadedAJ);
            ShadedAC1.CalcLAI(this.Canopy, SunlitAC1);
            ShadedAJ.CalcLAI(this.Canopy, SunlitAJ);

            Canopy.Run(this, EnvModel);

            SunlitAC1.Run(Canopy.NLayers, this, ShadedAC1);
            SunlitAJ.Run(Canopy.NLayers, this, ShadedAJ);
            ShadedAC1.Run(Canopy.NLayers, this, SunlitAC1);
            ShadedAJ.Run(Canopy.NLayers, this, SunlitAJ);

            TranspirationMode mode = TranspirationMode.unlimited;

            if (maxHourlyT != -1)
            {
                mode = TranspirationMode.limited;
            }

            bool useAirTemp = false;

            List<bool> results = new List<bool>();

            results.Add(SunlitAC1.CalcPhotosynthesis(this, useAirTemp, 0, EnvModel.GetTemp(Time), mode, maxHourlyT, sunlitFraction));
            results.Add(SunlitAJ.CalcPhotosynthesis(this, useAirTemp, 0, EnvModel.GetTemp(Time), mode, maxHourlyT, sunlitFraction));
            results.Add(ShadedAC1.CalcPhotosynthesis(this, useAirTemp, 0, EnvModel.GetTemp(Time), mode, maxHourlyT, shadedFraction));
            results.Add(ShadedAJ.CalcPhotosynthesis(this, useAirTemp, 0, EnvModel.GetTemp(Time), mode, maxHourlyT, shadedFraction));

            Count = 1;

            bool caughtError = false;

            while (results.Contains(false) && !caughtError)
            {
                results.Clear();
                results.Add(SunlitAC1.CalcPhotosynthesis(this, useAirTemp, 0, SunlitAC1.LeafTemp[0], mode, maxHourlyT, sunlitFraction));
                results.Add(SunlitAJ.CalcPhotosynthesis(this, useAirTemp, 0, SunlitAJ.LeafTemp[0], mode, maxHourlyT, sunlitFraction));
                results.Add(ShadedAC1.CalcPhotosynthesis(this, useAirTemp, 0, ShadedAC1.LeafTemp[0], mode, maxHourlyT, shadedFraction));
                results.Add(ShadedAJ.CalcPhotosynthesis(this, useAirTemp, 0, ShadedAJ.LeafTemp[0], mode, maxHourlyT, shadedFraction));

                Count++;

                if (Count > 50 && !caughtError)
                {
                    ZeroVariables();

                    //writeScenario(swAvail);
                    caughtError = true;

                    return;
                }
            }

            // canopy.calcCanopyBiomassAccumulation(this);

            if (sendNotification && NotifyFinish != null)
            {
                NotifyFinish();
            }
           // writeScenario(swAvail);
        }

        public void ZeroVariables()
        {
            SunlitAC1.A[0] = 0;
            SunlitAJ.A[0] = 0;
            ShadedAC1.A[0] = 0;
            ShadedAJ.A[0] = 0;
            SunlitAC1.Elambda_[0] = 0;
            SunlitAJ.Elambda_[0] = 0;
            ShadedAC1.Elambda_[0] = 0;
            ShadedAJ.Elambda_[0] = 0;
        }

        public void WriteScenario(double swAvail)
        {
            StreamWriter sr = new StreamWriter("scenario.csv",true);

            sr.WriteLine("Lat," + EnvModel.LatitudeD);
            sr.WriteLine("DOY," + EnvModel.DOY);
            sr.WriteLine("Maxt," + EnvModel.MaxT);
            sr.WriteLine("AvailableWater," + swAvail);
            sr.WriteLine("Mint," + EnvModel.MinT);
            sr.WriteLine("Radn," + EnvModel.Radn);
            sr.WriteLine("Ratio," + EnvModel.AtmTransmissionRatio);
            sr.WriteLine("LAI," + Canopy.LAI);
            sr.WriteLine("SLN," + Canopy.CPath.SLNAv);

            List<double> temps = new List<double>();

            for (int i = 0; i <= 12; i++)
            {
                temps.Add(EnvModel.GetTemp(i + 6));
            }

            sr.WriteLine("Temps," + String.Join(",", temps.ToArray()));

            sr.Flush();

            sr.Close();
        }

        public override double[] RunApsim(int DOY, double latitude, double maxT, double minT, double radn, double lai, double SLN, double soilWaterAvail, double RootShootRatio)
        {

            EnvModel.Initialised = false;

            EnvModel.LatitudeD = latitude;
            EnvModel.DOY = DOY;
            EnvModel.MaxT = maxT;
            EnvModel.MinT = minT;
            EnvModel.Radn = radn;  // Check that this changes ratio
            EnvModel.ATM = 1.013;

            Canopy.LAI = lai;
            Canopy.CPath.SLNAv = SLN;
            

            EnvModel.Initialised = true;
            EnvModel.Run();

            Initialised = true;

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

                if (time > EnvModel.Sunrise && time < EnvModel.Sunset)
                {
                    Run(time, soilWaterAvail);
                    double sunlitWaterDemand = Math.Min(SunlitAC1.Elambda_[0], SunlitAJ.Elambda_[0]);
                    double shadedWaterDamand = Math.Min(ShadedAC1.Elambda_[0], ShadedAJ.Elambda_[0]);

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

                    hourlyWaterDemandsmm.Add((sunlitWaterDemands.Last() + shadedWaterDemands.Last()) / Canopy.Lambda * 1000 * 0.001 * 3600);
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
                    sunlitAssimilations.Add(Math.Min(SunlitAC1.A[0], SunlitAJ.A[0]));
                    shadedAssimilations.Add(Math.Min(ShadedAC1.A[0], ShadedAJ.A[0]));
                }
                catch (Exception)
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

                if (time > EnvModel.Sunrise && time < EnvModel.Sunset)
                {
                    Run(time, soilWaterAvail, hourlyWaterSuppliesmm[time - startHour], sunlitWaterDemand / totalWaterDemand, shadedWaterDemand / totalWaterDemand);
                    double sunlitAssimilation = Math.Min(SunlitAC1.A[0], SunlitAJ.A[0]);
                    double shadedAssimilation = Math.Min(ShadedAC1.A[0], ShadedAJ.A[0]);

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

                    double propIntRadn = Canopy.PropnInterceptedRadns.Sum();
                    interceptedRadn.Add(EnvModel.TotalIncidentRadiation * propIntRadn * 3600);
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
            //results[0] = (sunlitAssimilations.Sum() + shadedAssimilations.Sum()) * 3600 / 1000000 * 44 * B * 100 / ((1) * 100);
            results[1] = hourlyWaterDemandsmm.Sum();
            results[2] = hourlyWaterSuppliesmm.Sum();
            results[3] = interceptedRadn.Sum();

            return results;
        }
    }
}
