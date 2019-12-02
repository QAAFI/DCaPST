using System;
using LayerCanopyPhotosynthesis;
using System.Linq;

namespace CCMMethodExtensions
{
    public static class CCMMethodExtensions
    {
        //---------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="PM"></param>
        /// <param name="useAirTemp"></param>
        /// <param name="layer"></param>
        /// <param name="leafTemperature"></param>
        /// <param name="cm"></param>
        /// <param name="mode"></param>
        /// <param name="maxHourlyT"></param>
        /// <param name="Tfraction"></param>
        /// <returns></returns>
        public static bool CalcPhotosynthesis(this SunlitShadedCanopy s, PhotosynthesisModel PM, bool useAirTemp, int layer, double leafTemperature, double cm, double cc, double oc,
            TranspirationMode mode, double maxHourlyT, double Tfraction)
        {
            LeafCanopy canopy = PM.Canopy;

            s.LeafTemp__[layer] = leafTemperature;
            if (useAirTemp)
            {
                s.LeafTemp__[layer] = PM.EnvModel.GetTemp(PM.Time);
            }

            s.CalcConductanceResistance(PM, canopy);

            s.Cm__[layer] = cm;
            s.Cc[layer] = cc;
            s.Oc[layer] = oc;

            s.VcMaxT[layer] = TemperatureFunction.Val2(s.LeafTemp__[layer], s.VcMax25[layer], canopy.CPath.VcTEa);
            s.RdT[layer] = TemperatureFunction.Val2(s.LeafTemp__[layer], s.Rd25[layer], canopy.CPath.RdTEa);
            s.JMaxT[layer] = TemperatureFunction.Val(s.LeafTemp__[layer], s.JMax25[layer], canopy.CPath.JMaxC, canopy.CPath.JTMax, canopy.CPath.JTMin, canopy.CPath.JTOpt, canopy.CPath.JBeta);
            s.VpMaxT[layer] = TemperatureFunction.Val2(s.LeafTemp__[layer], s.VpMax25[layer], canopy.CPath.VpMaxTEa);
            s.GmT[layer] = TemperatureFunction.Val(s.LeafTemp__[layer], s.Gm25[layer], canopy.CPath.GmC, canopy.CPath.GmTMax, canopy.CPath.GmTMin, canopy.CPath.GmTOpt, canopy.CPath.GmBeta);
            s.Vpr[layer] = canopy.Vpr_l * s.LAIS[layer];

            canopy.Ja = (1 - canopy.F) / 2;

            s.J[layer] = (canopy.Ja * s.AbsorbedIrradiance[layer] + s.JMaxT[layer] - Math.Pow(Math.Pow(canopy.Ja * s.AbsorbedIrradiance[layer] + s.JMaxT[layer], 2) -
            4 * canopy.Theta * s.JMaxT[layer] * canopy.Ja * s.AbsorbedIrradiance[layer], 0.5)) / (2 * canopy.Theta);

            s.Kc[layer] = TemperatureFunction.Val2(s.LeafTemp__[layer], canopy.CPath.KcP25, canopy.CPath.KcTEa);
            s.Ko[layer] = TemperatureFunction.Val2(s.LeafTemp__[layer], canopy.CPath.KoP25, canopy.CPath.KoTEa);
            s.VcVo[layer] = TemperatureFunction.Val2(s.LeafTemp__[layer], canopy.CPath.VcMax_VoMaxP25, canopy.CPath.VcMax_VoMaxTEa);

            s.Oi[layer] = canopy.OxygenPartialPressure;
            s.Om[layer] = canopy.OxygenPartialPressure;

            s.ScO[layer] = s.Ko[layer] / s.Kc[layer] * s.VcVo[layer];
            s.G_[layer] = 0.5 / s.ScO[layer];
            canopy.Sco = s.ScO[layer]; 

            s.K_[layer] = s.Kc[layer] * (1 + canopy.OxygenPartialPressure / s.Ko[layer]);
            s.Kp[layer] = TemperatureFunction.Val2(s.LeafTemp__[layer], canopy.CPath.KpP25, canopy.CPath.KpTEa);
            s.Gbs[layer] = canopy.Gbs_CO2 * s.LAIS[layer];

            if (mode == TranspirationMode.unlimited)
            {
                s.Rm[layer] = s.RdT[layer] * 0.5;
                s.Gbs[layer] = canopy.Gbs_CO2 * s.LAIS[layer];
                s.Ci[layer] = canopy.CPath.CiCaRatio * canopy.Ca;

                s.p = s.Ci[layer];
                s.q = 1 / s.GmT[layer];

                //Caculate A's
                if (s.type == SSType.Ac1)
                {
                    s.A[layer] = CalcAc1(s, canopy, layer, TranspirationMode.unlimited);                    
                }
                else if (s.type == SSType.Ac2)
                {
                    s.A[layer] = CalcAc2(s, canopy, layer, TranspirationMode.unlimited);
                }
                else if (s.type == SSType.Aj)
                {
                    s.A[layer] = CalcAj(s, canopy, layer, TranspirationMode.unlimited);
                }                        

                s.DoWaterInteraction(PM, canopy, mode);
            }
            else if (mode == TranspirationMode.limited)
            {
                s.WaterUse[layer] = maxHourlyT * Tfraction;
                s.DoWaterInteraction(PM, canopy, mode);

                double Gt = 1 / (1 / s.GbCO2[layer] + 1 / s.GsCO2[layer]);

                s.p = canopy.Ca - s.WaterUseMolsSecond[layer] * canopy.Ca / (Gt + s.WaterUseMolsSecond[layer] / 2);
                s.q = 1 / (Gt + s.WaterUseMolsSecond[layer] / 2) + 1 / s.GmT[layer];

                //Caculate A's
                if (s.type == SSType.Ac1)
                {
                    s.A[layer] = CalcAc1(s, canopy, layer, TranspirationMode.limited);
                }
                else if (s.type == SSType.Ac2)
                {
                    s.A[layer] = CalcAc2(s, canopy, layer, TranspirationMode.limited);
                }
                else if (s.type == SSType.Aj)
                {
                    s.A[layer] = CalcAj(s, canopy, layer, TranspirationMode.limited);
                }

                s.Ci[layer] = ((Gt - s.WaterUseMolsSecond[0] / 2) * canopy.Ca - s.A[layer]) / (Gt + s.WaterUseMolsSecond[0] / 2);                
            }

            s.Oc[layer] = canopy.Alpha * s.A[layer] / (canopy.Constant * s.Gbs[layer]) + s.Om[layer];
            s.Cc[layer] = (s.Ci[layer] - s.A[layer] / s.GmT[layer]) + (((s.Ci[layer] - s.A[layer] / s.GmT[layer]) * s.x_4 + s.x_5) - s.x_6 * s.A[layer] - s.m - s.x_7) * s.x_8 / s.Gbs[layer];
            s.Cm[layer] = s.Ci[layer] - s.A[layer] / s.GmT[layer];
            s.LeafTemp[layer] = (s.LeafTemp[layer] + s.LeafTemp__[layer]) / 2;

            // Test if A is sensible
            if (double.IsNaN(s.A[layer]) || s.A[layer] <= 0.0)
                return true;
            // Test if WaterUse is sensible
            else if (double.IsNaN(s.WaterUse[layer]) || s.WaterUse[layer] <= 0)
                return true;
            // If both are sensible, return false (no errors fonud)
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="canopy"></param>
        /// <param name="layer"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static double CalcAc1(SunlitShadedCanopy s, LeafCanopy canopy, int layer, TranspirationMode mode)
        {
            s.x_1 = s.VcMaxT[layer];
            s.x_2 = s.Kc[layer] / s.Ko[layer];
            s.x_3 = s.Kc[layer];
            s.x_4 = s.VpMaxT[layer] / (s.Cm__[layer] + s.Kp[layer]);
            s.x_5 = 0;
            s.x_6 = 0;
            s.x_7 = s.Cc[layer] * s.VcMaxT[layer] / (s.Cc[layer] + s.Kc[layer] * (1 + s.Oc[layer] / s.Ko[layer]));
            s.x_8 = 1;
            s.x_9 = 1;

            return CalcAssimilation(s, layer, canopy);
        }

        public static double CalcAc2(SunlitShadedCanopy s, LeafCanopy canopy, int layer, TranspirationMode mode)
        {
            s.x_1 = s.VcMaxT[layer];
            s.x_2 = s.Kc[layer] / s.Ko[layer];
            s.x_3 = s.Kc[layer];
            s.x_4 = 0;
            s.x_5 = s.Vpr[layer];
            s.x_6 = 0;
            s.x_7 = s.Cc[layer] * s.VcMaxT[layer] / (s.Cc[layer] + s.Kc[layer] * (1 + s.Oc[layer] / s.Ko[layer])); ;
            s.x_8 = 1;
            s.x_9 = 1;

            return CalcAssimilation(s, layer, canopy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="canopy"></param>
        /// <param name="layer"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static double CalcAj(SunlitShadedCanopy s, LeafCanopy canopy, int layer, TranspirationMode mode)
        {
            s.x_1 = (1 - canopy.CPath.X) * canopy.z * s.J[layer] / 3.0;
            s.x_2 = 7.0 / 3.0 * s.G_[layer];
            s.x_3 = 0;
            s.x_4 = 0;
            s.x_5 = canopy.CPath.X * canopy.z * s.J[layer] / canopy.CPath.Phi;
            s.x_6 = 0;
            s.x_7 = s.Cc[layer] * (1 - canopy.CPath.X) * canopy.z * s.J[layer] / (3 * s.Cc[layer] + 7 * s.G_[layer] * s.Oc[layer]);
            s.x_8 = 1;
            s.x_9 = 1;

            return CalcAssimilation(s, layer, canopy); ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x_1"></param>
        /// <param name="x_2"></param>
        /// <param name="x_3"></param>
        /// <param name="x_4"></param>
        /// <param name="x_5"></param>
        /// <param name="layer"></param>
        /// <param name="canopy"></param>
        /// <returns></returns>
        public static double CalcAssimilation(SunlitShadedCanopy s, int layer, LeafCanopy canopy)
        {
            s.m = s.Rm[layer];
            s.t = s.G_[layer];
            s.b = 0.1 / canopy.Constant;
            s.j = s.Gbs[layer];
            s.e = s.Om[layer];
            s.R = s.RdT[layer];

            return s.CalcAssimilation();
        }        
    }
}
