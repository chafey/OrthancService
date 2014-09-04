OrthancService
==============

Run Orthac as Windows Service
Veresion 0.00

# What is OrthancService

This C# project provides the Orthanc to run as service on Microsoft Windows Environment.

# Building and Developing

* This project was originally developed on Microsoft Visual Studio 2013 Professional. Building it on other versions such as Express version has not been tested. 

* It targets .NET Framework 4.5.1 though it can be built to run on anything later than .NET 3.5. Originally it is targetted for the 32-bit environment.

The OrthancService exe can take a command line argument of -console and if that option is used, you do not need to install it as a service, instead the main processing thread will start and you will be able to debug core of the service monitor code. This will be handy for debugging in the Visual Studio.

# Installation

Note that we are planning to add an MSI installer and most of this information will change.

## Manual Service Installation Scripts Included

You can manually install/uninstall OrthancService.exe as a Windows Service using two .BAT files included. This will be handy during the devlopment to test the service without a need to build the MSI insatller. Files are;

* Manual-Install-Service.bat
* Manual-Remove-Service.bat

# How It Works

* When it starts it takes configuration values stored in OrthancService.exe.config file, it contains

** ExeAbsolutePath: Points to the Orthanc executable. For example this can be C:\Orthanc\Orthanc-0.8.2-Release.exe
** ExeArgumentsOrLeaveBlank: You can add command line arugments sent to Orthanc or just leave it blanc.
** ProcessCheckIntervalFloatSec: This is the pollin interval in number of seconds to check if the Orthanc process is still alive. You can specify any fraction of a second (e.g., 2.25 for 2500 milliseconds).

## About Two Phase Retry

One of the main feature of this service is that when and if the Orthanc executable crashes for any reason, it attempts to restart it. 

It does so in two phases. In the first phase, it is possible to configure restart to occur frequently for some number of times. In most cases, if a crash is a transitionary situation, a few quick retries may completely recover the operation.

After a set number of retries, you can have it retry at much less fequent intervals, for example every 5 to 10 mintues for 100 times. This can be a good strategy, if, for example you antitipate a fairly long lasting network outage.

## Confiuring Retry

Again in the OrthancService.exe.config file you will find the following parameters to adjust.

* RetryPhase1MaxRettryCounts
* RetryPhase1IntervalFloatSec : Yes, you can specify any fraction of a second.
* RetryPhase2MaxRetryCounts
* RetryPhase2IntervalFloatSec
