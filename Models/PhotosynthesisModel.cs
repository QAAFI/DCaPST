using System;
using System.Linq;

using DCAPST.Canopy;
using DCAPST.Environment;
using DCAPST.Interfaces;

namespace DCAPST
{
    public class PhotosynthesisModel : IPhotosynthesisModel
    {
        /// <summary>
        /// The solar geometry
        /// </summary>
        private ISolarGeometry Solar { get; set; }

        /// <summary>
        /// The solar radiation
        /// </summary>
        private ISolarRadiation Radiation { get; set; }

        /// <summary>
        /// The environmental temperature
        /// </summary>
        private ITemperature Temperature { get; set; }
        
        /// <summary>
        /// The canopy undergoing photosynthesis
        /// </summary>
        private ITotalCanopy Canopy { get; set; }

        private IPathwayParameters pathway;

        /// <summary>
        /// Biochemical Conversion & Maintenance Respiration
        /// </summary>
        public double B { get; set; } = 0.409;

        public double PotentialBiomass { get; private set; }
        public double ActualBiomass { get; private set; }
        public double WaterDemanded { get; private set; }
        public double WaterSupplied { get; private set; }
        public double InterceptedRadiation { get; private set; }

        private readonly double start = 6.0;
        private readonly double end = 18.0;
        private readonly double timestep = 1.0;
        private int iterations;

        public PhotosynthesisModel(
            ISolarGeometry solar, 
            ISolarRadiation radiation, 
            ITemperature temperature, 
            IPathwayParameters pathway,
            ITotalCanopy canopy)
        {
            Solar = solar;
            Radiation = radiation;
            Temperature = temperature;
            this.pathway = pathway;
            Canopy = canopy;
        }

        public void Initialise(ICanopyParameters canopy)
        {
            Solar.Initialise();

            iterations = (int)Math.Floor(1.0 + ((end - start) / timestep));
        }

        /// <summary>
        /// Calculates the potential and actual biomass growth of a canopy across the span of a day,
        /// as well as the water requirements for both cases.
        /// </summary>
        public void DailyRun(
            double lai,
            double SLN, 
            double soilWater, 
            double RootShootRatio, 
            double MaxHourlyTRate = 100)
        {            
            Canopy.InitialiseDay(lai, SLN);

            // POTENTIAL CALCULATIONS
            // Note: In the potential case, we assume unlimited water and therefore supply = demand
            CalculatePotential(out double intercepted, out double[] assimilations, out double[] sunlitDemand, out double[] shadedDemand);
            var waterDemands = sunlitDemand.Zip(shadedDemand, (x, y) => x + y).ToArray();
            var potential = assimilations.Sum();
            var totalDemand = waterDemands.Sum();

            // ACTUAL CALCULATIONS
            // Limit water to supply available from Apsim
            double maxHourlyT = Math.Min(waterDemands.Max(), MaxHourlyTRate);
            waterDemands = waterDemands.Select(w => w > maxHourlyT ? maxHourlyT : w).ToArray();

            var limitedSupply = CalculateWaterSupplyLimits(soilWater, maxHourlyT, waterDemands);

            var actual = (soilWater > totalDemand) ? potential : CalculateActual(limitedSupply, sunlitDemand, shadedDemand);

            ActualBiomass = actual * 3600 / 1000000 * 44 * B / (1 + RootShootRatio);
            PotentialBiomass = potential * 3600 / 1000000 * 44 * B / (1 + RootShootRatio);
            WaterDemanded = totalDemand;
            WaterSupplied = (soilWater < totalDemand) ? limitedSupply.Sum() : waterDemands.Sum();
            InterceptedRadiation = intercepted;            
        }

        /// <summary>
        /// Attempt to initialise models based on the current time, and test if they are sensible
        /// </summary>
        private bool TryInitiliase(double time)
        {
            Temperature.UpdateAirTemperature(time);
            Radiation.UpdateRadiationValues(time);
            var sunAngle = Solar.SunAngle(time);            
            Canopy.DoSolarAdjustment(sunAngle);

            return IsSensible();
        }

