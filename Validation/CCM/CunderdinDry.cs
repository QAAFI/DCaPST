using System;
using NUnit.Framework;

using DCAPST;
using DCAPST.Environment;
using DCAPST.Interfaces;
namespace Validation.CCM
{
    [TestFixture]
    public class CunderdinDry
    {
        private double delta = 0.0000000000001;

        [TestCase(228, -31.3999996185303, 13, 2.90000009536743, 14.5, 0.561035096645355, 1.87829542160034, 0, 0.554309725761414, 0, 0.495386168595403, 0, 5.97844157619763, 2.76022202465798)]
        [TestCase(231, -31.3999996185303, 16.2999992370605, -1.10000002384186, 17.8999996185303, 0.525810301303864, 1.85285043716431, 0, 0.693877518177032, 0, 1.03433244206527, 0, 8.46848982578894, 3.73424338954495)]
        [TestCase(236, -31.3999996185303, 17.2999992370605, 3.29999995231628, 17.1000003814697, 0.449850261211395, 1.8142204284668, 0, 1.01821279525757, 0, 1.45691055488475, 0, 10.0080146311784, 6.28740234581365)]
        [TestCase(245, -31.3999996185303, 13.5, 1.29999995231628, 14.5, 0.30253678560257, 1.70214831829071, 0, 1.72223508358002, 0, 1.30257968728247, 0, 10.6811304979051, 7.84853714722494)]
        [TestCase(245, -31.3999996185303, 13.5, 1.29999995231628, 14.5, 0.300000011920929, 1.70214831829071, 1.55352914333344, 1.72223508358002, 7.86385250283408, 1.30257968728247, 1.30257968728247, 10.6811304979051, 7.86385250283408)]
        [TestCase(257, -31.3999996185303, 17.5, 10.1000003814697, 14.8000001907349, 0.300000011920929, 1.5280567407608, 1.63610899448395, 2.66363716125488, 12.7246787820742, 2.01186085721887, 1.63610912773333, 12.3976452555733, 13.3554663248404)]
        [TestCase(272, -31.3999996185303, 25.8999996185303, 7.59999990463257, 23.2999992370605, 0.25436145067215, 1.71684801578522, 0, 1.25181806087494, 0, 3.50097451399276, 0, 13.5096268864946, 12.0404930503324)]
        [TestCase(284, -31.3999996185303, 22.2000007629395, 3.5, 25.7999992370605, 0.121832832694054, 1.88160455226898, 0.910509645938873, 0.645247459411621, 5.67704239913101, 1.93553588263863, 0.910509901655659, 9.59524534509422, 8.03808794738454)]
        [TestCase(295, -31.3999996185303, 31.8999996185303, 12.8000001907349, 22, 0, 1.29674053192139, 0, 0.334723025560379, 0, 1.35928845123217, 0, 4.87641147002442, 4.34776906675278)]
        [TestCase(299, -31.3999996185303, 26.1000003814697, 3, 27.2999992370605, 0, 0.9865802526474, 0.549221158027649, 0.309116929769516, 2.2120577486884, 0.858800299033414, 0.549220606025134, 5.63098554247458, 2.60372183944383)]
        [TestCase(308, -31.3999996185303, 24, 14, 19.8999996185303, 0, 0.450942844152451, 0, 0.21750770509243, 0, 0.147136565553468, 0, 2.9894965019919, 0.721375916363099)]
        [TestCase(327, -31.3999996185303, 28.7000007629395, 7.59999990463257, 31.5, 0, 0.387778669595718, 0.343205273151398, 0.205235481262207, 0.495437714993896, 0.219802532243409, 0.219802532243409, 4.36955630451826, 0.495437714993896)]
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
            var CPath = Initialise.NewWheat() as ICanopyParameters;

            ISolarGeometry Solar = new SolarGeometryModel(DOY, latitude);
            ISolarRadiation Radiation = new SolarRadiationModel(Solar, radn) { RPAR = 0.5 };
            ITemperature Temperature = new TemperatureModel(Solar, maxT, minT) { AtmosphericPressure = 1.01325 };

            var PM = new PhotosynthesisModel(Solar, Radiation, Temperature, CPath);
            //Model.B = 0.409;     //BiomassConversionCoefficient - CO2-to-biomass conversion efficiency
            //Model.Radiation.RPAR = 0.5;     //RPAR - Fraction of PAR energy to that of the total solar
            //Model.Temperature.AtmosphericPressure = 1.01325;   

            var dcaps = PM.DailyRun(lai, SLN, SWAvailable, RootShootRatio);
            double BIOshootDAY = dcaps[0];            
            double EcanDemand = dcaps[1];
            double EcanSupply = dcaps[2];
            double RadIntDcaps = dcaps[3];
            double BIOshootDAYPot = dcaps[4];

            Assert.AreEqual(expectedBIOshootDAY, BIOshootDAY, delta);
            Assert.AreEqual(expectedEcanDemand, EcanDemand, delta);
            Assert.AreEqual(expectedEcanSupply, EcanSupply, delta);
            Assert.AreEqual(expectedRadIntDcaps, RadIntDcaps, delta);
            Assert.AreEqual(expectedBIOshootDAYPot, BIOshootDAYPot, delta);
        }
    }
}
