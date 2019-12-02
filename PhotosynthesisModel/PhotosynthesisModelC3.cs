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
        public PhotosynthesisModelC3() : base()
        {
            photoPathway = PhotoPathway.C3;
        }
                
        public override double[] RunApsim(int DOY, double latitude, double maxT, double minT, double radn, 
            double lai, double SLN, double soilWaterAvail, double RootShootRatio, double MaxHourlyTRate = 100)
        {
            EnvModel.Initialised = false;

            EnvModel.LatitudeD = latitude;
            EnvModel.DOY = DOY;
            EnvModel.MaxT = maxT;
            EnvModel.MinT = minT;
            EnvModel.Radn = radn;  // Check that this changes ratio

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

            // Calculate the potential photosynthesis and water demand
            for (int time = startHour; time <= endHour; time++)
            {
                double rads = EnvModel.Ios.Value(time);
                if (double.IsNaN(rads) || rads <= 0)
                {
                    interceptedRadn.Add(0);

                    sunlitWaterDemands.Add(0);
                    shadedWaterDemands.Add(0);
                    hourlyWaterDemandsmm.Add(0);
                    hourlyWaterSuppliesmm.Add(0);

                    sunlitAssimilations.Add(0);
                    shadedAssimilations.Add(0);
                }
                else
                {                   
                    // Run the photosynthesis model
                    Run(time, soilWaterAvail);

                    // Calculate hourly radiation
                    double propIntRadn = Canopy.PropnInterceptedRadns.Sum();
                    double radiation = EnvModel.TotalIncidentRadiation * propIntRadn * 3600;
                    interceptedRadn.Add(radiation);

                    // Retrieve water demand
                    double sunlitWaterDemand = Math.Min(SunlitAc1.WaterUse[0], SunlitAj.WaterUse[0]);
                    double shadedWaterDemand = Math.Min(ShadedAc1.WaterUse[0], ShadedAj.WaterUse[0]);

                    // Retrieve assimilations
                    double sunlitAssimilation = Math.Min(SunlitAc1.A[0], SunlitAj.A[0]);
                    double shadedAssimilation = Math.Min(ShadedAc1.A[0], ShadedAj.A[0]);

                    // Note: Values are explicitly defined to be 0 or Sensible inside the Run() method 
                    sunlitWaterDemands.Add(sunlitWaterDemand);
                    shadedWaterDemands.Add(shadedWaterDemand);
                    hourlyWaterDemandsmm.Add((sunlitWaterDemands.Last() + shadedWaterDemands.Last()));
                    hourlyWaterSuppliesmm.Add(hourlyWaterDemandsmm.Last());

                    sunlitAssimilations.Add(sunlitAssimilation);
                    shadedAssimilations.Add(shadedAssimilation);
                }
            }

            potentialAssimilation = sunlitAssimilations.Sum() + shadedAssimilations.Sum();

            // Setting water supply to water available from Apsim
            double maxHourlyT = Math.Min(hourlyWaterSuppliesmm.Max(), MaxHourlyTRate);
            for (int i = 0; i < hourlyWaterSuppliesmm.Count; i++)
            {
                if (hourlyWaterSuppliesmm[i] > maxHourlyT)
                {
                    hourlyWaterSuppliesmm[i] = maxHourlyT;
                }
            }
            
            if (soilWaterAvail > 0.0001)
            {
                if (hourlyWaterDemandsmm.Sum() > soilWaterAvail)
                {
                    double tolerance = 0.000001;

                    double highTestValue = maxHourlyT;
                    double lowTestValue = 0;
                    double trialTestValue = 0;

                    double supplyTrial = hourlyWaterSuppliesmm.Sum();
                    double supplyHighTest = 0;
                    double supplyLowTest = 0;

                    while ((soilWaterAvail + tolerance) < supplyTrial || (soilWaterAvail - tolerance) > supplyTrial)
                    {
                        supplyHighTest = CalcTestSoilWater(hourlyWaterSuppliesmm, highTestValue);
                        supplyLowTest = CalcTestSoilWater(hourlyWaterSuppliesmm, lowTestValue);

                        trialTestValue = (highTestValue + lowTestValue) / 2;
                        supplyTrial = CalcTestSoilWater(hourlyWaterSuppliesmm, trialTestValue);

                        if (supplyHighTest > soilWaterAvail && supplyTrial < soilWaterAvail)
                        {
                            lowTestValue = trialTestValue;
                        }
                        else if (supplyTrial > soilWaterAvail && lowTestValue < soilWaterAvail)
                        {
                            highTestValue = trialTestValue;
                        }
                    }

                    for (int i = 0; i < hourlyWaterSuppliesmm.Count; i++)
                    {
                        if (hourlyWaterSuppliesmm[i] > trialTestValue)
                        {
                            hourlyWaterSuppliesmm[i] = trialTestValue;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < hourlyWaterSuppliesmm.Count; i++)
                {
                    hourlyWaterSuppliesmm[i] = 0;
                }
            }

            // Calculate the water limited photosynthesis (if necessary)
            if (soilWaterAvail < hourlyWaterDemandsmm.Sum())
            {
                sunlitAssimilations.Clear();
                shadedAssimilations.Clear();

                // Now that we have our hourly supplies we can calculate again
                for (int time = startHour; time <= endHour; time++)
                {
                    double sunlitWaterDemand = sunlitWaterDemands[time - startHour];
                    double shadedWaterDemand = shadedWaterDemands[time - startHour];
                    double totalWaterDemand = sunlitWaterDemand + shadedWaterDemand;

                    if (interceptedRadn[time - 6] > 0)
                    {
                        Run(time, soilWaterAvail, hourlyWaterSuppliesmm[time - startHour], sunlitWaterDemand / totalWaterDemand, shadedWaterDemand / totalWaterDemand);
                        
                        double sunlitAssimilation = Math.Min(SunlitAc1.A[0], SunlitAj.A[0]);
                        double shadedAssimilation = Math.Min(ShadedAc1.A[0], ShadedAj.A[0]);
                        
                        sunlitAssimilations.Add(sunlitAssimilation);
                        shadedAssimilations.Add(shadedAssimilation);
                    }
                    else
                    {
                        sunlitAssimilations.Add(0);
                        shadedAssimilations.Add(0);
                    }
                }
            }

            double[] results = new double[5];

            for (int i = 0; i < sunlitAssimilations.Count; i++)
            {
                if (double.IsNaN(sunlitAssimilations[i]))
                {
                    sunlitAssimilations[i] = 0;
                }
                if (double.IsNaN(shadedAssimilations[i]))
                {
                    shadedAssimilations[i] = 0;
                }
                if (double.IsNaN(hourlyWaterDemandsmm[i]))
                {
                    hourlyWaterDemandsmm[i] = 0;
                }
                if (double.IsNaN(hourlyWaterSuppliesmm[i]))
                {
                    hourlyWaterSuppliesmm[i] = 0;
                }

                if (double.IsNaN(interceptedRadn[i]))
                {
                    interceptedRadn[i] = 0;
                }
            }

            results[0] = (sunlitAssimilations.Sum() + shadedAssimilations.Sum()) * 3600 / 1000000 * 44 * B * 100 / ((1 + RootShootRatio) * 100);
            results[1] = hourlyWaterDemandsmm.Sum();
            results[2] = hourlyWaterSuppliesmm.Sum();
            results[3] = interceptedRadn.Sum();
            results[4] = potentialAssimilation * 3600 / 1000000 * 44 * B * 100 / ((1 + RootShootRatio) * 100);

            return results;
        }

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

            // Sets the environment
            EnvModel.Run(this.Time);
            Canopy.CalcCanopyStructure(this.EnvModel.SunAngle.Rad);

            // Sets the canopy status
            SunlitAc1 = new SunlitCanopy(Canopy.NLayers, SSType.Ac1);
            SunlitAj = new SunlitCanopy(Canopy.NLayers, SSType.Aj);
            ShadedAc1 = new ShadedCanopy(Canopy.NLayers, SSType.Ac1);
            ShadedAj = new ShadedCanopy(Canopy.NLayers, SSType.Aj);

            double temp = EnvModel.GetTemp(Time);

            // Don't perform photosynthesis beyond temperature boundaries
            if (temp > Canopy.CPath.JTMax || temp < Canopy.CPath.JTMin || temp > Canopy.CPath.GmTMax || temp < Canopy.CPath.GmTMin)
            {
                ZeroVariables();
                return;
            }

            // Don't perform photosynthesis if the sun isn't up
            if (EnvModel.Ios.Value(this.Time) <= (0 + double.Epsilon))
            {
                ZeroVariables();
                return;
            }

            // Calculate the leaf area index
            SunlitAc1.CalcLAI(this.Canopy, ShadedAc1);
            SunlitAj.CalcLAI(this.Canopy, ShadedAj);
            ShadedAc1.CalcLAI(this.Canopy, SunlitAc1);
            ShadedAj.CalcLAI(this.Canopy, SunlitAj);

            // Run the canopy model
            Canopy.Run(this, EnvModel);
            SunlitAc1.Run(Canopy.NLayers, this, ShadedAc1);
            SunlitAj.Run(Canopy.NLayers, this, ShadedAj);
            ShadedAc1.Run(Canopy.NLayers, this, SunlitAc1);
            ShadedAj.Run(Canopy.NLayers, this, SunlitAj);

            // Set the transpirtation mode
            TranspirationMode mode = TranspirationMode.unlimited;
            if (maxHourlyT != -1)
            {
                mode = TranspirationMode.limited;
            }

            // For the initial photosynthesis calculation each hour we use air temperature
            bool useAirTemp = true;

            var results = new bool[4];
            // Initial calculation
            results[0] = (SunlitAc1.CalcPhotosynthesis(this, useAirTemp, 0, EnvModel.GetTemp(Time), mode, maxHourlyT, sunlitFraction));
            results[1] = (SunlitAj.CalcPhotosynthesis(this, useAirTemp, 0, EnvModel.GetTemp(Time), mode, maxHourlyT, sunlitFraction));
            results[2] = (ShadedAc1.CalcPhotosynthesis(this, useAirTemp, 0, EnvModel.GetTemp(Time), mode, maxHourlyT, shadedFraction));
            results[3] = (ShadedAj.CalcPhotosynthesis(this, useAirTemp, 0, EnvModel.GetTemp(Time), mode, maxHourlyT, shadedFraction));

            // After initialisation we use new leaf temperature
            useAirTemp = false;

            // SUNLIT CALCULATIONS
            double defaultAC1A = 0.0;
            double defaultAC1Water = 0.0;
            double defaultAJA = 0.0;
            double defaultAJWater = 0.0;

            // If the initial calculation fails, set values to 0
            if (results[0] || results[1])
            {
                SunlitAc1.A[0] = 0.0;
                SunlitAc1.WaterUse[0] = 0.0;
                SunlitAj.A[0] = 0.0;
                SunlitAj.WaterUse[0] = 0.0;
            }
            else
            {
                defaultAC1A = SunlitAc1.A[0];
                defaultAC1Water = SunlitAc1.WaterUse[0];
                defaultAJA = SunlitAj.A[0];
                defaultAJWater = SunlitAj.WaterUse[0];
            }

            // Ben Ababaei (2019.10.14): If NONE of the As is > 0.5, don't get inside the following loop.
            // If there is sufficient photosynthetic rate
            if (SunlitAc1.A[0] > 0.5 && SunlitAj.A[0] > 0.5)
            {
                for (int n = 0; n < 3; n++)
                {
                    results[0] = SunlitAc1.CalcPhotosynthesis(this, useAirTemp, 0, SunlitAc1.LeafTemp[0], mode, maxHourlyT, sunlitFraction);
                    results[1] = SunlitAj.CalcPhotosynthesis(this, useAirTemp, 0, SunlitAj.LeafTemp[0], mode, maxHourlyT, sunlitFraction);

                    // If any of the above is < 0, set both values zero (for each type of leaf separately).
                    if (results[0] || results[1])
                    {
                        SunlitAc1.A[0] = defaultAC1A;
                        SunlitAc1.WaterUse[0] = defaultAC1Water;
                        SunlitAj.A[0] = defaultAJA;
                        SunlitAj.WaterUse[0] = defaultAC1Water;

                        break;
                    }
                }
            }

            // SHADED CALCULATIONS
            if (results[2] || results[3])
            {
                ShadedAc1.A[0] = 0.0;
                ShadedAc1.WaterUse[0] = 0.0;
                ShadedAj.A[0] = 0.0;
                ShadedAj.WaterUse[0] = 0.0;
            }
            else
            {
                defaultAC1A = ShadedAc1.A[0];
                defaultAC1Water = ShadedAc1.WaterUse[0];
                defaultAJA = ShadedAj.A[0];
                defaultAJWater = ShadedAj.WaterUse[0];
            }

            // If there is sufficient photosynthetic rate
            if (ShadedAc1.A[0] > 0.5 && ShadedAj.A[0] > 0.5)
            {
                for (int n = 0; n < 3; n++)
                {
                    results[2] = ShadedAc1.CalcPhotosynthesis(this, useAirTemp, 0, ShadedAc1.LeafTemp[0], mode, maxHourlyT, shadedFraction);
                    results[3] = ShadedAj.CalcPhotosynthesis(this, useAirTemp, 0, ShadedAj.LeafTemp[0], mode, maxHourlyT, shadedFraction);

                    // If any of the above is < 0, set both values zero (for each type of leaf separately).
                    if (results[2] || results[3])
                    {
                        ShadedAc1.A[0] = defaultAC1A;
                        ShadedAc1.WaterUse[0] = defaultAC1Water;
                        ShadedAj.A[0] = defaultAJA;
                        ShadedAj.WaterUse[0] = defaultAJWater;
                    }
                }
            }

            if (sendNotification && NotifyFinish != null)
            {
                NotifyFinish();
            }
        }

        public void ZeroVariables()
        {
            SunlitAc1.A[0] = 0;
            SunlitAj.A[0] = 0;
            ShadedAc1.A[0] = 0;
            ShadedAj.A[0] = 0;

            SunlitAc1.WaterUse[0] = 0;
            SunlitAj.WaterUse[0] = 0;
            ShadedAc1.WaterUse[0] = 0;
            ShadedAj.WaterUse[0] = 0;
        }

    }
}