        /// <summary>
        /// Tests if the basic conditions for photosynthesis to occur are met
        /// </summary>
        private bool IsSensible()
        {
            var CPath = Canopy.Canopy;
            var temp = Temperature.AirTemperature;

            bool[] tempConditions = new bool[4]
            {
                temp > pathway.ElectronTransportRateParams.TMax,
                temp < pathway.ElectronTransportRateParams.TMin,
                temp > pathway.MesophyllCO2ConductanceParams.TMax,
                temp < pathway.MesophyllCO2ConductanceParams.TMin
            };

            bool invalidTemp = tempConditions.Any(b => b == true);
            bool invalidRadn = Radiation.Total <= double.Epsilon;

            if (invalidTemp || invalidRadn)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Determine the total potential biomass for the day under ideal conditions
        /// </summary>
        public void CalculatePotential(out double intercepted, out double[] assimilations, out double[] sunlitDemand, out double[] shadedDemand)
        {
            // Water demands
            intercepted = 0.0;
            sunlitDemand = new double[iterations];
            shadedDemand = new double[iterations];
            assimilations = new double[iterations];

            for (int i = 0; i < iterations; i++)
            {
                double time = start + i * timestep;

                // Note: double arrays default value is 0.0, which is the intended case if initialisation fails
                if (!TryInitiliase(time)) continue;

                intercepted += Radiation.Total * Canopy.GetInterceptedRadiation() * 3600;

                DoTimestepUpdate();

                sunlitDemand[i] = Canopy.Sunlit.WaterUse;
                shadedDemand[i] = Canopy.Shaded.WaterUse;
                assimilations[i] = Canopy.Sunlit.CO2AssimilationRate + Canopy.Shaded.CO2AssimilationRate;
            }
        }

        /// <summary>
        /// Determine the total biomass that can be assimilated under the actual conditions 
        /// </summary>
        public double CalculateActual(double[] waterSupply, double[] sunlitDemand, double[] shadedDemand)
        {
            double assimilation = 0.0;
            for (int i = 0; i < iterations; i++)
            {
                double time = start + i * timestep;

                // Note: double array values default to 0.0, which is the intended case if initialisation fails
                if (!TryInitiliase(time)) continue;

                double total = sunlitDemand[i] + shadedDemand[i];
                DoTimestepUpdate(waterSupply[i], sunlitDemand[i] / total, shadedDemand[i] / total);

                assimilation += Canopy.Sunlit.CO2AssimilationRate + Canopy.Shaded.CO2AssimilationRate;
            }
            return assimilation;
        }

        /// <summary>
        /// Updates the model to a new timestep
        /// </summary>
        public void DoTimestepUpdate(double maxHourlyT = -1, double sunFraction = 0, double shadeFraction = 0)
        {
            var Params = new WaterParameters
            {
                maxHourlyT = maxHourlyT,
                limited = false
            };
            if (maxHourlyT != -1) Params.limited = true;

            Canopy.DoTimestepAdjustment(Radiation);

            var heat = Canopy.CalcBoundaryHeatConductance();
            var sunlitHeat = Canopy.CalcSunlitBoundaryHeatConductance();

            Params.BoundaryHeatConductance = sunlitHeat;
            Params.fraction = sunFraction;
            Canopy.Sunlit.DoPhotosynthesis(Temperature, Params);

            Params.BoundaryHeatConductance = heat - sunlitHeat;
            Params.fraction = shadeFraction;
            Canopy.Shaded.DoPhotosynthesis(Temperature, Params);
        }

        /// <summary>
        /// In the case where there is greater water demand than supply allows, the water supply limit for each hour
        /// must be calculated. 
        /// 
        /// This is done by adjusting the maximum rate of water supply each hour, until the total water demand across
        /// the day is within some tolerance of the actual water available, as we want to make use of all the 
        /// accessible water.
        /// </summary>
        private double[] CalculateWaterSupplyLimits(double soilWaterAvail, double maxHourlyT, double[] demand)
        {
            double initialDemand = demand.Sum();
            if (soilWaterAvail < 0.0001) return demand.Select(d => 0.0).ToArray();
            if (initialDemand < soilWaterAvail) return demand;
            
            double maxDemandRate = maxHourlyT;
            double minDemandRate = 0;
            double averageDemandRate = 0;

            double dailyDemand = initialDemand;

            // While the daily demand is outside some tolerance of the available water
            while (dailyDemand < (soilWaterAvail - 0.000001) || (0.000001 + soilWaterAvail) < dailyDemand)
            {
                averageDemandRate = (maxDemandRate + minDemandRate) / 2;

                // Find the total daily demand when the hourly rate is limited to the average rate
                dailyDemand = demand.Select(d => d > averageDemandRate ? averageDemandRate : d).Sum();

                // Find the total daily demand when the hourly rate is limited to the maximum rate
                var maxDemand = demand.Select(d => d > maxDemandRate ? maxDemandRate : d).Sum();

                // If there is more water available than is being demanded, adjust the minimum demand upwards
                if (dailyDemand < soilWaterAvail) minDemandRate = averageDemandRate;
                // Else, there is less water available than is being demanded, so adjust the maximum demand downwards
                else maxDemandRate = averageDemandRate;
            }
            return demand.Select(d => d > averageDemandRate ? averageDemandRate : d).ToArray();
        }
    }

    public struct WaterParameters
    {
        public bool limited;
        public double BoundaryHeatConductance;
        public double maxHourlyT;
        public double fraction;
    }
}
