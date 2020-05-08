using DCAPST.Canopy;
using DCAPST.Interfaces;
using DCAPST;

namespace Validation.CCM
{
    public static class WheatCCM
    {        
        public static void SetCanopy(CanopyParameters c)
        {
            c.Type = CanopyType.CCM;

            c.AirCO2 = 370;
            c.CurvatureFactor = 0.7;
            c.DiffusivitySolubilityRatio = 0.047;
            c.AirO2 = 210000;

            c.DiffuseExtCoeff = 0.78;
            c.DiffuseExtCoeffNIR = 0.8;
            c.DiffuseReflectionCoeff = 0.036;
            c.DiffuseReflectionCoeffNIR = 0.389;

            c.LeafAngle = 60;
            c.LeafScatteringCoeff = 0.15;
            c.LeafScatteringCoeffNIR = 0.8;
            c.LeafWidth = 0.05;

            c.SLNRatioTop = 1.3;
            c.MinimumN = 14;

            c.Windspeed = 1.5;
            c.WindSpeedExtinction = 1.5;
        }

        public static void SetPathway(PathwayParameters p)
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
                At25 = 17.52 * 273.422964228666,
                Factor = 93720.0
            };

            var rubiscoOxygenation = new TemperatureResponseValues()
            {
                At25 = 1.34 * 165824.064155384,
                Factor = 33600.0
            };

            var rubiscoCarboxylationToOxygenation = new TemperatureResponseValues()
            {
                At25 = 13.07 * 4.59217066521612,
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

            p.PEPRegeneration = 400;
            p.SpectralCorrectionFactor = 0.15;
            p.PS2ActivityFraction = 0.1;
            p.BundleSheathConductance = 0.003;

            p.MaxRubiscoActivitySLNRatio = 1.1 * PsiFactor;
            p.MaxElectronTransportSLNRatio = 1.9484 * PsiFactor;
            p.RespirationSLNRatio = 0.0 * PsiFactor;
            p.MaxPEPcActivitySLNRatio = 1.0 * PsiFactor;
            p.MesophyllCO2ConductanceSLNRatio = 0.00412 * PsiFactor;

            p.ExtraATPCost = 0.75;
            p.IntercellularToAirCO2Ratio = 0.7;

            p.RubiscoCarboxylation = rubiscoCarboxylation;
            p.RubiscoOxygenation = rubiscoOxygenation;
            p.RubiscoCarboxylationToOxygenation = rubiscoCarboxylationToOxygenation;
            p.RubiscoActivity = rubiscoActivity;
            p.PEPc = pepc;
            p.PEPcActivity = pepcActivity;
            p.Respiration = respiration;

            p.ElectronTransportRateParams = j;
            p.MesophyllCO2ConductanceParams = g;

            p.MesophyllElectronTransportFraction = p.ExtraATPCost / (3.0 + p.ExtraATPCost);
            p.FractionOfCyclicElectronFlow = 0.25 * p.ExtraATPCost;
            p.ATPProductionElectronTransportFactor = (3.0 - p.FractionOfCyclicElectronFlow) / (4.0 * (1.0 - p.FractionOfCyclicElectronFlow));

        }
    }
}
