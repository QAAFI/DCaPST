using System;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

using DCAPST;
using DCAPST.Environment;
using DCAPST.Interfaces;

namespace Validation.C4
{
    [TestFixture]
    public class BW5_GxE
    {
        private IServiceProvider provider;
        private double delta = 0.0000000000001;

        [SetUp]
        public void SetUp()
        {
            provider = Services.Register();
        }

        [TearDown]
        public void TearDown()
        {
            provider.Dispose();
        }

        // T1_short
        [TestCase(308, 18.6299991607666, 30, 12.5, 21.1000003814697, 1, 1.23923454979607, 5.18116129578529, 0.517031252935243, 1.959258268823121, 0.89069224977906825, 0.89069224977906825, 7.23789499947097, 1.959258268823121)]
        [TestCase(330, 18.6299991607666, 28.5, 11.8000001907349, 12.8000001907349, 0.33, 1.22219336009147, 12.9535455633961, 2.08643323248674, 7.5147420808585, 1.8145622706076, 1.8145622706076, 10.2857793902752, 7.5147420808585)]
        [TestCase(360, 18.6299991607666, 27, 11.5, 16.5, 0.087, 1.21271432800368, 20.3526170349121, 2.65511225270317, 10.8561001480905, 2.25713679038763, 2.25713679038763, 14.4885772219704, 10.8561001480905)]
        [TestCase(15, 18.6299991607666, 25.7999992370605, 17, 9.19999980926514, 0, 1.18454962481981, 23.7952599716186, 2.65511225270317, 10.3157759058664, 1.18296874919525, 1.18296874919525, 7.98636994199447, 10.3157759058664)]
        [TestCase(40, 18.6299991607666, 32.7999992370605, 14, 19.5, 0, 0.506217639803644, 19.5029139328003, 1.46144549436159, 3.1544024600902, 1.08237787207629, 1.08237787207629, 12.7972402167539, 3.1544024600902)]

        // T2_short
        [TestCase(310, 18.6299991607666, 29.5, 12.5, 20.7000007629395, 1, 1.2502997963605, 5.33653244686483, 0.587468392937812, 2.19721869036802, 0.969171763261619, 0.969171763261619, 7.84901631554099, 2.19721869036802)]
        [TestCase(330, 18.6299991607666, 28.5, 11.8000001907349, 12.8000001907349, 0.33, 1.22220740327011, 12.897439689826, 2.23453964724374, 7.83394495192283, 1.88866365903551, 1.88866365903551, 10.53663371574, 7.83394495192283)]
        [TestCase(350, 18.6299991607666, 28, 16, 13, 0.33, 1.21260172202049, 20.3044370962315, 3.06965619690384, 9.47353312907194, 2.04168426561638, 2.04168426561638, 11.8284567334544, 9.47353312907194)]
        [TestCase(5, 18.6299991607666, 27, 15, 15.1000003814697, 0, 1.21260172202049, 18.1454326629639, 3.06965619690384, 12.9569267917239, 2.1875272418155, 2.1875272418155, 13.6941582466213, 12.9569267917239)]
        [TestCase(35, 18.6299991607666, 29.5, 12, 20.1000003814697, 0, 1.21260172202049, 18.9953569412231, 3.06965619690384, 14.8691158705904, 3.30218526524078, 3.30218526524078, 17.7366606587102, 14.8691158705904)]

        // T3_short
        [TestCase(320, 18.6299991607666, 30, 18.6000003814697, 17.7000007629395, 0.33, 1.40603095909628, 8.73687517733383, 1.82518705710656, 9.71044193003593, 2.3311371610365, 2.3311371610365, 13.3040371596435, 9.71044193003593)]
        [TestCase(340, 18.6299991607666, 27.5, 15.5, 14.5, 0.33, 1.50471347788687, 16.5072945390211, 4.05192362695371, 12.5388084630618, 2.64266732998811, 2.64266732998811, 13.8346820887887, 12.5388084630618)]
        [TestCase(360, 18.6299991607666, 27, 11.5, 16.5, 0.087, 1.50512982266957, 19.4727304458618, 4.11137269670317, 15.2219757151277, 2.98009683755099, 2.98009683755099, 15.8033210915565, 15.2219757151277)]
        [TestCase(10, 18.6299991607666, 26, 17.5, 13.8000001907349, 0, 1.50512982266957, 23.3330610275269, 4.11137269670317, 16.6386686667434, 2.22973918752325, 2.22973918752325, 13.169132197812, 16.6386686667434)]
        [TestCase(30, 18.6299991607666, 29.5, 15, 18.7000007629395, 0, 1.50512982266957, 19.1538464736939, 4.11137269670317, 19.509076582336, 3.73405938565086, 3.73405938565086, 17.6360153856467, 19.509076582336)]
        public void Validate
        (
            int DOY, 
            double latitude, 
            double maxT, 
            double minT, 
            double radn, 
            double RootShootRatio, 
            double SLN, 
            double SWAvailable, 
            double lai,
            double expectedBIOshootDAY,
            double expectedEcanDemand,
            double expectedEcanSupply,
            double expectedRadIntDcaps,
            double expectedBIOshootDAYPot
        )
        {
            var geometry = provider.GetService<ISolarGeometry>() as SolarGeometryModel;
            geometry.Latitude = latitude.ToRadians();
            geometry.DayOfYear = DOY;

            var radiation = provider.GetService<ISolarRadiation>() as SolarRadiationModel;
            radiation.Daily = radn;
            radiation.RPAR = 0.5;

            var temperature = provider.GetService<ITemperature>() as TemperatureModel;
            temperature.MaxTemperature = maxT;
            temperature.MinTemperature = minT;
            temperature.AtmosphericPressure = 1.01325;

            var pathway = provider.GetService<IPathwayParameters>() as PathwayParameters;
            pathway.UseSorghumValues();

            var canopy = provider.GetService<ICanopyParameters>() as CanopyParameters;
            canopy.UseSorghumValues();

            var PM = provider.GetService<IPhotosynthesisModel>() as PhotosynthesisModel;
            PM.Initialise(canopy);
            //Model.B = 0.409;     //BiomassConversionCoefficient - CO2-to-biomass conversion efficiency
            //Model.Radiation.RPAR = 0.5;     //RPAR - Fraction of PAR energy to that of the total solar
            //Model.Temperature.AtmosphericPressure = 1.01325;   

            PM.DailyRun(lai, SLN, SWAvailable, RootShootRatio);

            Assert.AreEqual(expectedBIOshootDAY, PM.ActualBiomass, delta);
            Assert.AreEqual(expectedEcanDemand, PM.WaterDemanded, delta);
            Assert.AreEqual(expectedEcanSupply, PM.WaterSupplied, delta);
            Assert.AreEqual(expectedRadIntDcaps, PM.InterceptedRadiation, delta);
            Assert.AreEqual(expectedBIOshootDAYPot, PM.PotentialBiomass, delta);
        }
    }
}
