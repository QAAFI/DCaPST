using System;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

using DCAPST;
using DCAPST.Environment;
using DCAPST.Interfaces;

namespace Validation.C4
{
    [TestFixture]
    public class ClassicTests
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

        [TestCaseSource(typeof(SorghumData), "HE_T1_Output")]
        public void Hermitage_T1
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
            var geometry = provider.GetService<ISolarGeometry>() as SolarGeometry;
            geometry.Latitude = latitude.ToRadians();
            geometry.DayOfYear = DOY;

            var radiation = provider.GetService<ISolarRadiation>() as SolarRadiation;
            radiation.Daily = radn;
            radiation.RPAR = 0.5;

            var temperature = provider.GetService<ITemperature>() as Temperature;
            temperature.MaxTemperature = maxT;
            temperature.MinTemperature = minT;
            temperature.AtmosphericPressure = 1.01325;

            var pathway = provider.GetService<IPathwayParameters>() as PathwayParameters;
            SorghumC4.SetPathway(pathway);

            var canopy = provider.GetService<ICanopyParameters>() as CanopyParameters;
            SorghumC4.SetCanopy(canopy);

            var PM = provider.GetService<IPhotosynthesisModel>() as DCAPSTModel;

            PM.DailyRun(lai, SLN, SWAvailable, RootShootRatio);

            Assert.AreEqual(expectedBIOshootDAY, PM.ActualBiomass, delta);
            Assert.AreEqual(expectedEcanDemand, PM.WaterDemanded, delta);
            Assert.AreEqual(expectedEcanSupply, PM.WaterSupplied, delta);
            Assert.AreEqual(expectedRadIntDcaps, PM.InterceptedRadiation, delta);
            Assert.AreEqual(expectedBIOshootDAYPot, PM.PotentialBiomass, delta);
        }
    }
}
