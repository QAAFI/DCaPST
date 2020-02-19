using DCAPST.Interfaces;

namespace DCAPST
{
    /// <summary>
    /// Defines the pathway functions for a C4 canopy
    /// </summary>
    public class AssimilationC4 : Assimilation
    {
        public AssimilationC4(IPartialCanopy partial, ITemperature temperature) : base(partial, temperature)
        { }

        protected override void UpdateIntercellularCO2(AssimilationPathway pathway, double gt, double waterUseMolsSecond)
        {
            pathway.IntercellularCO2 = ((gt - waterUseMolsSecond / 2.0) * canopy.AirCO2 - pathway.CO2Rate) / (gt + waterUseMolsSecond / 2.0);
        }

        protected override void UpdateMesophyllCO2(AssimilationPathway pathway)
        {
            pathway.MesophyllCO2 = pathway.IntercellularCO2 - pathway.CO2Rate / pathway.Leaf.GmT;
        }

        protected override AssimilationFunction GetAc1Function(AssimilationPathway pathway)
        {
            var x = new double[9];

            x[0] = pathway.Leaf.VcMaxT;
            x[1] = pathway.Leaf.Kc / pathway.Leaf.Ko;
            x[2] = pathway.Leaf.Kc;
            x[3] = pathway.Leaf.VpMaxT / (pathway.MesophyllCO2 + pathway.Leaf.Kp);
            x[4] = 0.0;
            x[5] = 1.0;
            x[6] = 0.0;
            x[7] = 1.0;
            x[8] = 1.0;

            var func = new AssimilationFunction()
            {
                X = x,

                MesophyllRespiration = pathway.Leaf.GmRd,
                HalfRubiscoSpecificityReciprocal = pathway.Leaf.Gamma,
                FractionOfDiffusivitySolubilityRatio = 0.1 / canopy.DiffusivitySolubilityRatio,
                BundleSheathConductance = Gbs,
                Oxygen = canopy.AirO2,
                Respiration = pathway.Leaf.RdT
            };

            return func;
        }

        protected override AssimilationFunction GetAc2Function(AssimilationPathway pathway)
        {
            var x = new double[9];

            x[0] = pathway.Leaf.VcMaxT;
            x[1] = pathway.Leaf.Kc / pathway.Leaf.Ko;
            x[2] = pathway.Leaf.Kc;
            x[3] = 0.0;
            x[4] = Vpr;
            x[5] = 1.0;
            x[6] = 0.0;
            x[7] = 1.0;
            x[8] = 1.0;

            var func = new AssimilationFunction()
            {
                X = x,

                MesophyllRespiration = pathway.Leaf.GmRd,
                HalfRubiscoSpecificityReciprocal = pathway.Leaf.Gamma,
                FractionOfDiffusivitySolubilityRatio = 0.1 / canopy.DiffusivitySolubilityRatio,
                BundleSheathConductance = Gbs,
                Oxygen = canopy.AirO2,
                Respiration = pathway.Leaf.RdT
            };

            return func;
        }

        protected override AssimilationFunction GetAjFunction(AssimilationPathway pathway)
        {
            var x = new double[9];

            x[0] = (1.0 - pway.MesophyllElectronTransportFraction) * pathway.Leaf.J / 3.0;
            x[1] = 7.0 / 3.0 * pathway.Leaf.Gamma;
            x[2] = 0.0;
            x[3] = 0.0;
            x[4] = pway.MesophyllElectronTransportFraction * pathway.Leaf.J / pway.ExtraATPCost;
            x[5] = 1.0;
            x[6] = 0.0;
            x[7] = 1.0;
            x[8] = 1.0;

            var func = new AssimilationFunction()
            {
                X = x,

                MesophyllRespiration = pathway.Leaf.GmRd,
                HalfRubiscoSpecificityReciprocal = pathway.Leaf.Gamma,
                FractionOfDiffusivitySolubilityRatio = 0.1 / canopy.DiffusivitySolubilityRatio,
                BundleSheathConductance = Gbs,
                Oxygen = canopy.AirO2,
                Respiration = pathway.Leaf.RdT
            };

            return func;
        }
    }
}
