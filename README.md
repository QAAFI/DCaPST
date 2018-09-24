# DCaPST Package 
## *(Diurnal Canopy Photosynthesis-Stomatal Conductance Simulator )*

## About DCaPST
Diurnal C3 and C4 canopy photosynthesis-stomatal conductance modules (DCaPST) were developed and connected with the wheat and sorghum crop models of the Agricultural Production Systems sIMulator (APSIM v7.8). This connection was facilitated by partitioning the crop canopy into sunlit and shade leaf fractions. The cross-scale model involved (1) the crop models determining relevant crop attributes, such as leaf area index and specific leaf nitrogen (or leaf nitrogen content), and extent of soil water uptake for transpiration supply, and (2) leaf and canopy photosynthesis models determining potential daily canopy biomass increment and transpiration demand. The dynamic interplay of these components with the prevailing environment allowed the simulation of crop growth, development and yield through the crop life cycle.

## System Reqirements

#### *Minimum System Requirements*
1 GHz CPU </br>
512MB RAM

#### *Recommended System Requirements*
Quad Core 3GHz + CPU<br/>
4GB RAM


## Software Requirements

This software runs on the Windows operating system.

#### *Minimum Operating System*
Windows XP

#### *Recommended Operating System*
Windows 10

## Installation Guide
The DCaPST (Diurnal Canopy Photosynthesis-Stomatal Conductance Simulator) model works in conjunction with the The Agricultural Production Systems Simulator (APSIM : [www.apsim.info](http:www.apsim.info)), in particular APSIM version 7.8. 

#### *Instructions*
To install this software and utilise the demonstratation files, the following procedure should be followed

1. Register and download the APSIM (v7.8) installation software from the  [www.apsim.info](http:www.apsim.info) website.

 Click [here](https://www.apsim.info/APSIM.Registration.Portal/Main.aspx) to go to the APSIM registration page </br>

2. Download and Install DCaPST software. This software can be downloaded from [here](https://github.com/QAAFI/DCaPST/blob/master/DCaPSTInstall.msi). This installer will:

 * Install the necessary libraries for APSIM v7.8 to send and recieve information from the DCaPST software
 * Maintain the original versions of the libraries in a separate folder
 * Install the sample simulation files. These files will be located in a folder called 'DCaPST Samples'    inside your *Documents* folder. These simulations are the same simualtions used to create the figures for the paper. 
  * R scripts (located in the above folder) to recreate some of the figures from the paper.


#### *Typical install time*
#####Installing APSIM installer:
 * Registration time : 2 minutes
 * Download time: 2 minutes
 * Install time : 2 minutes
 
#####Installing the DCaPST patch:
 * Download time: 1 minute
 * Install time : < 1 minute
 
## Demo
After the installation instructions have been completed, the sample simulations will be available in folder called 'DCaPST Samples' inside your *Documents* folder. This folder will contain a sample 'apsim' file called 'PS perturbation sims.apsim', this file is an input file to the APSIM user interface that can be launched via (start menu ) or by double clicking the file. If you have multiple versions of APSIM installed on your computer, you may need to launch the APSIM 7.8 user interface from the 'Start menu', to ensure the correct version is loaded.

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

