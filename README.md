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

1. Register and download the APSIM (v7.8) installation software from the  [www.apsim.info](http:www.apsim.info) website.<br/><br/>
 Click [here](https://www.apsim.info/APSIM.Registration.Portal/Main.aspx) to go to the APSIM registration page </br>

2. Download and Install DCaPST software. This software can be downloaded from [here](https://github.com/QAAFI/DCaPST/blob/master/DCaPSTInstall.msi). This installer will:
 * Install the necessary libraries for APSIM v7.8 to send and recieve information from the DCaPST software
 * Maintain the original versions of the libraries in a separate folder
 * Install the sample simulation files. These files will be located in a folder called 'DCaPST Samples'    inside your *Documents* folder. These simulations are the same simualtions used to create the figures for the paper. 
 * R scripts (located in the above folder) to recreate some of the figures from the paper.


#### *Typical install time*
##### Installing APSIM installer:
 * Registration time : 2 minutes
 * Download time: 2 minutes
 * Install time : 2 minutes
 
##### Installing the DCaPST patch:
 * Download time: 1 minute
 * Install time : < 1 minute
 
## Demo
After the installation instructions have been completed, the sample simulations will be available in folder called 'DCaPST Samples' inside your *Documents* folder. This folder will contain a sample 'apsim' file called 'PS perturbation sims.apsim', this file is an input file to the APSIM user interface that can be launched via (start menu ) or by double clicking the file. If you have multiple versions of APSIM installed on your computer, you may need to launch the APSIM 7.8 user interface from the 'Start menu', to ensure the correct version is loaded.

Training modules for using the APSIM User interface can be found by clicking this link, [APSIM Training Modules](https://www.apsim.info/Documentation/TrainingManualsandResources/APSIMGeneralTrainingManual.aspx)

### How to run the software on data

Once the sample simulation has be opened within the APSIM (v7.8) user interface the simulations can be run by selecting the top node in the simualtion tree and then clicking the 'Run' button - as shown in the following image (Figure 1).

Figure1:![alt text](https://github.com/QAAFI/DCaPST/blob/master/Screenshots/Screenshot1.png "Logo Title Text 1")

### Expected output

Once the simulations have completed there is a windows batch file in the sample simualtions directory called *RunRScript.bat*. This file will suimmarise the newly created (APSIM) output files and recreate Figure 4 and Figure 5 from the paper. *Note:* *The R Project for Statistical Computing* ([https://www.r-project.org/](https://www.r-project.org/)), must be installed for this part of the analysis to work.

### Expected run time 

Expected run times are as follows:
 * Single simulation : 3.5 minutes
 * All simulations : 30 minutes
 * R script : 5 seconds


## Instructions for use

### How to run the software on your data
The sample simulations can be changed to use data based on location, soil and genetics.

#### Location
To change the location, the met (meteorology) file can be changed. Prepared met files are available for download from [Queensland Government SILO Weather Station Data Download Mirror](https://legacy.longpaddock.qld.gov.au/silo/) or users can create their own following the procedures outlined [here](https://www.apsim.info/Documentation/Model,CropandSoil/InfrastuctureandManagementDocumentation/MET.aspx).

Once a user has a prepared file the location can be changed by selecting the met file input tree node (see Figure 2) and seleting their new data file. *Note:* There are two places to change the met file. They are both lacated in the '*shared*' folder and are *shared* by the C4 (Sorghum) and C3 (Wheat simualtions), respectively.

Figure 2:![alt text](https://github.com/QAAFI/DCaPST/blob/master/Screenshots/Screenshot2.png "Logo Title Text 2")

#### Soil
The soil can be modified according using the *shared* soil tree node (see Figure 3). The APSIM soil component is discussed in detail in the [APSIM Training Modules](https://www.apsim.info/Documentation/TrainingManualsandResources/APSIMGeneralTrainingManual.aspx).

*Note:* There are 2 *shared* sections, one for the C4 and on for the C3 simulations. You may need to change the parameters in both places

Figure 3:![alt text](https://github.com/QAAFI/DCaPST/blob/master/Screenshots/Screenshot3.png "Logo Title Text 3")

#### Genetics
The genetics can be modified throught the The DCaPST manager script inside the simulations. As there are several simulations in this set, there are also several DCaPST manager scripts to change the genetics of the Photosynthetic mechanisms that are input to the DCaPST model. See Figure 4, which demonstrates where the different scripts are located.

The manager script in the left pane of Figure 4, contained in the Manager folder allows users to adjust DCaPST parameters, which include parameters from the biochemical models of photosynthesis, CO2 conductance model, and canopy attributes. Their functions can be found in the Supplementary Information accompanying the main text. Location in the Supplementary Inforamtion is indicated in brackets (scroll to the tight of the script). Default value of the parameters used for simulations in the main text are given.

Figure 4:![alt text](https://github.com/QAAFI/DCaPST/blob/master/Screenshots/Screenshot4.png "Logo Title Text 4")

To set up a simulation scenario with your particular photosynthetic attribute, firstly, the desired photosynthetic pathway (i.e. C3 or C4) is defined at Line 30 by changing the text in the double quotation:

    30	string PhotosyntheticPathway = "C4"; //Change this to change pathway

Parameters for the C3 model are between Line 46 to 111; C4 model parameters are between Line 128 to 206.

To set up, for example, the scenario with 20% enhancement in photosynthetic electron transport in C3 (described in the main text), the value of the psiJ parameter at Line 51 is increase by 20% (i.e. from the default 1.85 to 2.22).

    51	         PM.Canopy.CPath.PsiJ = 2.22 ;                     //psiJ-Psi J

This is the set up for the simulations that demonstrate an increase in the JMax parameter (simulations ending _Jmax) in the simulation set for C3 and C4 pathways.


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


## Citation

To be announced


## Feedback

https://github.com/QAAFI/DCaPST

