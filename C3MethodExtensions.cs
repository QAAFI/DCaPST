using System;
using LayerCanopyPhotosynthesis;
using System.Linq;

namespace C3MethodExtensions
{

    public static class C3MethodExtensions
    {

        //---------------------------------------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="PM"></param>
        /// <param name="useAirTemp"></param>
        /// <param name="layer"></param>
        /// <param name="leafTemperature"></param>
        /// <param name="mode"></param>
        /// <param name="maxHourlyT"></param>
        /// <param name="Tfraction"></param>
        /// <returns></returns>
        public static bool CalcPhotosynthesis(this SunlitShadedCanopy s, PhotosynthesisModel PM, bool useAirTemp, int layer, double leafTemperature,
            TranspirationMode mode, double maxHourlyT, double Tfraction)
        { 

            LeafCanopy canopy = PM.Canopy;

            //calcPhotosynthesis(PM, layer);

            s.Oi[layer] = canopy.OxygenPartialPressure;

            s.Om[layer] = canopy.OxygenPartialPressure;

            s.Oc[layer] = s.Oi[layer];

            s.LeafTemp__[layer] = leafTemperature;

            if (useAirTemp)
            {
                s.LeafTemp__[layer] = PM.EnvModel.GetTemp(PM.Time);
            }

            s.CalcConductanceResistance(PM, canopy);

            //s.VcMaxT[layer] = TempFunctionExp.Val(s.LeafTemp__[layer], s.VcMax25[layer], canopy.CPath.VcMax_c, canopy.CPath.VcMax_b);
            //s.RdT[layer] = TempFunctionExp.Val(s.LeafTemp__[layer], s.Rd25[layer], canopy.CPath.Rd_c, canopy.CPath.Rd_b);
            //s.JMaxT[layer] = TempFunctionNormal.Val(s.LeafTemp__[layer], s.JMax25[layer], canopy.CPath.JMax_TOpt, canopy.CPath.JMax_Omega);
            ////s.VpMaxT[layer] = TempFunctionExp.val(s.leafTemp__[layer], s.VpMax25[layer], canopy.CPath.VpMax_c, canopy.CPath.VpMax_b);

            s.VcMaxT[layer] = TemperatureFunction.Val(s.LeafTemp__[layer], s.VcMax25[layer], canopy.CPath.VcMaxC, canopy.CPath.VcTMax, canopy.CPath.VcTMin, canopy.CPath.VcTOpt, canopy.CPath.beta);
            s.RdT[layer] = TemperatureFunction.Val(s.LeafTemp__[layer], s.Rd25[layer], canopy.CPath.RdC, canopy.CPath.RdTMax, canopy.CPath.RdTMin, canopy.CPath.RdTOpt, canopy.CPath.beta);
            s.JMaxT[layer] = TemperatureFunction.Val(s.LeafTemp__[layer], s.JMax25[layer], canopy.CPath.JMaxC, canopy.CPath.JTMax, canopy.CPath.JTMin, canopy.CPath.JTOpt, canopy.CPath.beta);



            // s.Vpr[layer] = canopy.Vpr_l * s.LAIS[layer];///

            canopy.Ja = (1 - canopy.F) / 2;

            s.J[layer] = (canopy.Ja * s.AbsorbedIrradiance[layer] + s.JMaxT[layer] - Math.Pow(Math.Pow(canopy.Ja * s.AbsorbedIrradiance[layer] + s.JMaxT[layer], 2) -
            4 * canopy.θ * s.JMaxT[layer] * canopy.Ja * s.AbsorbedIrradiance[layer], 0.5)) / (2 * canopy.θ);

            //s.Kc[layer] = TempFunctionExp.Val(s.LeafTemp__[layer], canopy.CPath.Kc_P25, canopy.CPath.Kc_c, canopy.CPath.Kc_b);
            //s.Ko[layer] = TempFunctionExp.Val(s.LeafTemp__[layer], canopy.CPath.Ko_P25, canopy.CPath.Ko_c, canopy.CPath.Ko_b);
            //s.VcVo[layer] = TempFunctionExp.Val(s.LeafTemp__[layer], canopy.CPath.VcMax_VoMax_P25, canopy.CPath.VcMax_VoMax_c, canopy.CPath.VcMax_VoMax_b);

            s.Kc[layer] = TemperatureFunction.Val(s.LeafTemp__[layer], canopy.CPath.KcP25, canopy.CPath.KcC, canopy.CPath.KcTMax, canopy.CPath.KcTMin, canopy.CPath.KcTOpt, canopy.CPath.beta);
            s.Ko[layer] = TemperatureFunction.Val(s.LeafTemp__[layer], canopy.CPath.KoP25, canopy.CPath.KoC, canopy.CPath.KoTMax, canopy.CPath.KoTMin, canopy.CPath.KoTOpt, canopy.CPath.beta);
            s.VcVo[layer] = TemperatureFunction.Val(s.LeafTemp__[layer], canopy.CPath.VcMax_VoMaxP25, canopy.CPath.VcMax_VoMaxC, canopy.CPath.VcMax_VoMaxTMax, canopy.CPath.VcMax_VoMaxTMin, canopy.CPath.VcMax_VoMaxTOpt, canopy.CPath.beta);


            s.ScO[layer] = s.Ko[layer] / s.Kc[layer] * s.VcVo[layer];

            s.G_[layer] = 0.5 / s.ScO[layer];

            s.r_[layer] = s.G_[layer] * s.Oc[layer];

            canopy.Sco = s.ScO[layer]; //For reporting ??? 

//            s.gm_CO2T[layer] = s.LAIS[layer] * TempFunctionNormal.Val(s.LeafTemp__[layer], canopy.CPath.Gm_P25, canopy.CPath.Gm_TOpt, canopy.CPath.Gm_Omega);
            s.gm_CO2T[layer] = s.LAIS[layer] * TemperatureFunction.Val(s.LeafTemp__[layer], canopy.CPath.GmP25, canopy.CPath.GmC, canopy.CPath.GmTMax , canopy.CPath.GmTMin , canopy.CPath.GmTOpt, canopy.CPath.beta);


            if (mode == TranspirationMode.unlimited)
            {
                //Caculate A's
                if (s.type == SSType.AC1)
                {
                    s.A[layer] = CalcAc(s, canopy, layer, TranspirationMode.unlimited);
                }
                else if (s.type == SSType.AJ)
                {
                    s.A[layer] = CalcAj(s, canopy, layer, TranspirationMode.unlimited);
                }

                if (s.A[layer] < 0 || double.IsNaN(s.A[layer]))
                {
                    s.A[layer] = 0;
                }

                if (PM.conductanceModel == PhotosynthesisModel.ConductanceModel.DETAILED)
                {
                    s.Ci[layer] = canopy.Ca - s.A[layer] / s.Gb_CO2[layer] - s.A[layer] / s.gs_CO2[layer];
                }
                else
                {
                    s.Ci[layer] = canopy.CPath.CiCaRatio * canopy.Ca;
                }

                s.Cc[layer] = s.Ci[layer] - s.A[layer] / s.gm_CO2T[layer];

                if (s.Cc[layer] < 0 || double.IsNaN(s.Cc[layer]))
                {
                    s.Cc[layer] = 0;
                }

                s.CiCaRatio[layer] = s.Ci[layer] / canopy.Ca;

                s.CalcWaterUse(PM, canopy);

                s.TDelta[layer] = s.Rbh[layer] * (s.Rn[layer] - s.Elambda_[layer]) / canopy.Rcp;

                s.LeafTemp[layer] = PM.EnvModel.GetTemp(PM.Time) + s.TDelta[layer];
            }

            else if (mode == TranspirationMode.limited)
            {
                double supplymmhr = maxHourlyT * Tfraction;

                s.Elambda[layer] = supplymmhr / (0.001 * 3600) * canopy.Lambda / 1000;

                double totalAbsorbed = s.AbsorbedIrradiancePAR[layer] + s.AbsorbedIrradianceNIR[layer];
                s.Rn[layer] = totalAbsorbed - 2 * (canopy.Sigma * Math.Pow(273 + s.LeafTemp__[layer], 4) - canopy.Sigma * Math.Pow(273 + PM.EnvModel.GetTemp(PM.Time), 4));

                s.LeafTemp[layer] = s.Rbh[layer] * (s.Rn[layer] - s.Elambda[layer]) / canopy.Rcp + PM.EnvModel.GetTemp(PM.Time);

                s.VPD_la[layer] = PM.EnvModel.CalcSVP(s.LeafTemp__[layer]) - PM.EnvModel.CalcSVP(PM.EnvModel.MinT);

                s.Rsw[layer] = ((canopy.S * s.Rn[layer] + s.VPD_la[layer] * canopy.Rcp / s.Rbh[layer]) / s.Elambda[layer] - canopy.S) *
                    s.Rbh[layer] / canopy.G - s.Rbw[layer];

                s.Gsw[layer] = canopy.Rair / s.Rsw[layer] * PM.EnvModel.ATM;


                s.GsCO2[layer] = s.Gsw[layer] / 1.6;

            //    s.gm_CO2T[layer] = s.LAIS[layer] * TempFunctionNormal.Val(s.LeafTemp__[layer], canopy.CPath.Gm_P25, canopy.CPath.Gm_TOpt, canopy.CPath.Gm_Omega);
                s.gm_CO2T[layer] = s.LAIS[layer] * TemperatureFunction.Val(s.LeafTemp__[layer], canopy.CPath.GmP25, canopy.CPath.GmC, canopy.CPath.GmTMax, canopy.CPath.GmTMin, canopy.CPath.GmTOpt, canopy.CPath.beta);

                //Caculate A's
                if (s.type == SSType.AC1)
                {
                    s.A[layer] = CalcAc(s, canopy, layer, TranspirationMode.limited);
                }
                else if (s.type == SSType.AJ)
                {
                    s.A[layer] = CalcAj(s, canopy, layer, TranspirationMode.limited);
                }

                s.Cb[layer] = canopy.Ca - s.A[layer] / s.GbCO2[layer];

                s.Ci[layer] = s.Cb[layer] - s.A[layer] / s.GsCO2[layer];

            }

            double airTemp = PM.EnvModel.GetTemp(PM.Time);

            if (useAirTemp)
            {
                s.LeafTemp[layer] = PM.EnvModel.GetTemp(PM.Time);
            }

            double diffTemp = s.LeafTemp__[layer] - s.LeafTemp[layer];
              

            s.LeafTemp[layer] = (s.LeafTemp[layer] + s.LeafTemp__[layer]) / 2;

            if ((Math.Abs(diffTemp) > s.leafTempTolerance) || double.IsNaN(s.LeafTemp[layer]))
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
        public static double CalcAj(SunlitShadedCanopy s, LeafCanopy canopy, int layer, TranspirationMode mode)
        {
            double assimilation = 0;
            double x_1 = s.J[layer] / 4;
            double x_2 = 2 * s.r_[layer];

            if (mode == TranspirationMode.unlimited)
            {
                assimilation = CalcAssimilation(s, x_1, x_2, layer, canopy);
            }
            else
            {
                assimilation = CalcAssimilationDiffusion(s, x_1, x_2, layer, canopy);
            }

            return assimilation;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x_1"></param>
        /// <param name="x_2"></param>
        /// <param name="layer"></param>
        /// <param name="canopy"></param>
        /// <returns></returns>
        public static double CalcAssimilation(SunlitShadedCanopy s, double x_1, double x_2, int layer, LeafCanopy canopy)
        {
            double a, b, c, d;

            double g_m = s.gm_CO2T[layer];
            double R_d = s.RdT[layer];
            double C_a = canopy.Ca;
            double Γ_ = s.r_[layer];
            double x = canopy.CPath.CiCaRatio;
            double C_i = C_a * canopy.CPath.CiCaRatio;

            a = -C_i / C_a * C_a * g_m - g_m * x_2 + R_d - x_1;  // (A3)
            b = -C_i / C_a * C_a * g_m * R_d + C_i / C_a * C_a * g_m * x_1 - g_m * R_d * x_2 - g_m * Γ_ * x_1;  //   (A4)
            c = C_i / C_a * C_a * g_m + g_m * x_2 - R_d + x_1;   // (A5)
            d = 1;

            return s.SolveQuadratic(a, b, c, d); //Eq (A55)
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x_1"></param>
        /// <param name="x_2"></param>
        /// <param name="layer"></param>
        /// <param name="canopy"></param>
        /// <returns></returns>
        public static double CalcAssimilationDiffusion(SunlitShadedCanopy s, double x_1, double x_2, int layer, LeafCanopy canopy)
        {
            double a, b, c, d;

            double g_m = s.gm_CO2T[layer];
            double R_d = s.RdT[layer];
            double C_a = canopy.Ca;
            double g_s = s.GsCO2[layer];
            double g_b = s.GbCO2[layer];
            double Γ_ = s.r_[layer];

            a = -C_a * g_b * g_s * g_m + g_b * g_m * R_d - g_b * g_m * g_s * x_2 - g_b * g_m * x_1 + g_b * g_s * R_d - g_b * g_s * x_1 + g_m * g_s * R_d - g_m * g_s * x_1;
            b = (g_b * g_m + g_b * g_s + g_s * g_m) * (-C_a * g_b * g_s * g_m * R_d + C_a * g_b * g_s * g_m * x_1 - g_b * g_s * g_m * R_d * x_2 - g_b * g_s * g_m * Γ_ * x_1);
            c = C_a * g_b * g_s * g_m - g_b * g_m * R_d + g_b * g_m * g_s * x_2 + g_b * g_m * x_1 - g_b * g_s * R_d + g_b * g_s * x_1 - g_s * g_m * R_d + g_s * g_m * x_1;
            d = g_b * g_m + g_b * g_s + g_s * g_m;

            return s.SolveQuadratic(a, b, c, d); //Eq (A55)
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="canopy"></param>
        /// <param name="layer"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static double CalcAc(SunlitShadedCanopy s, LeafCanopy canopy, int layer, TranspirationMode mode)
        {
            double assimilation;

            double x_1 = s.VcMaxT[layer];
            double x_2 = s.Kc[layer] * (1 + canopy.OxygenPartialPressure / s.Ko[layer]);

            if (mode == TranspirationMode.unlimited)
            {
                assimilation = CalcAssimilation(s, x_1, x_2, layer, canopy);
            }
            else
            {
                assimilation = CalcAssimilationDiffusion(s, x_1, x_2, layer, canopy);
            }

            return assimilation;
        }
    }
}
