namespace DCAPST.Interfaces
{
    public interface ILeafWaterInteraction
    {
        /// <summary>
        /// Sets the current conditions of the leaf water model
        /// </summary>
        void SetConditions(double temperature, double gbs);

        /// <summary>
        /// Calculates the resistance to water when supply is unlimited
        /// </summary>
        double UnlimitedWaterResistance(double Assimilation, double AirCO2, double IntercellularCO2);

        /// <summary>
        /// Calculates the resistance to water when supply is limited
        /// </summary>
        double LimitedWaterResistance(double wateruse, double radiation);

        /// <summary>
        /// Finds the total leaf CO2 conductance at a given resistance to water
        /// </summary>
        double TotalCO2Conductance(double resistance);

        /// <summary>
        /// Finds the leaf temperature as a result of the water interaction
        /// </summary>
        double LeafTemperature(double resistance, double radiation);

        /// <summary>
        /// Finds the water demand of a leaf canopy across an hour
        /// </summary>
        double HourlyWaterUse(double resistance, double radiation);
    }
}
