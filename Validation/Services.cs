using DCAPST;
using DCAPST.Canopy;
using DCAPST.Environment;
using DCAPST.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Validation
{
    static class Services
    {
        public static IServiceProvider Register()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IPhotosynthesisModel, DCAPSTModel>();
            collection.AddSingleton<ISolarGeometry, SolarGeometry>();
            collection.AddSingleton<ISolarRadiation, SolarRadiation>();
            collection.AddSingleton<ITemperature, Temperature>();
            collection.AddSingleton<ICanopyParameters, CanopyParameters>();
            collection.AddSingleton<IPathwayParameters, PathwayParameters>();
            collection.AddSingleton<ICanopyAttributes, CanopyAttributes>();
            collection.AddSingleton<IWaterInteraction, WaterInteraction>();
            collection.AddSingleton(typeof(TemperatureResponse));
            collection.AddSingleton(typeof(IAssimilation), sp => AssimilationFactory(sp));
            collection.AddSingleton(typeof(Transpiration));

            collection.AddTransient<IAssimilationArea, AssimilationArea>();
            collection.AddTransient(typeof(AssimilationPathway));

            return collection.BuildServiceProvider();
        }

        public static void Dispose(this IServiceProvider provider)
        {
            if (provider == null)
            {
                return;
            }
            if (provider is IDisposable)
            {
                ((IDisposable)provider).Dispose();
            }
        }

        private static IAssimilation AssimilationFactory(IServiceProvider sp)
        {
            var canopy = sp.GetService<ICanopyParameters>();
            var pathway = sp.GetService<IPathwayParameters>();

            switch(canopy.Type)
            {
                case (CanopyType.C3):
                    return new AssimilationC3(canopy, pathway);

                case (CanopyType.C4):
                    return new AssimilationC4(canopy, pathway);

                case (CanopyType.CCM):
                    return new AssimilationCCM(canopy, pathway);

                default:
                    throw new Exception("You have reached unreachable code. Congratulations.");
            }
        }
    }
}
