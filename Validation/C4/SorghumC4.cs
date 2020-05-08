using DCAPST.Interfaces;
using DCAPST;

namespace Validation.C4
{
    public static class SorghumC4
    {
        public static void SetCanopy(CanopyParameters c)
        {
            c.Type = CanopyType.C4;

            c.AirCO2 = 363;
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
            c.LeafWidth = 0.15;

            c.SLNRatioTop = 1.3;
            c.MinimumN = 14;

            c.Windspeed = 1.5;
            c.WindSpeedExtinction = 1.5;
        }

        public static void SetPathway(PathwayParameters p)
        {
            double PsiFactor = 0.4;

            var j = new LeafTemperatureParameters()
            {
                TMin = 0,
                TOpt = 37.8649150880407,
                TMax = 55,
                C = 0.711229539802063,
                Beta = 1
            };

            var g = new LeafTemperatureParameters()
            {
                TMin = 0,
                TOpt = 42,
                TMax = 55,
                C = 0.462820450976839,
                Beta = 1,
            };

            var rubiscoCarboxylation = new TemperatureResponseValues()
            {
                At25 = 1210,
                Factor = 64200
            };

            var rubiscoOxygenation = new TemperatureResponseValues()
            {
                At25 = 292000,
                Factor = 10500
            };

            var rubiscoCarboxylationToOxygenation = new TemperatureResponseValues()
            {
                At25 = 5.51328906454566,
                Factor = 21265.4029552906
            };

            var pepc = new TemperatureResponseValues()
            {
                At25 = 75,
                Factor = 36300
            };

            var rubiscoActivity = new TemperatureResponseValues()
            {
                Factor = 78000
            };

            var respiration = new TemperatureResponseValues()
            {
                Factor = 46390
            };

            var pepcActivity = new TemperatureResponseValues()
            {
                Factor = 57043.2677590512
            };

            p.PEPRegeneration = 120;
            p.SpectralCorrectionFactor = 0.15;
            p.PS2ActivityFraction = 0.1;
            p.BundleSheathConductance = 0.003;

            p.MaxRubiscoActivitySLNRatio = 0.465 * PsiFactor;
            p.MaxElectronTransportSLNRatio = 2.7 * PsiFactor;
            p.RespirationSLNRatio = 0.0 * PsiFactor;
            p.MaxPEPcActivitySLNRatio = 1.55 * PsiFactor;
            p.MesophyllCO2ConductanceSLNRatio = 0.0135 * PsiFactor;

            p.ExtraATPCost = 2;
            p.MesophyllElectronTransportFraction = 0.4;
            p.IntercellularToAirCO2Ratio = 0.45;

            p.RubiscoCarboxylation = rubiscoCarboxylation;
            p.RubiscoOxygenation = rubiscoOxygenation;
            p.RubiscoCarboxylationToOxygenation = rubiscoCarboxylationToOxygenation;
            p.PEPc = pepc;
            p.RubiscoActivity = rubiscoActivity;
            p.Respiration = respiration;
            p.PEPcActivity = pepcActivity;

            p.ElectronTransportRateParams = j;
            p.MesophyllCO2ConductanceParams = g;
        }
    }
}
