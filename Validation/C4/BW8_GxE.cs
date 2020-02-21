using System;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

using DCAPST;
using DCAPST.Environment;
using DCAPST.Interfaces;

namespace Validation.C4
{
    [TestFixture]
    public class BW8_GxE
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
        [TestCase(305, 18, 30, 16, 20.7000007629395, 1, 1.22749041428519, 6.04185654865598, 0.584505334838906, 2.31409494593741, 0.950150208937644, 0.950150208937644, 7.63420358736702, 2.31409494593741)]
        [TestCase(330, 18, 28.5, 11.8000001907349, 12.8000001907349, 0.33, 1.24082843280798, 7.81355857005778, 2.41497300545325, 8.33284499874166, 1.99062154552176, 1.99062154552176, 10.7658891329223, 8.33284499874166)]
        [TestCase(365, 18, 26, 13, 13.8000001907349, 0, 1.27062357125684, 11.0870656867175, 2.68048905213516, 11.7277825274655, 1.8636167829996, 1.8636167829996, 12.0896417517557, 11.7277825274655)]
        [TestCase(15, 18, 25.7999992370605, 17, 9.19999980926514, 0, 1.27062357125684, 13.0307374432691, 2.68048905213516, 10.9568539101822, 1.23347581829087, 1.23347581829087, 7.98025121929878, 10.9568539101822)]
        [TestCase(35, 18, 29.5, 12, 20.1000003814697, 0, 0.738575222446511, 8.45019463334225, 2.13329406447713, 7.11478483000626, 1.8886769876057, 1.8886769876057, 15.7164508316762, 7.11478483000626)]

        // T7_short
        [TestCase(310, 18, 29.5, 12.5, 20.7000007629395, 0.33, 1.3918047413314, 7.39216523706926, 0.708030804555144, 4.39137414690285, 1.25353161996076, 1.25353161996076, 8.88134807006139, 4.39137414690285)]
        [TestCase(340, 18, 27.5, 15.5, 14.5, 0.33, 1.5046104720661, 3.14138528148854, 4.09292885504344, 12.6537481745249, 2.65567353771901, 2.65567353771901, 13.829539956725, 12.6537481745249)]
        [TestCase(365, 18, 26, 13, 13.8000001907349, 0.087, 1.56078227066919, 0.529503678886558, 3.4221700332861, 4.65054287764583, 2.29196669520562, 0.529503460222797, 12.7921664919686, 13.9781876643876)]
        [TestCase(20, 18, 26.5, 16.2000007629395, 11.3000001907349, 0, 0.905547602385301, 0.105404917539928, 0.864295837440788, 1.32301302700279, 0.515768642060006, 0.105404782211603, 5.74357391466666, 3.62605335043431)]
        [TestCase(40, 18, 32.7999992370605, 14, 19.5, 0, 0.793482857909968, 0.0230400075071777, 0.160945939206253, 0.152312943545511, 0.197492795076432, 0.0230404986085315, 2.47511876540153, 0.742348761981791)]

        // T12_short
        [TestCase(310, 18, 29.5, 12.5, 20.7000007629395, 0.33, 1.35347088701938, 7.32203042578138, 0.880904469626966, 5.17445034107981, 1.49684836554916, 1.49684836554916, 10.2563266764217, 5.17445034107981)]
        [TestCase(335, 18, 28.2000007629395, 11, 18.7999992370605, 0.33, 1.48625329875361, 4.32395066808339, 3.47185014806027, 12.6303554539449, 3.36338442671948, 3.36338442671948, 17.4095048998863, 12.6303554539449)]
        [TestCase(360, 18, 27, 11.5, 16.5, 0, 1.47849330144525, 0.770140067919342, 3.47185014806027, 5.9991700191788, 2.80685993770738, 0.770140335730145, 15.3525056882965, 15.5983938667501)]
        [TestCase(10, 18, 26, 17.5, 13.8000001907349, 0, 0.890955753532598, 0.229342489661371, 1.31350735161841, 2.47621106852101, 0.800807543331986, 0.229343183795099, 9.06099135566263, 5.42416364371319)]
        [TestCase(30, 18, 29.5, 15, 18.7000007629395, 0, 0.597142911145001, 0.0504525942401726, 0.518084707580792, 0.392846888158404, 0.358602183924876, 0.050453062480784, 6.51647621228987, 1.43861781447528)]
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
