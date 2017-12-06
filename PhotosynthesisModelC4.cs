using System;
using System.Collections.Generic;
using System.IO;
using C4MethodExtensions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayerCanopyPhotosynthesis
{
    public class PhotosynthesisModelC4 : PhotosynthesisModel
    {

        public SunlitCanopy sunlitAC2;
        public ShadedCanopy shadedAC2;


        public PhotosynthesisModelC4() : base() { }

        //---------------------------------------------------------------------------
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
            sunlitAC2 = new SunlitCanopy(Canopy.NLayers, SSType.AC2);
            SunlitAJ = new SunlitCanopy(Canopy.NLayers, SSType.AJ);
            ShadedAC1 = new ShadedCanopy(Canopy.NLayers, SSType.AC1);
            shadedAC2 = new ShadedCanopy(Canopy.NLayers, SSType.AC2);
            ShadedAJ = new ShadedCanopy(Canopy.NLayers, SSType.AJ);

            SunlitAC1.CalcLAI(this.Canopy, ShadedAC1);
            sunlitAC2.CalcLAI(this.Canopy, shadedAC2);
            SunlitAJ.CalcLAI(this.Canopy, ShadedAJ);
            ShadedAC1.CalcLAI(this.Canopy, SunlitAC1);
            shadedAC2.CalcLAI(this.Canopy, sunlitAC2);
            ShadedAJ.CalcLAI(this.Canopy, SunlitAJ);

            Canopy.Run(this, EnvModel);

            SunlitAC1.Run(Canopy.NLayers, this, ShadedAC1);
            sunlitAC2.Run(Canopy.NLayers, this, shadedAC2);
            SunlitAJ.Run(Canopy.NLayers, this, ShadedAJ);
            ShadedAC1.Run(Canopy.NLayers, this, SunlitAC1);
            shadedAC2.Run(Canopy.NLayers, this, sunlitAC2);
            ShadedAJ.Run(Canopy.NLayers, this, SunlitAJ);

            TranspirationMode mode = TranspirationMode.unlimited;

            if (maxHourlyT != -1)
            {
                mode = TranspirationMode.limited;
            }

            bool useAirTemp = false;

            double defaultCm = 160;

            List<bool> results = new List<bool>();

            results.Add(SunlitAC1.CalcPhotosynthesis(this, useAirTemp, 0, EnvModel.GetTemp(Time), defaultCm, mode, maxHourlyT, sunlitFraction));
            results.Add(sunlitAC2.CalcPhotosynthesis(this, useAirTemp, 0, EnvModel.GetTemp(Time), defaultCm, mode, maxHourlyT, sunlitFraction));
            results.Add(SunlitAJ.CalcPhotosynthesis(this, useAirTemp, 0, EnvModel.GetTemp(Time), defaultCm, mode, maxHourlyT, sunlitFraction));
            results.Add(ShadedAC1.CalcPhotosynthesis(this, useAirTemp, 0, EnvModel.GetTemp(Time), defaultCm, mode, maxHourlyT, shadedFraction));
            results.Add(shadedAC2.CalcPhotosynthesis(this, useAirTemp, 0, EnvModel.GetTemp(Time), defaultCm, mode, maxHourlyT, shadedFraction));
            results.Add(ShadedAJ.CalcPhotosynthesis(this, useAirTemp, 0, EnvModel.GetTemp(Time), defaultCm, mode, maxHourlyT, shadedFraction));

            Count = 1;

            bool caughtError = false;

            while (results.Contains(false) && !caughtError)
            {
                results.Clear();
                results.Add(SunlitAC1.CalcPhotosynthesis(this, useAirTemp, 0, SunlitAC1.LeafTemp[0], SunlitAC1.Cm[0], mode, maxHourlyT, sunlitFraction));
                results.Add(sunlitAC2.CalcPhotosynthesis(this, useAirTemp, 0, sunlitAC2.LeafTemp[0], sunlitAC2.Cm[0], mode, maxHourlyT, sunlitFraction));
                results.Add(SunlitAJ.CalcPhotosynthesis(this, useAirTemp, 0, SunlitAJ.LeafTemp[0], SunlitAJ.Cm[0], mode, maxHourlyT, sunlitFraction));
                results.Add(ShadedAC1.CalcPhotosynthesis(this, useAirTemp, 0, ShadedAC1.LeafTemp[0], ShadedAC1.Cm[0], mode, maxHourlyT, shadedFraction));
                results.Add(shadedAC2.CalcPhotosynthesis(this, useAirTemp, 0, shadedAC2.LeafTemp[0], shadedAC2.Cm[0], mode, maxHourlyT, shadedFraction));
                results.Add(ShadedAJ.CalcPhotosynthesis(this, useAirTemp, 0, ShadedAJ.LeafTemp[0], ShadedAJ.Cm[0], mode, maxHourlyT, shadedFraction));

                if (double.IsNaN(SunlitAC1.Cm[0]) ||
                    double.IsNaN(sunlitAC2.Cm[0]) ||
                    double.IsNaN(SunlitAJ.Cm[0]) ||
                    double.IsNaN(ShadedAC1.Cm[0]) ||
                    double.IsNaN(shadedAC2.Cm[0]) ||
                    double.IsNaN(ShadedAJ.Cm[0]) ||
                    Count == 30)
                {
                    SunlitAC1.Cm[0] = defaultCm;
                    sunlitAC2.Cm[0] = defaultCm;
                    SunlitAJ.Cm[0] = defaultCm;
                    ShadedAC1.Cm[0] = defaultCm;
                    shadedAC2.Cm[0] = defaultCm;
                    ShadedAJ.Cm[0] = defaultCm;

                    useAirTemp = true;
                }

                Count++;

                if (Count > 100 && !caughtError)
                {
                    StreamWriter sr = new StreamWriter("scenario.csv");

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
                        temps.Add(EnvModel.GetTemp(i + 5));
                    }

                    sr.WriteLine("Temps," + String.Join(",", temps.ToArray()));

                    sr.Flush();

                    sr.Close();

                    caughtError = true;
                }
            }

            Canopy.CalcCanopyBiomassAccumulation(this);

            if (sendNotification && NotifyFinish != null)
            {
                NotifyFinish();
            }
        }
    }
}
