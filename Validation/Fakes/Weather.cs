using DCAPST.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Startup.Models
{
    public class Weather : IWeather
    {
        public DateTime StartDate { get; }

        /// <summary>Gets the end date of the weather file.</summary>
        public DateTime EndDate { get; }

        /// <summary>Gets or sets the maximum temperature (oc)</summary>
        public double MaxT { get; set; }

        /// <summary>Gets or sets the minimum temperature (oc)</summary>
        public double MinT { get; set; }

        /// <summary>Mean temperature  /// </summary>
        public double MeanT { get; set; }

        /// <summary>Daily mean VPD  /// </summary>
        public double VPD { get; }

        /// <summary>Gets or sets the rainfall (mm)</summary>
        public double Rain { get; }

        /// <summary>Gets or sets the solar radiation. MJ/m2/day</summary>
        public double Radn { get; set; }

        /// <summary>Gets or sets the vapor pressure</summary>
        public double VP { get; }

        /// <summary>
        /// Gets or sets the wind value found in weather file or zero if not specified.
        /// </summary>
        public double Wind { get; }

        /// <summary>
        /// Gets or sets the CO2 level. If not specified in the weather file the default is 350.
        /// </summary>
        public double CO2 { get; }

        /// <summary>
        /// Gets or sets the atmospheric air pressure. If not specified in the weather file the default is 1010 hPa.
        /// </summary>
        public double AirPressure { get; }

        /// <summary>Gets the latitude</summary>
        public double Latitude { get; set; }

        /// <summary>Gets the average temperature</summary>
        public double Tav { get; }

        /// <summary>Gets the temperature amplitude.</summary>
        public double Amp { get; }

        /// <summary>
        /// Gets the duration of the day in hours.
        /// </summary>
        public double CalculateDayLength(double Twilight) { return 0; }
    }
}
