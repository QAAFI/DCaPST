using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DCAPST;
using Microsoft.Extensions.DependencyInjection;

namespace Startup
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigureServices();
        }

        public static IServiceProvider provider;

        static void ConfigureServices()
        {
            provider = new ServiceCollection()
                .AddSingleton()
                .AddScoped()
                .BuildServiceProvider();
        }
    }
}
