using DCAPST;
using DCAPST.Canopy;
using DCAPST.Environment;
using DCAPST.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Startup.Models;
using System;

namespace Validation
{
    static class Services
    {
        public static IServiceProvider Register()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IPhotosynthesisModel, PhotosynthesisModel>();
            collection.AddSingleton<ISolarGeometry, SolarGeometryModel>();
            collection.AddSingleton<ISolarRadiation, SolarRadiationModel>();
            collection.AddSingleton<ITemperature, TemperatureModel>();
            collection.AddSingleton<ICanopyParameters, CanopyParameters>();
            collection.AddSingleton<IPathwayParameters, PathwayParameters>();
            collection.AddSingleton<ITotalCanopy, TotalCanopy>();

            collection.AddTransient<IPartialCanopy, PartialCanopy>();

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
    }
}
