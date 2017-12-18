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
        public static bool CalcPhotosynthesis(this SunlitShadedCanopy s,PhotosynthesisModel PM, bool useAirTemp, int layer, double leafTemperature, double cm,
            TranspirationMode mode, double maxHourlyT, double Tfraction)
        {
            LeafCanopy canopy = PM.Canopy;

            //leafTemp[layer] = PM.envModel.getTemp(PM.time);
            s.LeafTemp__[layer] = leafTemperature;

            if (useAirTemp)
            {
                s.LeafTemp__[layer] = PM.EnvModel.GetTemp(PM.Time);
            }

            s.Cm__[layer] = cm;

            double vpd = PM.EnvModel.GetVPD(PM.Time);

            //s.VcMaxT[layer] = TempFunctionExp.Val(s.LeafTemp__[layer], s.VcMax25[layer], canopy.CPath.VcMax_c, canopy.CPath.VcMax_b);
            //s.RdT[layer] = TempFunctionExp.Val(s.LeafTemp__[layer], s.Rd25[layer], canopy.CPath.Rd_c, canopy.CPath.Rd_b);
            //s.JMaxT[layer] = TempFunctionNormal.Val(s.LeafTemp__[layer], s.JMax25[layer], canopy.CPath.JMax_TOpt, canopy.CPath.JMax_Omega);
            //s.VpMaxT[layer] = TempFunctionExp.Val(s.LeafTemp__[layer], s.VpMax25[layer], canopy.CPath.VpMax_c, canopy.CPath.VpMax_b);


            s.VcMaxT[layer] = TemperatureFunction.Val(s.LeafTemp__[layer], s.VcMax25[layer], canopy.CPath.VcMaxC, canopy.CPath.VcTMax, canopy.CPath.VcTMin, canopy.CPath.VcTOpt, canopy.CPath.beta);
            s.RdT[layer] = TemperatureFunction.Val(s.LeafTemp__[layer], s.Rd25[layer], canopy.CPath.RdC, canopy.CPath.RdTMax, canopy.CPath.RdTMin, canopy.CPath.RdTOpt, canopy.CPath.beta);
            s.JMaxT[layer] = TemperatureFunction.Val(s.LeafTemp__[layer], s.JMax25[layer], canopy.CPath.JMaxC, canopy.CPath.JTMax, canopy.CPath.JTMin, canopy.CPath.JTOpt, canopy.CPath.beta);
            s.VpMaxT[layer] = TemperatureFunction.Val(s.LeafTemp__[layer], s.VpMax25[layer], canopy.CPath.VpMaxC,  canopy.CPath.VpMaxTMax, canopy.CPath.VpMaxTMin, canopy.CPath.VpMaxTOpt, canopy.CPath.beta);


            s.Vpr[layer] = canopy.Vpr_l * s.LAIS[layer];

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

            canopy.Sco = s.ScO[layer]; //For reporting ??? 

            s.K_[layer] = s.Kc[layer] * (1 + canopy.OxygenPartialPressure / s.Ko[layer]);

//            s.Kp[layer] = TempFunctionExp.Val(s.LeafTemp__[layer], canopy.CPath.Kp_P25, canopy.CPath.Kp_c, canopy.CPath.Kp_b);
            s.Kp[layer] = TemperatureFunction.Val(s.LeafTemp__[layer], canopy.CPath.KpP25, canopy.CPath.KpC, canopy.CPath.KpTMax, canopy.CPath.KpTMin, canopy.CPath.KpTOpt, canopy.CPath.beta);

            s.Gbs[layer] = canopy.Gbs_CO2 * s.LAIS[layer];

            s.Oi[layer] = canopy.OxygenPartialPressure;

            s.Om[layer] = canopy.OxygenPartialPressure;

            if (mode == TranspirationMode.unlimited)
            {
                s.VPD[layer] = PM.EnvModel.GetVPD(PM.Time);

       //         s.gm_CO2T[layer] = s.LAIS[layer] * TempFunctionNormal.Val(s.LeafTemp__[layer], canopy.CPath.Gm_P25, canopy.CPath.Gm_TOpt, canopy.CPath.Gm_Omega);
                s.gm_CO2T[layer] = s.LAIS[layer] * TemperatureFunction.Val(s.LeafTemp__[layer], canopy.CPath.GmP25, canopy.CPath.GmC, canopy.CPath.GmTMax, canopy.CPath.GmTMin, canopy.CPath.GmTOpt, canopy.CPath.beta);


                s.Rm[layer] = s.RdT[layer] * 0.5;

                canopy.Z = (2 + canopy.FQ - canopy.CPath.Fcyc) / (canopy.H * (1 - canopy.CPath.Fcyc));

                s.Gbs[layer] = canopy.Gbs_CO2 * s.LAIS[layer];

                //Caculate A's
                if (s.type == SSType.AC1)
                {
                    s.A[layer] = CalcAc(s, canopy, layer, TranspirationMode.unlimited);
                }
                else if (s.type == SSType.AC2)
                {
                    s.A[layer] = CalcAc(s, canopy, layer, TranspirationMode.unlimited);
                }
                else if (s.type == SSType.AJ)
                {
                    s.A[layer] = CalcAj(s, canopy, layer, TranspirationMode.unlimited);
                }


                s.CalcConductanceResistance(PM, canopy);

                s.Ci[layer] = canopy.CPath.CiCaRatio * canopy.Ca;

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

                s.CalcConductanceResistance(PM, canopy);

                s.LeafTemp[layer] = s.Rbh[layer] * (s.Rn[layer] - s.Elambda[layer]) / canopy.Rcp + PM.EnvModel.GetTemp(PM.Time);

                s.VPD_la[layer] = PM.EnvModel.CalcSVP(s.LeafTemp__[layer]) - PM.EnvModel.CalcSVP(PM.EnvModel.MinT);

                s.Rsw[layer] = ((canopy.S * s.Rn[layer] + s.VPD_la[layer] * canopy.Rcp / s.Rbh[layer]) / s.Elambda[layer] - canopy.S) *
                    s.Rbh[layer] / canopy.G - s.Rbw[layer];

                s.Gsw[layer] = canopy.Rair / s.Rsw[layer] * PM.EnvModel.ATM;


                s.GsCO2[layer] = s.Gsw[layer] / 1.6;

       //         s.gm_CO2T[layer] = s.LAIS[layer] * TempFunctionNormal.Val(s.LeafTemp__[layer], canopy.CPath.Gm_P25, canopy.CPath.Gm_TOpt, canopy.CPath.Gm_Omega);
                s.gm_CO2T[layer] = s.LAIS[layer] * TemperatureFunction.Val(s.LeafTemp__[layer], canopy.CPath.GmP25, canopy.CPath.GmC, canopy.CPath.GmTMax, canopy.CPath.GmTMin, canopy.CPath.GmTOpt, canopy.CPath.beta);


                //Caculate A's
                if (s.type == SSType.AC1)
                {
                    s.A[layer] = CalcAc(s, canopy, layer, TranspirationMode.limited);
                }
                else if (s.type == SSType.AC2)
                {
                    s.A[layer] = CalcAc(s, canopy, layer, TranspirationMode.limited);
                }
                else if (s.type == SSType.AJ)
                {
                    s.A[layer] = CalcAj(s, canopy, layer, TranspirationMode.limited);
                }

                s.Cb[layer] = canopy.Ca - s.A[layer] / s.GbCO2[layer];

                s.Ci[layer] = s.Cb[layer] - s.A[layer] / s.GsCO2[layer];

                s.Cm[layer] = s.Ci[layer] - s.A[layer] / s.gm_CO2T[layer];

                s.XX = s.Cm[layer] * s.X_4 + s. X_5;

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
        public static double CalcAj(SunlitShadedCanopy s, LeafCanopy canopy, int layer, TranspirationMode mode)
        {
            double assimilation = 0;
            double x_1 = (1 - canopy.CPath.X) * s.J[layer] / 3.0;
            double x_2 = 7.0 / 3.0 * s.G_[layer];
            double x_3 = 0;
            double x_4 = 0;
            double x_5 = canopy.CPath.X * s.J[layer] / 2.0;

            if (mode == TranspirationMode.unlimited)
            {
                assimilation = CalcAssimilation(s, x_1, x_2, x_3, x_4, x_5, layer, canopy);
            }
            else
            {
                assimilation = CalcAssimilationDiffusion(s, x_1, x_2, x_3, x_4, x_5, layer, canopy);
            }

            return assimilation;
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
        public static double CalcAssimilation(SunlitShadedCanopy s, double x_1, double x_2, double x_3, double x_4, double x_5, int layer, LeafCanopy canopy)
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

            double x = canopy.CPath.CiCaRatio;
            double C_i = C_a * canopy.CPath.CiCaRatio;

            a = -C_a * x * 0.047 * g_m * g_bs - C_a * x * 0.047 * g_m * x_4 - α * g_m * R_d * x_2 - α * g_m * γ_ * x_1 - O_m * 0.047 * g_m * g_bs * x_2 - 0.047 * g_m * g_bs * x_3 + 0.047 * g_m * R_m + 0.047 * g_m * R_d - 0.047 * g_m * x_1 - 0.047 * g_m * x_5 + 0.047 * g_bs * R_d - 0.047 * g_bs * x_1 + 0.047 * R_d * x_4 - 0.047 * x_1 * x_4; // Eq   (A56)
            b = (-α * g_m * x_2 + 0.047 * g_m + 0.047 * g_bs + 0.047 * x_4) * (-C_a * x * 0.047 * g_m * g_bs * R_d + C_a * x * 0.047 * g_m * g_bs * x_1 - C_a * x * 0.047 * g_m * R_d * x_4 + C_a * x * 0.047 * g_m * x_1 * x_4 - O_m * 0.047 * g_m * g_bs * R_d * x_2 - 0.047 * g_m * g_bs * R_d * x_3 - O_m * 0.047 * g_m * g_bs * γ_ * x_1 + 0.047 * g_m * R_m - 0.047 * g_m * R_m * x_1 - 0.047 * g_m * R_d * x_5 + 0.047 * g_m * x_1 * x_5); // Eq	(A57)
            c = C_a * x * 0.047 * g_m * g_bs + C_a * x * 0.047 * g_m * x_4 + α * g_m * R_d * x_2 + α * g_m * γ_ * x_1 + O_m * 0.047 * g_m * g_bs * x_2 + 0.047 * g_m * g_bs * x_3 - 0.047 * g_m * R_m - 0.047 * g_m * R_d + 0.047 * g_m * x_1 + 0.047 * g_m * x_5 - 0.047 * g_bs * R_d + 0.047 * g_bs * x_1 - 0.047 * R_d * x_4 + 0.047 * x_1 * x_4; // Eq(A58)
            d = -α * g_m * x_2 + 0.047 * g_m + 0.047 * g_bs + 0.047 * x_4;  // Eq (A59)

            return s.SolveQuadratic(a, b, c, d); //Eq (A55)
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
        public static double CalcAssimilationDiffusion(SunlitShadedCanopy s, double x_1, double x_2, double x_3, double x_4, double x_5, int layer, LeafCanopy canopy)
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

            a = -C_a * 0.047 * g_bs * g_s * g_m * g_b - C_a * 0.047 * x_4 * g_s * g_m * g_b - α * R_d * x_2 * g_s * g_m * g_b - α * γ_ * x_1 * g_s * g_m * g_b + 0.047 * g_bs * R_d * g_s * g_b + 0.047 * g_bs * R_d * g_m * g_b - 0.047 * g_bs * x_1 * g_s * g_b - 0.047 * g_bs * x_1 * g_m * g_b - O_m * 0.047 * g_bs * x_2 * g_s * g_m * g_b - 0.047 * g_bs * x_3 * g_s * g_m * g_b + 0.047 * R_m * g_s * g_m * g_b + 0.047 * R_d * x_4 * g_s * g_m + 0.047 * R_d * x_4 * g_s * g_b + 0.047 * R_d * x_4 * g_m * g_b + 0.047 * R_d * g_s * g_m * g_b - 0.047 * x_1 * x_4 * g_s * g_m - 0.047 * x_1 * x_4 * g_s * g_b - 0.047 * x_1 * x_4 * g_m * g_b - 0.047 * x_1 * g_s * g_m * g_b - 0.047 * x_5 * g_s * g_m * g_b;
            b = (-α * x_2 * g_s * g_m * g_b + 0.047 * g_bs * g_s * g_b + 0.047 * g_bs * g_m * g_b + 0.047 * x_4 * g_s * g_m + 0.047 * x_4 * g_s * g_b + 0.047 * x_4 * g_m * g_b + 0.047 * g_s * g_m * g_b) * (-C_a * 0.047 * g_bs * R_d * g_s * g_m * g_b + C_a * 0.047 * g_bs * x_1 * g_s * g_m * g_b - C_a * 0.047 * R_d * x_4 * g_s * g_m * g_b + C_a * 0.047 * x_1 * x_4 * g_s * g_m * g_b - O_m * 0.047 * g_bs * R_d * x_2 * g_s * g_m * g_b - 0.047 * g_bs * R_d * x_3 * g_s * g_m * g_b - O_m * 0.047 * g_bs * γ_ * x_1 * g_s * g_m * g_b + 0.047 * R_m * R_d * g_s * g_m * g_b - 0.047 * R_m * x_1 * g_s * g_m * g_b - 0.047 * R_d * x_5 * g_s * g_m * g_b + 0.047 * x_1 * x_5 * g_s * g_m * g_b);
            c = C_a * 0.047 * g_bs * g_s * g_m * g_b + C_a * 0.047 * x_4 * g_s * g_m * g_b + α * R_d * x_2 * g_s * g_m * g_b + α * γ_ * x_1 * g_s * g_m * g_b - 0.047 * g_bs * R_d * g_s * g_b - 0.047 * g_bs * R_d * g_m * g_b + 0.047 * g_bs * x_1 * g_s * g_b + 0.047 * g_bs * x_1 * g_m * g_b + O_m * 0.047 * g_bs * x_2 * g_s * g_m * g_b + 0.047 * g_bs * x_3 * g_s * g_m * g_b - 0.047 * R_m * g_s * g_m * g_b - 0.047 * R_d * x_4 * g_s * g_m - 0.047 * R_d * x_4 * g_s * g_b - 0.047 * R_d * x_4 * g_m * g_b - 0.047 * R_d * g_s * g_m * g_b + 0.047 * x_1 * x_4 * g_s * g_m + 0.047 * x_1 * x_4 * g_s * g_b + 0.047 * x_1 * x_4 * g_m * g_b + 0.047 * x_1 * g_s * g_m * g_b + 0.047 * x_5 * g_s * g_m * g_b;
            d = -α * x_2 * g_s * g_m * g_b + 0.047 * g_bs * g_s * g_b + 0.047 * g_bs * g_m * g_b + 0.047 * x_4 * g_s * g_m + 0.047 * x_4 * g_s * g_b + 0.047 * x_4 * g_m * g_b + 0.047 * g_s * g_m * g_b;

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
            double x_2 = s.Kc[layer] / s.Ko[layer];
            double x_3 = s.Kc[layer];

            double x_4 = 0;
            double x_5 = 0;

            if (s.type == SSType.AC1)
            {
                x_4 = s.VpMaxT[layer] / (s.Cm__[layer] + s.Kp[layer]); //Delta Eq (A56)
                x_5 = 0;
            }
            else if (s.type == SSType.AC2)
            {
                x_4 = 0;
                x_5 = s.Vpr[layer];
            }

            if (mode == TranspirationMode.unlimited)
            {
                assimilation = CalcAssimilation(s, x_1, x_2, x_3, x_4, x_5, layer, canopy);
            }
            else
            {
                assimilation = CalcAssimilationDiffusion(s, x_1, x_2, x_3, x_4, x_5, layer, canopy);
            }

            return assimilation;
        }
    }
}
