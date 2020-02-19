using DCAPST.Canopy;
using DCAPST.Interfaces;
using DCAPST;

namespace Validation.CCM
{
    public static class Initialise
    {        
        public static CanopyParameters NewWheat()
        {
            double PsiFactor = 1.0;

            var j = new LeafTemperatureParameters()
            {
                TMin = 0.0,
                TOpt = 30.0,
                TMax = 45.0,
                C = 0.911017958600129,
                Beta = 1.0
            };

            var g = new LeafTemperatureParameters()
            {
                TMin = 0.0,
                TOpt = 29.2338417788683,
                TMax = 45.0,
                C = 0.875790608584141,
                Beta = 1.0
            };

            var rubiscoCarboxylation = new TemperatureResponseValues()
            {
                At25 = 273.422964228666,
                Factor = 93720.0
            };

            var rubiscoOxygenation = new TemperatureResponseValues()
            {
                At25 = 165824.064155384,
                Factor = 33600.0
            };

            var rubiscoCarboxylationToOxygenation = new TemperatureResponseValues()
            {
                At25 = 4.59217066521612,
                Factor = 35713.1987127717
            };

            var pepc = new TemperatureResponseValues()
            {
                At25 = 75,
                Factor = 36300
            };

            var rubiscoActivity = new TemperatureResponseValues()
            {
                Factor = 65330.0
            };

            var respiration = new TemperatureResponseValues()
            {
                Factor = 46390
            };

            var pepcActivity = new TemperatureResponseValues()
            {
                Factor = 57043.2677590512
            };

            var CPath = new PathwayParameters()
            {
                PEPRegeneration = 400,
                SpectralCorrectionFactor = 0.15,
                PS2ActivityFraction = 0.1,
                BundleSheathConductance = 0.5,

                MaxRubiscoActivitySLNRatio = 1.1 * PsiFactor,
                MaxElectronTransportSLNRatio = 1.9484 * PsiFactor,
                RespirationSLNRatio = 0.0 * PsiFactor,
                MaxPEPcActivitySLNRatio = 0.373684157583268 * PsiFactor,
                MesophyllCO2ConductanceSLNRatio = 0.00412 * PsiFactor,

                ExtraATPCost = 0.75,
                IntercellularToAirCO2Ratio = 0.7,

                RubiscoCarboxylation = rubiscoCarboxylation,
                RubiscoOxygenation = rubiscoOxygenation,
                RubiscoCarboxylationToOxygenation = rubiscoCarboxylationToOxygenation,
                RubiscoActivity = rubiscoActivity,                
                PEPc = pepc,
                PEPcActivity = pepcActivity,
                Respiration = respiration,

                ElectronTransportRateParams = j,
                MesophyllCO2ConductanceParams = g                
            };
            CPath.MesophyllElectronTransportFraction = CPath.ExtraATPCost / (3.0 + CPath.ExtraATPCost);
            CPath.FractionOfCyclicElectronFlow = 0.25 * CPath.ExtraATPCost;
            CPath.ATPProductionElectronTransportFactor = (3.0 - CPath.FractionOfCyclicElectronFlow) / (4.0 * (1.0 - CPath.FractionOfCyclicElectronFlow));

            var canopy = new CanopyParameters()
            {
                Type = CanopyType.CCM,

                Pathway = CPath,

                AirCO2 = 370,
                CurvatureFactor = 0.7,
                DiffusivitySolubilityRatio = 0.047,
                AirO2 = 210000,

                DiffuseExtCoeff = 0.78,
                DiffuseExtCoeffNIR = 0.8,
                DiffuseReflectionCoeff = 0.036,
                DiffuseReflectionCoeffNIR = 0.389,

                LeafAngle = 60,
                LeafScatteringCoeff = 0.15,
                LeafScatteringCoeffNIR = 0.8,
                LeafWidth = 0.05,

                SLNRatioTop = 1.3,
                MinimumN = 14,

                Windspeed = 1.5,
                WindSpeedExtinction = 1.5
            };

            return canopy;
        }
    }
}
