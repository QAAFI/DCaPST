using System;
using DCAPST.Interfaces;

namespace DCAPST.Environment
{
    /// <summary>
    /// Models how temperature impacts the water used by the leaf during photosynthesis
    /// </summary>
    public class LeafWaterInteractionModel : ILeafWaterInteraction
    {
        #region Constants
        /// <summary>
        /// Boltzmann's constant
        /// </summary>
        private double kb = 0.0000000567;

        /// <summary>
        /// Volumetric heat capacity of air
        /// </summary>
        private double sAir = 1200;

        /// <summary>
        /// Psychrometric constant
        /// </summary>
        private double g = 0.066;

        private double latentHeatOfVapourisation = 2447000;
        #endregion

        /// <summary> Environment temperature model </summary>
        private readonly ITemperature temp;

        /// <summary> Current leaf temperature </summary>
        private double leafTemp;

        /// <summary> Canopy boundary heat conductance</summary>
        private double gbh;

        /// <summary> Boundary H20 conductance </summary>
        private double Gbw => gbh / 0.92;

        /// <summary> Boundary heat resistance </summary>
        private double Rbh => 1 / gbh;

        /// <summary> Boundary CO2 conductance </summary>
        private double GbCO2 => temp.AtmosphericPressure * temp.AirMolarDensity * Gbw / 1.37;

        /// <summary> Outgoing thermal radiation</summary>
        private double ThermalRadiation => 8 * kb * Math.Pow(temp.AirTemperature + 273, 3) * (leafTemp - temp.AirTemperature);
                
        /// <summary> Vapour pressure at the leaf temperature </summary>
        private double VpLeaf => 0.61365 * Math.Exp(17.502 * leafTemp / (240.97 + leafTemp));

        /// <summary> Vapour pressure at the air temperature</summary>
        private double VpAir => 0.61365 * Math.Exp(17.502 * temp.AirTemperature / (240.97 + temp.AirTemperature));

        /// <summary> Vapour pressure at one degree above air temperature</summary>
        private double VpAir1 => 0.61365 * Math.Exp(17.502 * (temp.AirTemperature + 1) / (240.97 + (temp.AirTemperature + 1)));

        /// <summary> Vapour pressure at the daily minimum temperature</summary>
        private double VptMin => 0.61365 * Math.Exp(17.502 * temp.MinTemperature / (240.97 + temp.MinTemperature));

        /// <summary> Difference in air vapour pressures </summary>
        private double DeltaAirVP => VpAir1 - VpAir;

        /// <summary> Leaf to air vapour pressure deficit </summary>
        private double vpd => VpLeaf - VptMin;

        public LeafWaterInteractionModel(ITemperature temperature)
        {
            temp = temperature ?? throw new Exception("The temperature model cannot be null");
        }

        /// <summary>
        /// Sets conditions for the water interaction
        /// </summary>
        /// <param name="leafTemp">Leaf temperature</param>
        /// <param name="gbh">Boundary heat conductance</param>
        public void SetConditions(double leafTemp, double gbh)
        {
            this.leafTemp = leafTemp;
            this.gbh = (gbh != 0) ? gbh : throw new Exception("Gbh cannot be 0");
        }
        
        /// <summary>
        /// Calculates the leaf resistance to water when the supply is unlimited
        /// </summary>
        /// <param name="A">CO2 assimilation rate</param>
        /// <param name="Ca">Air CO2 partial pressure</param>
        /// <param name="Ci">Intercellular CO2 partial pressure</param>
        public double UnlimitedWaterResistance(double A, double Ca, double Ci)
        {
            // Leaf water mol fraction
            double Wl = VpLeaf / (temp.AtmosphericPressure * 100) * 1000;
            // Air water mol fraction
            double Wa = VptMin / (temp.AtmosphericPressure * 100) * 1000;

            // Boundary CO2 Resistance
            double a = 1 / GbCO2;

            // dummy variables
            double b = (Wl - Wa) / (1000 - (Wl + Wa) / 2) * (Ca + Ci) / 2;
            double c = A;
            double d = Ca - Ci;

            // Boundary water diffusion factor
            double m = 1.37;
            // Stomata water diffusion factor
            double n = 1.6;

            // dummy variables
            double e = c * a * m + c * a * n + b * m * n - d * m;
            double g = c * m * (c * Math.Pow(a, 2) * n + a * b * m * n - a * d * n);
            double h = m * A;
            
            // Stomatal CO2 conductance
            double gsCO2 = 2 * h / (Math.Pow((Math.Pow(e, 2) - 4 * g), 0.5) - e);
            // Total leaf water conductance
            double Gtw = 1 / (1 / (m * GbCO2) + 1 / (n * gsCO2));
            
            double rtw = temp.AirMolarDensity / Gtw * temp.AtmosphericPressure;
            return rtw;
        }

        /// <summary>
        /// Calculates the leaf resistance to water when supply is limited
        /// </summary>
        public double LimitedWaterResistance(double availableWater, double Rn)
        {
            // Transpiration in kilos of water per second
            double ekg = latentHeatOfVapourisation * availableWater / 3600;
            double rtw = (DeltaAirVP * Rbh * (Rn - ThermalRadiation - ekg) + vpd * sAir) / (ekg * g);
            return rtw;
        }

        /// <summary>
        /// Calculates the hourly water requirements
        /// </summary>
        /// <param name="rtw">Resistance to water</param>
        /// <param name="rn">Radiation</param>
        public double HourlyWaterUse(double rtw, double rn)
        {
            // TODO: Make this work with the timestep model

            // dummy variables
            double a_lump = DeltaAirVP * (rn - ThermalRadiation) + vpd * sAir / Rbh;
            double b_lump = DeltaAirVP + g * rtw / Rbh;
            double latentHeatLoss = a_lump / b_lump;

            return (latentHeatLoss / latentHeatOfVapourisation) * 3600;
        }

        /// <summary>
        /// Calculates the total CO2 conductance across the leaf
        /// </summary>
        /// <param name="rtw">Resistance to water</param>
        public double TotalCO2Conductance(double rtw)
        {
            // Limited water gsCO2
            var gsCO2 = temp.AirMolarDensity * (temp.AtmosphericPressure / (rtw - (1 / Gbw))) / 1.6;
            var boundaryCO2Resistance = 1 / GbCO2;
            var stomatalCO2Resistance = 1 / gsCO2;
            return 1 / (boundaryCO2Resistance + stomatalCO2Resistance);
        }

        /// <summary>
        /// Finds the leaf temperature after the water interaction
        /// </summary>
        /// <param name="rtw">Resistance to water</param>
        public double LeafTemperature(double rtw, double rn)
        {
            // dummy variables
            double a = g * (rn - ThermalRadiation) * rtw / sAir - vpd;
            double d = DeltaAirVP + g * rtw / Rbh;

            double deltaT = a / d;

            return temp.AirTemperature + deltaT;
        }
    }
}
