using System;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

using DCAPST;
using DCAPST.Environment;
using DCAPST.Interfaces;
using System.IO;

namespace Validation.C3
{
    [TestFixture]
    public class ClassicTests
    {
        private IServiceProvider provider;
        private double delta = 0.00001;

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


        [TestCaseSource(typeof(WheatData), "Avalon_84_Output")]
        public void Avalon_84
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

            var canopy = provider.GetService<ICanopyParameters>() as CanopyParameters;
            WheatC3.SetCanopy(canopy);

            var pathway = provider.GetService<IPathwayParameters>() as PathwayParameters;
            WheatC3.SetPathway(pathway);

            var PM = provider.GetService<IPhotosynthesisModel>() as DCAPSTModel;

            PM.DailyRun(lai, SLN, SWAvailable, RootShootRatio);

            Assert.AreEqual(expectedBIOshootDAYPot, PM.PotentialBiomass, delta);
            Assert.AreEqual(expectedBIOshootDAY, PM.ActualBiomass, delta);
            Assert.AreEqual(expectedEcanDemand, PM.WaterDemanded, delta);
            Assert.AreEqual(expectedEcanSupply, PM.WaterSupplied, delta);
            Assert.AreEqual(expectedRadIntDcaps, PM.InterceptedRadiation, delta);            
        }
    }
}
