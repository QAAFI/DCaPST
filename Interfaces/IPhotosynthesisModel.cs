using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCAPST.Interfaces
{
    /// <summary>
    /// Represents a model that simulates daily photosynthesis
    /// </summary>
    public interface IPhotosynthesisModel
    {
        void DailyRun(double lai, double SLN, double soilWater, double RootShootRatio);
    }
}
