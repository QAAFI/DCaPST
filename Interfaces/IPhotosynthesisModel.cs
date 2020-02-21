using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCAPST.Interfaces
{
    public interface IPhotosynthesisModel
    {
        void DailyRun(double lai, double SLN, double soilWater, double RootShootRatio, double MaxHourlyTRate = 100);
    }
}
