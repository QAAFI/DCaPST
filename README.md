# DCaPST Package 
##*(Diurnal Canopy Photosynthesis-Stomatal Conductance Simulator )*

## About DCaPST
Diurnal C3 and C4 canopy photosynthesis-stomatal conductance modules (DCaPST) were developed and connected with the wheat and sorghum crop models of the Agricultural Production Systems sIMulator (APSIM v7.8). This connection was facilitated by partitioning the crop canopy into sunlit and shade leaf fractions. The cross-scale model involved (1) the crop models determining relevant crop attributes, such as leaf area index and specific leaf nitrogen (or leaf nitrogen content), and extent of soil water uptake for transpiration supply, and (2) leaf and canopy photosynthesis models determining potential daily canopy biomass increment and transpiration demand. The dynamic interplay of these components with the prevailing environment allowed the simulation of crop growth, development and yield through the crop life cycle.

## System Reqirements

>####*Minimum System Requirements*
> 1 GHz CPU 

> 512MB RAM

>#### *Recommended System Requirements*
>Quad Core 3GHz + CPU

>4GB RAM


## Software Requirements

This software runs on the Windows operating system.

>#### *Minimum Operating System*
>Windows XP

>#### *Recommended Operating System*
>Windows 10

## Installation Guide
The DCaPST (Diurnal Canopy Photosynthesis-Stomatal Conductance Simulator) model works in conjunction with the The Agricultural Production Systems Simulator (APSIM : [www.apsim.info](http:www.apsim.info)), inparticular APSIM version 7.8. 

>#### *Instructions*
>To install this software and utilise the demonstratation files, the following procedure should be followed

* Register and download APSIM

 https://www.apsim.info/APSIM.Registration.Portal/Main.aspx </br>
  Register V7.8
  


* Download and INstall patch


>#### *Typical install time*
* Installing APSIM
* INstallin the patch

## Demo
After the patch is installed a folder named 'PSManipulationDemo' will be available in the directory that you installed APSIM v7.8 into. This folder will contain a sample 'apsim' file called 'PS perturbation sims', this file is an input file to the APSIM user interface that can be launched via (start menu ) or double click file.

Training modules for using the APSIM User interface can be found by clicking this link, [APSIM Training Modules](https://www.apsim.info/Documentation/TrainingManualsandResources/APSIMGeneralTrainingManual.aspx)

### How to run the software on data

### Expected output

### Expected run time 


## Instructions for use

### How to run the software on your data
The sample simulation 

Once the sample simulation has be open within the APSIM (v7.8) user interface the simulations can be run by 

Test:![alt text](https://github.com/QAAFI/DCaPST/blob/master/Screenshots/Screenshot1.png "Logo Title Text 1")

### Reproduction instructions

## Code Structure

There are several classes which are all owned by the PhotosynthesisModel class. Following is a list of the main sub model and a brief list of their functionality:

* EnvironmentModel
     * Calculates solar geometry
     * Calculates diurnal radiation, temperature and VPD
* LeafCanopy
     * Calculates diurnal sunlit / shaded LAI (Leaf Area Index)
     * Calculates absorbed radiation
     * Calculates leaf nitrogen
* SunlitCanopy / ShadedCanopy
     * Calculates absorbed radiation
     * Calculates assimilation through rubisco and electron transport  


## Using the model

The source code is a C# .Net project and can be used with several development UI's eg. Microsoft Visual Studio.

The code does not use any non standard libraries.

Included in the project is a console application that instantiates a PhotoSynthesis model and runs it for both the C3 and C4 pathways. All of the parameters and variables are properties of the particular class and can be set directly.

All parameters have default values.


* Step 1 - Instantiate a PhotosynthesisModel

```csharp
PhotosynthesisModel PM = new PhotosynthesisModel();
```

* Step 2 - Set the desired photosynthetic pathway

```csharp
PM.photoPathway = PhotosynthesisModel.PhotoPathway.C3;
```        

* Step 3 - Set the location (latitude) and daily environmental parameters

```csharp
//Set the latitude
PM.envModel.latitude = new Angle(-27.5, AngleType.Deg);

//Set Ambient air CO2 partial pressure
PM.canopy.Ca = 400;

//Set the daily environmental variables
PM.envModel.DOY = 298;
PM.envModel.maxT = 21;
PM.envModel.minT = 7;
```

* Step 4 - Set the leaf architecture and nitrogen status

```csharp
//Set the leaf angle
PM.canopy.leafAngle = 60;

//Set daily LAI and SLN values
PM.canopy.LAI = 6;
PM.canopy.CPath.SLNAv = 1.45;
```

* Step 5 - Run the model

```csharp
PM.envModel.initilised = true;
PM.initialised = true;

//Run the model
PM.runDaily();
```

## Citation

To be announced


## Feedback

https://github.com/QAAFI/DCaPST

