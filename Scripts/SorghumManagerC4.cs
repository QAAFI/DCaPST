-using System;
using ModelFramework;

using DCAPST;
using DCAPST.Interfaces;
using DCAPST.Utilities;
 

public class Script 
{      
   [Link]  public Simulation MySimulation;
   [Link] Paddock MyPaddock; // Can be used to dynamically get access to simulation structure and variables
   [Input] DateTime Today;   // Equates to the value of the current simulation date - value comes from CLOCK
   [Output] public double[] dcapst = new double[5];
   
   //Additional Outputs
   [Output] public double BIOtotalDAY;
   [Output] public double BIOshootDAY;
   [Output] public double RootShoot;
   [Output] public double EcanDemand; 
   [Output] public double EcanSupply;
   [Output] public double RUE;
   [Output] public double TE; 
   [Output] public double RadIntDcaps; 
   [Output] public double BIOshootDAYPot;
   [Output] public double SoilWater;
   
   public CanopyParameters CP;
   public PathwayParameters PP;
   public DCAPSTModel DM;
   
   public double LAITrigger = 0.5;
   public double PsiFactor = 0.4; //psiFactor-Psi Reduction Factor
   

   // The following event handler will be called once at the beginning of the simulation
   [EventHandler] public void OnInitialised()
   {
      CP = Classic.SetUpCanopy(
         CanopyType.C4, // canopyType
         363,     // airCO2
         0.7,     // curvatureFactor
         0.047,   // diffusivitySolubilityRatio
         210000,  // airO2
         0.78,    // diffuseExtCoeff
         0.8,     // diffuseExtCoeffNIR
         0.036,   // diffuseReflectionCoeff
         0.389,   // diffuseReflectionCoeffNIR
         60,      // leafAngle
         0.15,    // leafScatteringCoeff
         0.8,     // leafScatteringCoeffNIR
         0.15,    // leafWidth
         1.3,     // slnRatioTop
         14,      // minimumN
         1.5,     // windspeed
         1.5      // windSpeedExtinction
      );

      PP = Classic.SetUpPathway(
         0,                   // jTMin
         37.8649150880407,    // jTOpt
         55,                  // jTMax
         0.711229539802063,   // jC
         1,                   // jBeta
         0,                   // gTMin
         42,                  // gTOpt
         55,                  // gTMax
         0.462820450976839,   // gC
         1,                   // gBeta
         1210,    // KcAt25
         64200,   // KcFactor
         292000,  // KoAt25
         10500,   // KoFactor
         5.51328906454566, // VcVoAt25
         21265.4029552906, // VcVoFactor
         75,    // KpAt25
         36300, // KpFactor
         78000, // VcFactor
         46390, // RdFactor
         57043.2677590512, // VpFactor
         120,     // pepRegeneration
         0.15,    // spectralCorrectionFactor
         0.1,     // ps2ActivityFraction
         0.003,   // bundleSheathConductance
         0.465 * PsiFactor,   // maxRubiscoActivitySLNRatio
         2.7 * PsiFactor,     // maxElectronTransportSLNRatio
         0.0 * PsiFactor,     // respirationSLNRatio
         1.55 * PsiFactor,    // maxPEPcActivitySLNRatio
         0.0135 * PsiFactor,  // mesophyllCO2ConductanceSLNRatio
         2,    // extraATPCost
         0.45  // IntercellularToAirCO2Ratio
      );

      //Set the LAI trigger
      MyPaddock.Set("laiTrigger", LAITrigger);
   }
   
   // This routine is called when the plant model wants us to do the calculation
   [EventHandler] public void Ondodcapst() 
   {
      int DOY = 0;
      double latitude = 0;
      double maxT = 0;
      double minT = 0;
      double radn = 0;
      double RootShootRatio = 0;
      double SLN = 0;
      double SWAvailable = 0;
      double lai = 0;
     
      MyPaddock.Get("DOY", out DOY);
      MyPaddock.Get("latitude", out latitude);
      MyPaddock.Get("maxT", out maxT);
      MyPaddock.Get("minT", out minT);
      MyPaddock.Get("radn", out radn);
      MyPaddock.Get("RootShootRatio", out RootShootRatio);
      MyPaddock.Get("SLN", out SLN);
      MyPaddock.Get("SWAvailable", out SWAvailable);
      MyPaddock.Get("lai", out lai);
            
      // Model the photosynthesis
      DCAPSTModel DM = Classic.SetUpModel(CP, PP, DOY, latitude, maxT, minT, radn);
      DM.DailyRun(lai, SLN, SWAvailable, RootShootRatio);
      
      // Outputs
      RootShoot = RootShootRatio;
      BIOshootDAY = dcapst[0] = DM.ActualBiomass;
      BIOtotalDAY = BIOshootDAY * (1 + RootShoot);
      EcanDemand = dcapst[1] = DM.WaterDemanded; 
      EcanSupply = dcapst[2] = DM.WaterSupplied;
      RadIntDcaps = dcapst[3] = DM.InterceptedRadiation;
      RUE = (RadIntDcaps == 0 ? 0 : BIOshootDAY / RadIntDcaps);
      TE = (EcanSupply == 0 ? 0 : BIOshootDAY / EcanSupply);
      BIOshootDAYPot = dcapst[4] = DM.PotentialBiomass;
      SoilWater = SWAvailable;
   }
      
   // Set its default value to garbage so that we find out quickly
   [EventHandler] public void OnPrepare()
   {
      RootShoot = 0;
      BIOshootDAY = 0;
      BIOtotalDAY = 0;
      EcanDemand = 0; 
      EcanSupply = 0;
      RadIntDcaps = 0;
      RUE = 0;
      TE = 0;
      BIOshootDAYPot = 0;
      
      for(int i = 0; i < 5; i++) { dcapst[i] = -1.0f;}
   }
}