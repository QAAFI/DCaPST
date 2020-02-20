using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DCAPST.Canopy;
using DCAPST.Environment;
using DCAPST.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DCAPST
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSingleton(this IServiceCollection services)
        {
            services.AddSingleton<ITemperature, TemperatureModel>();
            services.AddSingleton<ISolarRadiation, SolarRadiationModel>();
            services.AddSingleton<ISolarGeometry, SolarGeometryModel>();            

            return services;
        }

        public static IServiceCollection AddScoped(this IServiceCollection services)
        {
            services.AddScoped<ITotalCanopy, TotalCanopy>();

            return services;
        }
    }
}
