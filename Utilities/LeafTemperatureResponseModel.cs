using System;
using DCAPST.Interfaces;

namespace DCAPST
{
    /// <summary>
    /// Models the parameters of the leaf necessary to calculate photosynthesis
    /// </summary>
    public class LeafTemperatureResponseModel
    {
        /// <summary>
        /// The canopy which is calculating leaf temperature properties
        /// </summary>
        private IPartialCanopy partial;

        /// <summary>
        /// A collection of parameters as valued at 25 degrees Celsius
        /// </summary>
        private ParameterRates rateAt25;

        /// <summary>
        /// The parameters describing the canopy
        /// </summary>
        private ICanopyParameters canopy;

        /// <summary>
        /// The static parameters describing the assimilation pathway
        /// </summary>
        private IPathwayParameters pathway;

        public LeafTemperatureResponseModel(IPartialCanopy partial)
        {
            this.partial = partial;
            rateAt25 = partial.At25C;
            canopy = partial.Canopy;
            pathway = canopy.Pathway;
        }

        /// <summary>
        /// The current leaf temperature
        /// </summary>
        public double Temperature { get; set; } = 0;

        /// <summary>
        /// Maximum rate of rubisco carboxylation at the current leaf temperature (micro mol CO2 m^-2 ground s^-1)
        /// </summary>
        public double VcMaxT => Value(Temperature, rateAt25.VcMax, pathway.RubiscoActivity.Factor);

        /// <summary>
        /// Leaf respiration at the current leaf temperature (micro mol CO2 m^-2 ground s^-1)
        /// </summary>
        public double RdT => Value(Temperature, rateAt25.Rd, pathway.Respiration.Factor);

        /// <summary>
        /// Maximum rate of electron transport at the current leaf temperature (micro mol CO2 m^-2 ground s^-1)
        /// </summary>
        public double JMaxT => ValueOptimum(Temperature, rateAt25.JMax, pathway.ElectronTransportRateParams);

        /// <summary>
        /// Maximum PEP carboxylase activity at the current leaf temperature (micro mol CO2 m^-2 ground s^-1)
        /// </summary>
        public double VpMaxT => Value(Temperature, rateAt25.VpMax, pathway.PEPcActivity.Factor);

        /// <summary>
        /// Mesophyll conductance at the current leaf temperature (mol CO2 m^-2 ground s^-1 bar^-1)
        /// </summary>
        public double GmT => ValueOptimum(Temperature, rateAt25.Gm, pathway.MesophyllCO2ConductanceParams);

        /// <summary>
        /// Michaelis-Menten constant of Rubsico for CO2 (microbar)
        /// </summary>
        public double Kc => Value(Temperature, pathway.RubiscoCarboxylation.At25, pathway.RubiscoCarboxylation.Factor);

        /// <summary>
        /// Michaelis-Menten constant of Rubsico for O2 (microbar)
        /// </summary>
        public double Ko => Value(Temperature, pathway.RubiscoOxygenation.At25, pathway.RubiscoOxygenation.Factor);

        /// <summary>
        /// Ratio of Rubisco carboxylation to Rubisco oxygenation
        /// </summary>
        public double VcVo => Value(Temperature, pathway.RubiscoCarboxylationToOxygenation.At25, pathway.RubiscoCarboxylationToOxygenation.Factor);

        /// <summary>
        /// Michaelis-Menten constant of PEP carboxylase for CO2 (micro bar)
        /// </summary>
        public double Kp => Value(Temperature, pathway.PEPc.At25, pathway.PEPc.Factor);

        /// <summary>
        /// Electron transport rate
        /// </summary>
        public double J => CalcElectronTransportRate();

        /// <summary>
        /// Relative CO2/O2 specificity of Rubisco (bar bar^-1)
        /// </summary>
        public double Sco => Ko / Kc * VcVo;

        /// <summary>
        /// Half the reciprocal of the relative rubisco specificity
        /// </summary>
        public double Gamma => 0.5 / Sco;

        /// <summary>
        /// Mesophyll respiration
        /// </summary>
        public double GmRd => RdT * 0.5;        

        /// <summary>
        /// Uses an exponential function to model temperature response parameters
        /// </summary>
        /// <remarks>
        /// See equation (1), A. Wu et al (2018) for details
        /// </remarks>
        private double Value(double temp, double P25, double tMin)
        {
            return P25 * Math.Exp(tMin * (temp + 273 - 298.15) / (298.15 * 8.314 * (temp + 273)));
        }

        /// <summary>
        /// Uses a normal distribution to model parameters with an apparent optimum in temperature response
        /// </summary>
        /// /// <remarks>
        /// See equation (2), A. Wu et al (2018) for details
        /// </remarks>
        private double ValueOptimum(double temp, double P25, LeafTemperatureParameters p)
        {
            double alpha = Math.Log(2) / (Math.Log((p.TMax - p.TMin) / (p.TOpt - p.TMin)));
            double numerator = 2 * Math.Pow((temp - p.TMin), alpha) * Math.Pow((p.TOpt - p.TMin), alpha) - Math.Pow((temp - p.TMin), 2 * alpha);
            double denominator = Math.Pow((p.TOpt - p.TMin), 2 * alpha);
            double funcT = P25 * Math.Pow(numerator / denominator, p.Beta) / p.C;

            return funcT;
        }

        /// <summary>
        /// Calculates the electron transport rate of the leaf
        /// </summary>
        private double CalcElectronTransportRate()
        {
            var factor = partial.PhotonCount * (1.0 - pathway.SpectralCorrectionFactor) / 2.0;
            return (factor + JMaxT - Math.Pow(Math.Pow(factor + JMaxT, 2) - 4 * canopy.CurvatureFactor * JMaxT * factor, 0.5))
            / (2 * canopy.CurvatureFactor);
        }
    }

    /// <summary>
    /// Describes parameters used in leaf temperature calculations
    /// </summary>
    public struct LeafTemperatureParameters
    {
        /// <summary>
        /// 
        /// </summary>
        public double C;

        /// <summary>
        /// The maximum temperature
        /// </summary>
        public double TMax;
        
        /// <summary>
        /// The minimum temperature
        /// </summary>
        public double TMin;
        
        /// <summary>
        /// The optimum temperature
        /// </summary>
        public double TOpt;

        /// <summary>
        /// 
        /// </summary>
        public double Beta;
    }
}
