using System;
using LayerCanopyPhotosynthesis;
using System.Linq;

namespace C4MethodExtensions
{
    public static class C4MethodExtensions
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
        public static bool CalcPhotosynthesis(this SunlitShadedCanopy s, PhotosynthesisModel PM, bool useAirTemp, int layer, double leafTemperature, double cm,
            TranspirationMode mode, double maxHourlyT, double Tfraction)
        {
            double p, q;

            LeafCanopy canopy = PM.Canopy;

            //leafTemp[layer] = PM.envModel.getTemp(PM.time);
            s.LeafTemp__[layer] = leafTemperature;

            if (useAirTemp)
            {
                s.LeafTemp__[layer] = PM.EnvModel.GetTemp(PM.Time);
            }

            s.Cm__[layer] = cm;

            double vpd = PM.EnvModel.GetVPD(PM.Time);

            s.VcMaxT[layer] = TemperatureFunction.Val2(s.LeafTemp__[layer], s.VcMax25[layer], canopy.CPath.VcTMin);
            s.RdT[layer] = TemperatureFunction.Val2(s.LeafTemp__[layer], s.Rd25[layer], canopy.CPath.RdTMin);
            s.JMaxT[layer] = TemperatureFunction.Val(s.LeafTemp__[layer], s.JMax25[layer], canopy.CPath.JMaxC, canopy.CPath.JTMax, canopy.CPath.JTMin, canopy.CPath.JTOpt, canopy.CPath.JBeta);
            s.VpMaxT[layer] = TemperatureFunction.Val2(s.LeafTemp__[layer], s.VpMax25[layer], canopy.CPath.VpMaxTMin);


            s.Vpr[layer] = canopy.Vpr_l * s.LAIS[layer];

            canopy.Ja = (1 - canopy.F) / 2;

            s.J[layer] = (canopy.Ja * s.AbsorbedIrradiance[layer] + s.JMaxT[layer] - Math.Pow(Math.Pow(canopy.Ja * s.AbsorbedIrradiance[layer] + s.JMaxT[layer], 2) -
            4 * canopy.Theta * s.JMaxT[layer] * canopy.Ja * s.AbsorbedIrradiance[layer], 0.5)) / (2 * canopy.Theta);

            s.Kc[layer] = TemperatureFunction.Val2(s.LeafTemp__[layer], canopy.CPath.KcP25, canopy.CPath.KcTMin);
            s.Ko[layer] = TemperatureFunction.Val2(s.LeafTemp__[layer], canopy.CPath.KoP25, canopy.CPath.KoTMin);
            s.VcVo[layer] = TemperatureFunction.Val2(s.LeafTemp__[layer], canopy.CPath.VcMax_VoMaxP25, canopy.CPath.VcMax_VoMaxTMin);

            s.ScO[layer] = s.Ko[layer] / s.Kc[layer] * s.VcVo[layer];

            s.G_[layer] = 0.5 / s.ScO[layer];

            canopy.Sco = s.ScO[layer]; //For reporting ??? 

            s.K_[layer] = s.Kc[layer] * (1 + canopy.OxygenPartialPressure / s.Ko[layer]);

            s.Kp[layer] = TemperatureFunction.Val2(s.LeafTemp__[layer], canopy.CPath.KpP25, canopy.CPath.KpTMin);

            s.Gbs[layer] = canopy.Gbs_CO2 * s.LAIS[layer];

            s.Oi[layer] = canopy.OxygenPartialPressure;

            s.Om[layer] = canopy.OxygenPartialPressure;

            if (mode == TranspirationMode.unlimited)
            {
                s.VPD[layer] = PM.EnvModel.GetVPD(PM.Time);

                s.gm_CO2T[layer] = s.LAIS[layer] * TemperatureFunction.Val(s.LeafTemp__[layer], canopy.CPath.GmP25, canopy.CPath.GmC, canopy.CPath.GmTMax, canopy.CPath.GmTMin, canopy.CPath.GmTOpt, canopy.CPath.GmBeta);

                s.Rm[layer] = s.RdT[layer] * 0.5;

                s.Gbs[layer] = canopy.Gbs_CO2 * s.LAIS[layer];

                s.Ci[layer] = canopy.CPath.CiCaRatio * canopy.Ca;

                p = s.Ci[layer];

                q = 1 / s.gm_CO2T[layer];

                //Caculate A's
                if (s.type == SSType.AJ)
                {
                    s.A[layer] = CalcAj(s, canopy, layer, TranspirationMode.unlimited, p, q);
                }
                else
                {
                    s.A[layer] = CalcAc(s, canopy, layer, TranspirationMode.unlimited, p, q);
                }

                s.CalcConductanceResistance(PM, canopy);

                s.Cm[layer] = s.Ci[layer] - s.A[layer] / s.gm_CO2T[layer];

                s.XX = s.Cm[layer] * s.X_4 + s.X_5;

                if (s.type == SSType.AC1 || s.type == SSType.AC2)
                {
                    s.Vp[layer] = Math.Min(s.Cm[layer] * s.VpMaxT[layer] / (s.Cm[layer] + s.Kp[layer]), s.Vpr[layer]);
                }
                else if (s.type == SSType.AJ)
                {
                    s.Vp[layer] = canopy.CPath.X * s.J[layer] / 2;
                }

                s.Oc[layer] = canopy.Alpha * s.A[layer] / (0.047 * s.Gbs[layer]) + s.Om[layer];

                s.r_[layer] = s.G_[layer] * s.Oc[layer];

                s.Cc[layer] = s.Cm[layer] + (s.XX - s.A[layer] - s.Rm[layer]) / s.Gbs[layer];

                if (s.Cc[layer] < 0 || double.IsNaN(s.Cc[layer]))
                {
                    s.Cc[layer] = 0;
                }

                s.F[layer] = s.Gbs[layer] * (s.Cc[layer] - s.Cm[layer]) / s.XX;

                s.DoWaterInteraction(PM, canopy, mode);
            }

            else if (mode == TranspirationMode.limited)
            {
                s.WaterUse[layer] = maxHourlyT * Tfraction;

                s.Elambda[layer] = s.WaterUse[layer] / (0.001 * 3600) * canopy.Lambda / 1000;

                double totalAbsorbed = s.AbsorbedIrradiancePAR[layer] + s.AbsorbedIrradianceNIR[layer];
                s.Rn[layer] = totalAbsorbed - 2 * (canopy.Sigma * Math.Pow(273 + s.LeafTemp__[layer], 4) - canopy.Sigma * Math.Pow(273 + PM.EnvModel.GetTemp(PM.Time), 4));

                s.CalcConductanceResistance(PM, canopy);

                s.DoWaterInteraction(PM, canopy, mode);

                s.gm_CO2T[layer] = s.LAIS[layer] * TemperatureFunction.Val(s.LeafTemp__[layer], canopy.CPath.GmP25, canopy.CPath.GmC, canopy.CPath.GmTMax, canopy.CPath.GmTMin, canopy.CPath.GmTOpt, canopy.CPath.GmBeta);

                double Gt = 1 / (1 / s.GbCO2[layer] + 1 / s.GsCO2[layer]);

                p = canopy.Ca - s.WaterUseMolsSecond[layer] * canopy.Ca / (Gt + s.WaterUseMolsSecond[layer]/2);
                q = 1 / (Gt + s.WaterUseMolsSecond[layer] / 2) + 1 / s.gm_CO2T[layer];

                //Caculate A's
                if (s.type == SSType.AJ)
                {
                    s.A[layer] = CalcAj(s, canopy, layer, TranspirationMode.limited, p, q);
                }
                else
                {
                    s.A[layer] = CalcAc(s, canopy, layer, TranspirationMode.limited, p, q);
                }

                s.Cb[layer] = canopy.Ca - s.A[layer] / s.GbCO2[layer];

                s.Ci[layer] = ((Gt - s.WaterUseMolsSecond[0] / 2) * canopy.Ca - s.A[layer]) / (Gt + s.WaterUseMolsSecond[0] / 2);

                s.Cm[layer] = s.Ci[layer] - s.A[layer] / s.gm_CO2T[layer];

                s.XX = s.Cm[layer] * s.X_4 + s.X_5;

                s.Oc[layer] = canopy.Alpha * s.A[layer] / (0.047 * s.Gbs[layer]) + s.Om[layer];

                s.r_[layer] = s.G_[layer] * s.Oc[layer];

                s.Cc[layer] = s.Cm[layer] + (s.XX - s.A[layer] - s.Rm[layer]) / s.Gbs[layer];

                if (s.Cc[layer] < 0 || double.IsNaN(s.Cc[layer]))
                {
                    s.Cc[layer] = 0;
                }

                s.F[layer] = s.Gbs[layer] * (s.Cc[layer] - s.Cm[layer]) / s.XX;

            }

            double airTemp = PM.EnvModel.GetTemp(PM.Time);

            if (useAirTemp)
            {
                s.LeafTemp[layer] = PM.EnvModel.GetTemp(PM.Time);
            }

            double diffCm = (s.type == SSType.AC1 ? Math.Abs(s.Cm__[layer] - s.Cm[layer]) : 0);
            double diffTemp = s.LeafTemp__[layer] - s.LeafTemp[layer];


            s.LeafTemp[layer] = (s.LeafTemp[layer] + s.LeafTemp__[layer]) / 2;

            s.Cm[layer] = (s.Cm[layer] + s.Cm__[layer]) / 2;

            if ((Math.Abs(diffCm) > s.CmTolerance) ||
                (Math.Abs(diffTemp) > s.leafTempTolerance) ||
                double.IsNaN(s.Cm[layer]) || double.IsNaN(s.LeafTemp[layer]))
            {
                return false;
            }
            return true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="canopy"></param>
        /// <param name="layer"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static double CalcAj(SunlitShadedCanopy s, LeafCanopy canopy, int layer, TranspirationMode mode, double p, double q)
        {
            double x_1, x_2, x_3, x_4, x_5, x_6, x_7;
            x_1 = (1 - canopy.CPath.X) * s.J[layer] / 3.0;
            x_2 = 7.0 / 3.0 * s.G_[layer];
            x_3 = 0;
            x_4 = 0;
            x_5 = canopy.CPath.X * s.J[layer] / canopy.CPath.Phi;
            x_6 = 1;
            x_7 = 1;

            s.X_4 = x_4;
            s.X_5 = x_5;

            return CalcAssimilation(s, x_1, x_2, x_3, x_4, x_5, x_6, x_7, p, q, layer, canopy); ;
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
        public static double CalcAssimilation(SunlitShadedCanopy s, double x_1, double x_2, double x_3, double x_4, double x_5, double x_6, double x_7, double p, double q, int layer, LeafCanopy canopy)
        {
            double a, b, c, d;

            double g_m = s.gm_CO2T[layer];
            double g_bs = s.Gbs[layer];
            double α = canopy.Alpha;
            double R_d = s.RdT[layer];
            double γ_ = s.G_[layer];
            double O_m = s.Om[layer];
            double R_m = s.Rm[layer];

            double C_a = canopy.Ca;
            double g_s = s.GsCO2[layer];
            double g_b = s.GbCO2[layer];


            a = -1 * (p * g_bs + p * x_4 * x_6 + α / (0.047 * g_bs) * g_bs * x_2 * x_7 * R_d + α / (0.047 * g_bs) * g_bs * x_1 * x_7 * γ_ -
                q * g_bs * R_d + q * g_bs * x_1 + g_bs * O_m * x_2 + g_bs * x_3 - x_6 * R_m - q * x_4 * x_6 * R_d - x_6 * R_d + q * x_1 * x_4 * x_6 +
                x_1 * x_6 + x_5 * x_6);
            d = -α / (0.047 * g_bs) * g_bs * x_2 * x_7 + q * g_bs + q * x_4 * x_6 + x_6;
            b = -d * (p * g_bs * R_d - p * g_bs * x_1 + p * x_4 * x_6 * R_d - p * x_1 * x_4 * x_6 + g_bs * x_2 * O_m * R_d + g_bs * x_3 * R_d +
              g_bs * x_1 * γ_ * O_m - x_6 * R_m * R_d + x_1 * x_6 * R_m + x_5 * x_6 * R_d - x_1 * x_5 * x_6);
            c = -a;

            return s.SolveQuadratic(a, b, c, d); //Eq (A55)
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="canopy"></param>
        /// <param name="layer"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static double CalcAc(SunlitShadedCanopy s, LeafCanopy canopy, int layer, TranspirationMode mode, double p, double q)
        {
            double x_1, x_2, x_3, x_4, x_5, x_6, x_7;
            x_1 = s.VcMaxT[layer];
            x_2 = s.Kc[layer] / s.Ko[layer];
            x_3 = s.Kc[layer];
            x_4 = 0;
            x_5 = 0;
            x_6 = 1;
            x_7 = 1;

            if (s.type == SSType.AC1)
            {
                x_4 = s.VpMaxT[layer] / (s.Cm__[layer] + s.Kp[layer]); //Delta Eq (A56)
                                                                       //x_5 = 0;
            }
            else if (s.type == SSType.AC2)
            {
                x_5 = s.Vpr[layer];
            }

            s.X_4 = x_4;
            s.X_5 = x_5;

            return CalcAssimilation(s, x_1, x_2, x_3, x_4, x_5, x_6, x_7, p, q, layer, canopy);
        }
    }
}
