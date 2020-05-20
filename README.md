# AIDE Frontend
[![Build Status](https://dev.azure.com/rsx-labs/aide/_apis/build/status/rsx-labs.aide-frontend?branchName=master)](https://dev.azure.com/rsx-labs/aide/_build/latest?definitionId=1&branchName=master) [![Build status](https://ci.appveyor.com/api/projects/status/kth1cyh42tqy11vb?svg=true)](https://ci.appveyor.com/project/trashvin/aide-frontend)  [![contributions welcome](https://img.shields.io/badge/contributions-welcome-brightgreen.svg?style=flat)](https://github.com/dwyl/esta/issues)

This is the frontend component of AIDE.

## Overview

Adaptive Intelligent Dashboard for Employees (AIDE) is a digital Communication Cell Board with additional features such as Asset Inventory and Tracker.
AIDE is meant to be ran on the employee machine once Outlook is already running

## System Requiremets

- Windows Operating Sytem - The recommended version to be installed is Windows 10.
- Microsoft Outlook - The recommended version to be installed to run alongside AIDE is Microsoft Outlook 2016

## AIDE Front End Installation

### Pre-requisites

```
Note: Do this only for first-time installation of AIDE. 
For upgrade installation, you may skip this if you have 
already done it during first-time installation.
```

#### Microsoft .Net Framework
1. Go to Start and search for Windows features
2. Open Windows features
3. Under Windows Features, ensure that the following are checked
- .Net Framework 4.7 Advanced Services -> ASP.NET 4.7
- .Net Framework 4.7 Advanced Services -> WCF Services -> HTTP Activation

#### Internet Information Services (IIS)
1. Go to Start and search for Windows features
2. Open Windows features.
3. Under Windows Features, ensure that the following are checked
- Internet Information Services -> Web Management Tools -> IIS Management Services

#### Additional Windows Feature
1. Download the installwindowsfeaturewin10.bat batch file from the installer site. The batch file is uploaded as a text file, you should rename the extension to *.bat. 
2. Open Command Prompt as Administrator and change directory to the folder where installwindowsfeaturewin10.bat is located.
3. Type installwindowsfeaturewin10.bat from the Command Prompt and press the Enter key. Wait until the installation is completed.

## AIDE Backend Service
1. Download the installer from AIDE installer site.
2. Run the MSI. Accept the default settings.

### AIDE CommCell
## AIDE Backend Service
1. Download the installer from AIDE installer site.
2. Run the MSI. Accept the default settings.


## Technology Stack

### Frontend
- .Net Framework
- WPF
- WCF

### Backend
- .Net Framework
- MS SQL Server
- WCF
