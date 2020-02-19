namespace DCAPST.Interfaces
{
    
    public interface IAssimilation
    {
        /// <summary>
        /// A leaf water interaction model
        /// </summary>
        ILeafWaterInteraction LeafWater { get; }

        /// <summary>
        /// Attempts to calculate possible changes to the assimilation value under current conditions.
        /// </summary>
        void UpdateAssimilation(WaterParameters Params);        

        /// <summary>
        /// Gets the rate of CO2 assimilation
        /// </summary>
        double GetCO2Rate();

        /// <summary>
        /// Gets the water used by the CO2 assimilation
        /// </summary>
        double GetWaterUse();
    }
}
