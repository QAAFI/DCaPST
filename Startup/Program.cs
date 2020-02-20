using DCAPST;
using DCAPST.Environment;
using DCAPST.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Startup.Models;
using System;

namespace Startup
{
    class Program
    {
        private static IServiceProvider _serviceProvider;
        static void Main(string[] args)
        {
            RegisterServices();

            var photosynthesis = _serviceProvider.GetService<IPhotosynthesisModel>();
            var clock = _serviceProvider.GetService<IClock>() as Clock;
            var weather = _serviceProvider.GetService<IWeather>() as Weather;
            //service.DailyRun();
            //[TestCase(308, 18.6299991607666, 30, 12.5, 21.1000003814697, 1, 1.23923454979607, 5.18116129578529, 0.517031252935243, 1.959258268823121, 0.89069224977906825, 0.89069224977906825, 7.23789499947097, 1.959258268823121)]
            //[TestCase(330, 18.6299991607666, 28.5, 11.8000001907349, 12.8000001907349, 0.33, 1.22219336009147, 12.9535455633961, 2.08643323248674, 7.5147420808585, 1.8145622706076, 1.8145622706076, 10.2857793902752, 7.5147420808585)]
            //[TestCase(360, 18.6299991607666, 27, 11.5, 16.5, 0.087, 1.21271432800368, 20.3526170349121, 2.65511225270317, 10.8561001480905, 2.25713679038763, 2.25713679038763, 14.4885772219704, 10.8561001480905)]
            //[TestCase(40, 18.6299991607666, 32.7999992370605, 14, 19.5, 0, 0.506217639803644, 19.5029139328003, 1.46144549436159, 3.1544024600902, 1.08237787207629, 1.08237787207629, 12.7972402167539, 3.1544024600902)]
            //[TestCase(15, 18.6299991607666, 25.7999992370605, 17, 9.19999980926514, 0, 1.18454962481981, 23.7952599716186, 2.65511225270317, 10.3157759058664, 1.18296874919525, 1.18296874919525, 7.98636994199447, 10.3157759058664)]

            //In Apsim, the weather responds to an event that is called when the clock moves days
            //not trying to repeat that here
            clock.Today = new DateTime(2019, 1, 15);
            weather.Latitude = 18.6299991607666;
            weather.MaxT = 25.7999992370605;
            weather.MinT = 17;
            weather.Radn = 9.19999980926514;
            var result = photosynthesis.DailyRun(2.65511225270317, 1.18454962481981, 23.7952599716186, 0);

            clock.Today.AddDays(25);
            weather.MaxT = 32.7999992370605;
            weather.MinT = 14;
            weather.Radn = 19.5;
            var result2 = photosynthesis.DailyRun(1.46144549436159, 0.506217639803644, 19.502913932800, 0);


            DisposeServices();
        }
        private static void RegisterServices()
        {
            var collection = new ServiceCollection();
            collection.AddSingleton<IPhotosynthesisModel, PhotosynthesisModel>();
            collection.AddSingleton<ISolarGeometry, SolarGeometryModel>();
            collection.AddSingleton<ISolarRadiation, SolarRadiationModel>();
            collection.AddSingleton<ITemperature, TemperatureModel>();
            collection.AddSingleton<IClock, Clock>();
            collection.AddSingleton<IWeather, Weather>();
            // ...
            // Add other services
            // ...
            _serviceProvider = collection.BuildServiceProvider();
        }

        private static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }
    }
}
