namespace DCAPST.Interfaces
{
    public interface IPartialCanopy
    {
        /// <summary>
        /// A collection of predefined parameters used by the canopy
        /// </summary>
        ICanopyParameters Canopy { get; }

        /// <summary>
        /// The rates of various parameters at 25 Celsius
        /// </summary>
        ParameterRates At25C { get; }

        /// <summary>
        /// Leaf area index of this region of the canopy
        /// </summary>
        double LAI { get; set; }

        /// <summary>
        /// The energy the canopy absorbs through solar radiation
        /// </summary>
        double AbsorbedRadiation { get; set; }

        /// <summary>
        /// The number of photosynthetic active photons which reach the canopy
        /// </summary>
        double PhotonCount { get; set; }

        /// <summary>
        /// Rate of biomass conversion
        /// </summary>
        double CO2AssimilationRate { get; set; }

        /// <summary>
        /// How much water the canopy consumes
        /// </summary>
        double WaterUse { get; set; }        

        /// <summary>
        /// Runs the photosynthesis calculations for the canopy
        /// </summary>
        void DoPhotosynthesis(ITemperature temperature, WaterParameters Params);
    }

    public class ParameterRates
    {
        /// <summary>
        /// Maximum rubisco activity
        /// </summary>
        public double VcMax { get; set; }

        /// <summary>
        /// Maximum respiration
        /// </summary>
        public double Rd { get; set; }

        /// <summary>
        /// Maximum electron transport rate
        /// </summary>
        public double JMax { get; set; }

        /// <summary>
        /// Maximum PEPc activity
        /// </summary>
        public double VpMax { get; set; }

        /// <summary>
        /// Maximum mesophyll CO2 conductance
        /// </summary>
        public double Gm { get; set; }
    }
}
