using System;
using System.Collections.Generic;
using System.Linq;
using DCAPST.Environment;
using DCAPST.Interfaces;

namespace DCAPST
{
    /// <summary>
    /// Tracks the state of an assimilation type
    /// </summary>
    public abstract class Assimilation : IAssimilation
    {
        /// <summary>
        /// The part of the canopy undergoing CO2 assimilation
        /// </summary>
        protected IPartialCanopy partial;

        /// <summary>
        /// The parameters describing the canopy
        /// </summary>
        protected ICanopyParameters canopy;

        /// <summary>
        /// The parameters describing the pathways
        /// </summary>
        protected IPathwayParameters pway;

        /// <summary>
        /// Models the leaf water interaction
        /// </summary>
        public ILeafWaterInteraction LeafWater { get; }

        /// <summary>
        /// The possible assimilation pathways
        /// </summary>
        protected List<AssimilationPathway> pathways;        

        /// <summary>
        /// Bundle sheath conductance
        /// </summary>
        public double Gbs => pway.BundleSheathConductance * partial.LAI;
        
        /// <summary>
        /// PEP regeneration
        /// </summary>
        public double Vpr => pway.PEPRegeneration * partial.LAI;

        public Assimilation(IPartialCanopy partial, ITemperature temperature)
        {
            this.partial = partial;
            canopy = partial.Canopy;
            pway = partial.Canopy.Pathway;

            pathways = new List<AssimilationPathway>()
            {
                /*Ac1*/ new AssimilationPathway(partial) { Type = PathwayType.Ac1 },
                /*Ac2*/ this is AssimilationC3 ? null : new AssimilationPathway(partial) { Type = PathwayType.Ac2 },
                /*Aj */ new AssimilationPathway(partial) { Type = PathwayType.Aj }
            };
            pathways.ForEach(p => p.Leaf.Temperature = temperature.AirTemperature);

            LeafWater = new LeafWaterInteractionModel(temperature);
        }
        
        /// <summary>
        /// Recalculates the assimilation values for each pathway
        /// </summary>
        public void UpdateAssimilation(WaterParameters water) => pathways.ForEach(p => UpdatePathway(water, p));        

        /// <summary>
        /// Finds the CO2 assimilation rate
        /// </summary>
        public double GetCO2Rate() => pathways.Min(p => p.CO2Rate);

        /// <summary>
        /// Finds the water used during CO2 assimilation
        /// </summary>
        public double GetWaterUse() => pathways.Min(p => p.WaterUse);

        /// <summary>
        /// Updates the state of an assimilation pathway
        /// </summary>
        private void UpdatePathway(WaterParameters water, AssimilationPathway pathway)
        {
            if (pathway == null) return;

            LeafWater.SetConditions(pathway.Leaf.Temperature, water.BoundaryHeatConductance);

            double resistance;

            var func = GetFunction(pathway);
            if (!water.limited) /* Unlimited water calculation */
            {
                pathway.IntercellularCO2 = pway.IntercellularToAirCO2Ratio * canopy.AirCO2;

                func.Ci = pathway.IntercellularCO2;
                func.Rm = 1 / pathway.Leaf.GmT;

                pathway.CO2Rate = func.Value();

                resistance = LeafWater.UnlimitedWaterResistance(pathway.CO2Rate, canopy.AirCO2, pathway.IntercellularCO2);
                pathway.WaterUse = LeafWater.HourlyWaterUse(resistance, partial.AbsorbedRadiation);
            }
            else /* Limited water calculation */
            {
                pathway.WaterUse = water.maxHourlyT * water.fraction;
                var WaterUseMolsSecond = pathway.WaterUse / 18 * 1000 / 3600;

                resistance = LeafWater.LimitedWaterResistance(pathway.WaterUse, partial.AbsorbedRadiation);
                var Gt = LeafWater.TotalCO2Conductance(resistance);

                func.Ci = canopy.AirCO2 - WaterUseMolsSecond * canopy.AirCO2 / (Gt + WaterUseMolsSecond / 2.0);
                func.Rm = 1 / (Gt + WaterUseMolsSecond / 2) + 1.0 / pathway.Leaf.GmT;

                pathway.CO2Rate = func.Value();

                UpdateIntercellularCO2(pathway, Gt, WaterUseMolsSecond);
            }

            UpdateMesophyllCO2(pathway);
            UpdateChloroplasticO2(pathway);
            UpdateChloroplasticCO2(pathway, func);

            // New leaf temperature
            pathway.Leaf.Temperature = (LeafWater.LeafTemperature(resistance, partial.AbsorbedRadiation) + pathway.Leaf.Temperature) / 2.0;

            // If the assimilation is not sensible zero the values
            if (double.IsNaN(pathway.CO2Rate) || pathway.CO2Rate <= 0.0 || double.IsNaN(pathway.WaterUse) || pathway.WaterUse <= 0.0)
            {
                pathway.CO2Rate = 0;
                pathway.WaterUse = 0;
            }
        }
        
        /// <summary>
        /// Factory method for accessing the different possible terms for assimilation
        /// </summary>
        private AssimilationFunction GetFunction(AssimilationPathway pathway)
        {
            if (pathway.Type == PathwayType.Ac1) return GetAc1Function(pathway);
            else if (pathway.Type == PathwayType.Ac2) return GetAc2Function(pathway);
            else return GetAjFunction(pathway);
        }

        /// <summary>
        /// Updates the intercellular CO2 parameter
        /// </summary>
        protected virtual void UpdateIntercellularCO2(AssimilationPathway pathway, double gt, double waterUseMolsSecond) 
        { /*C4 & CCM overwrite this.*/ }

        /// <summary>
        /// Updates the mesophyll CO2 parameter
        /// </summary>
        protected virtual void UpdateMesophyllCO2(AssimilationPathway pathway) 
        { /*C4 & CCM overwrite this.*/ }

        /// <summary>
        /// Updates the chloroplastic O2 parameter
        /// </summary>
        protected virtual void UpdateChloroplasticO2(AssimilationPathway pathway) 
        { /*CCM overwrites this.*/ }

        /// <summary>
        /// Updates the chloroplastic CO2 parameter
        /// </summary>
        protected virtual void UpdateChloroplasticCO2(AssimilationPathway pathway, AssimilationFunction func) 
        { /*CCM overwrites this.*/ }

        /// <summary>
        /// Retrieves a function describing assimilation along the Ac1 pathway
        /// </summary>
        protected abstract AssimilationFunction GetAc1Function(AssimilationPathway pathway);

        /// <summary>
        /// Retrieves a function describing assimilation along the Ac2 pathway
        /// </summary>
        protected abstract AssimilationFunction GetAc2Function(AssimilationPathway pathway);

        /// <summary>
        /// Retrieves a function describing assimilation along the Aj pathway
        /// </summary>
        protected abstract AssimilationFunction GetAjFunction(AssimilationPathway pathway);
    }
}
