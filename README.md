
Media Butler Framework 
==========================
Introduction
-------------
Media Butler framework (MBF) is a VOD workflow automation framework for Azure Media Services. It supports create different workflow using configuration, combining pre-defined steps or using customs steps create by code. The basic workflow implementation is a folder watch folder but you can automate more complex scenarios.

MBF  is composed by 2 workers process: **Watcher** and **Workflow Manager** role. First one implements “watch folder” pattern on Azure Blob Storage. It takes the new files and submit it to Workflow Manager by **ButlerSend Queue**. When a new job is summited, this process moves the original files form Incoming folder to Processing. Once the process finish, success or fail, this process receives a message and process it. If the process was success, it will move the original date from Processing to Success folder. In the fail case, will move to Fail folder.

**Workflow Manager** is Media Butler's core, it is the workflow coordinator. It receives jobs from ButlerSend queue, and process it following the process definition in **ButlerConfiguration** table. This role, follow and control the process and execute each step. When the process finish, it sends the notification as is configured.

![Alt text](https://github.com/DX-TED-GEISV-Americas/Media-Butler-Framework/blob/master/docs/MediaButlerFrameworkOverview.JPG)

How to deploy Media Butler Framework
------------------------------------
### Setup pre requisites
* Microsoft Azure Subscription
* Staging storage account
* Azure App Service Plan, App Service and Web Job 

### Deploy Media Butler on a Web JOB
Media Butler Framework (MBF) has a deployment [PowerShell script](https://raw.githubusercontent.com/DX-TED-GEISV-Americas/Media-Butler-Framework/master/Deployment/zeroTouchDeploy.ps1). This script deploys MBF in Azure Web Job host always running. The script will create:
* MBF resource group
* Staging storage account
* Azure App Service Plan, App Service and Web Job

As part of the deployment, this script will create a sample process. This sample process is for testing propose and has this steps:
* Ingest the mezzanine file
* Encode using default profile
* Delete the original mezzanine asset
* Create a Streaming locator
* Create a SAS Locator
* Write the process output info in the LOG file

The scripts parameters are:
* MediaServiceAccountName: Media services account name.
* MediaServiceAccountKey: Media services account Key.
* MediaServiceStorageName: Media services storage account name.
* MediaServiceStorageKey: Media Services account key.
* SubscriptionName: Subscription name where deploy MBF.
* MyClearTextUsername: (optional) user account name to use to execute the script.
* MyClearTextPassword: (optional) user password.
* appName: New resource manager name.
* appRegion: Azure region, it must be the same of Azure Media Services.
* overWriteRG: (true/false) overwrite if resource group exist.

This script may be execute manually using user identity executing Login-AzureRmAccount command first. If you want to execute with other credential from Azure Active directory, you can set parameters MyClearTextUsername and MyClearTextPassword.

After execute the script you will receive this output, with the storage account name and key.
```
Stage Storaga Account Name mbfstagedeploytestXXXXX
Stage Storage Account Key XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXx==
WebSite Plan Name mbfwebjobhostdeploytestXXXXX
```
On Portal the resource group will see as this

![Alt text](https://github.com/DX-TED-GEISV-Americas/Media-Butler-Framework/blob/master/docs/ResourceGroupComponents.JPG)

The storage account inside the resorce group is **Staging storage** account and you will use to upload the new videos there.

### Test MBF deployment
To test this deployment, you can follow this step by step:
* Upload MP4 for **“testbasicprocess”** container in “Incoming” folder.
* Check in Media Services content a new asset with pattern name **“testbasicprocess_[your MP4 video Name]Butler[GUID]”**
* Check in Media Services JOB list a new job
* When the job finish, check the final asset Encoded and published with the patter name **“testbasicprocess_[your MP4 video Name]__Butler_[GUID]_mb”**
* Now, you could go to the Media Butler Storage, and review the output info in the file **testbasicprocess/Completed/[your MP4 video Name].[date and time].log**

Reporting issues and feedback
-----------------------------
If you encounter any bugs with the tool please file an issue in the Issues section of our GitHub repo.

License
------------
MIT
