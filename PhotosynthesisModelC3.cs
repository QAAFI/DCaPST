using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using C3MethodExtensions;

namespace LayerCanopyPhotosynthesis
{
    public class PhotosynthesisModelC3 : PhotosynthesisModel
    {
        public PhotosynthesisModelC3() : base() { }

        public override void Run(bool sendNotification, double swAvail = 0, double maxHourlyT = -1, double sunlitFraction = 0, double shadedFraction = 0)
        {
            if (!Initialised)
            {
                return;
            }

            if (sendNotification && NotifyStart != null)
            {
                NotifyStart(false);
            }

            EnvModel.Run(this.Time);
            Canopy.CalcCanopyStructure(this.EnvModel.SunAngle.Rad);

            SunlitAC1 = new SunlitCanopy(Canopy.NLayers, SSType.AC1);
            SunlitAJ = new SunlitCanopy(Canopy.NLayers, SSType.AJ);
            ShadedAC1 = new ShadedCanopy(Canopy.NLayers, SSType.AC1);
            ShadedAJ = new ShadedCanopy(Canopy.NLayers, SSType.AJ);

            SunlitAC1.CalcLAI(this.Canopy, ShadedAC1);
            SunlitAJ.CalcLAI(this.Canopy, ShadedAJ);
            ShadedAC1.CalcLAI(this.Canopy, SunlitAC1);
            ShadedAJ.CalcLAI(this.Canopy, SunlitAJ);

            Canopy.Run(this, EnvModel);

            SunlitAC1.Run(Canopy.NLayers, this, ShadedAC1);
            SunlitAJ.Run(Canopy.NLayers, this, ShadedAJ);
            ShadedAC1.Run(Canopy.NLayers, this, SunlitAC1);
            ShadedAJ.Run(Canopy.NLayers, this, SunlitAJ);

            TranspirationMode mode = TranspirationMode.unlimited;

            if (maxHourlyT != -1)
            {
                mode = TranspirationMode.limited;
            }

            bool useAirTemp = false;

            List<bool> results = new List<bool>();

            results.Add(SunlitAC1.CalcPhotosynthesis(this, useAirTemp, 0, EnvModel.GetTemp(Time), mode, maxHourlyT, sunlitFraction));
            results.Add(SunlitAJ.CalcPhotosynthesis(this, useAirTemp, 0, EnvModel.GetTemp(Time), mode, maxHourlyT, sunlitFraction));
            results.Add(ShadedAC1.CalcPhotosynthesis(this, useAirTemp, 0, EnvModel.GetTemp(Time), mode, maxHourlyT, shadedFraction));
            results.Add(ShadedAJ.CalcPhotosynthesis(this, useAirTemp, 0, EnvModel.GetTemp(Time), mode, maxHourlyT, shadedFraction));

            Count = 1;

            bool caughtError = false;

            while (results.Contains(false) && !caughtError)
            {
                results.Clear();
                results.Add(SunlitAC1.CalcPhotosynthesis(this, useAirTemp, 0, SunlitAC1.LeafTemp[0], mode, maxHourlyT, sunlitFraction));
                results.Add(SunlitAJ.CalcPhotosynthesis(this, useAirTemp, 0, SunlitAJ.LeafTemp[0], mode, maxHourlyT, sunlitFraction));
                results.Add(ShadedAC1.CalcPhotosynthesis(this, useAirTemp, 0, ShadedAC1.LeafTemp[0], mode, maxHourlyT, shadedFraction));
                results.Add(ShadedAJ.CalcPhotosynthesis(this, useAirTemp, 0, ShadedAJ.LeafTemp[0], mode, maxHourlyT, shadedFraction));

                Count++;

                if (Count > 100 && !caughtError)
                {
                    //writeScenario(swAvail);
                    caughtError = true;
                }
            }

            // canopy.calcCanopyBiomassAccumulation(this);

            if (sendNotification && NotifyFinish != null)
            {
                NotifyFinish();
            }
           // writeScenario(swAvail);
        }

        public void WriteScenario(double swAvail)
        {
            StreamWriter sr = new StreamWriter("scenario.csv",true);

            sr.WriteLine("Lat," + EnvModel.LatitudeD);
            sr.WriteLine("DOY," + EnvModel.DOY);
            sr.WriteLine("Maxt," + EnvModel.MaxT);
            sr.WriteLine("AvailableWater," + swAvail);
            sr.WriteLine("Mint," + EnvModel.MinT);
            sr.WriteLine("Radn," + EnvModel.Radn);
            sr.WriteLine("Ratio," + EnvModel.AtmTransmissionRatio);
            sr.WriteLine("LAI," + Canopy.LAI);
            sr.WriteLine("SLN," + Canopy.CPath.SLNAv);

            List<double> temps = new List<double>();

            for (int i = 0; i <= 12; i++)
            {
                temps.Add(EnvModel.GetTemp(i + 6));
            }

            sr.WriteLine("Temps," + String.Join(",", temps.ToArray()));

            sr.Flush();

            sr.Close();
        }
    }
}
